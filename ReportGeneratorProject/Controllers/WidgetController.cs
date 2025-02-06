using Core.Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace ReportGeneratorProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetController : ControllerBase
    {
        private readonly IWidgetService widgetService;
        public WidgetController(IWidgetService widgetService)
        {
            this.widgetService = widgetService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await widgetService.GetAllAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromQuery]WidgetCreateDto dto)
        {
            try
            {
                var result = await widgetService.CreateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
