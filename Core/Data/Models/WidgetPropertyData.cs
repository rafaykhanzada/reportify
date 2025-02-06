using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    public class WidgetPropertyData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public int propId { get; set; }
       
        [ForeignKey("propId")]
        public WidgetProperty WidgetProperty { get; set; }
    }
}
