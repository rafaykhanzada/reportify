namespace Core.Data.DTOs
{
    /// <summary>
    /// Matches the full report design JSON sent by the frontend.
    /// Top-level wrapper containing page settings, data sources, and elements.
    /// </summary>
    public class ReportDesignMeta
    {
        public string? name { get; set; }
        public Pagesettings? pageSettings { get; set; }
        public List<Datasource>? dataSources { get; set; }
        public List<Group>? groups { get; set; }
        public List<Runningtotal>? runningTotals { get; set; }
        public List<Formulafield>? formulaFields { get; set; }
        public List<Sorting>? sorting { get; set; }
        public List<Element>? elements { get; set; }
    }

    /// <summary>
    /// A single report element as sent by the frontend designer.
    /// Used for preview population (text, table, formula widgets).
    /// </summary>
    public class Element
    {
        public string? id { get; set; }
        public int wId { get; set; }
        public string? type { get; set; }
        public string? name { get; set; }
        public string? text { get; set; }
        public float left { get; set; }
        public float top { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public Borderstyles? borderStyles { get; set; }
        public Textboxstyles? textboxStyles { get; set; }
        public Miscvalues? miscValues { get; set; }
        public Dbmeta[]? dbMeta { get; set; }
        public Tabledata? tableData { get; set; }
        public int pageIndex { get; set; }
        public string? section { get; set; }
    }

    public class Pagesettings
    {
        public string? paperSize { get; set; }
        public string? orientation { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Margins? margins { get; set; }
    }

    public class Margins
    {
        public int top { get; set; }
        public int right { get; set; }
        public int bottom { get; set; }
        public int left { get; set; }
    }

    public class Datasource
    {
        public string? name { get; set; }
        public string? sourceType { get; set; }
        public string? connectionString { get; set; }
        public string? sourceDefinition { get; set; }
        public List<Procedureparameter>? parameters { get; set; }
        public bool isPrimary { get; set; }
    }

    public class Group
    {
        public string? groupName { get; set; }
        public string? field { get; set; }
        public int level { get; set; }
        public string? sortDirection { get; set; }
        public bool showHeader { get; set; }
        public bool showFooter { get; set; }
        public string? headerTemplate { get; set; }
        public List<Summary>? summaries { get; set; }
    }

    public class Summary
    {
        public string? name { get; set; }
        public string? field { get; set; }
        public string? function { get; set; }
        public string? format { get; set; }
        public string? label { get; set; }
        public bool showInHeader { get; set; }
        public bool showInFooter { get; set; }
    }

    public class Runningtotal
    {
        public string? name { get; set; }
        public string? field { get; set; }
        public string? function { get; set; }
        public string? format { get; set; }
        public string? evaluationTime { get; set; }
        public string? resetTime { get; set; }
        public string? resetOn { get; set; }
    }

    public class Formulafield
    {
        public string? name { get; set; }
        public string? formula { get; set; }
        public string? resultType { get; set; }
        public string? format { get; set; }
        public string? evaluationContext { get; set; }
    }

    public class Sorting
    {
        public string? field { get; set; }
        public string? direction { get; set; }
        public int order { get; set; }
    }

    public class Symbology
    {
        public string? Value { get; set; }
        public string? Type { get; set; }
        public bool Visibility { get; set; }
        public string? correctionlevel { get; set; }
    }

    public class Borderstyles
    {
        public string? style { get; set; }
        public string? color { get; set; }
        public string? size { get; set; }
        public string? backgroundcolor { get; set; }
    }

    public class Textboxstyles
    {
        public string? fontFamily { get; set; }
        public string? fontSize { get; set; }
        public string? fontWeight { get; set; }
        public string? fontStyle { get; set; }
        public string? Color { get; set; }
        public string? textDecoration { get; set; }
        public string? horizontal { get; set; }
        public string? vertical { get; set; }
        public string? paddingLeft { get; set; }
        public string? paddingRight { get; set; }
        public string? paddingTop { get; set; }
        public string? paddingBottom { get; set; }
        public string? lineHeight { get; set; }
        public string? writingMode { get; set; }
        public string? linkTo { get; set; }
    }

    public class Miscvalues
    {
        public string? tooltip { get; set; }
        public string? documentMap { get; set; }
        public bool? canGrow { get; set; }
        public bool? canShrink { get; set; }
        public string? formula { get; set; }
        public bool? useFormula { get; set; }
        public string? formulaQuery { get; set; }
    }

    public class Tabledata
    {
        public List<TabledataHeaders?>? Headers { get; set; }
        public List<List<string?>>? Rows { get; set; }
    }

    public class TabledataHeaders
    {
        public string? column { get; set; }
        public string? width { get; set; }
        public ColumnFormatConfig? format { get; set; }
        public Headerstyle? headerStyle { get; set; }
        public Style? style { get; set; }
        public string? symbol { get; set; }
    }

    public class ColumnFormatConfig
    {
        public string? type { get; set; }
        public string? template { get; set; }
        public string? symbol { get; set; }
    }

    public class Headerstyle
    {
        public string? backgroundColor { get; set; }
        public string? color { get; set; }
        public string? align { get; set; }
    }

    public class Style
    {
        public string? align { get; set; }
        public string? fontSize { get; set; }
        public string? fontStyle { get; set; }
        public string? fontWeight { get; set; }
        public string? color { get; set; }
    }

    public class Dbmeta
    {
        public string? db { get; set; }
        public string? table { get; set; }
        public string? column { get; set; }
        public string? type { get; set; }
        public string? alias { get; set; }
        public string? source { get; set; }
        public string? tabletype { get; set; }
        public bool? isProcedureParameter { get; set; }
        public bool? isProcedureResultColumn { get; set; }
        public string? procedureName { get; set; }
        public string? parameterMode { get; set; }
        public Procedureparameter[]? procedureParameters { get; set; }
        public string? formula { get; set; }
        public string? formulaQuery { get; set; }
    }

    public class Procedureparameter
    {
        public string? name { get; set; }
        public string? value { get; set; }
        public string? dataType { get; set; }
    }
}
