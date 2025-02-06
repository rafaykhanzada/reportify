using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    public class WidgetSettings
    {
        public int Id { get; set; }
        public string Name { get; set; }
       
        public ICollection<Widgets> Widgets { get; set; } = new List<Widgets>();//many-to many

        public ICollection<WidgetProperty> WidgetProperty { get; set; }//one to many
    }
}
