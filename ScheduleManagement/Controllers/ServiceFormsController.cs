using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceFormsController : ControllerBase
    {
        private readonly IServiceFormService _serviceFormService;

        public ServiceFormsController(IServiceFormService serviceFormService)
        {
            _serviceFormService = serviceFormService;
        }

        [HttpGet]
        public IActionResult Get(Guid? id)
        {
            var result = _serviceFormService.Get(id);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ServiceFormCreateModel model)
        {
            var result = _serviceFormService.Add(model);

            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut]
        public IActionResult Put([FromBody] ServiceFormUpdateModel model)
        {
            var result = _serviceFormService.Update(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var result = _serviceFormService.Delete(id);
            if (result.Succeed) return Ok();
            return BadRequest(result.ErrorMessage);
        }
    }
}
