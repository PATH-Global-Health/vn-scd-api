using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using System;
using System.Linq;

namespace Services
{
    public interface IPersonService
    {
        
    }

    public class PersonService : IPersonService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public PersonService(IMapper mapper, AppDbContext appDbContext)
        {
            _mapper = mapper;
            _dbContext = appDbContext;
        }
        
        
    }
}
