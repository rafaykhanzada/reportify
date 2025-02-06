using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    public class Widgets
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DefaultImage { get; set; }
        public string DragImage { get; set; }
      
        public ICollection<WidgetSettings> WidgetSettings { get; set; } = new List<WidgetSettings>();

    }
}
