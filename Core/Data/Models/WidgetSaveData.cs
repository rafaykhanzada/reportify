using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    public class WidgetSaveData
    {
        public int Id { get; set; }

        public string pName { get; set; }
        public string pLabel { get; set; }

        public string pValue { get; set; }
        public string pType { get; set; }
        public int pWId { get; set; }
        public int pId { get; set; }
        public int RId { get; set; }
        public string pWidgetName { get; set; }

        [ForeignKey("pId")]
        public WidgetProperty WidgetProperty { get; set; }

        [ForeignKey("RId")]
        public WidgetReportData WidgetReportData { get; set; }

    }
}
