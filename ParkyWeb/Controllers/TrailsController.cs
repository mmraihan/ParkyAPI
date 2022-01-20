using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _np;
        private readonly ITrailRepository _trailRepo;

        public TrailsController(INationalParkRepository np, ITrailRepository trailRepo)
        {
            _np = np;
            _trailRepo = trailRepo;
        }

        public IActionResult Index()
        {
            return View(new Trail() { }); //Empty Object, Forloading data table
        }

        public async Task<IActionResult> GetAllTrails()
        {
            return Json(new { data = await _trailRepo.GetAllAsync(SD.TrailApiPath, HttpContext.Session.GetString("JWToken")) });
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _np.GetAllAsync(SD.NationalParkApiPath, HttpContext.Session.GetString("JWToken"));

            TrailsVM objVM = new TrailsVM()
            {
                NationalParkList = npList.Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Id.ToString()

                }),    
                Trail = new Trail() //otherwise object reference will be null
            };
      

           
            if (id==null)
            {              
                return View(objVM); // --True for Insert
            }

            objVM.Trail = await _trailRepo.GetAsync(SD.TrailApiPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken")); //--Flow will come for Update
            if (objVM .Trail==null)
            {
                return NotFound();
            }
            return View(objVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM obj)
        {
            if (ModelState.IsValid)
            {
               
                if (obj.Trail.Id==0)
                {
                    await _trailRepo.CreateAsync(SD.TrailApiPath, obj.Trail, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _trailRepo.UpdateAsync(SD.TrailApiPath + obj.Trail.Id, obj.Trail, HttpContext.Session.GetString("JWToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await _np.GetAllAsync(SD.NationalParkApiPath, HttpContext.Session.GetString("JWToken"));

                TrailsVM objVM = new TrailsVM()
                {
                    NationalParkList = npList.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()

                    }),
                    Trail = obj.Trail //otherwise object reference will be null
                };
                return View(objVM);
            }

           
        }
            
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _trailRepo.DeleteAsync(SD.TrailApiPath, id, HttpContext.Session.GetString("JWToken"));
            if (status)
            {
                return Json(new { success = true, message = "Successfully Deleted" });
            }
            return Json(new { success = false, message = "Not Deleted" });
        }
    }
}
