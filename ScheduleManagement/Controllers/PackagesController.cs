using Data.Models;
using Data.Models.SMDModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.SMDServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackagesController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int pageIndex, int pageSize = int.MaxValue)
        {
            var result = await _packageService.GetAsync(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PackageCreateModel model)
        {
            var result = await _packageService.CreateAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(PackageUpdateModel model)
        {
            var result = await _packageService.UpdateAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _packageService.DeleteAsync(id);
            if (result.Succeed)
                return NoContent();
            return BadRequest(result);
        }

        [HttpGet("ImplementPackages")]
        public async Task<IActionResult> GetImplementPackage(Guid? packageId, string province, int pageIndex, int pageSize = int.MaxValue)
        {
            var result = await _packageService.GetIPackageAsync(packageId, province, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost("ImplementPackages")]
        public async Task<IActionResult> Create(ImplementPackageCreateModel model)
        {
            var result = await _packageService.CreateIPackageAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("ImplementPackages")]
        public async Task<IActionResult> Update(ImplementPackageUpdateModel model)
        {
            var result = await _packageService.UpdateIPackageAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("Contracts")]
        public async Task<IActionResult> GetContracts(Guid? cboId, Guid? ipackageId, int pageIndex, int pageSize = int.MaxValue)
        {
            var result = await _packageService.GetContractAsync(cboId, ipackageId, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost("Contracts")]
        public async Task<IActionResult> Create(ContractCreateModel model)
        {
            var result = await _packageService.CreateContractAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("Contracts")]
        public async Task<IActionResult> Update(ContractUpdateModel model)
        {
            var result = await _packageService.UpdateContractAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("ActiveContract")]
        public async Task<IActionResult> SetActiveContract(SetContractModel model)
        {
            var result = await _packageService.SetCurrentContractAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("Contracts/{id}")]
        public async Task<IActionResult> DeleteContract(Guid id)
        {
            var result = await _packageService.DeleteContractAsync(id);
            if (result.Succeed)
                return NoContent();
            return BadRequest(result);
        }

        [HttpGet("Targets")]
        public async Task<IActionResult> GetTargets(Guid? ipackageId, int pageIndex, int pageSize = int.MaxValue)
        {
            var result = await _packageService.GetTargetAsync(ipackageId, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost("Targets")]
        public async Task<IActionResult> CreateTarget(TargetCreateModel model)
        {
            var result = await _packageService.CreateTargetAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("Targets")]
        public async Task<IActionResult> Update(TargetUpdateModel model)
        {
            var result = await _packageService.UpdateTargetAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
