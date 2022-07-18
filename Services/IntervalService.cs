using Data.DbContexts;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;

namespace Services
{
    public interface IIntervalService
    {
        ResultModel OrderUnOrderIntervel(OrderIntervalModel model);
    }
    public class IntervalService : IIntervalService
    {
        private readonly AppDbContext _dbContext;

        public IntervalService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel OrderUnOrderIntervel(OrderIntervalModel model)
        {
            var result = new ResultModel();
            try
            {
                var interval = _dbContext.Intervals.FirstOrDefault(_i => _i.Id == model.Id);
                if (interval == null)
                {
                    throw new Exception("Invalid interval Id");
                }

                if (interval.IsAvailable == false || interval.AvailableQuantity == 0)
                {
                    throw new Exception("Unavailable interval");
                }
                interval.AvailableQuantity -= 1;
                _dbContext.Update(interval);
                _dbContext.SaveChanges();

                result.Data = interval.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
//        public ResultModel OrderIntervel(Guid id)
//        {
//            var result = new ResultModel();
//            try
//            {
//                var intervel = _dbContext.Intervals.FirstOrDefault(_i => _i.Id == id);
//                if (intervel == null)
//                {
//                    throw new Exception("Invalid interval Id");
//                }
//                intervel.IsAvailable = false;
//                _dbContext.Update(intervel);
//                _dbContext.SaveChanges();
//
//                result.Data = intervel.Id;
//                result.Succeed = true;
//            }
//            catch (Exception e)
//            {
//                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
//            }
//            return result;
//        }
    }
}
