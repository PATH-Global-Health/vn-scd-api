using System.Linq;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Services;


namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost]
        public IActionResult SubmitPatient(PatientCreateModel model)
        {
            var rs = _patientService.SubmitPatient(model);
            if (rs.Succeed) return Ok(rs.Data);
            return BadRequest(rs.Failed);
        }
    }
}
