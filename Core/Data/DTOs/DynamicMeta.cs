namespace Core.Data.DTOs
{



    public class DynamicWidgetMeta
    {
        public string? id { get; set; }
        public string? wId { get; set; }
        public string? type { get; set; }
        public float? left { get; set; }
        public float? top { get; set; }
        public float? width { get; set; }
        public float? height { get; set; }
        public string? text { get; set; }
        public Borderstyles? borderStyles { get; set; }
        public Textboxstyles? textboxStyles { get; set; }
        public int? pageIndex { get; set; }
        public string? name { get; set; }
        public string? displayText { get; set; }
        public Miscvalues? miscValues { get; set; }
        public string? tooltip { get; set; }
        public string? imageUrl { get; set; }
        public Dbmeta[]? dbMeta { get; set; }
        public Tabledata? tableData { get; set; }
        public Symbology? Symbology { get; set; }
    }

    public class Symbology
    {
        public string? Value { get; set; }
        public string? Type { get; set; }
        public  bool Visibility { get; set; }
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
    }
    public class Tabledata
    {
        public List<TabledataHeaders?>? Headers { get; set; }
        public List<List<string?>>? Rows { get; set; }
    }
    public class TabledataHeaders
    {
        public string? Column { get; set; }
        public string? Width { get; set; }
    }
    public class Dbmeta
    {
        public string? db { get; set; }
        public string? table { get; set; }
        public string? column { get; set; }
        public string? type { get; set; }
        public string? alias { get; set; }
        public string? tabletype { get; set; }
        public bool? isProcedureParameter { get; set; }
        public bool? isProcedureResultColumn { get; set; }
        public string? procedureName { get; set; }
        public string? parameterMode { get; set; }
        public Procedureparameter[]? procedureParameters { get; set; }
    }
    public class Procedureparameter
    {
        public string? name { get; set; }
        public string? value { get; set; }
        public string? dataType { get; set; }
    }



}
