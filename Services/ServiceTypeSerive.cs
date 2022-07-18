using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public interface IServiceTypeService
    {
        ResultModel Add(ServiceTypeCreateModel model);
        ResultModel Update(ServiceTypeUpdateModel model);
        ResultModel Get(Guid? id);
        ResultModel Delete(Guid id);
        ResultModel GetByServiceForm(Guid serviceFormId);
    }
    public class ServiceTypeService : IServiceTypeService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ServiceTypeService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ResultModel Add(ServiceTypeCreateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var data = _mapper.Map<ServiceTypeCreateModel, ServiceType>(model);
                _dbContext.Add(data);
                _dbContext.SaveChanges();
                result.Data = _mapper.Map<ServiceType, ServiceTypeModel>(data);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel Update(ServiceTypeUpdateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var data = _dbContext.ServiceTypes.FirstOrDefault(h => h.Id == model.Id);
                if (data != null)
                {
                    data.UnitId = model.UnitId;
                    data.CanChooseDoctor = model.CanChooseDoctor;
                    data.CanChooseHour = model.CanChooseHour;
                    data.CanPostPay = model.CanPostPay;
                    data.CanUseHealthInsurance = model.CanUseHealthInsurance;

                    data.DateUpdated = DateTime.Now;

                    _dbContext.Update(data);
                    _dbContext.SaveChanges();
                    result.Data = _mapper.Map<ServiceType, ServiceTypeModel>(data);
                    result.Succeed = true;
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public ResultModel Delete(Guid id)
        {
            ResultModel result = new ResultModel();
            try
            {
                var data = _dbContext.ServiceTypes.FirstOrDefault(h => h.Id == id);

                if (data != null)
                {
                    data.IsDeleted = true;
                    data.DateUpdated = DateTime.Now;

                    _dbContext.Update(data);
                    _dbContext.SaveChanges();

                    result.Succeed = true;
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public ResultModel Get(Guid? id)
        {
            ResultModel result = new ResultModel();
            try
            {
                if (id == null)
                {
                    var serviceTypes = _dbContext.ServiceTypes.ToList();
                    var data = _mapper.Map<List<ServiceType>, List<ServiceTypeModel>>(serviceTypes);
                    result.Data = data;
                }
                else
                {
                    var serviceType = _dbContext.ServiceTypes.FirstOrDefault(h => h.Id == id && h.IsDeleted == false);
                    var data = _mapper.Map<ServiceType, ServiceTypeModel>(serviceType);
                    result.Data = data;
                }
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public ResultModel GetByServiceForm(Guid serviceFormId)
        {
            var result = new ResultModel();
            try
            {
                var services = _dbContext.Services.Include(_s => _s.ServiceType).Where(_j => _j.ServiceFormId == serviceFormId).ToList();
                var data = new List<ServiceTypeModel>();
                foreach (var service in services)
                {
                    data.Add(_mapper.Map<ServiceType, ServiceTypeModel>(service.ServiceType));
                }
                data = data.DistinctBy(_d => _d.Id).ToList();
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
