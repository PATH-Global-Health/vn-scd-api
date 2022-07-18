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
    public interface IServiceFormService
    {
        ResultModel Add(ServiceFormCreateModel model);
        ResultModel Update(ServiceFormUpdateModel model);
        ResultModel Get(Guid? id);
        ResultModel Delete(Guid id);
    }
    public class ServiceFormService : IServiceFormService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ServiceFormService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ResultModel Add(ServiceFormCreateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var data = _mapper.Map<ServiceFormCreateModel, ServiceForm>(model);
                _dbContext.Add(data);
                _dbContext.SaveChanges();
                result.Data = _mapper.Map<ServiceForm, ServiceFormModel>(data);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel Update(ServiceFormUpdateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var data = _dbContext.ServiceForms.FirstOrDefault(h => h.Id == model.Id);
                if (data != null)
                {
                    data.Name = model.Name;
                    data.Description = model.Description;

                    data.DateUpdated = DateTime.Now;

                    _dbContext.Update(data);
                    _dbContext.SaveChanges();

                    result.Data = _mapper.Map<ServiceForm, ServiceFormModel>(data);
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
                var data = _dbContext.ServiceForms.FirstOrDefault(h => h.Id == id);

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
                    var serviceForms = _dbContext.ServiceForms.ToList();
                    var data = _mapper.Map<List<ServiceForm>, List<ServiceFormModel>>(serviceForms);
                    result.Data = data;
                }
                else
                {
                    var serviceForm = _dbContext.ServiceForms.FirstOrDefault(h => h.Id == id && h.IsDeleted == false);
                    var data = _mapper.Map<ServiceForm, ServiceFormModel>(serviceForm);
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
    }
}
