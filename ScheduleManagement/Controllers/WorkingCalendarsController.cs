using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using ScheduleManagement.Extensions;
using Services;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkingCalendarsController : ControllerBase
    {
        private readonly IWorkingCalendarService _workingCalendarService;
        public const string IndexName = "capitals";
        private readonly IIntervalService _intervalService;

        public WorkingCalendarsController(IWorkingCalendarService workingCalendarService, IIntervalService intervalService)
        {
            _workingCalendarService = workingCalendarService;
            _intervalService = intervalService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Add([FromBody] WorkingCalendarCreateModel model)
        {
            var result = await _workingCalendarService.AddAsync(model, User.GetUsername());
            if (result.Succeed)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("Publish")]
        public IActionResult Publish(List<Guid> ids)
        {
            var result = _workingCalendarService.PublishCalendar(ids);
            if (result.Succeed)
            {
                return Ok("Published");
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("Cancel")]
        public async Task<IActionResult> Cancel(List<Guid> ids)
        {
            var result = await _workingCalendarService.CancelCalendar(ids);
            if (result.Succeed)
            {
                return Ok("Canceled");
            }
            return BadRequest(result.ErrorMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetByUnit/{unitId}")]
        public IActionResult GetByUnit(Guid unitId)
        {
            var result = _workingCalendarService.Get(unitId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetByDoctor")]
        public IActionResult GetByDoctorId(string userId,DateTime fromDate, DateTime toDate)
        {
            var result = _workingCalendarService.GetByDoctorId(userId,fromDate,toDate);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetStaffScheduleByUserId")]
        public IActionResult GetStaffScheduleByUserId(string userId, DateTime fromDate, DateTime toDate)
        {
            var result = _workingCalendarService.GetStaffScheduleByDoctorId(userId, fromDate, toDate);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetDays/{calendarId}")]
        public IActionResult GetDays(Guid calendarId)
        {
            var result = _workingCalendarService.GetCalendarDetail(calendarId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetIntervals/{dayId}")]
        public IActionResult GetIntervals(Guid dayId)
        {
            var result = _workingCalendarService.GetIntervels(dayId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetIntervalsWithDayId/{dayId}")]
        public IActionResult GetIntervalWithDayId(Guid dayId)
        {
            var result = _workingCalendarService.GetIntervals(dayId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetIntervalsWithAvailableQuantity/{dayId}")]
        public IActionResult GetIntervalsWithAvailableQuantity(Guid dayId)
        {
            var result = _workingCalendarService.GetAvailableIntervals(dayId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("GetIntervals")]
        public IActionResult GetIntervals([FromBody]List<Guid> dayIds)
        {
            var result = _workingCalendarService.GetIntervalByListDayId(dayIds);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("Publish/Day")]
        public IActionResult PublishDay(List<Guid> ids)
        {
            var result = _workingCalendarService.PublishDay(ids);
            if (result.Succeed) return Ok("Published");
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("Cancel/Day")]
        public IActionResult CancelDay(List<Guid> ids)
        {
            var result = _workingCalendarService.CancelDay(ids);
            if (result.Succeed) return Ok("Canceled");
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("Publish/Interval")]
        public IActionResult PublishInterval(List<Guid> ids)
        {
            var result = _workingCalendarService.PublishInterval(ids);
            if (result.Succeed) return Ok("Published");
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("Cancel/Interval")]
        public IActionResult CancelInterval(List<Guid> ids)
        {
            var result = _workingCalendarService.CancelInterval(ids);
            if (result.Succeed) return Ok("Canceled");
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("GetByUnitAndService")]
        public IActionResult GetByServiceType(Guid serviceId, Guid unitId)
        {
            var result = _workingCalendarService.GetByUnitAndService(unitId, serviceId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("GetDaysByUnitAndService")]
        public IActionResult GetDaysByUnitAndService(Guid serviceId, Guid unitId)
        {
            var result = _workingCalendarService.GetDaysByUnitAndService(unitId, serviceId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("GetFullDaysByUnitAndService")]
        public IActionResult GetFullDaysByUnitAndService(Guid serviceId, Guid unitId)
        {
            var result = _workingCalendarService.GetFullDaysByUnitAndService(unitId, serviceId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("CheckScheduledDoctor")]
        public IActionResult CheckScheduledDoctor(Guid doctorId, DateTime fromDate, DateTime toDate)
        {
            var result = _workingCalendarService.CheckScheduledDoctor(doctorId, fromDate, toDate);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _workingCalendarService.DeleteWorkingCalendar(id);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("OrderUnOrderInterval")]
        public IActionResult OrderUnOrderInterval(OrderIntervalModel model)
        {
            var result = _intervalService.OrderUnOrderIntervel(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

    }
}
