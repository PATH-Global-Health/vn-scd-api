using System;
using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Services
{
    public interface IScheduleService
    {
    }

    public class ScheduleService : IScheduleService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly IIntervalService _intervalService;

        public ScheduleService(IMapper mapper, AppDbContext dbContext, IIntervalService intervalService)
        {
            _mapper = mapper;
            _intervalService = intervalService;
            _dbContext = dbContext;
        }
    }
}
