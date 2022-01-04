using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
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
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")] //Separate Documentation
    [ProducesResponseType(StatusCodes.Status400BadRequest)] //It's very generic for all 
    public class NationalParksV2Controller : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksV2Controller(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }
        /// <summary>
        /// Get List of National Parks.
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]

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
