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
    //[Route("api/Trails")]
    [Route("api/v{version:apiVersion}/trails")]   
    [ApiController]
   // [ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")] //Separate Documentation
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : Controller
    {
        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }
        /// <summary>
        /// Get list of Trails.
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]

        public IActionResult GetTrails()
        {
            var objListFromDb = _trailRepo.GetTrails();

            //Should not expose Domain Models to ouside, Should expose the DTO Models

            var objDto = new List<TrailDto>();

            foreach (var obj in objListFromDb)
            {
               objDto.Add(_mapper.Map<TrailDto>(obj)); //to=TrailDto, from = obj                           
            }
            return Ok(objDto);
        }

        /// <summary>
        /// Get Individual Trail
        /// </summary>
        /// <param name="trailId"> The id of Trail</param>
        /// <returns></returns>

        [HttpGet("{trailId:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int trailId)
        {
            var obj = _trailRepo.GetTrail(trailId);
            if (obj==null)
            {
                return NotFound();
            }

             var objDto = _mapper.Map<TrailDto>(obj); //to=TrailDto, from = obj

            return Ok(objDto);

        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        

        public IActionResult CreateTrail([FromBody] TrailCreatetDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_trailRepo.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDto);
            if (!_trailRepo.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }          
            return CreatedAtRoute("GetTrail", new { trailId = trailObj.Id }, trailObj); //Get data


        }


        [HttpPatch("{trailId}",Name = "UpdateTrail")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]      
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpdateDto trailDto)
        {
            if (trailDto==null || trailId!=trailDto.Id)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDto);
            if (!_trailRepo.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [HttpDelete ("{trailId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult DeleteTrail(int trailId)
        {

            if (!_trailRepo.TrailExists(trailId)) //Must checks, if trailId=0, then it
                                                             //will give null reference erro
            {
                return NotFound();
            }
            var trailObj = _trailRepo.GetTrail(trailId);
            if (!_trailRepo.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();



        }
    }
}
