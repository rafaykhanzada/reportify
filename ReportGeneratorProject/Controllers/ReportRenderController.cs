using Core.Data.DTOs.ReportDesigner;
using Microsoft.AspNetCore.Mvc;
using Service.IService;

namespace ReportGeneratorProject.Controllers
{
    /// <summary>
    /// API for rendering reports to HTML with dynamic parameters from query strings
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportRenderController : ControllerBase
    {
        private readonly IHtmlReportRenderService _renderService;
        private readonly ILogger<ReportRenderController> _logger;

        public ReportRenderController(
            IHtmlReportRenderService renderService,
            ILogger<ReportRenderController> logger)
        {
            _renderService = renderService;
            _logger = logger;
        }

        /// <summary>
        /// Render a report to HTML with parameters from query string
        /// Example: /api/ReportRender/5?startDate=2024-01-01&endDate=2024-12-31&customerId=123
        /// </summary>
        [HttpGet("{reportId}")]
        public async Task<IActionResult> RenderReport(
            int reportId,
            [FromQuery] string? format = "html",
            [FromQuery] bool standalone = true,
            [FromQuery] bool includeStyles = true)
        {
            try
            {
                // Extract all query parameters except the known ones
                var parameters = ExtractQueryParameters(new[] { "format", "standalone", "includeStyles" });

                var request = new ReportRenderRequestDto
                {
                    ReportId = reportId,
                    Parameters = parameters,
                    OutputFormat = format?.ToUpperInvariant() ?? "HTML",
                    Standalone = standalone,
                    IncludeStyles = includeStyles
                };

                var result = await _renderService.RenderReport(request);

                if (!result.Success)
                    return BadRequest(result);

                // Return HTML content directly
                if (result.ContentType == "text/html")
                {
                    return Content(result.Content!, "text/html");
                }
                else if (result.ContentType == "application/pdf")
                {
                    var bytes = Convert.FromBase64String(result.Content!);
                    return File(bytes, "application/pdf", $"report-{reportId}.pdf");
                }
                else
                {
                    return Content(result.Content!, result.ContentType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering report {ReportId}", reportId);
                return StatusCode(500, new { Success = false, Message = $"Error rendering report: {ex.Message}" });
            }
        }

        /// <summary>
        /// Render a report with POST body parameters
        /// </summary>
        [HttpPost("{reportId}")]
        public async Task<IActionResult> RenderReportWithBody(
            int reportId,
            [FromBody] Dictionary<string, object>? parameters = null,
            [FromQuery] string? format = "html",
            [FromQuery] bool standalone = true,
            [FromQuery] bool includeStyles = true)
        {
            try
            {
                var request = new ReportRenderRequestDto
                {
                    ReportId = reportId,
                    Parameters = parameters,
                    OutputFormat = format?.ToUpperInvariant() ?? "HTML",
                    Standalone = standalone,
                    IncludeStyles = includeStyles
                };

                var result = await _renderService.RenderReport(request);

                if (!result.Success)
                    return BadRequest(result);

                if (result.ContentType == "text/html")
                {
                    return Content(result.Content!, "text/html");
                }
                else if (result.ContentType == "application/pdf")
                {
                    var bytes = Convert.FromBase64String(result.Content!);
                    return File(bytes, "application/pdf", $"report-{reportId}.pdf");
                }
                else
                {
                    return Content(result.Content!, result.ContentType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering report {ReportId}", reportId);
                return StatusCode(500, new { Success = false, Message = $"Error rendering report: {ex.Message}" });
            }
        }

        /// <summary>
        /// Preview a report with sample data
        /// </summary>
        [HttpGet("{reportId}/preview")]
        public async Task<IActionResult> PreviewReport(int reportId)
        {
            try
            {
                var result = await _renderService.PreviewReport(reportId);

                if (!result.Success)
                    return BadRequest(result);

                return Content(result.Content!, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing report {ReportId}", reportId);
                return StatusCode(500, new { Success = false, Message = $"Error previewing report: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get report metadata including available parameters
        /// </summary>
        [HttpGet("{reportId}/metadata")]
        public async Task<IActionResult> GetReportMetadata(int reportId)
        {
            // This would return information about the report including parameters
            // For now, returning a placeholder
            return Ok(new
            {
                ReportId = reportId,
                Message = "Report metadata endpoint - to be implemented with report parameter definitions"
            });
        }

        /// <summary>
        /// Download report in specified format
        /// Example: /api/ReportRender/5/download?format=pdf&startDate=2024-01-01
        /// </summary>
        [HttpGet("{reportId}/download")]
        public async Task<IActionResult> DownloadReport(
            int reportId,
            [FromQuery] string format = "pdf")
        {
            try
            {
                var parameters = ExtractQueryParameters(new[] { "format" });

                var request = new ReportRenderRequestDto
                {
                    ReportId = reportId,
                    Parameters = parameters,
                    OutputFormat = format.ToUpperInvariant(),
                    Standalone = true,
                    IncludeStyles = true
                };

                var result = await _renderService.RenderReport(request);

                if (!result.Success)
                    return BadRequest(result);

                var fileName = $"report-{reportId}-{DateTime.Now:yyyyMMddHHmmss}";

                switch (format.ToLower())
                {
                    case "html":
                        Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}.html");
                        return Content(result.Content!, "text/html");
                    
                    case "pdf":
                        var bytes = Convert.FromBase64String(result.Content!);
                        return File(bytes, "application/pdf", $"{fileName}.pdf");
                    
                    default:
                        return Content(result.Content!, result.ContentType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading report {ReportId}", reportId);
                return StatusCode(500, new { Success = false, Message = $"Error downloading report: {ex.Message}" });
            }
        }

        /// <summary>
        /// Helper method to extract query parameters as a dictionary
        /// </summary>
        private Dictionary<string, object> ExtractQueryParameters(string[] excludeKeys)
        {
            var parameters = new Dictionary<string, object>();

            foreach (var key in Request.Query.Keys)
            {
                if (!excludeKeys.Contains(key, StringComparer.OrdinalIgnoreCase))
                {
                    var value = Request.Query[key].ToString();
                    
                    // Try to parse common data types
                    if (int.TryParse(value, out int intValue))
                        parameters[key] = intValue;
                    else if (decimal.TryParse(value, out decimal decimalValue))
                        parameters[key] = decimalValue;
                    else if (bool.TryParse(value, out bool boolValue))
                        parameters[key] = boolValue;
                    else if (DateTime.TryParse(value, out DateTime dateValue))
                        parameters[key] = dateValue;
                    else
                        parameters[key] = value;
                }
            }

            return parameters;
        }
    }
}
