using AutoMapper;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.ParkyMapper
{
    public class ParkyMapings : Profile
    {
        public ParkyMapings()
        {
            CreateMap<NationalPark, NationalParkDto>().ReverseMap(); //----- Map both ways
            CreateMap<Trail, TrailDto>().ReverseMap();
            CreateMap<Trail, TrailCreatetDto>().ReverseMap();
            CreateMap<Trail, TrailUpdateDto>().ReverseMap();
          
        }
    }
}
