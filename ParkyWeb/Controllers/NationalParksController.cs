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
            return View();
        }

        public async Task<IActionResult> GetAllNationalParks()
        {
            return Json(new { data = await _np.GetAllAsync(SD.NationalParkApiPath) });
        }
    }
}
