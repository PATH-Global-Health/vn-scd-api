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
    public class ServiceTypesController : ControllerBase
    {
        private readonly IServiceTypeService _serviceTypeService;

        public ServiceTypesController(IServiceTypeService serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
        }

        [HttpGet]
        public IActionResult Get(Guid? id)
        {
            var result = _serviceTypeService.Get(id);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ServiceTypeCreateModel model)
        {
            var result = _serviceTypeService.Add(model);

            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut]
        public IActionResult Put([FromBody] ServiceTypeUpdateModel model)
        {
            var result = _serviceTypeService.Update(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var result = _serviceTypeService.Delete(id);
            if (result.Succeed) return Ok();
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("{serviceFormId}")]
        public IActionResult GetByServiceType(Guid serviceFormId)
        {
            var result = _serviceTypeService.GetByServiceForm(serviceFormId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
    }
}
