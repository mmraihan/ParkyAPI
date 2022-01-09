﻿using Microsoft.AspNetCore.Mvc;
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
            return Json(new { data = await _np.GetAllAsync(SD.NationalParkApiPath) });
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark obj = new NationalPark();
            if (id==null)
            {              
                return View(obj); // --True for Insert
            }
               
            obj = await _np.GetAsync(SD.NationalParkApiPath, id.GetValueOrDefault()); //--Flow will come for Update
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
                var files = HttpContext.Request.Form.Files;
                if (files.Count>0)
                {
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
                else
                {
                    var objFromDb = await _np.GetAsync(SD.NationalParkApiPath, obj.Id);
                    obj.Picture = objFromDb.Picture;
                }
                if (obj.Id==0)
                {
                    await _np.CreateAsync(SD.NationalParkApiPath, obj);
                }
                else
                {
                    await _np.UpdateAsync(SD.NationalParkApiPath + obj.Id, obj);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }
            

    }
}
