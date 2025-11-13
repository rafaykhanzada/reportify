namespace Core.Data.DTOs
{
    public class ProjectDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsAchive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public List<ReportDto>? Reports { get; set; } = new();
    }
}
