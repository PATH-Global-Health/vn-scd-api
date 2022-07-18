using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Controllers.BaseController;
using Services.LookupServices;
using Services.Utilities;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilitiesController : SMDControllerBase
    {
        private readonly IUtilitiesService _utilities;

        public UtilitiesController(SMDUserLookupService userLookupService, IUtilitiesService utilities) : base(userLookupService)
        {
            this._utilities = utilities;
        }

        [HttpGet("InitUser")]
        public IActionResult Get()
        {
            _UserLookupService.InitSMDUsers();
            return Ok();
        }

        [HttpGet("FixDeletedProjects")]
        public IActionResult FixDeletedProjects()
        {
            try
            {
                _utilities.FixDeletedProject();
                return Ok();
            }
            catch (System.Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }

        [HttpGet("FakeData")]
        public IActionResult FakeData()
        {
            return Ok(_utilities.FakeData());
        }
    }
}
