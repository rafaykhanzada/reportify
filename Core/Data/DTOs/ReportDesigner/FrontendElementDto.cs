using System.Text.Json.Serialization;

namespace Core.Data.DTOs.ReportDesigner
{
    /// <summary>
    /// Matches the element JSON structure sent by the frontend report designer.
    /// This is the "raw" designer format before conversion to internal ReportElementDto.
    /// </summary>
    public class FrontendElementDto
    {
        public string? Id { get; set; }
        public int? WId { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Text { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public FrontendBorderStylesDto? BorderStyles { get; set; }
        public FrontendTextboxStylesDto? TextboxStyles { get; set; }
        public FrontendMiscValuesDto? MiscValues { get; set; }
        public List<FrontendDbMetaDto>? DbMeta { get; set; }
        public int PageIndex { get; set; }
        public string Section { get; set; } = "Details";

        /// <summary>
        /// Optional data binding for formula/running-total references.
        /// The frontend may include this for elements bound to @FormulaName or @RunningTotalName.
        /// </summary>
        public FrontendDataBindingDto? DataBinding { get; set; }
    }

    public class FrontendBorderStylesDto
    {
        public string? Style { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }

        [JsonPropertyName("backgroundcolor")]
        public string? BackgroundColor { get; set; }
    }

    public class FrontendTextboxStylesDto
    {
        public string? FontFamily { get; set; }
        public string? FontSize { get; set; }
        public string? FontWeight { get; set; }
        public string? FontStyle { get; set; }
        public string? Color { get; set; }
        public string? TextDecoration { get; set; }
        public string? Horizontal { get; set; }
        public string? Vertical { get; set; }
        public string? PaddingLeft { get; set; }
        public string? PaddingRight { get; set; }
        public string? PaddingTop { get; set; }
        public string? PaddingBottom { get; set; }
        public string? LineHeight { get; set; }
        public string? WritingMode { get; set; }
        public string? LinkTo { get; set; }
    }

    public class FrontendMiscValuesDto
    {
        public string? Tooltip { get; set; }
        public string? DocumentMap { get; set; }
        public bool CanGrow { get; set; }
        public bool CanShrink { get; set; }
    }

    public class FrontendDbMetaDto
    {
        public string? Db { get; set; }
        public string? Table { get; set; }
        public string? Column { get; set; }
        public string? Type { get; set; }
        public string? Alias { get; set; }
        public string? TableType { get; set; }
        public bool IsProcedureParameter { get; set; }
        public bool IsProcedureResultColumn { get; set; }
        public string? ProcedureName { get; set; }
        public string? ParameterMode { get; set; }
        public List<FrontendProcedureParameterDto>? ProcedureParameters { get; set; }
    }

    public class FrontendProcedureParameterDto
    {
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? DataType { get; set; }
    }

    public class FrontendDataBindingDto
    {
        public string? Field { get; set; }
        public string? Formula { get; set; }
        public string? Format { get; set; }
        public string? AggregateFunction { get; set; }
    }

    /// <summary>
    /// Full report design DTO matching the frontend JSON structure.
    /// Top-level properties reuse existing DTOs (Groups, RunningTotals, etc.)
    /// because the frontend already sends a compatible format for those.
    /// Only Elements differs from the internal ReportDesignDto.
    /// </summary>
    public class FrontendReportDesignDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ProjectId { get; set; }
        public PageSettingsDto PageSettings { get; set; } = new();
        public List<FrontendElementDto> Elements { get; set; } = new();
        public List<ReportParameterDto> Parameters { get; set; } = new();
        public List<ReportDataSourceDto> DataSources { get; set; } = new();
        public List<GroupingDefinitionDto> Groups { get; set; } = new();
        public List<SortingDto> Sorting { get; set; } = new();
        public List<FormulaFieldDto> FormulaFields { get; set; } = new();
        public List<RunningTotalDto> RunningTotals { get; set; } = new();
        public List<ConditionalFormatDto> ConditionalFormats { get; set; } = new();
    }
}
