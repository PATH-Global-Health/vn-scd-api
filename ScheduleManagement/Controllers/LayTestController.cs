using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayTestController : ControllerBase
    {
        private readonly ILayTestService _layTestService;

        public LayTestController(ILayTestService layTestService)
        {
            _layTestService = layTestService;
        }

        [HttpPost]
        public IActionResult SubmitPatient(LayTestCreateModel model)
        {
            var rs = _layTestService.SubmitLaytest(model);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Failed);
        }
    }
}
