using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Services;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaysController : ControllerBase
    {
        private readonly IDayService _dayService;

        public DaysController(IDayService dayService)
        {
            _dayService = dayService;
        }

        [HttpGet("WorkingDate")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetWorkingDate(Guid serviceId,DateTime date)
        {

            var result = _dayService.GetWorkingDate(serviceId, date);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("Available/{serviceId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult ValidDate(Guid serviceId)
        {

            var result = _dayService.GetDateByServiceId(serviceId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
    }
}
