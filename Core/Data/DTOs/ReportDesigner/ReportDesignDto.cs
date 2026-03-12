namespace Core.Data.DTOs.ReportDesigner
{
    /// <summary>
    /// Complete report definition for the designer
    /// </summary>
    public class ReportDesignDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ProjectId { get; set; }
        
        /// <summary>
        /// Report page settings
        /// </summary>
        public PageSettingsDto PageSettings { get; set; } = new();
        
        /// <summary>
        /// All elements in the report
        /// </summary>
        public List<ReportElementDto> Elements { get; set; } = new();
        
        /// <summary>
        /// Report parameters
        /// </summary>
        public List<ReportParameterDto> Parameters { get; set; } = new();
        
        /// <summary>
        /// Data sources for the report
        /// </summary>
        public List<ReportDataSourceDto> DataSources { get; set; } = new();
        
        /// <summary>
        /// Grouping configuration with expression support
        /// </summary>
        public List<GroupingDefinitionDto> Groups { get; set; } = new();
        
        /// <summary>
        /// Sorting configuration
        /// </summary>
        public List<SortingDto> Sorting { get; set; } = new();
        
        /// <summary>
        /// Custom formula fields
        /// </summary>
        public List<FormulaFieldDto> FormulaFields { get; set; } = new();
        
        /// <summary>
        /// Running totals
        /// </summary>
        public List<RunningTotalDto> RunningTotals { get; set; } = new();
        
        /// <summary>
        /// Conditional formatting rules
        /// </summary>
        public List<ConditionalFormatDto> ConditionalFormats { get; set; } = new();
        
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public class PageSettingsDto
    {
        public string PaperSize { get; set; } = "A4"; // A4, Letter, Legal, Custom
        public string Orientation { get; set; } = "Portrait"; // Portrait, Landscape
        public double Width { get; set; } = 210; // mm
        public double Height { get; set; } = 297; // mm
        public MarginDto Margins { get; set; } = new();
        public string? BackgroundColor { get; set; }
        public string? BackgroundImage { get; set; }
    }

    public class MarginDto
    {
        public double Top { get; set; } = 20;
        public double Right { get; set; } = 20;
        public double Bottom { get; set; } = 20;
        public double Left { get; set; } = 20;
    }
}
