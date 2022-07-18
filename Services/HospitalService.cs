using AutoMapper;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Services.RabbitMQ;

namespace Services
{
    public interface IUnitService
    {
        ResultModel Add(UnitCreateModel model, string userName);
        ResultModel Update(UnitUpdateModel model);
        ResultModel Get(Guid? id, string username);
        ResultModel Delete(Guid id);
        ResultModel Remove(Guid id);
        Task<ResultModel> UpdateLogoAsync(HospitalUpdateLogo model);
        Task<ResultModel> AddImagesAsync(List<IFormFile> files, Guid id);
        ResultModel GetAllImage(Guid id);
        ResultModel GetByUsername(string username);
        FileModel GetLogo(Guid fileId);
        ResultModel UpdateInformation(UserInformationModel model, string username);
        FileModel GetUnitImage(Guid id);
        ResultModel GetBySerive(Guid serviceId);
        ResultModel GetBySeriveAndDate(Guid serviceId, DateTime date);
        ResultModel SetTestingFacility(Guid id, SetTestingFacilityModel model);
        ResultModel SetPrEPFacility(Guid id, SetPrEPFacilityModel model);
        ResultModel SetARTFacility(Guid id, SetARTFacilityModel model);
        ResultModel FilterUnit(string username, bool? IsTestingFacility = null,
            bool? IsPrEPFacility = null, bool? IsARTFacility = null, int? pageIndex = 0, int? pageSize = 0);
        ResultModel GetHospitalByDoctor(string username, Guid? id = null);
        ResultModel CreateOrganization(CreateOrganizationModel model, string username);
        ResultModel GetUnit(string username, int? pageIndex = 0, int? pageSize = 0);
        ResultModel GetParentUnit(int? pageIndex = 0, int? pageSize = 0);
        ResultModel RemoveUnit(Guid id);
    }

    public class UnitService : IUnitService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly IUtilService _utilService;
        private readonly IProducer _producer;
        private readonly IProducerDeleteUser _producerDeleteUser;

