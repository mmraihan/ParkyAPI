using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        [HttpGet]

        public IActionResult GetNationalParks()
        {
            var objListFromDb = _npRepo.GetNationalParks();

            //Should not expose Domain Models to ouside, Should expose the DTO Models

            var objDto = new List<NationalParkDto>();

            foreach (var obj in objListFromDb)
            {
               objDto.Add(_mapper.Map<NationalParkDto>(obj)); //to=NatioNalParkDto, from = obj                           
            }
            return Ok(objDto);
        }

    }
}
