using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceUnitsController : ControllerBase
    {
        private readonly IServiceUnitService _serviceUnitService;

        public ServiceUnitsController(IServiceUnitService serviceUnitService)
        {
            _serviceUnitService = serviceUnitService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(Guid? id)
        {
            ResultModel result = null;

            if (id.HasValue)
            {
                result = _serviceUnitService.Get(id.Value);
            }
            else
            {
                result = _serviceUnitService.GetAll();
            }

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        // POST api/<ServiceUnitsController>
        [HttpPost]
        public IActionResult Add([FromBody] ServiceUnitCreateModel model)
        {
            ResultModel result = _serviceUnitService.Add(model);

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        // PUT api/<ServiceUnitsController>/5
        [HttpPut]
        public IActionResult Update([FromBody] ServiceUnitUpdateModel model)
        {
            ResultModel result = _serviceUnitService.Update(model);

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        // DELETE api/<ServiceUnitsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            ResultModel result = _serviceUnitService.Delete(id);

            if (result.Succeed)
            {
                return Ok("Succeed");
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
