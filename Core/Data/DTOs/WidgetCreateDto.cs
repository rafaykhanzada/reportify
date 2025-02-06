using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.DTOs
{
    public class WidgetCreateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DefaultImage { get; set; }
        public string DragImage { get; set; }
    }
}
