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
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;

        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }

        [HttpGet]
        public IActionResult Get(Guid? id)
        {
            var result = _servicesService.Get(id);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ServiceCreateModel model)
        {
            var result = _servicesService.Add(model);

            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut]
        public IActionResult Put([FromBody] ServiceUpdateModel model)
        {
            var result = _servicesService.Update(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var result = _servicesService.Delete(id);
            if (result.Succeed) return Ok();
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("ServiceFormAndServiceType")]
        public IActionResult GetByServiceType(Guid serviceTypeId, Guid serviceFormId, Guid? injectionObjectId)
        {
            var result = _servicesService.GetByServiceFormAndServiceType(serviceFormId, serviceTypeId, injectionObjectId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
    }
}