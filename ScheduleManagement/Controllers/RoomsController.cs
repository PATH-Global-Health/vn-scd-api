using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Extensions;
using Services;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Get(Guid? id, Guid? unitId)
        {
            var result = _roomService.Get(id, unitId, User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public IActionResult Post([FromBody] RoomCreateModel model)
        {
            var result = _roomService.Add(model);

            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }

        [HttpPut]
        public IActionResult Put([FromBody] RoomUpdateModel model)
        {
            var result = _roomService.Update(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var result = _roomService.Delete(id);
            if (result.Succeed) return Ok();
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("GetRoomsByUnitId/{unitId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Get(Guid unitId)
        {
            var result = _roomService.GetRoomByORGUnit(unitId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
    }
}
