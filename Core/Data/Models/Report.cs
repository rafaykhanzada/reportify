using Core.Data.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Data.Models
{
    public class Report:HasIdDate
    {
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public int? ProjectId { get; set; }
        public string? DataSource { get; set; }
        public string? Connection { get; set; }
        public string? Type { get; set; }
        public string? TableType { get; set; }
        public string? Widgets { get; set; }
        public string? Context { get; set; }
        [ForeignKey(nameof(ProjectId))]
        [JsonIgnore]
        public virtual Project? Project { get; set; }
    }
}
