using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;

namespace Services
{
    public interface ILayTestService
    {
        ResultModel SubmitLaytest(LayTestCreateModel model);
    }
    public class LayTestService : ILayTestService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public LayTestService(IMapper mapper, AppDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public ResultModel SubmitLaytest(LayTestCreateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                LayTest laytest = _mapper.Map<LayTestCreateModel, LayTest>(model);

                var patient = _dbContext.Patients.FirstOrDefault(f => f.UserId == model.UserId);

                if (patient == null)
                {
                    throw new Exception("Invalid user id");
                }

                laytest.PatientId = patient.Id;

                _dbContext.Add(laytest);
                _dbContext.SaveChanges();

                SuccessfulResult succ = new SuccessfulResult();
                succ.Message = "Success";
                succ.StatusCode = 200;

                result.Data = succ;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                result.Failed = new CustomeResponseFailed { Error = "Bad request", Message = result.ErrorMessage, StatusCode = 400 };

            }

            return result;
        }
    }
}
