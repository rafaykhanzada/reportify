using Core.Data.DTOs.ReportDesigner;
using Core.Utils;
using System.Data;

namespace Service.IService
{
    /// <summary>
    /// Service for rendering reports to HTML
    /// </summary>
    public interface IHtmlReportRenderService
    {
        /// <summary>
        /// Render a report to HTML
        /// </summary>
        Task<ReportRenderResultDto> RenderReport(ReportRenderRequestDto request);
        
        /// <summary>
        /// Render a report directly by ID with parameters from query string
        /// </summary>
        Task<ReportRenderResultDto> RenderReportById(int reportId, Dictionary<string, object>? parameters = null);
        
        /// <summary>
        /// Get a preview of the report with sample data
        /// </summary>
        Task<ReportRenderResultDto> PreviewReport(int reportId);
        
        /// <summary>
        /// Generate HTML for a single element
        /// </summary>
        string RenderElement(ReportElementDto element, DataRow? dataRow = null, Dictionary<string, object>? parameters = null);
        
        /// <summary>
        /// Generate CSS styles for the report
        /// </summary>
        string GenerateStyles(ReportDesignDto design);
    }
}
