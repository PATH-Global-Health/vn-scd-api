using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using Services.RabbitMQ;
using Profile = Data.Entities.Profile;

namespace Services
{
    public interface IPatientService
    {
        ResultModel SubmitPatient(PatientCreateModel model);
    }

    public class PatientService : IPatientService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public PatientService(IMapper mapper, AppDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public ResultModel SubmitPatient(PatientCreateModel model)
        {

            ResultModel result = new ResultModel();
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {

                Patient patient = _mapper.Map<PatientCreateModel, Patient>(model);

                patient.CustomerName = model.CustomerInfo.CustomerName;
                patient.PhoneNumber = model.CustomerInfo.PhoneNumber;

                if (!model.CustomerInfo.Gender.ToUpper().Equals("male".ToUpper()) &&
                    !model.CustomerInfo.Gender.ToUpper().Equals("female".ToUpper()) &&
                    !model.CustomerInfo.Gender.ToUpper().Equals("nam".ToUpper()) &&
                    !model.CustomerInfo.Gender.ToUpper().Equals("nu".ToUpper()) 
                    )
                {
                    model.CustomerInfo.Gender = "N/A";
                }

                patient.Gender = model.CustomerInfo.Gender;
                _dbContext.Add(patient);

                bool gender = model.CustomerInfo.Gender.ToUpper().Equals("male".ToUpper()) ||
                              model.CustomerInfo.Gender.ToUpper().Equals("nam".ToUpper());

                Profile profileData = new Profile
                {
                    ExternalId = model.UserId,
                    Fullname = model.CustomerInfo.CustomerName,
                    PhoneNumber = model.CustomerInfo.PhoneNumber,
                    Gender = gender,
                    SentFrom = model.FacilityId,
                    ReceptionId = model.ReceptionId,
                    EmployeeId = model.EmployeeId

                    //Gen 
                };
                _dbContext.Add(profileData);


                foreach (var item in model.CustomerInfo.RelatedIds)
                {
                    var related = new RelatedPatient()
                    {
                        RelatedId = item,
                        UserId = model.UserId
                    };
                    _dbContext.Add(related);
                }

                _dbContext.SaveChanges();


                 
                SuccessfulResult succ = new SuccessfulResult();
                succ.Message = "Success";
                succ.StatusCode = 200;

                result.Data = succ;
                result.Succeed = true;
                transaction.Commit();
            }
            catch (Exception e)
            {
                
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                result.Failed = new CustomeResponseFailed {Error = "Bad request",Message = result.ErrorMessage , StatusCode = 400};
                transaction.Rollback();
            }

            return result;

        }
    }
}
