using System;
using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using System.Collections.Generic;
using System.Linq;
using Data.Constants;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace Services
{
    public interface IDayService
    {
        ResultModel GetWorkingDate(Guid serviceId, DateTime date);
        ResultModel GetDateByServiceId(Guid serviceId);
    }

    public class DayService : IDayService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly IScheduleService _scheduleService;

        public DayService(IMapper mapper, AppDbContext dbContext, IScheduleService scheduleService)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _scheduleService = scheduleService;
        }

        public ResultModel GetWorkingDate(Guid serviceId, DateTime date)
        {
            var result = new ResultModel();
            try
            {
                var unitId = _dbContext.Days
                    .Include(d => d.WorkingCalendar.ServiceCalendars)
                    .Where(d => d.Date.Date == date.Date)
                    .Where(d => d.WorkingCalendar.Status == WorkingCalendarStatus.POSTED)
                    .Where(d => d.WorkingCalendar.ServiceCalendars.Select(x => x.ServiceId).Contains(serviceId))
                    .ToList()
                    .DistinctBy(d => d.WorkingCalendar.UnitId)
                    .Select(x=>x.WorkingCalendar.UnitId);

                result.Data = _mapper.Map<List<Unit>,List<UnitModel>>(_dbContext.Units.Where(u => unitId.Contains(u.Id)).ToList());
                result.Succeed = true;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel GetDateByServiceId(Guid serviceId)
        {
            var result = new ResultModel();
            try
            {
                var listWorkingCalendar = _dbContext.ServiceWorkingCalendars
                    .Include(d => d.WorkingCalendar)
                    .Where(d => d.ServiceId == serviceId)
                    .Where(d => d.WorkingCalendar.Status == WorkingCalendarStatus.POSTED)
                    .Where(d=> d.WorkingCalendar.ToDate >= DateTime.Now.Date)
                    .Select(x => x.WorkingCalendar.Id).ToList();

                var listDate = _dbContext.Days
                    .Where(d => listWorkingCalendar.Contains(d.CalendarId))
                    .Where(d=>d.Date >= DateTime.Now.Date)
                    .OrderBy(d => d.Date)
                    .ToList()
                    .Select(x => x.Date).DistinctBy(x => x.Date).ToList();
                   

                result.Data = listDate.ToList();
                result.Succeed = true;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
    }
}
