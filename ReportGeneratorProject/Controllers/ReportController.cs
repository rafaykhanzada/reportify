using Core.Constant;
using Core.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

// For more information on enabling Web API for empty Reports, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ReportGeneratorReport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController(IReportService reportService) : ControllerBase
    {
        private readonly IReportService _reportService = reportService;

        [HttpGet]
        public async Task<IActionResult> Get(int pageNo = 0, int pageSize = int.MaxValue, [FromQuery] FilterDto? model = null) => Ok(await _reportService.Get(pageNo, pageSize, model));

        // GET api/<EmailSetupController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (ModelState.IsValid)
                return Ok(await _reportService.Get(id));
            return BadRequest();
        }
        
        [HttpGet("report-preview")]
        public IActionResult GetPreview(int id)
        {
            if (ModelState.IsValid)
                return Ok(_reportService.GetPreview(id));
            return BadRequest();
        }

        // POST api/<EmailSetupController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReportDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(MessageString.ValidationError);
            _reportService.UserId = User.Claims.FirstOrDefault()?.Value!;
            return Ok(await _reportService.CreateOrUpdate(model));
        }

        // PUT api/<EmailSetupController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] ReportDto model)
        {
            if (ModelState.IsValid)
                return Ok(await _reportService.CreateOrUpdate(model));
            _reportService.UserId = User.Claims.FirstOrDefault()?.Value!;
            return BadRequest();
        }

        // DELETE api/<EmailSetupController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _reportService.UserId = User.Claims.FirstOrDefault()?.Value!;
            return Ok(await _reportService.Delete(id));
        }
        [HttpGet("export")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string? type = "xlsx", [FromQuery] FilterDto? SearchBy = null)
        {
            var result = await _reportService.Export(type, SearchBy);
            if (result.Success == false)
                return BadRequest(result);
            else
            {
                string FileName = ControllerContext.ActionDescriptor.ControllerName + "_" + DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss") + ".xlsx";
                Response.Headers.Add("Content-Disposition", "attachment;filename=" + FileName);
                Response.Headers.Add("Content-Type", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                return File((byte[])result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}
