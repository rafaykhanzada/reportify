using Core.Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;

namespace ReportGeneratorProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetPropertyDataController : ControllerBase
    {
        private readonly IWidgetPropertyDataService _widgetPropertyDataService;
        public WidgetPropertyDataController(IWidgetPropertyDataService widgetPropertyDataService)
        {
            _widgetPropertyDataService = widgetPropertyDataService;
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var data = _widgetPropertyDataService.GetAllAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] WidgetPropertyDataDto dto)
        {
            try
            {
                var result = await _widgetPropertyDataService.CreateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
