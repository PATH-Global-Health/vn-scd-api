using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public interface IUnitDoctorService
    {
        ResultModel Add(UnitDoctorCreateModel model);
        ResultModel Update(UnitDoctorUpdateModel model);
        ResultModel Delete(Guid id);
        ResultModel Get(Guid id, string username);
        ResultModel GetAll(string username);
    }

    public class UnitDoctorService : IUnitDoctorService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public UnitDoctorService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ResultModel Add(UnitDoctorCreateModel model)
        {
            ResultModel result = new ResultModel();

            try
            {
                UnitDoctor newModel = _mapper.Map<UnitDoctor>(model);

                _dbContext.UnitDoctors.Add(newModel);
                _dbContext.SaveChanges();

                result.Data = _mapper.Map<UnitDoctor, UnitDoctorModel>(newModel);
                result.Succeed = true;
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
                UnitDoctor unitDoctor = _dbContext.UnitDoctors.FirstOrDefault(ud => ud.Id == ud.Id);

                if (unitDoctor != null)
                {
                    unitDoctor.IsDeleted = true;
                    unitDoctor.DateUpdated = DateTime.Now;

                    _dbContext.UnitDoctors.Update(unitDoctor);
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
        public ResultModel Get(Guid id, string username)
        {
            ResultModel result = new ResultModel();

            try
            {
                UnitDoctor unitDoctor = _dbContext.UnitDoctors.FirstOrDefault(ud => ud.Id == id && ud.IsDeleted == false && ud.Username == username);

                if (unitDoctor != null)
                {
                    UnitDoctorModel unitDoctorModel = _mapper.Map<UnitDoctorModel>(unitDoctor);
                    result.Data = unitDoctorModel;
                    result.Succeed = true;
                }

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public ResultModel GetAll(string username)
        {
            ResultModel result = new ResultModel();

            try
            {
                ICollection<UnitDoctor> unitDoctors = _dbContext.UnitDoctors.Where(ud => ud.IsDeleted == false && ud.Username == username).ToList();

                if (unitDoctors != null)
                {
                    ICollection<UnitDoctorModel> unitDoctorModels = _mapper.Map<ICollection<UnitDoctorModel>>(unitDoctors);
                    result.Data = unitDoctorModels;
                    result.Succeed = true;
                }

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public ResultModel Update(UnitDoctorUpdateModel model)
        {
            ResultModel result = new ResultModel();

            try
            {
                UnitDoctor unitDoctor = _mapper.Map<UnitDoctorUpdateModel, UnitDoctor>(model);

                if (unitDoctor != null)
                {
                    unitDoctor.DateUpdated = DateTime.Now;

                    _dbContext.UnitDoctors.Update(unitDoctor);
                    _dbContext.SaveChanges();

                    result.Data = _mapper.Map<UnitDoctor, UnitDoctorModel>(unitDoctor);
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
