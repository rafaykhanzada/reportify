using Core.Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;

namespace ReportGeneratorProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetPropertyController : ControllerBase
    {
        private readonly IWidgetPropertyService _widgetPropertyService;
        public WidgetPropertyController(IWidgetPropertyService widgetPropertyService)
        {
            _widgetPropertyService = widgetPropertyService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] WidgetPropertyCreateDto dto)
        {
            try
            {
                var result = await _widgetPropertyService.CreateAsync(dto);
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
                var data = _widgetPropertyService.GetAllAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
