using Data.Models;
using Data.Models.SMDModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Controllers.BaseController;
using ScheduleManagement.Extensions;
using Services.LookupServices;
using Services.SMDServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ProjectsController : SMDControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService, SMDUserLookupService userLookupService) : base(userLookupService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string searchValue, int pageIndex, int pageSize = int.MaxValue)
        {
            var result = await _projectService.GetAsync(searchValue, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("ByUsername")]
        public async Task<IActionResult> GetByUsername()
        {
            var user = this.GetCustomUser();
            var result = await _projectService.GetByUsernameAsync(user);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateModel model)
        {
            var result = await _projectService.CreateAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProjectUpdateModel model)
        {
            var result = await _projectService.UpdateAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _projectService.DeleteAsync(id);
            if (result.Succeed)
                return NoContent();
            return BadRequest(result);
        }

        [HttpGet("Units")]
        public async Task<IActionResult> GetSMDUnits(string searchValue, int pageIndex, int pageSize = int.MaxValue)
        {
            var result = await _projectService.GetUnitAsync(searchValue, pageIndex, pageSize);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("UnitsInProject")]
        public async Task<IActionResult> GetSMDUnits(string searchValue, Guid projectId, int pageIndex, int pageSize = int.MaxValue)
        {
            var result = await _projectService.GetUnitsInProjectAsync(searchValue, projectId, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("UnitsInProjectByProjectUsername")]
        public async Task<IActionResult> UnitsInProjectByProjectUsername(string searchValue, int pageIndex, int pageSize = int.MaxValue)
        {
            var user = this.GetCustomUser();
            var result = await _projectService.GetUnitsInProjectByUsernameAsync(searchValue, user, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("UnitByUsername")]
        public async Task<IActionResult> GetSMDUnitByUsername()
        {
            var user = this.GetCustomUser();
            var result = await _projectService.GetUnitByUsernameAsync(user);
            return Ok(result);
        }

        [HttpPost("UnitsInProject")]
        public async Task<IActionResult> Create(SMDUnitCreateModel model)
        {
            var result = await _projectService.CreateUnitByProjectAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UnitsInProject")]
        public async Task<IActionResult> Update(SMDUnitUpdateModel model)
        {
            var result = await _projectService.UpdateUnitInProjectAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("UnitsInProject/{id}")]
        public async Task<IActionResult> DeleteSMDUnit(Guid id)
        {
            var result = await _projectService.DeleteUnitInProjectAsync(id);
            if (result.Succeed)
                return NoContent();
            return BadRequest(result);
        }

        [HttpPost("CreateAccountForProject")]
        public async Task<IActionResult> Create(AddAccountModel model)
        {
            var user = this.GetCustomUser();
            var result = await _projectService.AddAccountToProject(model, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
