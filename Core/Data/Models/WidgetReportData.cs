using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    public class WidgetReportData
    {
        public int Id { get; set; }

        public string ReportName { get; set; }

        public int Count { get; set; }

        // Navigation property for the one-to-many relationship
        public ICollection<WidgetSaveData> WidgetSaveData { get; set; }

    }
}
