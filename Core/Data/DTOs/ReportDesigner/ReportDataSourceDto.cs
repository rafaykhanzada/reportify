namespace Core.Data.DTOs.ReportDesigner
{
    /// <summary>
    /// Data source configuration
    /// </summary>
    public class ReportDataSourceDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string SourceType { get; set; } = "Table"; // Table, StoredProcedure, Query, API
        public string? ConnectionString { get; set; }
        public string? SourceDefinition { get; set; } // Table name, SP name, or SQL query
        public List<DataSourceParameterDto>? Parameters { get; set; }
        public bool IsPrimary { get; set; } = false;
        public string? ParentRelationship { get; set; }
    }

    public class DataSourceParameterDto
    {
        public string Name { get; set; }
        public string? Value { get; set; }
        public string DataType { get; set; } = "String";
        public bool UseReportParameter { get; set; } = false;
        public string? ReportParameterName { get; set; }
    }

    public class SortingDto
    {
        public string? Field { get; set; }
        public string Direction { get; set; } = "Asc"; // Asc, Desc
        public int Order { get; set; }
    }
}
