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
    public class InjectionObjectsController : ControllerBase
    {
        private readonly IInjectionObjectService _injectionObjectService;
        private readonly IInjectionObjectServiceTypeService _injectionObjectServiceTypeService;

        public InjectionObjectsController(IInjectionObjectService injectionObjectService, IInjectionObjectServiceTypeService injectionObjectServiceTypeService)
        {
            _injectionObjectService = injectionObjectService;
            _injectionObjectServiceTypeService = injectionObjectServiceTypeService;
        }

        [HttpGet]
        public IActionResult Get(Guid? id)
        {
            var result = _injectionObjectService.Get(id);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Add(InjectionObjectAddModel model)
        {
            var result = _injectionObjectService.Add(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut]
        public IActionResult Update(InjectionObjectUpdateModel model)
        {
            var result = _injectionObjectService.Update(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _injectionObjectService.Delete(id);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
        [HttpGet("{serviceTypeId}")]
        public IActionResult GetByServiceType(Guid serviceTypeId)
        {
            var result = _injectionObjectService.GetByServiceType(serviceTypeId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("SerivceType")]
        public IActionResult AddSerivceType([FromBody] InjectionObjectServiceTypeAddModel model)
        {
            var result = _injectionObjectServiceTypeService.Add(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("SerivceType")]
        public IActionResult DeleteSerivceType(Guid injectionObjectId, Guid serviceTypeId)
        {
            var result = _injectionObjectServiceTypeService.Delete(injectionObjectId, serviceTypeId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
    }
}
