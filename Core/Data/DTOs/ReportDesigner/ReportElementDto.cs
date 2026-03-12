namespace Core.Data.DTOs.ReportDesigner
{
    /// <summary>
    /// Represents any element in the report (TextBox, Table, Chart, etc.)
    /// </summary>
    public class ReportElementDto
    {
        public int? Id { get; set; }
        public string ElementType { get; set; } // TextBox, Table, Chart, Image, Line, Rectangle, etc.
        public string? Name { get; set; }
        public string Section { get; set; } = "Details"; // PageHeader, ReportHeader, Details, ReportFooter, PageFooter, GroupHeader1, GroupFooter1
        public int ZIndex { get; set; }
        
        /// <summary>
        /// Position and size
        /// </summary>
        public LayoutDto Layout { get; set; } = new();
        
        /// <summary>
        /// Visual styling
        /// </summary>
        public StyleDto Style { get; set; } = new();
        
        /// <summary>
        /// Data binding
        /// </summary>
        public DataBindingDto? DataBinding { get; set; }
        
        /// <summary>
        /// Element-specific properties
        /// </summary>
        public object? Properties { get; set; }
        
        /// <summary>
        /// Conditional visibility formula
        /// </summary>
        public string? VisibilityFormula { get; set; }
    }

    public class LayoutDto
    {
        public double X { get; set; } // Left position in mm
        public double Y { get; set; } // Top position in mm
        public double Width { get; set; } // Width in mm
        public double Height { get; set; } // Height in mm
    }

    public class StyleDto
    {
        // Font
        public string? FontFamily { get; set; } = "Arial";
        public double? FontSize { get; set; } = 12;
        public string? FontWeight { get; set; } = "normal"; // normal, bold
        public string? FontStyle { get; set; } = "normal"; // normal, italic
        public string? TextDecoration { get; set; } = "none"; // none, underline
        public string? TextAlign { get; set; } = "left"; // left, center, right, justify
        public string? VerticalAlign { get; set; } = "top"; // top, middle, bottom
        
        // Colors
        public string? Color { get; set; } = "#000000";
        public string? BackgroundColor { get; set; }
        
        // Border
        public BorderDto? Border { get; set; }
        
        // Padding
        public PaddingDto? Padding { get; set; }
        
        // Other
        public string? WordWrap { get; set; } = "normal"; // normal, break-word
        public double? Opacity { get; set; } = 1.0;
    }

    public class BorderDto
    {
        public string? Style { get; set; } = "solid"; // solid, dashed, dotted, none
        public double? Width { get; set; } = 1;
        public string? Color { get; set; } = "#000000";
        public bool? Top { get; set; } = true;
        public bool? Right { get; set; } = true;
        public bool? Bottom { get; set; } = true;
        public bool? Left { get; set; } = true;
        public double? Radius { get; set; } = 0;
    }

    public class PaddingDto
    {
        public double Top { get; set; } = 2;
        public double Right { get; set; } = 2;
        public double Bottom { get; set; } = 2;
        public double Left { get; set; } = 2;
    }

    public class DataBindingDto
    {
        /// <summary>
        /// Data source name
        /// </summary>
        public string? DataSource { get; set; }
        
        /// <summary>
        /// Field name from data source
        /// </summary>
        public string? Field { get; set; }
        
        /// <summary>
        /// Formula expression (if not using direct field)
        /// </summary>
        public string? Formula { get; set; }
        
        /// <summary>
        /// Aggregate function: Sum, Avg, Count, Min, Max
        /// </summary>
        public string? AggregateFunction { get; set; }
        
        /// <summary>
        /// Format string (e.g., "C2" for currency, "d" for date)
        /// </summary>
        public string? Format { get; set; }
    }

    // Specific element property classes
    public class TextBoxPropertiesDto
    {
        public string? StaticText { get; set; }
        public bool CanGrow { get; set; } = true;
        public bool CanShrink { get; set; } = false;
        public string? HyperlinkUrl { get; set; }
    }

    public class TablePropertiesDto
    {
        public List<TableColumnDto> Columns { get; set; } = new();
        public bool ShowHeader { get; set; } = true;
        public bool AlternateRowColors { get; set; } = true;
        public string? AlternateRowColor { get; set; } = "#f5f5f5";
        public bool ShowBorders { get; set; } = true;
        public double RowHeight { get; set; } = 20;
    }

    public class TableColumnDto
    {
        public string? Header { get; set; }
        public string? Field { get; set; }
        public double Width { get; set; } = 100;
        public string? Format { get; set; }
        public string? Alignment { get; set; } = "left";
        public string? AggregateFunction { get; set; }
    }

    public class ChartPropertiesDto
    {
        public string ChartType { get; set; } = "Bar"; // Bar, Line, Pie, Column, Area
        public string? Title { get; set; }
        public string? XAxisField { get; set; }
        public string? YAxisField { get; set; }
        public List<ChartSeriesDto> Series { get; set; } = new();
        public bool ShowLegend { get; set; } = true;
        public bool ShowGridLines { get; set; } = true;
    }

    public class ChartSeriesDto
    {
        public string? Name { get; set; }
        public string? ValueField { get; set; }
        public string? Color { get; set; }
    }

    public class ImagePropertiesDto
    {
        public string? Source { get; set; } // URL or base64
        public string? SourceType { get; set; } = "URL"; // URL, Base64, Field
        public string? Sizing { get; set; } = "Fit"; // Fit, Fill, Stretch, Center
        public string? AlternateText { get; set; }
    }

    public class LinePropertiesDto
    {
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public double Thickness { get; set; } = 1;
        public string? LineStyle { get; set; } = "solid";
    }

    public class RectanglePropertiesDto
    {
        public bool Filled { get; set; } = true;
        public double CornerRadius { get; set; } = 0;
    }
}
