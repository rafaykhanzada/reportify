using Core.Data.DTOs.ReportDesigner;
using System.Globalization;

namespace Core.Utils
{
    /// <summary>
    /// Converts between the frontend designer element format and the internal ReportElementDto format.
    /// </summary>
    public static class FrontendElementConverter
    {
        public static ReportDesignDto ToReportDesignDto(this FrontendReportDesignDto frontend)
        {
            return new ReportDesignDto
            {
                Id = frontend.Id,
                Name = frontend.Name,
                Description = frontend.Description,
                ProjectId = frontend.ProjectId,
                PageSettings = frontend.PageSettings,
                Parameters = frontend.Parameters,
                DataSources = frontend.DataSources,
                Groups = frontend.Groups,
                Sorting = frontend.Sorting,
                FormulaFields = frontend.FormulaFields,
                RunningTotals = frontend.RunningTotals,
                ConditionalFormats = frontend.ConditionalFormats,
                Elements = frontend.Elements.Select(ToReportElementDto).ToList()
            };
        }

        public static ReportElementDto ToReportElementDto(FrontendElementDto fe)
        {
            var element = new ReportElementDto
            {
                ElementType = fe.Type ?? "TextBox",
                Name = fe.Name ?? fe.Id,
                Section = fe.Section,
                ZIndex = fe.PageIndex,
                Layout = new LayoutDto
                {
                    X = fe.Left,
                    Y = fe.Top,
                    Width = fe.Width,
                    Height = fe.Height
                },
                Style = BuildStyle(fe),
                DataBinding = BuildDataBinding(fe),
                Properties = BuildProperties(fe),
                VisibilityFormula = null
            };

            return element;
        }

        private static StyleDto BuildStyle(FrontendElementDto fe)
        {
            var style = new StyleDto();

            if (fe.TextboxStyles != null)
            {
                var ts = fe.TextboxStyles;
                style.FontFamily = ts.FontFamily;
                style.FontSize = ParsePixelValue(ts.FontSize);
                style.FontWeight = ts.FontWeight;
                style.FontStyle = ts.FontStyle;
                style.Color = ts.Color;
                style.TextDecoration = ts.TextDecoration;
                style.TextAlign = MapHorizontalAlignment(ts.Horizontal);
                style.VerticalAlign = MapVerticalAlignment(ts.Vertical);
                style.WordWrap = "normal";

                style.Padding = new PaddingDto
                {
                    Left = ParsePixelValue(ts.PaddingLeft) ?? 0,
                    Right = ParsePixelValue(ts.PaddingRight) ?? 0,
                    Top = ParsePixelValue(ts.PaddingTop) ?? 0,
                    Bottom = ParsePixelValue(ts.PaddingBottom) ?? 0
                };
            }

            if (fe.BorderStyles != null)
            {
                var bs = fe.BorderStyles;
                style.BackgroundColor = bs.BackgroundColor;
                style.Border = new BorderDto
                {
                    Style = bs.Style ?? "none",
                    Color = bs.Color ?? "#000000",
                    Width = ParsePixelValue(bs.Size) ?? 1
                };
            }

            return style;
        }

        private static DataBindingDto? BuildDataBinding(FrontendElementDto fe)
        {
            if (fe.DataBinding != null)
            {
                return new DataBindingDto
                {
                    Field = fe.DataBinding.Field,
                    Formula = fe.DataBinding.Formula,
                    Format = fe.DataBinding.Format,
                    AggregateFunction = fe.DataBinding.AggregateFunction
                };
            }

            var meta = fe.DbMeta?.FirstOrDefault();
            if (meta != null && !string.IsNullOrEmpty(meta.Column))
            {
                return new DataBindingDto
                {
                    DataSource = meta.ProcedureName ?? meta.Table,
                    Field = meta.Alias ?? meta.Column
                };
            }

            return null;
        }

        private static object? BuildProperties(FrontendElementDto fe)
        {
            bool hasStaticText = !string.IsNullOrEmpty(fe.Text) && fe.DbMeta?.Any() != true && fe.DataBinding == null;
            bool hasMisc = fe.MiscValues != null;

            if (!hasStaticText && !hasMisc)
                return null;

            return new TextBoxPropertiesDto
            {
                StaticText = hasStaticText ? fe.Text : null,
                CanGrow = fe.MiscValues?.CanGrow ?? false,
                CanShrink = fe.MiscValues?.CanShrink ?? false,
                HyperlinkUrl = fe.TextboxStyles?.LinkTo != "none" ? fe.TextboxStyles?.LinkTo : null
            };
        }

        private static double? ParsePixelValue(string? value)
        {
            if (string.IsNullOrEmpty(value) || value == "none")
                return null;

            var numeric = value.Replace("px", "").Replace("pt", "").Trim();
            if (double.TryParse(numeric, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            return null;
        }

        private static string? MapHorizontalAlignment(string? value)
        {
            return value switch
            {
                "left" => "left",
                "center" => "center",
                "right" => "right",
                "justify" => "justify",
                "default" => "left",
                _ => "left"
            };
        }

        private static string? MapVerticalAlignment(string? value)
        {
            return value switch
            {
                "top" => "top",
                "middle" => "middle",
                "bottom" => "bottom",
                "default" => "top",
                _ => "top"
            };
        }
    }
}
