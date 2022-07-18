using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ScheduleManagement.Extensions;
using Services;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileLinksController : ControllerBase
    {
        private readonly IProfileLinkService _profileLinkService;
        public ProfileLinksController(IProfileLinkService profileLinkService)
        {
            _profileLinkService = profileLinkService;
        }

        [HttpGet()]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetAll()
        {

            var result = _profileLinkService.GetAll(User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("ResgisterCustomerByQR/{profileId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult ResgisterCustomerByQR(Guid profileId)
        {
            var result = _profileLinkService
                .RegisterCustomerByQR(Guid.Parse(User.GetUserId()), User.GetUsername(), profileId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
    }
}
