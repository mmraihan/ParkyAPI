using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _np;

        public NationalParksController(INationalParkRepository np)
        {
            _np = np;
        }

        public IActionResult Index()
        {
            return View(new NationalPark() { }); //Empty Object, Forloading data table
        }

        public async Task<IActionResult> GetAllNationalParks()
        {
            return Json(new { data = await _np.GetAllAsync(SD.NationalParkApiPath, HttpContext.Session.GetString("JWToken")) });
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark obj = new NationalPark();
            if (id==null)
            {              
                return View(obj); // --True for Insert
            }
               
            obj = await _np.GetAsync(SD.NationalParkApiPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken")); //--Flow will come for Update
            if (obj==null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files; //Get the file that was uploaded
                if (files.Count>0)
                { //Convert that image to a string
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using(var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }                       
                    }
                    obj.Picture = p1;
                }
                else //If image is not uploaded
                {
                    var objFromDb = await _np.GetAsync(SD.NationalParkApiPath, obj.Id, HttpContext.Session.GetString("JWToken"));
                    obj.Picture = objFromDb.Picture;
                }

                if (obj.Id==0)
                {
                    await _np.CreateAsync(SD.NationalParkApiPath, obj, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _np.UpdateAsync(SD.NationalParkApiPath + obj.Id, obj, HttpContext.Session.GetString("JWToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }

           
        }
            
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _np.DeleteAsync(SD.NationalParkApiPath, id, HttpContext.Session.GetString("JWToken"));
            if (status)
            {
                return Json(new { success = true, message = "Successfully Deleted" });
            }
            return Json(new { success = false, message = "Not Deleted" });
        }
    }
}
