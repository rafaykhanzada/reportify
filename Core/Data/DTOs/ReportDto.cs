namespace Core.Data.DTOs
{
    public class ReportDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? DataSource { get; set; }
        public string? Widgets { get; set; }
        public string? Connection { get; set; }
        public string? Type { get; set; }
        public string? TableType { get; set; }
        public string? Context { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsActive { get; set; }

    }
}
