using Core.Data.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Models
{
    /// <summary>
    /// Represents a data source for the report (table, stored procedure, query)
    /// </summary>
    public class ReportDataSource : HasIdDate
    {
        public int ReportId { get; set; }
        
        /// <summary>
        /// Name identifier for this data source
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Type: Table, StoredProcedure, Query, API
        /// </summary>
        public string SourceType { get; set; }
        
        /// <summary>
        /// Connection string (encrypted)
        /// </summary>
        public string? ConnectionString { get; set; }
        
        /// <summary>
        /// Table name, SP name, or query text
        /// </summary>
        public string? SourceDefinition { get; set; }
        
        /// <summary>
        /// Parameters for stored procedure or query (JSON)
        /// </summary>
        public string? Parameters { get; set; }
        
        /// <summary>
        /// Whether this is the primary data source
        /// </summary>
        public bool IsPrimary { get; set; }
        
        /// <summary>
        /// Relationship to parent data source (for sub-reports)
        /// </summary>
        public string? ParentRelationship { get; set; }

        [ForeignKey(nameof(ReportId))]
        public virtual Report? Report { get; set; }
    }
}
