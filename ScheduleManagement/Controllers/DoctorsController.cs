using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Extensions;
using Services;
using System;
using System.Threading.Tasks;
using Data.Common.PaginationModel;
using Data.Constants;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Add([FromBody] DoctorCreateModel model)
        {
            var result = _doctorService.Add(model, User.GetUsername());
            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("RegisterDoctor")]
        [Authorize(AuthenticationSchemes = "Bearer")]

        public IActionResult RegisterDoctor([FromBody] RegisterDoctorModel model)
        {
            var result = _doctorService.RegisterDoctor(model, User.GetUsername());
            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result);
        }
        [HttpGet("GetDoctorIdByUserId/{userId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetDoctorIdByUserId(string userId)
        {
            var result = _doctorService.GetdoctorIdByUserId(userId);
            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut]
        public IActionResult Update([FromBody] DoctorUpdateModel model)
        {
            var result = _doctorService.Update(model, User.GetUsername());
            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        public IActionResult Get(Guid? id)
        {
            var result = _doctorService.GetDoctorInUnit(id, User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetAllDoctor")]
        public IActionResult GetAllDoctor(int? pageIndex = null, int? pageSize = null, string textSearch = null)
        {
            var result = _doctorService.SearchDoctor(textSearch, pageIndex, pageSize);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _doctorService.Delete(id, User.GetUsername());
            if (result.Succeed)
            {
                return Ok("Succeed");
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("unit/{id}")]
        public async Task<IActionResult> GetDoctorByUnitId(Guid id, [FromQuery] PagingParam<BaseSortCriteria> paginationModel)
        {
            var result = await _doctorService.GetdoctorByUnitId(id, paginationModel);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }
    }
}