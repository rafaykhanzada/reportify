using Core.Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;

namespace ReportGeneratorProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetSettingsController : ControllerBase
    {
        private readonly IWidgetSettingsService _widgetSettingsService;
        public WidgetSettingsController(IWidgetSettingsService widgetSettingsService)
        {
            _widgetSettingsService = widgetSettingsService;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] WidgetSettingsCreateDto dto,[FromQuery] int wid)
        {
            try
            {
                var result = await _widgetSettingsService.CreateAsync(dto,wid);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var data = _widgetSettingsService.GetAllAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
