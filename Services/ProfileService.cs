using AutoMapper;
using Data.DbContexts;
using Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Castle.Core.Internal;
using Data.Constants;
using Data.Entities;
using Services.RabbitMQ;
using Profile = AutoMapper.Profile;

namespace Services
{
    public interface IProfileService
    {
        ResultModel Add(ProfileAddModel model, string username);
        ResultModel Get(Guid? userId, string username);
        ResultModel GetRelation(string username);
        ResultModel Update(ProfileUpdateModel model);
        ResultModel Delete(Guid id);
        ResultModel SearchByName(string fullname,int? status = null);
        ResultModel FilterProfile(string username, string name, int? status = null);
        ResultModel GetProfileFromDealth(string fullname);
        ResultModel ProfileByUnitId(Guid unitId);
        ResultModel AddProfileByFacility(ProfileAddModel model, string username, Guid unitId);
        ResultModel CheckProfileFromDealth(string externalId);
        ResultModel AddByDoctor(ProfileAddModel model, string username);
        ResultModel SetStatusProfileById(StatusProfileModel model);
        ResultModel CustomerByQR(string userNameEmp, CustomerQR model);
        ResultModel RegisterCustomerIdentification(string userNameEmp, CustomerIdentification model);
        ResultModel RegisterCustomerIdentification(CustomerIdentification model);

    }
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IProducer _producer;
        private readonly IProducerCheckConfirmedCustomer _producerCheckConfirmedCustomer;
        public ProfileService(AppDbContext dbContext, IMapper mapper,IProducer producer, IProducerCheckConfirmedCustomer producerCheckConfirmedCustomer)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _producer = producer;
            _producerCheckConfirmedCustomer = producerCheckConfirmedCustomer;
        }

        public ResultModel Add(ProfileAddModel model, string username)
        {
            var result = new ResultModel();
            try
            {
                var profile = _dbContext.Profiles.FirstOrDefault(_u => _u.Username == username);
                Data.Entities.Profile newProfile = newProfile = _mapper.Map<ProfileAddModel, Data.Entities.Profile>(model);
                if (profile == null)
                {
                    newProfile.Username = username;
                }
                else
                {
                    newProfile.RelationProfileId = profile.Id;
                }

                _dbContext.Add(newProfile);
                _dbContext.SaveChanges();

                result.Data = newProfile.Id;
                result.Succeed = true;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel AddByDoctor(ProfileAddModel model, string username)
        {
            var result = new ResultModel();
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var profile = _dbContext.Profiles.FirstOrDefault(_u => _u.Username == username);
                Data.Entities.Profile newProfile = newProfile = _mapper.Map<ProfileAddModel, Data.Entities.Profile>(model);
                if (profile == null)
                {
                    newProfile.Username = username;
                    _dbContext.Add(newProfile);
                    _dbContext.SaveChanges();
                }
                else
                {

                    newProfile.RelationProfileId = profile.Id;
                    _dbContext.Add(newProfile);
                    _dbContext.SaveChanges();

                    var empId = _dbContext.Doctors.FirstOrDefault(x => x.Username == username);
                    var unitDoctor = _dbContext.UnitDoctors.FirstOrDefault(x => x.DoctorId == empId.Id && x.IsDeleted == false);

                    var profileLinkses = new ConcurrentQueue<ProfileLinks>();
                    profileLinkses.Enqueue(new ProfileLinks()
                    {
                        LinkTo = profile.Id,
                        Type = TypeFacitily.EMPLOYEE,
                        ProfileId = newProfile.Id
                    });
                    profileLinkses.Enqueue(new ProfileLinks()
                    {
                        LinkTo = unitDoctor.UnitId.Value,
                        Type = TypeFacitily.FACILITY,
                        ProfileId = newProfile.Id
                    });
                    _dbContext.AddRange(profileLinkses);
                    _dbContext.SaveChanges();
                }
                result.Data = newProfile.Id;
                result.Succeed = true;
                transaction.Commit();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                transaction.Rollback();
            }
            return result;
        }

        public ResultModel AddProfileByFacility(ProfileAddModel model, string username,Guid unitId)
        {
            var result = new ResultModel();
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var profile = _dbContext.Profiles.FirstOrDefault(_u => _u.Username == username);

                Data.Entities.Profile newProfile = newProfile = _mapper.Map<ProfileAddModel, Data.Entities.Profile>(model);

                if (profile == null)
                {
                    newProfile.Username = username;
                }
                else
                {
                    newProfile.RelationProfileId = profile.Id;
                }

                _dbContext.Add(newProfile);
                _dbContext.SaveChanges();

                var profileLinks= new ProfileLinks
                {
                    ProfileId = newProfile.Id,
                    LinkTo = unitId,
                    Type = TypeFacitily.FACILITY

                };
                _dbContext.Add(profileLinks);
                _dbContext.SaveChanges();

                result.Data = newProfile.Id;
                result.Succeed = true;
                transaction.Commit();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                transaction.Rollback();

            }
            return result;
        }
        public ResultModel Delete(Guid id)
        {
            var result = new ResultModel();
            try
            {
                var profile = _dbContext.Profiles.FirstOrDefault(_u => _u.Id == id);

                profile.IsDeleted = true;

                _dbContext.Update(profile);
                _dbContext.SaveChanges();

                result.Data = profile.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel Update(ProfileUpdateModel model)
        {
            var result = new ResultModel();
            try
            {
                //if (model.PassportNumber != null)
                //{
                //    var existedPassportNumber = _dbContext.Profiles.Any(_p => _p.PassportNumber == model.PassportNumber && _p.Id != model.Id);
                //    if (existedPassportNumber)
                //    {
                //        var errorMessage = new ErrorMessage() { Validate = "Passport Number is already existed" };
                //        return new ResultModel() { Succeed = false, ErrorMessage = JsonConvert.SerializeObject(errorMessage) };
                //    }
                //}

                var profile = _dbContext.Profiles.FirstOrDefault(_u => _u.Id == model.Id);

                profile.Fullname = model.Fullname;
                profile.Gender = model.Gender;
                profile.DateOfBirth = model.DateOfBirth;
                profile.PhoneNumber = model.PhoneNumber;
                profile.Email = model.Email;
                profile.VaccinationCode = model.VaccinationCode;
                profile.Address = model.Address;
                profile.Province = model.Province;
                profile.District = model.District;
                profile.Ward = model.Ward;
                profile.IdentityCard = model.IdentityCard;
                profile.PassportNumber = model.PassportNumber;
                profile.Nation = model.Nation;
                profile.Status = model.Status;

                _dbContext.Update(profile);
                _dbContext.SaveChanges();

                result.Data = _mapper.Map<Data.Entities.Profile, ProfileViewModel>(profile);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        
        public ResultModel Get(Guid? userId, string username)
        {
            var result = new ResultModel();
            try
            {
                Data.Entities.Profile profile;
                if (userId == null)
                {
                    profile = _dbContext.Profiles.FirstOrDefault(_u => _u.Username == username && _u.IsDeleted == false);
                    if (profile == null)
                    {
                        profile = new Data.Entities.Profile()
                        {
                            Address = "",
                            DateOfBirth = DateTime.Now,
                            Description = "",
                            Email = "",
                            Gender = false,
                            District = "",
                            Province = "",
                            PhoneNumber = "",
                            Username = username,
                            VaccinationCode = "",
                            Ward = "",
                            Fullname = username
                        };

                        _dbContext.Add(profile);
                        _dbContext.SaveChanges();
                    }
                }
                else
                {
                    profile = _dbContext.Profiles.FirstOrDefault(_u => _u.Id == userId);
                    if (profile == null)
                    {
                        throw new Exception("Invalid Id");
                    }
                }

                result.Data = _mapper.Map<Data.Entities.Profile, ProfileViewModel>(profile);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel GetRelation(string username)
        {
            var result = new ResultModel();
            try
            {
                var profile = _dbContext.Profiles.FirstOrDefault(_u => _u.Username == username && _u.IsDeleted == false);
                if (profile == null)
                {
                    profile = new Data.Entities.Profile()
                    {
                        Address = "",
                        DateOfBirth = DateTime.Now,
                        Description = "",
                        Email = "",
                        Gender = false,
                        District = "",
                        Province = "",
                        PhoneNumber = "",
                        Username = username,
                        VaccinationCode = "",
                        Ward = "",
                        Fullname = username
                    };

                    _dbContext.Add(profile);
                    _dbContext.SaveChanges();
                }
                var profiles = _dbContext.Profiles.Where(_u => _u.RelationProfileId == profile.Id).ToList();

                var profilesViewModel = new List<ProfileViewModel>();

                try
                {
                    profilesViewModel.Add(_mapper.Map<Data.Entities.Profile, ProfileViewModel>(profile));
                    profilesViewModel.AddRange(_mapper.Map<List<Data.Entities.Profile>, List<ProfileViewModel>>(profiles));
                }
                catch (Exception) { }

                result.Data = profilesViewModel;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel FilterProfile(string username,string name , int? status = null)
        {
            var result = new ResultModel();
            try
            {
                var profile = _dbContext.Profiles.FirstOrDefault(_u => _u.Username == username && _u.IsDeleted == false);

                var profileRelated = _dbContext.Profiles.Where(x => x.RelationProfileId == profile.Id);

                if (!string.IsNullOrEmpty(name))
                {
                    profileRelated = profileRelated
                        .Where(x => x.Fullname.ToLower().Trim().Contains(name.ToLower()));
                }

                if (status.HasValue)
                {
                    profileRelated = profileRelated.Where(x => x.Status == status);
                }

                var listProfile = profileRelated.ToList();

                result.Data = _mapper.Map<List<Data.Entities.Profile>, List<ProfileViewModel>>(listProfile);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel ProfileByUnitId(Guid unitId)
        {
            var result = new ResultModel();
            try
            {
                var profiles = _dbContext.ProfileLinks.Where(x => x.LinkTo == unitId).Select(x => x.Profile).ToList();
                result.Data = _mapper.Map<List<Data.Entities.Profile>, List<ProfileViewModel>>(profiles);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }


        public ResultModel SearchByName(string fullname,int? status=null)
        {
            var result = new ResultModel();
            try
            {
                var profiles = _dbContext.Profiles
                    .Where((_p =>
                        string.IsNullOrEmpty(fullname) ||
                        _p.Fullname.ToLower().Trim().Contains(fullname.Trim().ToLower()) && _p.IsDeleted == false));

                if (status.HasValue) profiles = profiles.Where(x => x.Status == status);

                var listProfile = profiles.ToList();

                result.Data = _mapper.Map<List<Data.Entities.Profile>, List<ProfileViewModel>>(listProfile);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel GetProfileFromDealth(string fullname)
        {
            var result = new ResultModel();
            try
            {
                var profiles = _dbContext.Profiles
                    .Where((_p => string.IsNullOrEmpty(fullname)  || _p.PhoneNumber.Contains(fullname) || _p.Fullname.ToLower().Trim().Contains(fullname.Trim().ToLower()) && _p.IsDeleted == false))
                    .Where(x => !string.IsNullOrEmpty(x.ExternalId))
                    .ToList();

                result.Data = _mapper.Map<List<Data.Entities.Profile>, List<ProfileViewModel>>(profiles);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel CheckProfileFromDealth(string externalId)
        {
            var result = new ResultModel();
            try
            {
                var profilesExternalId = _dbContext.Profiles.FirstOrDefault(x => x.ExternalId == externalId).ExternalId;
                if (profilesExternalId == null)
                {
                    throw new Exception("Invalid external");
                }
                result.Data = profilesExternalId;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel SetStatusProfileById(StatusProfileModel model)
        {
            var result = new ResultModel();
            try
            {
                var profile = _dbContext.Profiles
                    .FirstOrDefault(x => (x.Id == model.CustomerId ||(!string.IsNullOrEmpty(model.UserName) && x.Username == model.UserName)));
                if (profile == null)
                {
                    throw new Exception("Invalid profileId or username");
                }

                if (model.Status > -1)
                {
                    profile.Status = model.Status;
                }
                if (!string.IsNullOrEmpty(model.Phone))
                {
                    profile.PhoneNumber = model.Phone;
                }

                if (!string.IsNullOrEmpty(model.Email))
                {
                    profile.Email = model.Email;
                }
                if (!string.IsNullOrEmpty(model.FullName))
                {
                    profile.Fullname = model.FullName;
                }

                profile.IsDeleted = model.IsDelete;
                _dbContext.Update(profile);
                _dbContext.SaveChanges();
                result.Data = profile.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel CustomerByQR(string userNameEmp,CustomerQR model)
        {
            var result = new ResultModel();
            try
            {
                var isExistEmp = _dbContext.Profiles.FirstOrDefault(x => x.Username == userNameEmp);
                if (isExistEmp == null)
                {
                    throw new Exception("Employee profile does not exist");
                }

                var rs = _producerCheckConfirmedCustomer.CheckConfirmedCustomer(model.UserName);
                ResultModel rsMQ = JsonConvert.DeserializeObject<ResultModel>(rs);
                if (!rsMQ.Succeed)
                {
                    result.ErrorMessage = rsMQ.ErrorMessage;
                    return result;
                }
                var isExistCustomer = _dbContext.Profiles.FirstOrDefault(x => x.Username == model.UserName);
                var idEmployee = isExistEmp.Id;
                if (isExistCustomer != null)
                {
                    isExistCustomer.RelationProfileId = idEmployee;
                    isExistCustomer.Status = (bool)rsMQ.Data? 1 : 0;
                    _dbContext.Update(isExistCustomer);
                    _dbContext.SaveChanges();
                    result.Data = isExistCustomer.Id;
                }
                else
                {
                    Data.Entities.Profile customer = new Data.Entities.Profile
                    {
                        Username = model.UserName,
                        RelationProfileId = idEmployee,
                        Fullname = model.Fullname,
                        Email = model.Email,
                        Gender = model.Gender,
                        PhoneNumber = model.PhoneNumber,
                        Status = 1
                    };
                    _dbContext.Add(customer);
                    _dbContext.SaveChanges();
                    result.Data = customer.Id;
                }
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel RegisterCustomerIdentification(string userNameEmp, CustomerIdentification model)
        {
            var result = new ResultModel();
            try
            {
                var empProfileExisted = _dbContext.Profiles.FirstOrDefault(x => x.Username == userNameEmp);
                if (empProfileExisted == null)
                {
                    throw new Exception("Employee profile does not exist");
                }
                UserCreateModel user = new UserCreateModel
                {
                    UserName = model.UserName,
                    Password = model.Password,

                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FullName = model.Fullname,
                    GroupName = "CUSTOMER"
                };
                var rs = _producer.CreateAccount(JsonConvert.SerializeObject(user));
                ResultModel rsMQ = JsonConvert.DeserializeObject<ResultModel>(rs);
                if (rsMQ.Succeed == false)
                {
                    result.ErrorMessage = rsMQ.ErrorMessage;
                    return result;
                }
                var isExistCustomer = _dbContext.Profiles.FirstOrDefault(x => x.Username == model.UserName);
                if (isExistCustomer != null)
                {
                    isExistCustomer.RelationProfileId = empProfileExisted.Id;
                    isExistCustomer.Fullname = model.Fullname;
                    isExistCustomer.PhoneNumber = model.PhoneNumber;
                    isExistCustomer.Gender = model.Gender;
                    isExistCustomer.Email = model.Email;
                    _dbContext.Update(isExistCustomer);
                    _dbContext.SaveChanges();
                    result.Data = isExistCustomer.Id;
                }

                else
                {
                    Data.Entities.Profile customer = new Data.Entities.Profile
                    {
                        Username = model.UserName,
                        RelationProfileId = empProfileExisted.Id,
                        Fullname = model.Fullname,
                        Email = model.Email,
                        Gender = model.Gender,
                        PhoneNumber = model.PhoneNumber,
                        Status = 0
                    };
                    _dbContext.Add(customer);
                    _dbContext.SaveChanges();
                    result.Data = customer.Id;
                }

                result.Succeed = true;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel RegisterCustomerIdentification(CustomerIdentification model)
        {
            var result = new ResultModel();
            try
            {
                UserCreateModel user = new UserCreateModel
                {
                    UserName = model.UserName,
                    Password = model.Password,

                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FullName = model.Fullname,
                };
                var rs = _producer.CreateAccount(JsonConvert.SerializeObject(user));
                ResultModel rsMQ = JsonConvert.DeserializeObject<ResultModel>(rs);
                if (rsMQ.Succeed == false)
                {
                    result.ErrorMessage = rsMQ.ErrorMessage;
                    return result;
                }
                var isExistCustomer = _dbContext.Profiles.FirstOrDefault(x => x.Username == model.UserName);
                if (isExistCustomer != null)
                {
                    isExistCustomer.Fullname = model.Fullname;
                    isExistCustomer.PhoneNumber = model.PhoneNumber;
                    isExistCustomer.Gender = model.Gender;
                    isExistCustomer.Email = model.Email;
                    _dbContext.Update(isExistCustomer);
                    _dbContext.SaveChanges();
                    result.Data = isExistCustomer.Id;
                }

                else
                {
                    Data.Entities.Profile customer = new Data.Entities.Profile
                    {
                        Username = model.UserName,
                        Fullname = model.Fullname,
                        Email = model.Email,
                        Gender = model.Gender,
                        PhoneNumber = model.PhoneNumber,
                        Status = 0
                    };
                    _dbContext.Add(customer);
                    _dbContext.SaveChanges();
                    result.Data = customer.Id;
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