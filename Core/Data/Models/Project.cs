using Core.Data.Common;

namespace Core.Data.Models
{
    public class Project : HasIdDate
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsAchive { get; set; }
    }
}
