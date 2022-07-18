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
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Get(Guid? userId)
        {
            var result = _profileService.Get(userId, User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("AllRelation")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetAllRelation()
        {
            var result = _profileService.GetRelation(User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("ProfileByStatus")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult ProfileByStatus(int? status = null, string? name = null)
        {
            var result = _profileService.FilterProfile(User.GetUsername(), name, status);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult AddNew(ProfileAddModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _profileService.Add(model, User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("AddByDoctor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult AddByDoctor(ProfileAddModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _profileService.AddByDoctor(model, User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Update(ProfileUpdateModel model)
        {
            var result = _profileService.Update(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete]
        //[Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Delete(Guid id)
        {
            var result = _profileService.Delete(id);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("Filter")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Filter(string searchName, int? status = null)
        {
            var result = _profileService.SearchByName(searchName, status);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("ProfileFromDhealth")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult ProfileFromDhealth(string searchName)
        {
            var result = _profileService.GetProfileFromDealth(searchName);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("ProfileByUnitId/{unitId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult ProfileByUnitId(Guid unitId)
        {
            var result = _profileService.ProfileByUnitId(unitId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("ProfileByFacility/{unitId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult AddProfileByFacility(ProfileAddModel model, Guid unitId)
        {
            var result = _profileService.AddProfileByFacility(model, User.GetUsername(), unitId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("CustomerByQR")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult CustomerByQR(string userNameEmp, CustomerQR model)
        {
            var result = _profileService.CustomerByQR(userNameEmp, model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("CustomerIdentification")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult RegisterCustomerIdentification(string userNameEmp, CustomerIdentification model)
        {
            var result = _profileService.RegisterCustomerIdentification(userNameEmp, model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("CustomerAndProfile")]
        public IActionResult RegisterCustomerIdentification(CustomerIdentification model)
        {
            var result = _profileService.RegisterCustomerIdentification(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }


    }
}
