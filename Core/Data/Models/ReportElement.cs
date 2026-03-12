using Core.Data.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Data.Models
{
    /// <summary>
    /// Represents a single element/widget in the report designer
    /// Similar to Crystal Reports objects (text boxes, tables, charts, etc.)
    /// </summary>
    public class ReportElement : HasIdDate
    {
        public int ReportId { get; set; }
        
        /// <summary>
        /// Type of element: TextBox, Table, Chart, Image, Line, Rectangle, Header, Footer, etc.
        /// </summary>
        public string ElementType { get; set; }
        
        /// <summary>
        /// Display name of the element
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Position and size (JSON: {x, y, width, height})
        /// </summary>
        public string? Layout { get; set; }
        
        /// <summary>
        /// Styling information (JSON: {fontSize, color, backgroundColor, border, etc.})
        /// </summary>
        public string? Style { get; set; }
        
        /// <summary>
        /// Data binding configuration (JSON: {source, field, formula, etc.})
        /// </summary>
        public string? DataBinding { get; set; }
        
        /// <summary>
        /// Element-specific properties (JSON varies by ElementType)
        /// </summary>
        public string? Properties { get; set; }
        
        /// <summary>
        /// Z-index for layering
        /// </summary>
        public int ZIndex { get; set; }
        
        /// <summary>
        /// Section: PageHeader, ReportHeader, Details, ReportFooter, PageFooter, GroupHeader, GroupFooter
        /// </summary>
        public string Section { get; set; }
        
        /// <summary>
        /// Conditional visibility expression
        /// </summary>
        public string? VisibilityFormula { get; set; }

        [ForeignKey(nameof(ReportId))]
        public virtual Report? Report { get; set; }
    }
}
