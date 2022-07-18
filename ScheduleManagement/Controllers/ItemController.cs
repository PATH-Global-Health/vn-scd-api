using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Data.Common.PaginationModel;
using Data.Constants;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Services;

namespace ScheduleManagement.Controllers
{
    [Route("api/item")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IitemService _itemService;

        public ItemController(IitemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems([FromQuery] PagingParam<ItemSort> paginationModel, string name)
        {
            var result = await _itemService.GetItems(paginationModel, name);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(Guid id)
        {
            var result = await _itemService.GetItemById(id);
            if (result.Succeed) return Ok(result.Data);
            return NotFound(result);
        }

        [HttpPost()]
        public async Task<IActionResult> AddItem(ItemCreateModel model)
        {
            var result = await _itemService.AddItem(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(Guid id,ItemUpdateModel model)
        {
            var result = await _itemService.UpdateItem(id,model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var result = await _itemService.DeleteItem(id);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }




    }
}
