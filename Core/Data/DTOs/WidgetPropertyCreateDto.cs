using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.DTOs
{
    public class WidgetPropertyCreateDto
    {
        public int Id { get; set; }
        public string pName { get; set; }
        public string? pType { get; set; }
        public string? pValue { get; set; }
        public string? pLabel { get; set; }
        public string? Datasource { get; set; }
        public int WsId { get; set; }
        public int WidgetId { get; set; }
    }
}
