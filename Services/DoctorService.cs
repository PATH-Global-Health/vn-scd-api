using AutoMapper;
using Data.Constants;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Services.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Common.PaginationModel;
using Data.Utility.Paging;

namespace Services
{
    public interface IDoctorService
    {
        ResultModel Add(DoctorCreateModel model, string username);
        ResultModel Update(DoctorUpdateModel model, string username);
        ResultModel Get(Guid? id, string username);
        ResultModel GetDoctorInUnit(Guid? id, string username);
        ResultModel GetAllDoctor(string facilityId, string doctorName, int? page = null, int? pageSize = null);
        ResultModel SearchDoctor(string text, int? page = null, int? pageSize = null);
        ResultModel GetUnitByDoctorId(string doctorId);
        ResultModel GetAll(string username);
        ResultModel Delete(Guid id, string username);
        ResultModel RegisterDoctor(RegisterDoctorModel model, string adminId);
        ResultModel GetdoctorIdByUserId(string userId);
        Task<ResultModel> GetdoctorByUnitId(Guid unitid, PagingParam<BaseSortCriteria> paginationModel);


    }

    public class DoctorService : IDoctorService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly IProducer _producer;

        public DoctorService(IMapper mapper, AppDbContext dbContext, IProducer producer)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _producer = producer;
        }

        public ResultModel Add(DoctorCreateModel model, string username)
        {
            ResultModel result = new ResultModel();

            try
            {
                Doctor doctor = _mapper.Map<DoctorCreateModel, Doctor>(model);
                doctor.Username = username;

                if (ExistedCode(null, model.Code)) throw new Exception("Code existed.");


                _dbContext.Add(doctor);
                _dbContext.SaveChanges();

                result.Data = _mapper.Map<Doctor, DoctorModel>(doctor);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    if (e.InnerException.Message.Contains("duplicate"))
                    {
                        result.ErrorMessage = ErrorMessages.DUPLICATE_CODE;
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

        public ResultModel GetdoctorIdByUserId(string userId)
        {
            ResultModel result = new ResultModel();
            try
            {
                var getDoctorIdByUserId = _dbContext.Doctors.FirstOrDefault(d => d.UserId == userId);
                if (getDoctorIdByUserId == null) throw new Exception("UserId does not exist");
                result.Data = getDoctorIdByUserId.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }


        private bool ExistedCode(Guid? id, string code)
        {
            try
            {
                if (id == null)
                {
                    return _dbContext.Doctors.Any(s => s.Code.ToUpper().Equals(code.ToUpper()));
                }
                else
                {
                    return _dbContext.Doctors.Any(s => s.Code.ToUpper().Equals(code.ToUpper()) && s.Id != id);
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        public ResultModel RegisterDoctor(RegisterDoctorModel model, string adminId)
        {
            ResultModel result = new ResultModel();
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var unit = _dbContext.Units.FirstOrDefault(x => x.Username == adminId);
                if (unit == null) throw new Exception("Unit is not found");

                Doctor doctor = _mapper.Map<DoctorCreateModel, Doctor>(model);
                //                _dbContext.Add(doctor);
                //                _dbContext.SaveChanges();
                //
                //                result.Data = _mapper.Map<Doctor, DoctorModel>(doctor);

                if (ExistedCode(null, model.Code)) throw new Exception("Code existed.");

                UserCreateModel user = new UserCreateModel
                {
                    UserName = model.Username,
                    Password = model.Password,

                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    FullName = model.FullName,
                    IsEmailConfirmed = true,
                    GroupName = "EMPLOYEE"
                };
                var rs = _producer.CreateAccount(JsonConvert.SerializeObject(user));
                ResultModel rsMQ = JsonConvert.DeserializeObject<ResultModel>(rs);

                if (rsMQ.Succeed == false)
                {
                    result.ErrorMessage = rsMQ.ErrorMessage;
                    return result;
                }




                doctor.UserId = rsMQ.Data.ToString();
                doctor.Username = model.Username;

                _dbContext.Add(doctor);
                _dbContext.SaveChanges();

                result.Data = doctor.Id;
                result.Succeed = true;



                var unitDoctor = new UnitDoctor
                {
                    DoctorId = doctor.Id,
                    UnitId = unit.Id,
                    Username = adminId,
                    Code = doctor.Code
                };

                _dbContext.Add(unitDoctor);
                _dbContext.SaveChanges();

                transaction.Commit();
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    if (e.InnerException.Message.Contains("duplicate"))
                    {
                        result.ErrorMessage = ErrorMessages.DUPLICATE_CODE;
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
                transaction.Rollback();
            }

            return result;
        }

        public ResultModel Delete(Guid id, string username)
        {
            ResultModel result = new ResultModel();

            try
            {
                var doctor = _dbContext.UnitDoctors
                    .Where(x => x.Username == username)
                    .Select(x => x.Doctor)
                    .FirstOrDefault(x => x.Id == id);

                if (doctor != null)
                {
                    doctor.IsDeleted = true;
                    doctor.DateUpdated = DateTime.Now;

                    _dbContext.Update(doctor);
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

        public ResultModel Get(Guid? id, string username)
        {
            ResultModel result = new ResultModel();

            try
            {
                var doctors = _dbContext.Doctors.Where(p => (!id.HasValue || p.Id == id.Value) && p.Username == username).ToList();

                result.Data = _mapper.Map<List<Doctor>, List<DoctorModel>>(doctors);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        public ResultModel GetDoctorInUnit(Guid? id, string username)
        {
            ResultModel result = new ResultModel();

            try
            {
                var doctors = _dbContext.UnitDoctors
                    .Where(x => x.Username == username)
                    .Select(x => x.Doctor)
                    .Where(x => (!id.HasValue || x.Id == id.Value) && x.IsDeleted == false)
                    .ToList();

                result.Data = _mapper.Map<List<Doctor>, List<DoctorModel>>(doctors);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        public ResultModel GetAllDoctor(string facilityId = null, string doctorName = null, int? page = null, int? pageSize = null)
        {
            ResultModel result = new ResultModel();

            try
            {

                PagingModel paging = new PagingModel(page ?? 0, pageSize ?? 0, _dbContext.Doctors.Count(x => x.IsDeleted == false));

                var doctors = _dbContext.Doctors
                    .Where(x => x.IsDeleted == false)
                    .Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Take(paging.PageSize).ToList();

                var listDoctors = _mapper.Map<List<Doctor>, List<DoctorViewModel>>(doctors);

                foreach (var doctor in listDoctors)
                {
                    var unitDoctor =
                        _dbContext.UnitDoctors.Include(x => x.Unit)
                        .Where(x => x.DoctorId == doctor.Id)
                        .Select(x => x.Unit)
                        .ToList();
                    var dataUnit = _mapper.Map<List<Unit>, List<UnitModel>>(unitDoctor);
                    doctor.Unit = dataUnit;
                }

                //               paging.Data = _mapper.Map<List<Doctor>, List<DoctorModel>>(doctors);
                paging.Data = listDoctors;
                result.Data = paging;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        public ResultModel SearchDoctor(string text, int? page = null, int? pageSize = null)
        {
            ResultModel result = new ResultModel();

            try
            {

                var doctors = _dbContext.Doctors.Where(x => x.IsDeleted == false);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var list = _dbContext.UnitDoctors
                        .Where(x => x.Unit.Name.Contains(text))
                        .Select(x => x.DoctorId)
                        .ToList();
                    doctors = doctors
                        .Where(x => x.FullName.Contains(text) || x.Phone.Contains(text) || list.Contains(x.Id));
                }

                PagingModel paging = new PagingModel(page ?? 0, pageSize ?? 0, doctors.Count(x => x.IsDeleted == false));
                var listDoctor = doctors
                    .Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Take(paging.PageSize).ToList();

                var listDoctorsModel = _mapper.Map<List<Doctor>, List<DoctorViewModel>>(listDoctor);

                foreach (var doctor in listDoctorsModel)
                {
                    var unitDoctor =
                        _dbContext.UnitDoctors.Include(x => x.Unit)
                            .Where(x => x.DoctorId == doctor.Id)
                            .Select(x => x.Unit)
                            .ToList();
                    var dataUnit = _mapper.Map<List<Unit>, List<UnitModel>>(unitDoctor);
                    doctor.Unit = dataUnit;
                }

                paging.Data = listDoctorsModel;
                result.Data = paging;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }


        public ResultModel GetUnitByDoctorId(string doctorId)
        {
            ResultModel result = new ResultModel();

            try
            {

                result.Data = "";
                result.Succeed = true;
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
                ICollection<Doctor> doctors = _dbContext.Doctors.Where(d => d.IsDeleted == false && username == d.Username).ToList();

                if (doctors != null)
                {
                    result.Data = _mapper.Map<ICollection<Doctor>, ICollection<DoctorModel>>(doctors);
                    result.Succeed = true;
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        public ResultModel Update(DoctorUpdateModel model, string username)
        {
            ResultModel result = new ResultModel();

            try
            {
                //   Doctor doctor = _dbContext.Doctors.FirstOrDefault(_d => _d.Id == model.Id && _d.Username == username);

                var doctor = _dbContext.UnitDoctors
                    .Where(x => x.Username == username)
                    .Select(x => x.Doctor)
                    .FirstOrDefault(x => x.Id == model.Id);


                if (ExistedCode(doctor.Id, model.Code))
                {
                    {
                        result.ErrorMessage = "Code existed";
                        return result;

                    }
                }

                doctor.Code = model.Code;
                doctor.FullName = model.FullName;
                doctor.IdentityCard = model.IdentityCard;
                doctor.Title = model.Title;
                doctor.AcademicTitle = model.AcademicTitle;
                doctor.Gender = model.Gender;
                doctor.Email = model.Email;
                doctor.Phone = model.Phone;

                if (doctor != null)
                {
                    doctor.DateUpdated = DateTime.Now;

                    _dbContext.Doctors.Update(doctor);
                    _dbContext.SaveChanges();

                    result.Data = _mapper.Map<Doctor, DoctorModel>(doctor);
                    result.Succeed = true;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    if (e.InnerException.Message.Contains("duplicate"))
                    {
                        result.ErrorMessage = ErrorMessages.DUPLICATE_CODE;
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

        public async Task<ResultModel> GetdoctorByUnitId(Guid unitid,PagingParam<BaseSortCriteria> paginationModel)
        {
            ResultModel result = new ResultModel();
            try
            {
                var doctors = _dbContext.UnitDoctors
                    .Where(x => x.UnitId == unitid)
                    .Include(x => x.Doctor)
                    .Select(x => x.Doctor)
                    .Where(x => !x.IsDeleted);

                var paging = new PagingModel(paginationModel.PageIndex, paginationModel.PageSize, doctors.Count());

                doctors = doctors.GetWithSorting(paginationModel.SortKey.ToString(), paginationModel.SortOrder);
                doctors = doctors.GetWithPaging(paginationModel.PageIndex, paginationModel.PageSize);

                var viewModels = await _mapper.ProjectTo<DoctorViewModel>(doctors).ToListAsync();
                paging.Data = viewModels;
                result.Succeed = true;
                result.Data = paging;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

    }
}
