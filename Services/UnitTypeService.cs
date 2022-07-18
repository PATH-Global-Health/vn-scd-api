using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public interface IUnitTypeService
    {
        ResultModel Add(UnitTypeCreateModel model);
        ResultModel Update(UnitTypeUpdateModel model);
        ResultModel Get(Guid id);
        ResultModel GetAll();
        ResultModel Delete(Guid id);
    }

    public class UnitTypeService : IUnitTypeService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public UnitTypeService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ResultModel Add(UnitTypeCreateModel model)
        {
            ResultModel result = new ResultModel();

            try
            {
                UnitType newUnitType = _mapper.Map<UnitType>(model);
                //UnitType newUnitType = new UnitType()
                //{
                //    Code = model.Code,
                //    TypeName = model.TypeName,
                //    Description = model.Description,
                //};

                _dbContext.Add(newUnitType);
                _dbContext.SaveChanges();

                result.Data = _mapper.Map<UnitType, UnitTypeModel>(newUnitType);
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
                UnitType unitType = _dbContext.UnitTypes.FirstOrDefault(ut => ut.Id == id);

                if (unitType != null)
                {
                    unitType.IsDeleted = true;
                    unitType.DateUpdated = DateTime.Now;

                    _dbContext.UnitTypes.Update(unitType);
                    _dbContext.SaveChanges();

                    result.Succeed = true;
                }
                else
                {
                    result.ErrorMessage = "Id not found.";
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
                UnitType unitType = _dbContext.UnitTypes.FirstOrDefault(ut => ut.Id == id && ut.IsDeleted == false);

                if (unitType != null)
                {
                    UnitTypeModel getModel = _mapper.Map<UnitTypeModel>(unitType);
                    //UnitTypeModel model = new UnitTypeModel()
                    //{
                    //    Id = unitType.Id,
                    //    TypeName = unitType.TypeName,
                    //    Description = unitType.Description,
                    //};

                    result.Data = getModel;
                    result.Succeed = true;
                }
                else
                {
                    result.ErrorMessage = "Id not found.";
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
                ICollection<UnitType> unitTypes = _dbContext.UnitTypes.Where(ut => ut.IsDeleted == false).ToList();

                if (unitTypes != null)
                {
                    ICollection<UnitTypeModel> unitTypeModels = _mapper.Map<ICollection<UnitTypeModel>>(unitTypes);

                    result.Data = unitTypeModels;
                    result.Succeed = true;
                }
                else
                {
                    result.ErrorMessage = "Id not found.";
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        public ResultModel Update(UnitTypeUpdateModel model)
        {
            ResultModel result = new ResultModel();

            try
            {
                UnitType unitType = _mapper.Map<UnitTypeUpdateModel, UnitType>(model);

                if (unitType != null)
                {
                    unitType.DateUpdated = DateTime.Now;

                    _dbContext.UnitTypes.Update(unitType);
                    _dbContext.SaveChanges();

                    result.Data = _mapper.Map<UnitType, UnitTypeModel>(unitType);
                    result.Succeed = true;
                }
                else
                {
                    result.ErrorMessage = "Id not found.";
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
