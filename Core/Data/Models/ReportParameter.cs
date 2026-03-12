using Core.Data.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Models
{
    /// <summary>
    /// Represents a parameter that can be passed to the report
    /// Similar to Crystal Reports parameters
    /// </summary>
    public class ReportParameter : HasIdDate
    {
        public int ReportId { get; set; }
        
        /// <summary>
        /// Parameter name (used in query string)
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Display label
        /// </summary>
        public string? Label { get; set; }
        
        /// <summary>
        /// Data type: String, Integer, Decimal, Date, Boolean
        /// </summary>
        public string DataType { get; set; }
        
        /// <summary>
        /// Default value (JSON encoded)
        /// </summary>
        public string? DefaultValue { get; set; }
        
        /// <summary>
        /// Whether parameter is required
        /// </summary>
        public bool IsRequired { get; set; }
        
        /// <summary>
        /// Validation rules (JSON)
        /// </summary>
        public string? ValidationRules { get; set; }
        
        /// <summary>
        /// For dropdown parameters: list of allowed values (JSON)
        /// </summary>
        public string? AllowedValues { get; set; }
        
        /// <summary>
        /// Order in which parameters appear
        /// </summary>
        public int DisplayOrder { get; set; }

        [ForeignKey(nameof(ReportId))]
        public virtual Report? Report { get; set; }
    }
}
