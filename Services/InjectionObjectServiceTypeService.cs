using Data.DbContexts;
using Data.Entities;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public interface IInjectionObjectServiceTypeService
    {
        ResultModel Add(InjectionObjectServiceTypeAddModel model);
        ResultModel Delete(Guid injectionObjectId, Guid serviceTypeId);
    }
    public class InjectionObjectServiceTypeService : IInjectionObjectServiceTypeService
    {
        private readonly AppDbContext _dbContext;

        public InjectionObjectServiceTypeService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel Add(InjectionObjectServiceTypeAddModel model)
        {
            var result = new ResultModel();
            try
            {
                var existInjectionService = _dbContext.InjectionObjectServiceTypes
                    .FirstOrDefault(_i => _i.InjectionObjectId == model.InjectionObjectId && _i.ServiceTypeId == model.ServiceTypeId);

                if (existInjectionService != null)
                {
                    if (existInjectionService.IsDeleted == false)
                    {
                        throw new Exception("Data Existed!!");
                    }
                    else
                    {
                        existInjectionService.IsDeleted = false;
                        _dbContext.Update(existInjectionService);
                        _dbContext.SaveChanges();

                        result.Data = existInjectionService.InjectionObjectId;
                        result.Succeed = true;

                        return result;
                    }
                }

                var newInjectionService = new InjectionObjectServiceType()
                {
                    InjectionObjectId = model.InjectionObjectId,
                    ServiceTypeId = model.ServiceTypeId
                };
                _dbContext.Add(newInjectionService);
                _dbContext.SaveChanges();

                result.Data = newInjectionService.InjectionObjectId;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel Delete(Guid injectionObjectId, Guid serviceTypeId)
        {
            var result = new ResultModel();
            try
            {
                var existInjectionService = _dbContext.InjectionObjectServiceTypes
                    .FirstOrDefault(_i => _i.InjectionObjectId == injectionObjectId && _i.ServiceTypeId == serviceTypeId);
                if (existInjectionService == null)
                {
                    throw new Exception("Invalid Data!!");
                }

                existInjectionService.IsDeleted = true;
                existInjectionService.DateUpdated = DateTime.Now;

                _dbContext.Update(existInjectionService);
                _dbContext.SaveChanges();

                result.Data = existInjectionService.InjectionObjectId;
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
