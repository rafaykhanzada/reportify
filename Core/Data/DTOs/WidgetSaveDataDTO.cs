using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.DTOs
{
    public class WidgetSaveDataDTO
    {

        public string? pName { get; set; }
        public string? pLabel { get; set; }
        public string? pType { get; set; }
        public int? pWId { get; set; }

        public string pValue { get; set; }
        public int pId { get; set; }
        public int RId { get; set; }
    }
    public class SaveReportDTO
    {
        public List<WidgetSaveDataDTO> widgetSaveData { get; set; }
    }
}
