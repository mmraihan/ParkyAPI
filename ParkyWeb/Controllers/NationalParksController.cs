using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
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

    }
}
