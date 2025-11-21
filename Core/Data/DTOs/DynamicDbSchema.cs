namespace Core.Data.DTOs
{
    public class DynamicDbObject
    {
        public string? ObjectName { get; set; }
        public List<Procedureparameter>? Parameter { get; set; }
        public List<DynamicDbSchema>? Columns { get; set; }
    }
    public class DynamicDbSchema
    {
        public string? ColumnName { get; set; }
        public int? ColumnOrdinal { get; set; }
        public int? ColumnSize { get; set; }
        public int? NumericPrecision { get; set; }
        public bool? IsUnique { get; set; }
        public string? DataType { get; set; }
        public bool? AllowDBNull { get; set; }
        public int? ProviderType { get; set; }
        public bool? IsIdentity { get; set; }
        public bool? IsAutoIncrement { get; set; }
        public bool? IsColumnSet { get; set; }
    }

}
