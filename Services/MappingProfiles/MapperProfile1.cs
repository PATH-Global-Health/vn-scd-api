using AutoMapper;
using Data.Entities;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.MappingProfiles
{
    public class MapperProfile1 : AutoMapper.Profile
    {
        public MapperProfile1()
        {
            CreateMap<ServiceTypeCreateModel, ServiceType>();
            CreateMap<ServiceFormCreateModel, ServiceForm>();
        }
    }
}
