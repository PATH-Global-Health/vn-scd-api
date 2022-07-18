using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public interface IServiceUnitService
    {
        ResultModel Add(ServiceUnitCreateModel model);
        ResultModel Update(ServiceUnitUpdateModel model);
        ResultModel Get(Guid id);
        ResultModel GetAll();
        ResultModel Delete(Guid id);
    }

    public class ServiceUnitService : IServiceUnitService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public ServiceUnitService(IMapper mapper, AppDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public ResultModel Add(ServiceUnitCreateModel model)
        {
            ResultModel result = new ResultModel();

            try
            {
                ServiceUnit newModel = _mapper.Map<ServiceUnit>(model);

                _dbContext.ServiceUnits.Add(newModel);
                _dbContext.SaveChanges();

                result.Data = _mapper.Map<ServiceUnit, ServiceUnitModel>(newModel);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    if (e.InnerException.Message.Contains("duplicate"))
                    {
                        result.ErrorMessage = "Code existed.";
                    }
                    else
                    {
                        result.ErrorMessage = e.InnerException.Message;
                    }
                }
                else
                {
                    result.ErrorMessage = e.Message;
                }
            }

            return result;
        }

        public ResultModel Delete(Guid id)
        {
            ResultModel result = new ResultModel();

            try
            {
                ServiceUnit serviceUnit = _dbContext.ServiceUnits.FirstOrDefault(ud => ud.Id == ud.Id);

                if (serviceUnit != null)
                {
                    serviceUnit.IsDeleted = true;
                    serviceUnit.DateUpdated = DateTime.Now;

                    _dbContext.ServiceUnits.Update(serviceUnit);
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

        public ResultModel Get(Guid id)
        {
            ResultModel result = new ResultModel();

            try
            {
                ServiceUnit serviceUnit = _dbContext.ServiceUnits.FirstOrDefault(ud => ud.Id == id && ud.IsDeleted == false);

                if (serviceUnit != null)
                {
                    ServiceUnitModel serviceUnitModel = _mapper.Map<ServiceUnitModel>(serviceUnit);
                    result.Data = serviceUnitModel;
                    result.Succeed = true;
                }

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        public ResultModel GetAll()
        {
            ResultModel result = new ResultModel();

            try
            {
                ICollection<ServiceUnit> serviceUnits = _dbContext.ServiceUnits.Where(su => su.IsDeleted == false).ToList();

                if (serviceUnits != null)
                {
                    ICollection<ServiceUnitModel> serviceUnitModels = _mapper.Map<ICollection<ServiceUnitModel>>(serviceUnits);
                    result.Data = serviceUnitModels;
                    result.Succeed = true;
                }

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        public ResultModel Update(ServiceUnitUpdateModel model)
        {
            ResultModel result = new ResultModel();

            try
            {
                ServiceUnit serviceUnit = _mapper.Map<ServiceUnitUpdateModel, ServiceUnit>(model);

                if (serviceUnit != null)
                {
                    serviceUnit.DateUpdated = DateTime.Now;

                    _dbContext.ServiceUnits.Update(serviceUnit);
                    _dbContext.SaveChanges();

                    result.Data = _mapper.Map<ServiceUnit, ServiceUnitModel>(serviceUnit);
                    result.Succeed = true;
                }

            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    if (e.InnerException.Message.Contains("duplicate"))
                    {
                        result.ErrorMessage = "Code existed.";
                    }
                    else
                    {
                        result.ErrorMessage = e.InnerException.Message;
                    }
                }
                else
                {
                    result.ErrorMessage = e.Message;
                }
            }

            return result;
        }
    }
}
