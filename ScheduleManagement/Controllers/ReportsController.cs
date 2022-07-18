using Data.Constants;
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
    public class ReportsController : SMDControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(SMDUserLookupService userLookupService, IReportService reportService) : base(userLookupService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid? projectId, Guid? unitId, Guid? indicatorId, int pageIndex, int pageSize = int.MaxValue)
        {
            var result = await _reportService.GetAsync(projectId, unitId, indicatorId, pageIndex, pageSize);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("Get")]
        public async Task<IActionResult> Get(ReportBarChartFilterModel model)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.GetAsync(model, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("GetExcel")]
        public async Task<IActionResult> GetExcel(ReportGeneralFilterModel model)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.GetExcelAsync(model, user);
            if (result.Succeed)
            {
                var fileBytes = result.Data as byte[];
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"reports_{DateTime.UtcNow.Ticks}.xlsx");
            }
            return BadRequest(result);
        }

        [HttpPut("Summary")]
        public IActionResult Summary(ReportGeneralFilterModel model)
        {
            var user = base.GetCustomUser();
            var result = _reportService.Summary(model, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("BarChart")]
        public IActionResult BarChart(ReportBarChartFilterModel model)
        {
            var user = base.GetCustomUser();
            var result = _reportService.BarChart(model, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("Efficiency")]
        public IActionResult Efficiency(ReportEfficiencyFilterModel model)
        {
            var user = base.GetCustomUser();
            var result = _reportService.Efficiency(model, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("ByCBO")]
        public async Task<IActionResult> GetByCBO(Guid? indicatorId, int pageIndex, int pageSize = int.MaxValue)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.GetByCBOAsync(user, indicatorId, pageIndex, pageSize);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("ByProject")]
        public async Task<IActionResult> GetByProject(Guid? unitId, Guid? indicatorId, int pageIndex, int pageSize = int.MaxValue)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.GetByProjectAsync(user, unitId, indicatorId, pageIndex, pageSize);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReportCreateModel model)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.CreateAsync(model, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        //[HttpPost("CreateBatch")]
        //public async Task<IActionResult> CreateBatch(ICollection<ReportCreateModel> models)
        //{
        //    var result = await _reportService.CreateBatchAsync(models);
        //    if (result.Succeed)
        //        return Ok(result);
        //    return BadRequest(result);
        //}

        //[HttpPost("CreateBatchByFile")]
        //public async Task<IActionResult> CreateBatch(ICollection<ReportFileModel> models)
        //{
        //    var result = await _reportService.CreateBatchAsync(models);
        //    if (result.Succeed)
        //        return Ok(result);
        //    return BadRequest(result);
        //}

        [HttpPut]
        public async Task<IActionResult> Update(ReportUpdateModel model)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.UpdateAsync(model, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _reportService.DeleteAsync(id);
            if (result.Succeed)
                return NoContent();
            return BadRequest(result);
        }

        [HttpPost("ImportByCBO")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportByCBO([FromForm] IFormFile file, ReadType readType)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ReadReportExcelAggregateForCBOAsync(file, user, readType);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ImportByProject")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportByProject([FromForm] IFormFile file, ReadType readType)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ReadReportExcelAggregateForProjectAsync(file, user, readType);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ImportRawByCBO")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportRawByCBO([FromForm] IFormFile file, ReadType readType, bool forceDelete = false)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ReadReportExcelIndividualForCBOAsync(file, user, readType, forceDelete);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ImportRawByProject")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportRawByProject([FromForm] IFormFile file, ReadType readType, bool forceDelete = false)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ReadReportExcelIndividualForProjectAsync(file, user, readType, forceDelete);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ExposedAggregateForCBO")]
        public async Task<IActionResult> ExposedAggregateForCBOAsync(ICollection<ReportAggregateModel> models)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ExposedAggregateForCBOAsync(models, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ExposedAggregateForProject")]
        public async Task<IActionResult> ExposedAggregateForProjectAsync(ICollection<ReportAggregateModel> models)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ExposedAggregateForProjectAsync(models, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ExposedIndividualForCBO")]
        public async Task<IActionResult> ExposedIndividualForCBOAsync(ICollection<ReportIndividualModel> models, bool forceDelete = false)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ExposedIndividualForCBOAsync(models, user, forceDelete);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ExposedIndividualForProject")]
        public async Task<IActionResult> ExposedIndividualForProjectAsync(ICollection<ReportIndividualModel> models, bool forceDelete = false)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ExposedIndividualForProjectAsync(models, user, forceDelete);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ExposedPaymentForCBO")]
        public async Task<IActionResult> ExposedPaymentForCBOAsync(ICollection<ReportPaymentModel> models)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ExposedPaymentForCBOAsync(models, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ExposedPaymentForProject")]
        public async Task<IActionResult> ExposedPaymentForProjectAsync(ICollection<ReportPaymentModel> models)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ExposedPaymentForProjectAsync(models, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("Histories")]
        public async Task<IActionResult> GetHistory(Guid reportId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var result = await _reportService.GetHistoryAsync(reportId, pageIndex, pageSize);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpGet("SyncTarget")]
        public async Task<IActionResult> SyncTarget(DateTime? from, DateTime? to)
        {
            var result = await _reportService.SyncTarget(from, to);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("ListProvincesWithData")]
        public async Task<IActionResult> ListProvincesWithData(DateTime? from, DateTime? to)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ListProvinces(from, to, user);
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("ListCBOsWithData")]
        public async Task<IActionResult> ListCBOsWithData(DateTime? from, DateTime? to)
        {
            var user = base.GetCustomUser();
            var result = await _reportService.ListCBOs(from, to, user);
                        if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("GetLastUpdatedDate")]
        public IActionResult GetLastUpdatedDate()
        {
            var result = _reportService.GetLastUpdated();
            if (result.Succeed)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
