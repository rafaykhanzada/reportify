using Core.Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;

namespace ReportGeneratorProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetReportController : ControllerBase
    {
        private readonly IWidgetReportService _widgetReportService;
        public WidgetReportController(IWidgetReportService service)
        {
            _widgetReportService = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] string ReportName)
        {
            try
            {
                var result = await _widgetReportService.CreateReport(ReportName);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

       
    }

}

