using Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Data.DTOs
{
    public class WidgetSettingsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WId { get; set; }

        public List<WidgetPropertyDto> WidgetProperty { get; set; }
        public List<WidgetDto> Widgets { get; set; }
    }
}
