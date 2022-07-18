using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Common.PaginationModel;
using Data.Constants;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Services;

namespace ScheduleManagement.Controllers
{
    [Route("api/itemSource")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class ItemSourceController : ControllerBase
    {
        private readonly IitemSourceService _itemSourceService;
        public ItemSourceController(IitemSourceService itemSourceService)
        {
            _itemSourceService = itemSourceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetItemSource([FromQuery] PagingParam<ItemSort> paginationModel, string name)
        {
            var result = await _itemSourceService.GetItemSource(paginationModel, name);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemSourceById(Guid id)
        {
            var result = await _itemSourceService.GetItemSrcById(id);
            if (result.Succeed) return Ok(result.Data);
            return NotFound(result);
        }

        [HttpPost()]
        public async Task<IActionResult> AddItemSource(ItemSourceCreateModel model)
        {
            var result = await _itemSourceService.AddItemSource(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemSource(Guid id, ItemSourceUpdateModel model)
        {
            var result = await _itemSourceService.UpdateItemSource(id, model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemSource(Guid id)
        {
            var result = await _itemSourceService.DeleteItemSource(id);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }

    }
}
