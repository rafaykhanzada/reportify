namespace Core.Data.DTOs.ReportDesigner
{
    /// <summary>
    /// Report parameter definition
    /// </summary>
    public class ReportParameterDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string? Label { get; set; }
        public string DataType { get; set; } = "String"; // String, Integer, Decimal, Date, Boolean
        public object? DefaultValue { get; set; }
        public bool IsRequired { get; set; } = false;
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Validation rules
        /// </summary>
        public ParameterValidationDto? Validation { get; set; }
        
        /// <summary>
        /// For dropdown parameters
        /// </summary>
        public List<ParameterValueDto>? AllowedValues { get; set; }
    }

    public class ParameterValidationDto
    {
        public object? MinValue { get; set; }
        public object? MaxValue { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string? Pattern { get; set; }
        public string? CustomValidation { get; set; }
    }

    public class ParameterValueDto
    {
        public string? Label { get; set; }
        public object? Value { get; set; }
    }
}
