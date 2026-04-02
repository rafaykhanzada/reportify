using Core.Data.DTOs.ReportDesigner;
using Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace ReportGeneratorProject.Controllers
{
    /// <summary>
    /// API for report designer - creating and managing report designs
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportDesignerController : ControllerBase
    {
        private readonly IReportDesignerService _reportDesignerService;
        private readonly ILogger<ReportDesignerController> _logger;

        public ReportDesignerController(
            IReportDesignerService reportDesignerService,
            ILogger<ReportDesignerController> logger)
        {
            _reportDesignerService = reportDesignerService;
            _logger = logger;
        }

        /// <summary>
        /// Get all report designs
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNo = 0, [FromQuery] int pageSize = int.MaxValue)
        {
            var result = await _reportDesignerService.GetAll(pageNo, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific report design by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _reportDesignerService.Get(id);
            return Ok(result);
        }

        /// <summary>
        /// Create or update a report design
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReportDesignDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _reportDesignerService.UserId = User.Claims.FirstOrDefault()?.Value ?? "system";
            var result = await _reportDesignerService.CreateOrUpdate(model);
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Update a report design
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ReportDesignDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.Id = id;
            _reportDesignerService.UserId = User.Claims.FirstOrDefault()?.Value ?? "system";
            var result = await _reportDesignerService.CreateOrUpdate(model);
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Delete a report design
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _reportDesignerService.UserId = User.Claims.FirstOrDefault()?.Value ?? "system";
            var result = await _reportDesignerService.Delete(id);
            return Ok(result);
        }

        /// <summary>
        /// Duplicate a report design
        /// </summary>
        [HttpPost("{id}/duplicate")]
        public async Task<IActionResult> Duplicate(int id, [FromQuery] string newName)
        {
            if (string.IsNullOrEmpty(newName))
                return BadRequest("New name is required");

            _reportDesignerService.UserId = User.Claims.FirstOrDefault()?.Value ?? "system";
            var result = await _reportDesignerService.Duplicate(id, newName);
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Get all elements for a report
        /// </summary>
        [HttpGet("{reportId}/elements")]
        public async Task<IActionResult> GetElements(int reportId)
        {
            var result = await _reportDesignerService.GetElements(reportId);
            return Ok(result);
        }

        /// <summary>
        /// Save an element to a report
        /// </summary>
        [HttpPost("{reportId}/elements")]
        public async Task<IActionResult> SaveElement(int reportId, [FromBody] ReportElementDto element)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _reportDesignerService.UserId = User.Claims.FirstOrDefault()?.Value ?? "system";
            var result = await _reportDesignerService.SaveElement(reportId, element);
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Delete an element from a report
        /// </summary>
        [HttpDelete("elements/{elementId}")]
        public async Task<IActionResult> DeleteElement(int elementId)
        {
            _reportDesignerService.UserId = User.Claims.FirstOrDefault()?.Value ?? "system";
            var result = await _reportDesignerService.DeleteElement(elementId);
            return Ok(result);
        }

        /// <summary>
        /// Save a full report design from the frontend designer format.
        /// Accepts the frontend JSON schema (with groups, running totals, formula fields,
        /// and the frontend element format) and converts it to the internal format.
        /// </summary>
        [HttpPost("design")]
        public async Task<IActionResult> SaveDesign([FromBody] FrontendReportDesignDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _reportDesignerService.UserId = User.Claims.FirstOrDefault()?.Value ?? "system";

            var reportDesign = model.ToReportDesignDto();
            var result = await _reportDesignerService.CreateOrUpdate(reportDesign);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        /// <summary>
        /// Update a full report design from the frontend designer format.
        /// </summary>
        [HttpPut("design/{id}")]
        public async Task<IActionResult> UpdateDesign(int id, [FromBody] FrontendReportDesignDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.Id = id;
            _reportDesignerService.UserId = User.Claims.FirstOrDefault()?.Value ?? "system";

            var reportDesign = model.ToReportDesignDto();
            var result = await _reportDesignerService.CreateOrUpdate(reportDesign);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }
    }
}
