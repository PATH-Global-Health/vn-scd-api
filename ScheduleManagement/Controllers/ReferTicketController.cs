using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Constants;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using ScheduleManagement.Extensions;
using Services;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferTicketController : ControllerBase
    {
        private readonly IReferTicketService _ticketService;

        public ReferTicketController(IReferTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("ReceivedTicket")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetReceivedTicket(Guid? unitId=null,ReferType? status =null)
        {

            var result = _ticketService.GetReceivedTicket(User.GetUsername(), unitId,status);
            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Add([FromBody] ReferTicketCreateModel model)
        {
            var result = _ticketService.Add(model, User.GetUsername());
            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("RecviveTicket/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult RecviveTicket(Guid id, [FromBody] ReceiveTicketModel model)
        {
            var result = _ticketService.RecviveTicket(id,model);
            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