        public UnitService(IMapper mapper, AppDbContext dbContext, IUtilService utilService, IProducer producer, IProducerDeleteUser producerDeleteUser)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _utilService = utilService;
            _producer = producer;
            _producerDeleteUser = producerDeleteUser;
        }

        public ResultModel GetByUsername(string username)
        {
            var result = new ResultModel();
            try
            {
                var userInformation = _dbContext.UserInformation.FirstOrDefault(_u => _u.Username == username);
                if (userInformation == null)
                {
                    userInformation = new UserInformation() { Username = username };
                    _dbContext.Add(userInformation);
                    _dbContext.SaveChanges();
                }
                result.Data = _mapper.Map<UserInformation, UserInformationViewModel>(userInformation);
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel UpdateInformation(UserInformationModel model, string username)
        {
            var result = new ResultModel();
            try
            {
                var userInformation = _dbContext.UserInformation.FirstOrDefault(_u => _u.Username == username);
                if (userInformation == null)
                {
                    userInformation = new UserInformation() { Username = username };
                    _dbContext.Add(userInformation);
                    _dbContext.SaveChanges();
                }

                userInformation.Name = model.Name;
                userInformation.Address = model.Address;
                userInformation.Province = model.Province;
                userInformation.District = model.District;
                userInformation.Ward = model.Ward;
                userInformation.Website = model.Website;
                userInformation.Phone = model.Phone;
                userInformation.Email = model.Email;
                userInformation.Introduction = model.Introduction;
                userInformation.DateUpdated = DateTime.Now;

                _dbContext.Update(userInformation);
                _dbContext.SaveChanges();

                result.Data = userInformation.Username;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel Add(UnitCreateModel model, string userName)
        {
            ResultModel result = new ResultModel();
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    Unit data = new Unit
                    {
                        IsDeleted = false,
                        Address = model.Address,
                        Website = model.Website,
                        Email = model.Email,
                        Introduction = model.Introduction,
                        Name = model.Name,
                        UnitTypeId = model.UnitTypeId,
                        Phone = model.Phone,
                        Description = "",
                        Ward = model.Ward,
                        Province = model.Province,
                        District = model.District,
                        Username = userName
                    };
                    _dbContext.Add(data);
                    _dbContext.SaveChanges();
                    result.Data = _mapper.Map<Unit, UnitModel>(data);

                    Room virtualRoom = new Room { Name = "Virtual Room", UnitId = data.Id, IsDeleted = false };
                    _dbContext.Add(virtualRoom);
                    _dbContext.SaveChanges();
                    result.Succeed = true;
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                    transaction.Rollback();
                }
            };

            return result;
        }
        public ResultModel Update(UnitUpdateModel model)
        {
            ResultModel result = new ResultModel();
            try
            {
                var unit = _dbContext.Units.FirstOrDefault(h => h.Id == model.Id);
                if (unit != null)
                {
                    unit.Address = model.Address;
                    unit.Website = model.Website;
                    unit.Email = model.Email;
                    unit.Introduction = model.Introduction;
                    unit.Name = model.Name;
                    unit.UnitTypeId = model.UnitTypeId;
                    unit.Phone = model.Phone;
                    unit.District = model.District;
                    unit.Ward = model.Ward;
                    unit.Province = model.Province;
                    unit.Username = model.Username;

                    unit.DateUpdated = DateTime.Now;

                    _dbContext.Update(unit);
                    _dbContext.SaveChanges();

                    result.Succeed = true;
                    result.Data = _mapper.Map<Unit, UnitModel>(unit);
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
                var Unit = _dbContext.Units.FirstOrDefault(h => h.Id == id);

                if (Unit != null)
                {
                    Unit.IsDeleted = true;
                    Unit.DateUpdated = DateTime.Now;

                    _dbContext.Update(Unit);
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
                if (id == null)
                {
                    var units = _dbContext.Units.Where(_u => _u.Username == username && _u.IsDeleted == false).ToList();

                    var data = new List<UnitModel>();
                    foreach (var item in units)
                    {
                        UnitModel unit = _mapper.Map<Unit, UnitModel>(item);
                        unit.Logo = item.Logo;
                        data.Add(unit);
                    }
                    result.Data = data;
                }
                else
                {
                    var unit = _dbContext.Units.FirstOrDefault(h => h.Id == id && h.IsDeleted == false && h.Username == username);
                    var data = _mapper.Map<Unit, UnitModel>(unit);
                    data.Logo = unit.Logo;
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
        public async Task<ResultModel> UpdateLogoAsync(HospitalUpdateLogo model)
        {
            ResultModel result = new ResultModel() { Succeed = true };
            try
            {
                Unit unit = _dbContext.Units.FirstOrDefault(u => u.Id == model.Id);

                if (unit == null)
                {
                    return result = new ResultModel()
                    {
                        Succeed = false,
                        ErrorMessage = "Invalid Unit"
                    };
                }
                Image image = null;
                try
                {
                    image = await ResizeImageAsync(model.Picture);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                unit.Logo = _utilService.ImageToBytes(image);

                _dbContext.Units.Update(unit);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.Succeed = false;
            }
            return result;
        }
        public async Task<ResultModel> AddImagesAsync(List<IFormFile> files, Guid id)
        {
            ResultModel result = new ResultModel() { Succeed = true };
            try
            {
                var unit = _dbContext.Units.FirstOrDefault(u => u.Id == id);

                if (unit == null) throw new Exception("Invalid Unit");
                ConcurrentQueue<UnitImage> unitImages = new ConcurrentQueue<UnitImage>();

                foreach (var image in files)
                {
                    var resize = await _utilService.ResizeImageAsync(image);
                    var img = _utilService.ImageToBytes(resize);
                    UnitImage unitImage = new UnitImage() { UnitId = unit.Id, IsDeleted = false, Image = img };
                    unitImages.Enqueue(unitImage);
                }

                #region Cannot access a closed file
                //Parallel.ForEach(Partitioner.Create(files), async image =>
                //{
                //    var resize = await _utilService.ResizeImageAsync(image);
                //    var img = _utilService.ImageToBytes(resize);
                //    UnitImage unitImage = new UnitImage() { UnitId = unit.Id, IsDeleted = false, Image = img};
                //    unitImages.Enqueue(unitImage);
                //});
                #endregion

                await _dbContext.AddRangeAsync(unitImages);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
                result.Succeed = false;
            }
            return result;
        }
        public ResultModel GetAllImage(Guid id)
        {
            ResultModel result = new ResultModel();
            try
            {
                var unitImages = _dbContext.UnitImages.Where(h => h.UnitId == id && h.IsDeleted == false).ToList();

                if (unitImages != null)
                {
                    var data = new List<Guid>();
                    foreach (var unitImage in unitImages)
                    {
                        data.Add(unitImage.Id);
                    }
                    result.Data = data;
                    result.Succeed = true;
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel Remove(Guid id)
        {
            ResultModel result = new ResultModel();

            try
            {
                Unit unit = _dbContext.Units.FirstOrDefault(u => u.Id == id);
                _dbContext.Remove(unit);
                _dbContext.SaveChanges();

                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.Succeed = false;
                result.ErrorMessage = e.Message;
            }

            return result;
        }
        public FileModel GetLogo(Guid unitId)
        {
            var result = new FileModel();
            var unit = _dbContext.Units.FirstOrDefault(e => e.Id == unitId);

            if (unit == null)
            {
                throw new Exception("Invalid Id");
            }

            if (unit.Logo == null)
            {
                return null;
            }
            result.Id = unit.Id;
            result.Data = unit.Logo;
            result.FileType = "image/jpeg";

            return result;
        }
        public FileModel GetUnitImage(Guid id)
        {
            var result = new FileModel();
            var unit = _dbContext.UnitImages.FirstOrDefault(e => e.Id == id);

            if (unit == null)
            {
                throw new Exception("Invalid Id");
            }

            if (unit.Image == null)
            {
                return null;
            }
            result.Id = unit.Id;
            result.Data = unit.Image;
            result.FileType = "image/jpeg";

            return result;
        }
        public async Task<Image> ResizeImageAsync(IFormFile image)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    try
                    {
                        await image.CopyToAsync(memoryStream);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Copy Faild");
                    }

                    try
                    {
                        using (var img = Image.FromStream(memoryStream))
                        {

                            int x, y, w, h;

                            //HD Size
                            int desWidth = 1280;
                            int desHeight = 720;

                            // Vertical
                            if (img.Height > img.Width)
                            {
                                w = (img.Width * desHeight) / img.Height;
                                h = desHeight;
                                x = (desWidth - w) / 2;
                                y = 0;
                            }
                            else
                            {
                                //Horizontal
                                w = desWidth;
                                h = (img.Height * desWidth) / img.Width;
                                x = 0;
                                y = (desHeight - h) / 2;
                            }

                            var bmp = new Bitmap(desWidth, desHeight);
                            try
                            {

                                using (Graphics g = Graphics.FromImage(bmp))
                                {
                                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    g.DrawImage(img, x, y, w, h);
                                }
                            }
                            catch (Exception)
                            {
                                throw new Exception("Bitmap faild");
                            }
                            return bmp;
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("AAAAAAAAAAAAAAAAAAAAAAAAAA");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public ResultModel GetBySerive(Guid serviceId)
        {
            var result = new ResultModel();
            try
            {
                //var unitServices = _dbContext.ServiceUnits.Include(_s => _s.Unit).Where(_s => _s.ServiceId == serviceId).ToList();

                var workingCalendarServices = _dbContext.ServiceWorkingCalendars
                    .Include(_s => _s.WorkingCalendar)
                    .Where(_s => _s.ServiceId == serviceId)
                    .Where(_s => _s.WorkingCalendar.Status == Data.Constants.WorkingCalendarStatus.POSTED)
                    .Where(_s => DateTime.Compare(_s.WorkingCalendar.ToDate.Date, DateTime.Now.Date) >= 0)
                    .ToList();

                var data = new List<UnitModel>();
                foreach (var workingCalendarService in workingCalendarServices)
                {
                    var unit = _dbContext.Units.Where(_u => _u.Id == workingCalendarService.WorkingCalendar.UnitId).ToList();
                    data.AddRange(_mapper.Map<List<Unit>, List<UnitModel>>(unit));
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

        public ResultModel GetBySeriveAndDate(Guid serviceId, DateTime date)
        {
            var result = new ResultModel();
            try
            {
                var listWorkingCalendarsId = _dbContext.Days.Where(d => d.Date == date.Date).Select(x => x.CalendarId).ToList();

                var workingCalendarServices = _dbContext.ServiceWorkingCalendars
                    .Include(_s => _s.WorkingCalendar)
                    .Where(_s => _s.ServiceId == serviceId)
                    .Where(_s => listWorkingCalendarsId.Contains(_s.WorkingCalendarId))
                    .Where(_s => _s.WorkingCalendar.Status == Data.Constants.WorkingCalendarStatus.POSTED)
                    .Where(_s => DateTime.Compare(_s.WorkingCalendar.ToDate.Date, DateTime.Now.Date) >= 0)
                    .ToList();

                var data = new List<UnitModel>();
                foreach (var workingCalendarService in workingCalendarServices)
                {
                    var unit = _dbContext.Units.Where(_u => _u.Id == workingCalendarService.WorkingCalendar.UnitId).ToList();
                    data.AddRange(_mapper.Map<List<Unit>, List<UnitModel>>(unit));
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


        public ResultModel SetTestingFacility(Guid id, SetTestingFacilityModel model)
        {
            var result = new ResultModel();

            try
            {
                var unit = _dbContext.Units.FirstOrDefault(h => h.Id == id);
                if (unit != null)
                {
                    unit.IsTestingFacility = model.IsTestingFacility;
                    unit.DateUpdated = DateTime.Now;

                    _dbContext.Update(unit);
                    _dbContext.SaveChanges();

                    result.Succeed = true;
                    result.Data = _mapper.Map<Unit, UnitModel>(unit);
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public ResultModel SetPrEPFacility(Guid id, SetPrEPFacilityModel model)
        {
            var result = new ResultModel();

            try
            {
                var unit = _dbContext.Units.FirstOrDefault(h => h.Id == id);
                if (unit != null)
                {
                    unit.IsPrEPFacility = model.IsPrEPFacility;
                    unit.DateUpdated = DateTime.Now;

                    _dbContext.Update(unit);
                    _dbContext.SaveChanges();

                    result.Succeed = true;
                    result.Data = _mapper.Map<Unit, UnitModel>(unit);
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public ResultModel SetARTFacility(Guid id, SetARTFacilityModel model)
        {
            var result = new ResultModel();

            try
            {
                var unit = _dbContext.Units.FirstOrDefault(h => h.Id == id);
                if (unit != null)
                {
                    unit.IsARTFacility = model.IsARTFacility;
                    unit.DateUpdated = DateTime.Now;

                    _dbContext.Update(unit);
                    _dbContext.SaveChanges();

                    result.Succeed = true;
                    result.Data = _mapper.Map<Unit, UnitModel>(unit);
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }


        public ResultModel FilterUnit(string username, bool? IsTestingFacility = null, bool? IsPrEPFacility = null, bool? IsARTFacility = null, int? pageIndex = 0, int? pageSize = 0)
        {
            var result = new ResultModel();

            try
            {
                var units = _dbContext.Units.Where(_u => _u.IsDeleted == false);

                if (IsTestingFacility.HasValue) units = units.Where(x => x.IsTestingFacility == IsTestingFacility);
                if (IsPrEPFacility.HasValue) units = units.Where(x => x.IsPrEPFacility == IsPrEPFacility);
                if (IsARTFacility.HasValue) units = units.Where(x => x.IsARTFacility == IsARTFacility);

                PagingModel paging = new PagingModel(pageIndex ?? 0, pageSize ?? 0, units.Count());


                var listUnit = units
                    .Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Take(paging.PageSize)
                    .ToList();

                var data = new List<UnitModel>();
                foreach (var item in listUnit)
                {
                    UnitModel unit = _mapper.Map<Unit, UnitModel>(item);
                    unit.Logo = item.Logo;
                    data.Add(unit);
                }


                paging.Data = data;
                result.Succeed = true;
                result.Data = paging;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }


        public ResultModel GetHospitalByDoctor(string username, Guid? id = null)
        {
            var result = new ResultModel();

            try
            {
                var doctorId = _dbContext.Doctors.FirstOrDefault(x => x.UserId == id.ToString()).Id;
                var unitId = _dbContext.UnitDoctors.FirstOrDefault(x => x.DoctorId == doctorId).UnitId;
                if (!unitId.HasValue)
                {
                    throw new Exception("Empty hospital");
                }
                var unit = _dbContext.Units.FirstOrDefault(x => x.Id == unitId);
                result.Data = _mapper.Map<Unit, UnitModel>(unit);
                result.Succeed = true;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }


        public ResultModel CreateOrganization(CreateOrganizationModel model, string username)
        {
            var result = new ResultModel();
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // if username != null => create child unit, else => create parentUnit
                    var data = _mapper.Map<CreateOrganizationModel, Unit>(model);
                    if (!string.IsNullOrEmpty(username))
                    {
                        var unitParent = _dbContext.Units.FirstOrDefault(x => x.Username == username && x.ParentId == null && x.IsDeleted == false);
                        if (unitParent == null)
                        {
                            result.ErrorMessage ="Unit does not exist with Username or Username belong to childUnit";
                            return result;
                        }
                        data.ParentId = unitParent.Id;

                        // if username,password == null ==> assign username unit parent to unitchild
                        if (string.IsNullOrEmpty(model.Username) && string.IsNullOrEmpty(model.Password))
                        {
                            data.Username = unitParent.Username;
                        }
                    }

                    _dbContext.Units.Add(data);
                    _dbContext.SaveChanges();


                    // create vitual room
                    Room virtualRoom = new Room { Name = "Virtual Room", UnitId = data.Id, IsDeleted = false };
                    _dbContext.Add(virtualRoom);
                    _dbContext.SaveChanges();

                    // create account
                    if (string.IsNullOrEmpty(username) ||
                        (!string.IsNullOrEmpty(model.Username) || !string.IsNullOrEmpty(model.Password)))
                    {
                        UserCreateModel user = new UserCreateModel
                        {
                            UserName = model.Username,
                            Password = model.Password,
                            OnlyUsername = true,
                            GroupName = "FACILITY"
                        };

                        var rs = _producer.CreateAccount(JsonConvert.SerializeObject(user));
                        ResultModel rsMQ = JsonConvert.DeserializeObject<ResultModel>(rs);

                        if (rsMQ.Succeed == false)
                        {
                            result.ErrorMessage = rsMQ.ErrorMessage;
                            return result;
                        }
                    }

                    result.Data = _mapper.Map<Unit, UnitModel>(data);
                    result.Succeed = true;

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                    transaction.Rollback();
                }
            }

            ;

            return result;
        }
        public ResultModel GetUnit(string username, int? pageIndex = 0, int? pageSize = 0)
        {
            var result = new ResultModel();

            try
            {
                PagingModel paging = null;
                List<Unit> listUnit = null;

                var unitParent = _dbContext.Units
                    .FirstOrDefault(_u => _u.IsDeleted == false && (_u.Username == username && _u.ParentId == null));

                if (unitParent != null)
                {
                    var units = _dbContext.Units.Where(_u => _u.IsDeleted == false && _u.ParentId == unitParent.Id);
                    paging = new PagingModel(pageIndex ?? 0, pageSize ?? 0, units.Count());
                    listUnit = units
                        //                        .Skip((paging.PageIndex - 1) * paging.PageSize)
                        //                        .Take(paging.PageSize)
                        .ToList();
                    listUnit.Add(unitParent);
                }
                else
                {
                    var units = _dbContext.Units.Where(_u => _u.IsDeleted == false && (_u.Username == username && _u.ParentId != null));
                    paging = new PagingModel(pageIndex ?? 0, pageSize ?? 0, units.Count());
                    listUnit = units
                        //                        .Skip((paging.PageIndex - 1) * paging.PageSize)
                        //                        .Take(paging.PageSize)
                        .ToList();
                }

                var data = new List<UnitModel>();
                foreach (var item in listUnit)
                {
                    UnitModel unit = _mapper.Map<Unit, UnitModel>(item);
                    unit.Logo = item.Logo;
                    data.Add(unit);
                }

                paging.Data = data;
                result.Succeed = true;
                //                result.Data = paging;
                result.Data = data;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel GetParentUnit(int? pageIndex = 0, int? pageSize = 0)
        {
            var result = new ResultModel();

            try
            {
                var units = _dbContext.Units.Where(x => x.ParentId == null && x.IsDeleted == false);

                PagingModel paging = new PagingModel(pageIndex ?? 0, pageSize ?? 0, units.Count());

                var listUnit = units
                    .Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Take(paging.PageSize)
                    .ToList();

                var data = new List<UnitModel>();
                foreach (var item in listUnit)
                {
                    UnitModel unit = _mapper.Map<Unit, UnitModel>(item);
                    unit.Logo = item.Logo;
                    data.Add(unit);
                }

                paging.Data = data;
                result.Succeed = true;
                result.Data = paging;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }


        public ResultModel RemoveUnit(Guid id)
        {
            ResultModel result = new ResultModel();
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var listUser = new List<string>();
                    var fUnit = _dbContext.Units.FirstOrDefault(h => h.Id == id && h.IsDeleted == false);
                    if (fUnit == null)
                    {
                        throw new Exception("UnitId is not exist");
                    }

                    if (fUnit.ParentId == null)
                    {
                        var childUnits = _dbContext.Units
                            .Where(x => x.ParentId == fUnit.Id && x.IsDeleted == false)
                            .ToList();

                        foreach (var cUnit in childUnits)
                        {
                            cUnit.IsDeleted = true;
                            cUnit.DateUpdated = DateTime.Now;
                            if (cUnit.Username != fUnit.Username)
                            {
                                listUser.Add(cUnit.Username);
                            }
                            _dbContext.Update(cUnit);
                        }
                        listUser.Add(fUnit.Username);
                    }
                    else
                    {
                        var parentUnit = _dbContext.Units.FirstOrDefault(x => x.Username == fUnit.Username && x.ParentId == null);
                        if(parentUnit == null) listUser.Add(fUnit.Username);
                    }


                    fUnit.IsDeleted = true;
                    fUnit.DateUpdated = DateTime.Now;
                    _dbContext.Update(fUnit);
                    _dbContext.SaveChanges();


                    var rs = _producerDeleteUser.DeleteUser(JsonConvert.SerializeObject(listUser));
                    ResultModel rsMQ = JsonConvert.DeserializeObject<ResultModel>(rs);
                    if (rsMQ.Succeed == false)
                    {
                        throw new Exception("Can not delete account");
                    }

                    result.Data = fUnit.Id;
                    result.Succeed = true;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                    transaction.Rollback();
                }
            }

            return result;
        }

    }

}
