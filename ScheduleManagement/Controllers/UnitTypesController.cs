using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitTypesController : ControllerBase
    {
        private readonly IUnitTypeService _unitTypeService;

        public UnitTypesController(IUnitTypeService unitTypeService)
        {
            _unitTypeService = unitTypeService;
        }

        [HttpGet]
        public IActionResult Get(Guid? id)
        {
            ResultModel result = null;
            if (id.HasValue)
            {
                result = _unitTypeService.Get(id.Value);
            }
            else
            {
                result = _unitTypeService.GetAll();
            }

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }
        [HttpPost]
        public IActionResult Add([FromBody] UnitTypeCreateModel model)
        {
            ResultModel result = _unitTypeService.Add(model);

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }
        [HttpPut]
        public IActionResult Update([FromBody] UnitTypeUpdateModel model)
        {
            ResultModel result = _unitTypeService.Update(model);

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            ResultModel result = _unitTypeService.Delete(id);

            if (result.Succeed)
            {
                return Ok("Succeed");
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
