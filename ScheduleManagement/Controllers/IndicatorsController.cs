using Data.Models;
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
    public class IndicatorsController : ControllerBase
    {
        private readonly IIndicatorService _indicatorService;

        public IndicatorsController(IIndicatorService indicatorService)
        {
            _indicatorService = indicatorService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string searchValue, int pageIndex, int pageSize = int.MaxValue)
        {
            var result = await _indicatorService.GetIndicatorsAsync(searchValue, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IndicatorCreateModel model)
        {
            var result = await _indicatorService.CreateIndicatorAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(IndicatorUpdateModel model)
        {
            var result = await _indicatorService.UpdateIndicatorAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _indicatorService.DeleteIndicatorAsync(id);
            if (result.Succeed)
                return NoContent();
            return BadRequest(result);
        }

        [HttpGet("KPIs")]
        public async Task<IActionResult> GetKPIs(Guid indicatorId, int pageIndex, int pageSize = int.MaxValue)
        {
            var result = await _indicatorService.GetKPIsAsync(indicatorId, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost("KPIs")]
        public async Task<IActionResult> Create(KPICreateModel model)
        {
            var result = await _indicatorService.CreateKPIAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("KPIs")]
        public async Task<IActionResult> Update(KPIUpdateModel model)
        {
            var result = await _indicatorService.UpdateKPIAsync(model);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("KPIs/{id}")]
        public async Task<IActionResult> DeleteKPI(Guid id)
        {
            var result = await _indicatorService.DeleteKPIAsync(id);
            if (result.Succeed)
                return NoContent();
            return BadRequest(result);
        }
    }
}
