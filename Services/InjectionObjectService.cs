using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public interface IInjectionObjectService
    {
        ResultModel Add(InjectionObjectAddModel model);
        ResultModel Update(InjectionObjectUpdateModel model);
        ResultModel Delete(Guid id);
        ResultModel Get(Guid? id);
        ResultModel GetByServiceType(Guid serviceTypeId);
    }
    public class InjectionObjectService : IInjectionObjectService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public InjectionObjectService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ResultModel Add(InjectionObjectAddModel model)
        {
            var result = new ResultModel();
            try
            {
                var injectionObject = _mapper.Map<InjectionObjectAddModel, InjectionObject>(model);
                _dbContext.Add(injectionObject);
                _dbContext.SaveChanges();

                result.Succeed = true;
                result.Data = injectionObject.Id;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel Update(InjectionObjectUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                var injectionObject = _dbContext.InjectionObjects.FirstOrDefault(_i => _i.Id == model.Id);
                if (injectionObject == null)
                {
                    throw new Exception("Invalid Injection Object");
                }
                injectionObject.Name = model.Name;
                injectionObject.ToDaysOld = model.ToDaysOld;
                injectionObject.FromDaysOld = model.FromDaysOld;

                _dbContext.Update(injectionObject);
                _dbContext.SaveChanges();

                result.Succeed = true;
                result.Data = injectionObject.Id;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel Delete(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var injectionObject = _dbContext.InjectionObjects.FirstOrDefault(_i => _i.Id == id);
                if (injectionObject == null)
                {
                    throw new Exception("Invalid Injection Object");
                }

                injectionObject.IsDeleted = true;

                _dbContext.Update(injectionObject);
                _dbContext.SaveChanges();

                result.Succeed = true;
                result.Data = injectionObject.Id;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel Get(Guid? id)
        {
            var result = new ResultModel();
            try
            {
                var injectionObjects = _dbContext.InjectionObjects.Where(_i => id == null || _i.Id == id).Where(_i => _i.IsDeleted == false).ToList();
                if (injectionObjects.Count == 0 || injectionObjects == null)
                {
                    throw new Exception("Empty");
                }
                result.Data = _mapper.Map<List<InjectionObject>, List<InjectionObjectViewModel>>(injectionObjects).OrderBy(_m=>_m.FromDaysOld).ToList();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel GetByServiceType(Guid serviceTypeId)
        {
            var result = new ResultModel();
            try
            {
                var serviceTypes = _dbContext.InjectionObjectServiceTypes.Include(_s => _s.InjectionObject).Where(_s => _s.ServiceTypeId == serviceTypeId && _s.IsDeleted == false).ToList();
                var data = new List<InjectionObjectViewModel>();
                foreach (var serviceType in serviceTypes)
                {
                    data.Add(_mapper.Map<InjectionObject, InjectionObjectViewModel>(serviceType.InjectionObject));
                }

                data = data.OrderBy(_d => _d.FromDaysOld).ToList();

                result.Data = data;
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
