using AutoMapper;
using Data.Constants;
using Data.DbContexts;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public interface IWorkingCalendarService
    {
        Task<ResultModel> AddAsync(WorkingCalendarCreateModel model, string username);
        ResultModel PublishCalendar(List<Guid> ids);
        Task<ResultModel> CancelCalendar(List<Guid> ids);
        ResultModel Get(Guid unitId);
        ResultModel GetByDoctorId(string userId,DateTime formDate,DateTime toDate);
        ResultModel GetCalendarDetail(Guid workingCalendarId);
        ResultModel GetIntervels(Guid dayId);
        ResultModel PublishDay(List<Guid> ids);
        ResultModel CancelDay(List<Guid> ids);
        ResultModel PublishInterval(List<Guid> ids);
        ResultModel CancelInterval(List<Guid> ids);
        ResultModel GetByUnitAndService(Guid unitId, Guid serviceId);
        ResultModel GetDaysByUnitAndService(Guid unitId, Guid serviceId);
        ResultModel CheckScheduledDoctor(Guid doctorId, DateTime fromDate, DateTime toDate);
        ResultModel DeleteWorkingCalendar(Guid workingCalendarId);
        ResultModel GetIntervalByListDayId(List<Guid> dayIds);
        ResultModel GetFullDaysByUnitAndService(Guid unitId, Guid serviceId);
        ResultModel GetStaffScheduleByDoctorId(string userId, DateTime fromDate, DateTime toDate);
        ResultModel GetIntervals(Guid dayId);
        ResultModel GetAvailableIntervals(Guid dayId);
    }

    public class WorkingCalendarService : IWorkingCalendarService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly IElasticClient _elasticClient;

        public const string IndexName = "schedule";

        public WorkingCalendarService(IMapper mapper, AppDbContext dbContext, IElasticClient elasticClient)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _elasticClient = elasticClient;
        }

        public async Task<ResultModel> AddAsync(WorkingCalendarCreateModel model, string username)
        {
            var result = new ResultModel();
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                if (model.ShiftCount < 1)
                {
                    throw new Exception("Invalid ShiftCount");
                }
                Unit unit = _dbContext.Units.FirstOrDefault(_u =>_u.Id == model.UnitId && _u.IsDeleted ==false);
                if (unit == null)
                {
                    throw new Exception("Invalid User");
                }

                WorkingCalendar workingCalendar = new WorkingCalendar()
                {
                    Id = Guid.NewGuid(),
                    Description = model.Name,
                    IsDeleted = false,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Status = WorkingCalendarStatus.NOT_POST,
                    UnitId = unit.Id,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ShiftCount = model.ShiftCount
                };

                workingCalendar.BookingBeforeDate = (model.BookingBefore);
                workingCalendar.BookingAfterDate = (model.BookingAfter);

                _dbContext.Add(workingCalendar);
                _dbContext.SaveChanges();

                var serviceWorkingCalendars = new ConcurrentQueue<ServiceWorkingCalendar>();

                Parallel.ForEach(Partitioner.Create(model.Services), (service) =>
                {
                    serviceWorkingCalendars.Enqueue(new ServiceWorkingCalendar()
                    {
                        WorkingCalendarId = workingCalendar.Id,
                        ServiceId = service
                    });
                });

                _dbContext.AddRange(serviceWorkingCalendars);
                await _dbContext.SaveChangesAsync();

                foreach (var doctorRoom in model.DoctorRooms)
                {
                    var roomWorkingCalendars = new ConcurrentQueue<RoomWorkingCalendar>();
                    var doctorWorkingCalendars = new ConcurrentQueue<DoctorWorkingCalendar>();

                    roomWorkingCalendars.Enqueue(new RoomWorkingCalendar()
                    {
                        WorkingCalendarId = workingCalendar.Id,
                        RoomId = doctorRoom.RoomId
                    });

                    doctorWorkingCalendars.Enqueue(new DoctorWorkingCalendar()
                    {
                        WorkingCalendarId = workingCalendar.Id,
                        DoctorId = doctorRoom.DoctorId
                    });

                    _dbContext.AddRange(roomWorkingCalendars);
                    await _dbContext.SaveChangesAsync();

                    _dbContext.AddRange(doctorWorkingCalendars);
                    await _dbContext.SaveChangesAsync();
                }
                foreach (var dayCreateModel in model.DayCreateModels)
                {
                    var days = new List<Data.Entities.Day>();
                    var schedules = new ConcurrentQueue<Schedule>();
                    var intervals = new ConcurrentQueue<Data.Entities.Interval>();

                    var dayAddModels = new List<DayAddModel>();

                    foreach (var day in dayCreateModel.Date)
                    {
                        dayAddModels.Add(new DayAddModel()
                        {
                            Date = day.Date,
                            Schedules = new List<ScheduleCreateModel>() { new ScheduleCreateModel() { To = dayCreateModel.To, From = dayCreateModel.From } }
                        });
                    }

                    foreach (var dayAddModel in dayAddModels)
                    {
                        var dayCreate = (Data.Entities.Day)CreateDay(dayAddModel, workingCalendar.Id).Data;
                        days.Add(dayCreate);
                        days = days.DistinctBy(_d => _d.Id).ToList();

                        foreach (var schedule in dayAddModel.Schedules)
                        {
                            schedules.Enqueue((Schedule)CreateSchedule(schedule, dayCreate.Id).Data);
                        };
                    };

                    foreach (var schedule in schedules)
                    {
                        var intervalsCreate = (ConcurrentQueue<Data.Entities.Interval>)CreateIntervel(schedule, model.Interval,model.ShiftCount).Data;
                        foreach (var item in intervalsCreate)
                        {
                            intervals.Enqueue(item);
                        }
                    };

                    //_dbContext.AddRange(days);
                    //await _dbContext.SaveChangesAsync();
                    _dbContext.AddRange(schedules);
                    await _dbContext.SaveChangesAsync();

                    int pageIndex = 1;
                    int pageSize = 100;
                    bool end = false;
                    do
                    {
                        if ((intervals.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()).Count < pageSize)
                        {
                            end = true;
                        }
                        _dbContext.AddRange(intervals.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
                        await _dbContext.SaveChangesAsync();

                        pageIndex++;
                    } while (end == false);
                }

                transaction.Commit();

                result.Data = workingCalendar.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.Succeed = false;
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
            }
            return result;
        }
        private ResultModel CreateDay(DayAddModel model, Guid workingCalendarId)
        {
            ResultModel result = new ResultModel();
            try
            {
                var day = _dbContext.Days.FirstOrDefault(_d => _d.CalendarId == workingCalendarId && _d.Date == model.Date);
                if (day == null)
                {
                    day = new Data.Entities.Day()
                    {
                        Id = Guid.NewGuid(),
                        CalendarId = workingCalendarId,
                        IsDeleted = false,
                        Date = model.Date,
                        Description = "",
                        DateUpdated = DateTime.Now,
                        DateCreated = DateTime.Now
                    };
                    _dbContext.Add(day);
                }

                result.Data = day;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.Succeed = false;
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        private ResultModel CreateSchedule(ScheduleCreateModel model, Guid dateId)
        {
            var result = new ResultModel();
            try
            {
                Schedule schedule = new Schedule()
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                    Description = "",
                    DateUpdated = DateTime.Now,
                    DateCreated = DateTime.Now,
                    From = model.From,
                    To = model.To,
                    DayId = dateId,
                };
                result.Data = schedule;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.Succeed = false;
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        private ResultModel CreateIntervel(Schedule schedule, int step,int availableQuantity)
        {
            ResultModel result = new ResultModel();
            try
            {
                TimeSpan from = new TimeSpan(int.Parse(schedule.From.Split(":")[0]), int.Parse(schedule.From.Split(":")[1]), 0);
                TimeSpan to = new TimeSpan(int.Parse(schedule.To.Split(":")[0]), int.Parse(schedule.To.Split(":")[1]), 0);

                if (TimeSpan.Compare(from, to) > 0)
                {
                    result.Succeed = false;
                    result.ErrorMessage = "Invalid Schedule";
                    return result;
                }

                TimeSpan timeStep = new TimeSpan(0, step, 0);

                var intervals = new ConcurrentQueue<Data.Entities.Interval>();
                int i = 1;
                while (TimeSpan.Compare(from, to) < 0)
                {
                    TimeSpan start = new TimeSpan(from.Hours, from.Minutes, 0);

                    from = from.Add(timeStep);

                    if (TimeSpan.Compare(from, to) > 0)
                    {
                        from = to;
                    }

                    Data.Entities.Interval interval = new Data.Entities.Interval()
                    {
                        Id = Guid.NewGuid(),
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        Description = "",
                        From = string.Format("{0:00}", start.Hours) + ":" + string.Format("{0:00}", start.Minutes),
                        IsAvailable = true,
                        IsDeleted = false,
                        ScheduleId = schedule.Id,
                        To = string.Format("{0:00}", from.Hours) + ":" + string.Format("{0:00}", from.Minutes),
                        NumId = i,
                        AvailableQuantity = availableQuantity
                    };
                    intervals.Enqueue(interval);
                    i++;
                }
                result.Succeed = true;
                result.Data = intervals;
            }
            catch (Exception e)
            {
                result.Succeed = false;
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel PublishCalendar(List<Guid> ids)
        {
            ResultModel result = new ResultModel();

            try
            {
                foreach (var id in ids)
                {
                    var calendar = _dbContext.WorkingCalendars.FirstOrDefault(wc => wc.Id == id && wc.IsDeleted == false);

                    if (calendar == null)
                    {
                        result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                        return result;
                    }
                    if (calendar.Status.Equals(WorkingCalendarStatus.POSTED))
                    {
                        result.ErrorMessage = ErrorMessages.CALENDAR_POSTED;
                    }

                    calendar.Status = WorkingCalendarStatus.POSTED;
                    _dbContext.WorkingCalendars.Update(calendar);
                    _dbContext.SaveChanges();

                    //var calendarVM = _mapper.Map<WorkingCalendar, WorkingCalendarViewModel>(calendar);

                    //ServiceWorkingCalendar serviceWorkingCalendar = _dbContext.ServiceWorkingCalendars.FirstOrDefault(s => s.WorkingCalendarId == calendar.Id);
                    //RoomWorkingCalendar roomWorkingCalendar = _dbContext.RoomWorkingCalendars.FirstOrDefault(s => s.WorkingCalendarId == calendar.Id);
                    //DoctorWorkingCalendar doctorWorkingCalendar = _dbContext.DoctorWorkingCalendars.FirstOrDefault(s => s.WorkingCalendarId == calendar.Id);

                    //var a = await _elasticClient.IndexDocumentAsync(calendarVM);
                    //if (a.IsValid)
                    //{

                    //}

                    var days = _dbContext.Days.Where(_d => _d.CalendarId == id).Select(_d => _d.Id).ToList();
                    PublishDay(days);
                }
                result.Data = "Ok";
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public async Task<ResultModel> CancelCalendar(List<Guid> ids)
        {
            ResultModel result = new ResultModel();

            try
            {
                foreach (var id in ids)
                {
                    WorkingCalendar calendar = _dbContext.WorkingCalendars.FirstOrDefault(wc => wc.Id == id && wc.IsDeleted == false);

                    if (calendar == null)
                    {
                        result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                        return result;
                    }
                    if (calendar.Status.Equals(WorkingCalendarStatus.CANCEL_POST))
                    {
                        result.ErrorMessage = ErrorMessages.CALENDAR_CANCELED;
                    }

                    calendar.Status = WorkingCalendarStatus.CANCEL_POST;
                    _dbContext.WorkingCalendars.Update(calendar);
                    _dbContext.SaveChanges();

                    var calendarVM = _mapper.Map<WorkingCalendar, WorkingCalendarViewModel>(calendar);
                    await _elasticClient.UpdateAsync<WorkingCalendarViewModel>(calendarVM, w => w.Doc(calendarVM));

                    var days = _dbContext.Days.Where(_d => _d.CalendarId == id).Select(_d => _d.Id).ToList();
                    CancelDay(days);
                }
                result.Data = "Ok";
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        public ResultModel Get(Guid unitId)
        {
            var result = new ResultModel();
            try
            {
                List<WorkingCalendar> workingCalendars = _dbContext.WorkingCalendars.Where(_w => _w.UnitId == unitId && _w.IsDeleted == false).OrderByDescending(_w => _w.DateCreated).ToList();
                List<WorkingCalendarViewModel> data = new List<WorkingCalendarViewModel>();

                foreach (var workingCalendar in workingCalendars)
                {
                    var day = _dbContext.Days.FirstOrDefault(_d => _d.CalendarId == workingCalendar.Id);
                    if (day != null)
                    {
                        var shedules = _dbContext.Schedules.Where(_d => _d.DayId == day.Id).ToList();

                        var dataItem = _mapper.Map<WorkingCalendar, WorkingCalendarViewModel>(workingCalendar);
                        foreach (var shedule in shedules)
                        {
                            if (!string.IsNullOrEmpty(dataItem.FromTo))
                            {
                                dataItem.FromTo += " & ";
                            }
                            dataItem.FromTo += shedule.From + " - " + shedule.To;
                        }

                        dataItem.Doctor = _mapper.Map<Doctor, DoctorModel>(workingCalendar.DoctorCalendars.FirstOrDefault().Doctor);
                        dataItem.Room = _mapper.Map<Room, RoomModel>(workingCalendar.RoomCalendars.FirstOrDefault().Room);

                        data.Add(dataItem);
                    }

                }

                result.Data = data;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel GetByDoctorId(string userId, DateTime fromDate, DateTime toDate)
        {
            var result = new ResultModel();

            try
            {
                var avalibleUnit = _dbContext.Units.Where(u => u.IsDeleted == false).Select(s => s.Id).ToList();
                var getDoctorIdByUserId = _dbContext.Doctors.FirstOrDefault(d => d.UserId == userId).Id;

                var workingCalendars = _dbContext.DoctorWorkingCalendars
                    .Include(d => d.WorkingCalendar)
                    .Where(d => d.DoctorId.ToString() == getDoctorIdByUserId.ToString())
                    .Select(x => x.WorkingCalendar)
                    .Where(w => w.IsDeleted == false)
                    .Where(w => avalibleUnit.Contains(w.UnitId))
                    .Where(w => w.FromDate.Date >= fromDate.Date && w.ToDate.Date <= toDate.Date)
                    .OrderByDescending(w => w.DateCreated)
                    .ToList();

                List<WorkingCalendarViewModel> data = new List<WorkingCalendarViewModel>();


                foreach (var workingCalendar in workingCalendars)
                {
                    var day = _dbContext.Days.FirstOrDefault(_d => _d.CalendarId == workingCalendar.Id);
                    if (day != null)
                    {
                        var shedules = _dbContext.Schedules.Where(_d => _d.DayId == day.Id).ToList();

                        var dataItem = _mapper.Map<WorkingCalendar, WorkingCalendarViewModel>(workingCalendar);
                        foreach (var shedule in shedules)
                        {
                            if (!string.IsNullOrEmpty(dataItem.FromTo))
                            {
                                dataItem.FromTo += " & ";
                            }
                            dataItem.FromTo += shedule.From + " - " + shedule.To;
                        }

                        dataItem.Doctor = _mapper.Map<Doctor, DoctorModel>(workingCalendar.DoctorCalendars.FirstOrDefault().Doctor);
                        dataItem.Room = _mapper.Map<Room, RoomModel>(workingCalendar.RoomCalendars.FirstOrDefault().Room);

                        data.Add(dataItem);
                    }

                }
                result.Data = data;
                result.Succeed = true;

            }
            catch(Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }

        public ResultModel GetStaffScheduleByDoctorId(string userId, DateTime fromDate, DateTime toDate)
        {
            var result = new ResultModel();

            try
            {
                var avalibleUnit = _dbContext.Units.Where(u => u.IsDeleted == false).Select(s => s.Id).ToList();
                var getDoctorIdByUserId = _dbContext.Doctors.FirstOrDefault(d => d.UserId == userId).Id;

                var workingCalendars = _dbContext.DoctorWorkingCalendars
                    .Include(d => d.WorkingCalendar)
                    .Where(d => d.DoctorId == getDoctorIdByUserId)
                    .Select(x => x.WorkingCalendar)
                    .Where(w => w.IsDeleted == false)
                    .Where(w => avalibleUnit.Contains(w.UnitId))
                    .Where(w => (w.FromDate.Date >= fromDate.Date && w.FromDate.Date <= toDate.Date)||
                                (w.ToDate.Date >= fromDate.Date && w.ToDate.Date <= toDate.Date) ||
                                (w.FromDate.Date <= fromDate.Date &&w.ToDate.Date >= toDate.Date))
                    .ToList();
                List<DateDetailViewModel> data = new List<DateDetailViewModel>();

                foreach (var workingCalendar in workingCalendars)
                {
                    var days = _dbContext.Days
                        .Where(_d => _d.CalendarId == workingCalendar.Id)
                        .Where(_d => _d.Date >=fromDate.Date && _d.Date<=toDate)
                        .OrderBy(_d => _d.DateCreated).ToList();
                    List<DateDetailViewModel> datatmp = new List<DateDetailViewModel>();
                    foreach (var day in days)
                    {
                        var shedules = _dbContext.Schedules.Where(_d => _d.DayId == day.Id).ToList();

                        var doctor = _dbContext.DoctorWorkingCalendars.Include(_s => _s.Doctor)
                            .FirstOrDefault(s => s.WorkingCalendarId == workingCalendar.Id);

                        var room = _dbContext.RoomWorkingCalendars.Include(_s => _s.Room)
                            .FirstOrDefault(s => s.WorkingCalendarId == workingCalendar.Id);

                        var services = _dbContext.ServiceWorkingCalendars.Include(_s => _s.Service)
                            .Where(s => s.WorkingCalendarId == workingCalendar.Id).ToList();

                        var hospitalName = _dbContext.Units.FirstOrDefault(x => x.Id == workingCalendar.UnitId);

                        var serviceGet = new List<BaseModel>();
                        foreach (var service in services)
                        {
                            serviceGet.Add(new BaseModel() { Id = service.Service.Id, Description = service.Service.Name });
                        }

                        foreach (var shedule in shedules)
                        {
                            var sInterval = _dbContext.Intervals.FirstOrDefault(_i => _i.ScheduleId == shedule.Id);

                            var fromArray = sInterval.From.Split(":");
                            TimeSpan fromTime = new TimeSpan(int.Parse(fromArray[0]), int.Parse(fromArray[1]), 0);

                            var toArray = sInterval.To.Split(":");
                            TimeSpan toTime = new TimeSpan(int.Parse(toArray[0]), int.Parse(toArray[1]), 0);

                            var time = toTime.Subtract(fromTime);

                            datatmp.Add(new DateDetailViewModel()
                            {
                                Id = day.Id,
                                Date = day.Date,
                                Doctor = new BaseModel() { Id = doctor.Doctor.Id, Description = doctor.Doctor.FullName },
                                Room = new BaseModel() { Id = room.Room.Id, Description = room.Room.Name },
                                Service = serviceGet,
                                Time = time.TotalMinutes.ToString(),
                                Schedules = new FromToModel() { From = shedule.From, To = shedule.To },
                                Status = day.Status,
                                HospitalName = hospitalName != null?hospitalName.Name:""
                            });
                        }
                    }
                    data.AddRange(datatmp);
                }

                result.Data = data.OrderBy(x => x.Date).ToList();
                result.Succeed = true;

            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public ResultModel GetCalendarDetail(Guid workingCalendarId)
        {
            var result = new ResultModel();
            try
            {
                WorkingCalendar workingCalendar = _dbContext.WorkingCalendars.FirstOrDefault(_w => _w.Id == workingCalendarId);

                var days = _dbContext.Days.Where(_d => _d.CalendarId == workingCalendar.Id).OrderBy(_d => _d.DateCreated).ToList();

                List<DateDetailViewModel> data = new List<DateDetailViewModel>();

                foreach (var day in days)
                {
                    var shedules = _dbContext.Schedules.Where(_d => _d.DayId == day.Id).ToList();

                    var doctor = _dbContext.DoctorWorkingCalendars.Include(_s => _s.Doctor)
                        .FirstOrDefault(s => s.WorkingCalendarId == workingCalendar.Id);

                    var room = _dbContext.RoomWorkingCalendars.Include(_s => _s.Room)
                        .FirstOrDefault(s => s.WorkingCalendarId == workingCalendar.Id);

                    var services = _dbContext.ServiceWorkingCalendars.Include(_s => _s.Service)
                        .Where(s => s.WorkingCalendarId == workingCalendar.Id).ToList();

                    var serviceGet = new List<BaseModel>();
                    foreach (var service in services)
                    {
                        serviceGet.Add(new BaseModel() { Id = service.Service.Id, Description = service.Service.Name });
                    }

                    foreach (var shedule in shedules)
                    {
                        var sInterval = _dbContext.Intervals.FirstOrDefault(_i => _i.ScheduleId == shedule.Id);

                        var fromArray = sInterval.From.Split(":");
                        TimeSpan fromTime = new TimeSpan(int.Parse(fromArray[0]), int.Parse(fromArray[1]), 0);

                        var toArray = sInterval.To.Split(":");
                        TimeSpan toTime = new TimeSpan(int.Parse(toArray[0]), int.Parse(toArray[1]), 0);

                        var time = toTime.Subtract(fromTime);

                        data.Add(new DateDetailViewModel()
                        {
                            Id = day.Id,
                            Date = day.Date,
                            Doctor = new BaseModel() { Id = doctor.Doctor.Id, Description = doctor.Doctor.FullName },
                            Room = new BaseModel() { Id = room.Room.Id, Description = room.Room.Name },
                            Service = serviceGet,
                            Time = time.TotalMinutes.ToString(),
                            Schedules = new FromToModel() { From = shedule.From, To = shedule.To },
                            Status = day.Status
                        });
                    }
                }

                result.Data = data.OrderBy(_d => _d.Date).ToList();
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel GetIntervels(Guid dayId)
        {
            var result = new ResultModel();
            try
            {
                var day = _dbContext.Days.Include(d => d.WorkingCalendar).FirstOrDefault(_d => _d.Id == dayId);

                var schedules = _dbContext.Schedules.Where(_s => _s.DayId == dayId).ToList();
                List<Data.Entities.Interval> intervals = new List<Data.Entities.Interval>();
                foreach (var schedule in schedules)
                {
                    intervals.AddRange(_dbContext.Intervals.Where(_i => _i.ScheduleId == schedule.Id).ToList());
                }
                intervals = intervals.OrderBy(_i => _i.DateCreated).ToList();
                var data = _mapper.Map<List<Data.Entities.Interval>, List<IntervalViewModel>>(intervals);

                var intervelShifts = new List<IntervelShiftViewModel>();

                if (day.WorkingCalendar.ShiftCount == 0)
                {
                    throw new Exception("Invalid shiftcount");
                }
                int shiftsCount = data.Count() / day.WorkingCalendar.ShiftCount;
                int modulo = data.Count() % day.WorkingCalendar.ShiftCount;
                if (modulo > 0)
                {
                    shiftsCount++;
                }
                for (int i = 0; i < shiftsCount; i++)
                {
                    var intervelShift = new IntervelShiftViewModel();

                    for (int j = 0; j < day.WorkingCalendar.ShiftCount; j++)
                    {
                        int input = day.WorkingCalendar.ShiftCount * i + j;

                        if (j == 0)
                        {
                            intervelShift.From = data[input].From;
                            intervelShift.Status = data[input].Status;
                        }

                        intervelShift.Intervals.Add(data[input]);
                        if (i == (shiftsCount - 1))
                        {

                            if (j == (modulo - 1))
                            {
                                intervelShift.To = data[input].To;
                                break;
                            }
                        }
                        else
                        {
                            intervelShift.To = data[input].To;
                        }
                    }
                    if (intervelShift.To == null)
                    {
                        intervelShift.To = intervelShift.Intervals.Last().To;
                    }
                    intervelShifts.Add(intervelShift);
                }

                result.Data = intervelShifts;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel GetIntervals(Guid dayId)
        {
            var result = new ResultModel();
            try
            {
                var day = _dbContext.Days.Include(d => d.WorkingCalendar).FirstOrDefault(_d => _d.Id == dayId);

                var schedules = _dbContext.Schedules.Where(_s => _s.DayId == dayId).ToList();
                List<Data.Entities.Interval> intervals = new List<Data.Entities.Interval>();
                foreach (var schedule in schedules)
                {
                    intervals.AddRange(_dbContext.Intervals.Where(_i => _i.ScheduleId == schedule.Id).ToList());
                }
                intervals = intervals.OrderBy(_i => _i.DateCreated).ToList();
                var data = _mapper.Map<List<Data.Entities.Interval>, List<IntervalViewModel>>(intervals);
                if (day.WorkingCalendar.ShiftCount == 0)
                {
                    throw new Exception("Invalid shiftcount");
                }
                result.Data = data;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }

        public ResultModel GetAvailableIntervals(Guid dayId)
        {
            var result = new ResultModel();
            try
            {
                var day = _dbContext.Days.Include(d => d.WorkingCalendar).FirstOrDefault(_d => _d.Id == dayId);

                var schedules = _dbContext.Schedules.Where(_s => _s.DayId == dayId).ToList();
                List<Data.Entities.Interval> intervals = new List<Data.Entities.Interval>();
                foreach (var schedule in schedules)
                {
                    intervals.AddRange(_dbContext.Intervals.Where(_i => _i.ScheduleId == schedule.Id && (_i.IsAvailable == true || _i.NumId >0)).ToList());
                }
                intervals = intervals.OrderBy(_i => _i.DateCreated).ToList();
                var data = _mapper.Map<List<Data.Entities.Interval>, List<IntervalViewModel>>(intervals);
                if (day.WorkingCalendar.ShiftCount == 0)
                {
                    throw new Exception("Invalid shiftcount");
                }
                result.Data = data;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel PublishDay(List<Guid> ids)
        {
            ResultModel result = new ResultModel();

            try
            {
                foreach (var id in ids)
                {
                    var day = _dbContext.Days.FirstOrDefault(wc => wc.Id == id && wc.IsDeleted == false);
                    var workingCalendar = _dbContext.WorkingCalendars.FirstOrDefault(wc => wc.Id == day.CalendarId && wc.IsDeleted == false);
                    if (workingCalendar.Status != WorkingCalendarStatus.POSTED)
                    {
                        throw new Exception("Working Calendar is not POSTED");
                    }
                    if (day == null)
                    {
                        result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                        return result;
                    }
                    if (day.Status.Equals(WorkingCalendarStatus.POSTED))
                    {
                        result.ErrorMessage = ErrorMessages.CALENDAR_POSTED;
                    }

                    day.Status = WorkingCalendarStatus.POSTED;
                    _dbContext.Days.Update(day);
                    _dbContext.SaveChanges();

                    var schedules = _dbContext.Schedules.Where(_d => _d.DayId == id).Select(_d => _d.Id).ToList();
                    List<Guid> intervals = new List<Guid>();
                    foreach (var schedule in schedules)
                    {
                        intervals.AddRange(_dbContext.Intervals.Where(_d => _d.ScheduleId == schedule).Select(_d => _d.Id).ToList());
                    }
                    PublishInterval(intervals);
                }
                result.Data = "Ok";
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public ResultModel CancelDay(List<Guid> ids)
        {
            ResultModel result = new ResultModel();

            try
            {
                foreach (var id in ids)
                {
                    var calendar = _dbContext.Days.FirstOrDefault(wc => wc.Id == id && wc.IsDeleted == false);

                    if (calendar == null)
                    {
                        result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                        return result;
                    }
                    if (calendar.Status.Equals(WorkingCalendarStatus.CANCEL_POST))
                    {
                        result.ErrorMessage = ErrorMessages.CALENDAR_CANCELED;
                    }

                    calendar.Status = WorkingCalendarStatus.CANCEL_POST;
                    _dbContext.Days.Update(calendar);
                    _dbContext.SaveChanges();

                    var schedules = _dbContext.Schedules.Where(_d => _d.DayId == id).Select(_d => _d.Id).ToList();
                    List<Guid> intervals = new List<Guid>();
                    foreach (var schedule in schedules)
                    {
                        intervals.AddRange(_dbContext.Intervals.Where(_d => _d.ScheduleId == schedule).Select(_d => _d.Id).ToList());
                    }
                    CancelInterval(intervals);
                }
                result.Data = "Ok";
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel PublishInterval(List<Guid> ids)
        {
            ResultModel result = new ResultModel();

            try
            {
                foreach (var id in ids)
                {
                    var interval = _dbContext.Intervals.FirstOrDefault(wc => wc.Id == id && wc.IsDeleted == false);
                    var shedule = _dbContext.Schedules.FirstOrDefault(wc => wc.Id == interval.ScheduleId && wc.IsDeleted == false);
                    var day = _dbContext.Days.FirstOrDefault(wc => wc.Id == shedule.DayId && wc.IsDeleted == false);

                    if (day.Status != WorkingCalendarStatus.POSTED)
                    {
                        throw new Exception("Day is not POSTED");
                    }
                    if (interval == null)
                    {
                        result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                        return result;
                    }
                    if (interval.Status.Equals(WorkingCalendarStatus.POSTED))
                    {
                        result.ErrorMessage = ErrorMessages.CALENDAR_POSTED;
                    }

                    interval.Status = WorkingCalendarStatus.POSTED;
                    _dbContext.Intervals.Update(interval);
                    _dbContext.SaveChanges();
                }
                result.Data = "Ok";
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }

            return result;
        }
        public ResultModel CancelInterval(List<Guid> ids)
        {
            ResultModel result = new ResultModel();

            try
            {
                foreach (var id in ids)
                {
                    var calendar = _dbContext.Intervals.FirstOrDefault(wc => wc.Id == id && wc.IsDeleted == false);

                    if (calendar == null)
                    {
                        result.ErrorMessage = ErrorMessages.ID_NOT_FOUND;
                        return result;
                    }
                    if (calendar.Status.Equals(WorkingCalendarStatus.CANCEL_POST))
                    {
                        result.ErrorMessage = ErrorMessages.CALENDAR_CANCELED;
                    }

                    calendar.Status = WorkingCalendarStatus.CANCEL_POST;
                    _dbContext.Intervals.Update(calendar);
                    _dbContext.SaveChanges();
                }
                result.Data = "Ok";
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel GetByUnitAndService(Guid unitId, Guid serviceId)
        {
            var result = new ResultModel();
            try
            {
                var service = _dbContext.Services.FirstOrDefault(_s => _s.Id == serviceId);
                if (service == null)
                {
                    throw new Exception("Invalid Service");
                }

                var workingCalendarsWithoutService = _dbContext.WorkingCalendars.Include(_w => _w.ServiceCalendars)
                    .Where(_w => _w.UnitId == unitId)
                    .Where(_r => _r.Status == WorkingCalendarStatus.POSTED)
                    .Where(_w => _w.IsDeleted == false)
                    .ToList();

                var workingCalendars = new List<WorkingCalendar>();
                foreach (var workingCalendar in workingCalendarsWithoutService)
                {
                    foreach (var serviceCalendar in workingCalendar.ServiceCalendars)
                    {
                        if (serviceCalendar.ServiceId == serviceId)
                        {
                            workingCalendars.Add(workingCalendar);
                        }
                    }
                }
                if (workingCalendars.Count > 0)
                {
                    var data = new List<WorkingCalendarViewModel>();
                    foreach (var workingCalendar in workingCalendars)
                    {
                        var day = _dbContext.Days.FirstOrDefault(_d => _d.CalendarId == workingCalendar.Id);
                        var shedules = _dbContext.Schedules.Where(_d => _d.DayId == day.Id).ToList();

                        var dataItem = _mapper.Map<WorkingCalendar, WorkingCalendarViewModel>(workingCalendar);

                        foreach (var shedule in shedules)
                        {
                            if (!string.IsNullOrEmpty(dataItem.FromTo))
                            {
                                dataItem.FromTo += " & ";
                            }
                            dataItem.FromTo += shedule.From + " - " + shedule.To;
                        }

                        dataItem.Doctor = _mapper.Map<Doctor, DoctorModel>(workingCalendar.DoctorCalendars.FirstOrDefault().Doctor);
                        dataItem.Room = _mapper.Map<Room, RoomModel>(workingCalendar.RoomCalendars.FirstOrDefault().Room);

                        data.Add(dataItem);
                    }

                    result.Data = data;
                }
                else
                {
                    result.Data = new List<WorkingCalendarViewModel>();
                }
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel GetDaysByUnitAndService(Guid unitId, Guid serviceId)
        {
            var result = new ResultModel();
            try
            {
                var workingCalendar = GetByUnitAndService(unitId, serviceId);
                if (workingCalendar.Succeed == false)
                {
                    result.Data = new List<object>();
                }
                else
                {
                    var workingCalendarIds = (List<WorkingCalendarViewModel>)(workingCalendar.Data);
                    var data = new List<DateDetailViewModel>();
                    foreach (var workingCalendarId in workingCalendarIds)
                    {
                        var days = GetCalendarDetail(workingCalendarId.Id);
                        if (days.Succeed)
                        {
                            var dataItem = (List<DateDetailViewModel>)days.Data;
                            DateTime todate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            dataItem = dataItem
                                .Where(_d => _d.Date.Subtract(todate).TotalDays <= workingCalendarId.BookingBeforeDate)
                                .Where(_d => _d.Date.Subtract(todate).TotalDays >= workingCalendarId.BookingAfterDate)
                                .ToList();
                            
                            data.AddRange(dataItem);
                        }
                        else
                        {
                            throw new Exception("Invalid Data");
                        }
                    }
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
        public ResultModel GetFullDaysByUnitAndService(Guid unitId, Guid serviceId)
        {
            var result = new ResultModel();
            try
            {
                var workingCalendar = GetByUnitAndService(unitId, serviceId);
                if (workingCalendar.Succeed == false)
                {
                    result.Data = new object();
                }
                else
                {
                    var workingCalendarIds = (List<WorkingCalendarViewModel>)(workingCalendar.Data);
                    var data = new List<DateDetailViewModel>();
                    foreach (var workingCalendarId in workingCalendarIds)
                    {
                        var days = GetCalendarDetail(workingCalendarId.Id);
                        if (days.Succeed)
                        {
                            var dataItem = (List<DateDetailViewModel>)days.Data;
                            DateTime todate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            dataItem = dataItem
                                //.Where(_d => _d.Date.Subtract(todate).TotalDays <= workingCalendarId.BookingBeforeDate)
                                //.Where(_d => _d.Date.Subtract(todate).TotalDays >= workingCalendarId.BookingAfterDate)
                                .ToList();
                            data.AddRange(dataItem);
                        }
                        else
                        {
                            throw new Exception("Invalid Data");
                        }
                    }
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
        public ResultModel CheckScheduledDoctor(Guid doctorId, DateTime fromDate, DateTime toDate)
        {
            var result = new ResultModel();
            try
            {
                var _workingUnits = _dbContext.WorkingCalendars.Include(_r => _r.DoctorCalendars)
                                                .Where(_r => (_r.ToDate.Year < toDate.Year && _r.ToDate.Year >= fromDate.Year) || (_r.ToDate.Year == toDate.Year) || (_r.ToDate.Year > toDate.Year && toDate.Year >= _r.FromDate.Year))
                                                .Where(_r => _r.Status != WorkingCalendarStatus.CANCEL_POST)
                                                .Where(_r => _r.IsDeleted == false)
                                                .ToList();

                var _workingUnitsDate = new ConcurrentQueue<WorkingCalendar>();
                Parallel.ForEach(Partitioner.Create(_workingUnits), (_workingUnit) =>
                {
                    var calendarBigger = (_workingUnit.ToDate.Subtract(fromDate).TotalDays - _workingUnit.ToDate.Subtract(_workingUnit.FromDate).TotalDays - toDate.Subtract(fromDate).TotalDays);
                    var toDateBigger = (toDate.Subtract(_workingUnit.FromDate).TotalDays - _workingUnit.FromDate.Subtract(_workingUnit.ToDate).TotalDays - fromDate.Subtract(toDate).TotalDays);

                    if (DateTime.Compare(_workingUnit.ToDate, toDate) > 0 && calendarBigger <= 0)
                    {
                        _workingUnitsDate.Enqueue(_workingUnit);
                    }
                    else if (DateTime.Compare(_workingUnit.ToDate, toDate) < 0 && toDateBigger <= 0)
                    {
                        _workingUnitsDate.Enqueue(_workingUnit);
                    }
                    else if (DateTime.Compare(_workingUnit.ToDate, toDate) == 0)
                    {
                        _workingUnitsDate.Enqueue(_workingUnit);
                    }
                    else if (DateTime.Compare(_workingUnit.ToDate, toDate) < 0 && DateTime.Compare(fromDate, _workingUnit.FromDate) < 0)
                    {
                        _workingUnitsDate.Enqueue(_workingUnit);
                    }
                });

                Parallel.ForEach(Partitioner.Create(_workingUnitsDate), (workingUnit) =>
                {
                    foreach (var serviceCalendar in workingUnit.DoctorCalendars)
                    {
                        if (doctorId == (serviceCalendar.DoctorId))
                        {
                            result.Data = ("Doctor already scheduled!");
                            result.Succeed = true;
                        }
                    }
                });
                if (result.Succeed == false)
                {
                    result.ErrorMessage = ("Doctor is free!");
                }
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel DeleteWorkingCalendar(Guid workingCalendarId)
        {
            var result = new ResultModel();
            try
            {
                var calendar = _dbContext.WorkingCalendars.FirstOrDefault(_w => _w.Id == workingCalendarId && _w.IsDeleted == false);
                if (calendar == null) throw new Exception("Invalid id");

                calendar.IsDeleted = true;
                _dbContext.Update(calendar);
                _dbContext.SaveChanges();

                result.Data = calendar.Id;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.InnerException != null ? e.InnerException.Message : e.Message;
            }
            return result;
        }
        public ResultModel GetIntervalByListDayId(List<Guid> dayIds)
        {
            var result = new ResultModel();
            try
            {
                var data = new List<IntervelShiftViewModel>();
                foreach (var dayId in dayIds)
                {
                    data.AddRange((List<IntervelShiftViewModel>)GetIntervels(dayId).Data);
                }

                result.Data = data;
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
