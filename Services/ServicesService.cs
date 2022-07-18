using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services
{
    public interface IServicesService
    {
        ResultModel Add(ServiceCreateModel model);
        ResultModel Update(ServiceUpdateModel model);
        ResultModel Get(Guid? id);
        ResultModel Delete(Guid id);
        ResultModel GetByServiceFormAndServiceType(Guid serviceFormId, Guid serviceTypeId, Guid? injectionObjectId);
    }
    public class ServicesService : IServicesService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ServicesService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ResultModel Add(ServiceCreateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                Data.Entities.Service data = new Data.Entities.Service
                {
                    IsDeleted = false,
                    Description = "",
                    ServiceTypeId = model.ServiceTypeId,
                    ServiceFormId = model.ServiceFormId,
                    Code = model.Code,
                    Name = model.Name,
                    InjectionObjectId = model.InjectionObjectId
                };
                _dbContext.Add(data);
                _dbContext.SaveChanges();

                result.Data = _mapper.Map<Data.Entities.Service, ServiceModel>(data);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;

            }
            return result;
        }
        public ResultModel Update(ServiceUpdateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var service = _dbContext.Services.FirstOrDefault(h => h.Id == model.Id);
                if (service != null)
                {
                    service.Code = model.Code;
                    service.Name = model.Name;
                    service.ServiceFormId = model.ServiceFormId;
                    service.ServiceTypeId = model.ServiceTypeId;
                    service.InjectionObjectId = model.InjectionObjectId;

                    service.DateUpdated = DateTime.Now;

                    _dbContext.Update(service);
                    _dbContext.SaveChanges();

                    result.Data = _mapper.Map<Data.Entities.Service, ServiceModel>(service);
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
                var service = _dbContext.Services.FirstOrDefault(h => h.Id == id);

                if (service != null)
                {
                    service.IsDeleted = true;
                    service.DateUpdated = DateTime.Now;

                    _dbContext.Update(service);
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
                    var services = _dbContext.Services.Where(s => s.IsDeleted == false).ToList();
                    var data = _mapper.Map<List<Data.Entities.Service>, List<ServiceModel>>(services);
                    result.Data = data;
                }
                else
                {
                    var service = _dbContext.Services.FirstOrDefault(h => h.Id == id && h.IsDeleted == false);
                    var data = _mapper.Map<Data.Entities.Service, ServiceModel>(service);
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
        public ResultModel GetByServiceFormAndServiceType(Guid serviceFormId, Guid serviceTypeId, Guid? injectionObjectId)
        {
            var result = new ResultModel();
            try
            {
                var service = _dbContext.Services
                    .Where(_s => _s.ServiceFormId == serviceFormId && _s.ServiceTypeId == serviceTypeId && injectionObjectId == _s.InjectionObjectId)
                    .ToList();
                result.Data = _mapper.Map<List<Data.Entities.Service>, List<ServiceModel>>(service);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel InsertData()
        {
            var result = new ResultModel();
            var data = "";

            var objectData = JsonConvert.DeserializeObject<List<InsertObject>>(data);


            foreach (var item in objectData)
            {
                ServiceCreateModel model = new ServiceCreateModel()
                {
                    Name = item.Name,
                    Code = item.Code,
                    ServiceFormId = Guid.Parse("d2e89a14-b17c-4d86-d598-08d889ed7ae2"),
                    //change me
                    InjectionObjectId = Guid.Parse("6c588279-e0b0-4d69-99af-dee027042019"),

                };
                if (item.ServiceType == 70)
                {
                    model.ServiceTypeId = Guid.Parse("0e538be1-448d-4d0c-2d70-08d88ab71c2b");
                }
                else if (item.ServiceType == 71)
                {
                    model.ServiceTypeId = Guid.Parse("4c090002-2a07-4064-2d6f-08d88ab71c2b");
                }
                Add(model);
            }
            result.Succeed = true;
            return result;
        }
        public class InsertObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public object Image { get; set; }
            public string Code { get; set; }
            public string _id { get; set; }
            public int ServiceType { get; set; }
            public bool AutoSync { get; set; }
            public int InjectionObjectId { get; set; }
            public object VaccineId { get; set; }
        }
    }
}
