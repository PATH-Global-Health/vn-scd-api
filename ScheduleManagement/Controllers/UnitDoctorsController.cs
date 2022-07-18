using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Extensions;
using Services;
using System;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitDoctorsController : ControllerBase
    {
        private readonly IUnitDoctorService _unitDoctorService;

        public UnitDoctorsController(IUnitDoctorService unitDoctorService)
        {
            _unitDoctorService = unitDoctorService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Get(Guid? id)
        {
            ResultModel result;
            if (id.HasValue)
            {
                result = _unitDoctorService.Get(id.Value, User.GetUsername());
            }
            else
            {
                result = _unitDoctorService.GetAll(User.GetUsername());
            }

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Add([FromBody] UnitDoctorCreateModel createModel)
        {
            var result = _unitDoctorService.Add(createModel);

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpPut]
        public IActionResult Update([FromBody] UnitDoctorUpdateModel updateModel)
        {
            var result = _unitDoctorService.Update(updateModel);

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _unitDoctorService.Delete(id);

            if (result.Succeed)
            {
                return Ok("Succeed");
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
