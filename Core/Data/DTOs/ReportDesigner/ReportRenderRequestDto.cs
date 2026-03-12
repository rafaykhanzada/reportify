namespace Core.Data.DTOs.ReportDesigner
{
    /// <summary>
    /// Request to render a report with parameters
    /// </summary>
    public class ReportRenderRequestDto
    {
        public int ReportId { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
        public string OutputFormat { get; set; } = "HTML"; // HTML, PDF, Excel
        public bool IncludeStyles { get; set; } = true;
        public bool Standalone { get; set; } = true; // Generate complete HTML document vs. fragment
    }

    /// <summary>
    /// Result of report rendering
    /// </summary>
    public class ReportRenderResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Content { get; set; } // HTML content or base64 for binary formats
        public string ContentType { get; set; } = "text/html";
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
