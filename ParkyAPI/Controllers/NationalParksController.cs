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


        [HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var obj = _npRepo.GetNationalPark(nationalParkId);
            if (obj==null)
            {
                return NotFound();
            }

            var objDto = _mapper.Map<NationalParkDto>(obj);
            return Ok(objDto);

        }


        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National park Exists!");
                return StatusCode(404, ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkDto.Name}");
                return StatusCode(500, ModelState);
            }
        
           
            return CreatedAtRoute("GetNationalPark", new { nationalParkId = nationalParkObj.Id }, nationalParkObj); //Get data


        }


        [HttpPatch("{nationalParkId}",Name = "UpdateNationalPark")]

        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto==null || nationalParkId!=nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepo.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {nationalParkDto.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [HttpDelete ("{nationalParkId}")]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            #region Approach-1
            //var obj = _npRepo.GetNationalPark(nationalParkId);
            //if (obj==null)
            //{
            //    return NotFound(ModelState);
            //}
            //_npRepo.DeleteNationalPark(obj);
            //return Ok();
            #endregion


            if (!_npRepo.NationalParkExists(nationalParkId)) //Must checks, if nationalParkId=0, then it
                                                             //will give null reference error.

            {
                return NotFound();
            }
            var nationalParkObj = _npRepo.GetNationalPark(nationalParkId);
            if (!_npRepo.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();



        }
    }
}
