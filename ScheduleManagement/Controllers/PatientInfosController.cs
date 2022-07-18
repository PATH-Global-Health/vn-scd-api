using Data.Models.SMDModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Controllers.BaseController;
using ScheduleManagement.Extensions;
using Services.LookupServices;
using Services.SMDServices;
using System;
using System.Threading.Tasks;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PatientInfosController : SMDControllerBase
    {
        private readonly IPatientInfoService _infoService;

        public PatientInfosController(SMDUserLookupService userLookupService, IPatientInfoService infoService) : base(userLookupService)
        {
            _infoService = infoService;
        }

        [HttpPut("Get")]
        public async Task<IActionResult> Get(PatientInfoFilterModel model)
        {
            var user = base.GetCustomUser();
            var result = await _infoService.GetAsync(model, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatientInfoCreateModel model)
        {
            var user = base.GetCustomUser();
            var result = await _infoService.AddPatientInfoAsync(model, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(PatientInfoUpdateModel model)
        {
            var user = base.GetCustomUser();
            var result = await _infoService.UpdatePatientInfoAsync(model, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = base.GetCustomUser();
            var result = await _infoService.DeletePatientInfoAsync(id, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("Histories")]
        public async Task<IActionResult> GetHistory(Guid patientInfoId, int pageIndex, int pageSize = int.MaxValue)
        {
            var user = base.GetCustomUser();
            var result = await _infoService.GetHistoryAsync(patientInfoId, pageIndex, pageSize);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
