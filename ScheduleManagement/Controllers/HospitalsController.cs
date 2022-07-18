using AutoMapper;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScheduleManagement.Extensions;
using Services;
using System;
using System.Threading.Tasks;
//using Services.RabbitMQ;

namespace ScheduleManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalsController : ControllerBase
    {
        private readonly ILogger<HospitalsController> _logger;
        private readonly IUnitService _hospitalService;
        private readonly IMapper _mapper;
        //private readonly IProducer _producer;

        public HospitalsController(ILogger<HospitalsController> logger, IUnitService hospitalService, IMapper mapper)
        {
            _logger = logger;
            _hospitalService = hospitalService;
            _mapper = mapper;
            //_producer = producer;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Get(Guid? id)
        {
            var result = _hospitalService.Get(id, User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
        [HttpGet("infomation")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetInfo()
        {
            var result = _hospitalService.GetByUsername(User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
        [HttpPut("infomation")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult UpdateInfo(UserInformationModel model)
        {
            var result = _hospitalService.UpdateInformation(model, User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult Post([FromBody] HospitalRegister model)
        {
            var unit = _mapper.Map<HospitalRegister, UnitCreateModel>(model);
            var result = _hospitalService.Add(unit, User.GetUsername());
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("HospitalByAdmin/{username}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult PostByAdmin(string username, [FromBody] HospitalRegister model)
        {
            var unit = _mapper.Map<HospitalRegister, UnitCreateModel>(model);
            var result = _hospitalService.Add(unit, username);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }


        [HttpPut]
        public IActionResult Put([FromBody] UnitUpdateModel model)
        {
            var result = _hospitalService.Update(model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            var result = _hospitalService.Delete(id);
            if (result.Succeed) return Ok();
            return BadRequest(result.ErrorMessage);
        }
        [HttpPut("Logo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddLogo([FromForm] HospitalUpdateLogo model)
        {
            var result = await _hospitalService.UpdateLogoAsync(model);
            if (result.Succeed) return Ok("success");
            return BadRequest(result.ErrorMessage);
        }
        [HttpGet("Logo/{unitId}")]
        public IActionResult GetLogo(Guid unitId)
        {
            try
            {
                var rs = _hospitalService.GetLogo(unitId);
                if (rs == null) return Ok(null);
                return File(rs.Data, rs.FileType);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            };
        }
        [HttpPut("Images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddImages([FromForm] HospitalUpdateImages model)
        {
            var result = await _hospitalService.AddImagesAsync(model.Images, model.Id);
            if (result.Succeed) return Ok("success");
            return BadRequest(result.ErrorMessage);
        }
        [HttpGet("Images/{unitId}")]
        public IActionResult GetAllImage(Guid unitId)
        {
            var result = _hospitalService.GetAllImage(unitId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
        [HttpGet("Image/{imageId}")]
        public IActionResult GetImage(Guid imageId)
        {
            try
            {
                var rs = _hospitalService.GetUnitImage(imageId);
                if (rs == null) return Ok(null);
                return File(rs.Data, rs.FileType);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            };
        }
        [HttpGet("{serviceId}")]
        public IActionResult GetByService(Guid serviceId)
        {
            var result = _hospitalService.GetBySerive(serviceId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("GetByServiceAndDate/{serviceId}/{date}")]
        public IActionResult GetByServiceAndDate(Guid serviceId, DateTime date)
        {
            var result = _hospitalService.GetBySeriveAndDate(serviceId, date);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("SetTestingFacility/{id}")]
        public IActionResult SetTestingFacility(Guid id, [FromBody] SetTestingFacilityModel model)
        {
            var result = _hospitalService.SetTestingFacility(id, model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
        [HttpPut("SetPrEPFacility/{id}")]
        public IActionResult SetPrEPFacility(Guid id, [FromBody] SetPrEPFacilityModel model)
        {
            var result = _hospitalService.SetPrEPFacility(id, model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
        [HttpPut("SetARTFacility/{id}")]
        public IActionResult SetARTFacility(Guid id, [FromBody] SetARTFacilityModel model)
        {
            var result = _hospitalService.SetARTFacility(id, model);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("FilterByFunctionFacility")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult FilterFunctionFacility(bool? IsTestingFacility = null, bool? IsPrEPFacility = null, bool? IsARTFacility = null, int? pageIndex = 0, int? pageSize = 0)
        {
            var result = _hospitalService.FilterUnit(User.GetUsername(), IsTestingFacility, IsPrEPFacility, IsARTFacility, pageIndex, pageSize);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("GetHospitalByDoctor")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetHospitalByDoctor(Guid? doctorId)
        {
            var result = _hospitalService.GetHospitalByDoctor(User.GetUsername(), doctorId);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("CreateOrganization")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult CreateOrganization(CreateOrganizationModel model, string username)
        {
            var result = _hospitalService.CreateOrganization(model, username);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result);
        }


        [HttpGet("GetUnitByAdmin")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetUnitByAdmin(string username, int? pageIndex = 0, int? pageSize = 0)
        {
            var result = _hospitalService.GetUnit(username, pageIndex, pageSize);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("GetChildOrParentUnit")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetUnitByParentUnit(int? pageIndex = 0, int? pageSize = 0)
        {
            var result = _hospitalService.GetUnit(User.GetUsername(), pageIndex, pageSize);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("GetListParentUnit")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult GetParentUnit(int? pageIndex = 0, int? pageSize = 0)
        {
            var result = _hospitalService.GetParentUnit(pageIndex, pageSize);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("RemoveUnit/{id}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult RemoveUnit(Guid id)
        {
            var result = _hospitalService.RemoveUnit(id);
            if (result.Succeed) return Ok(result.Data);
            return BadRequest(result.ErrorMessage);
        }
    }
}