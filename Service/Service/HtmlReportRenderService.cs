using Core.Data.DTOs.ReportDesigner;
using Core.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.IService;
using System.Data;
using System.Text;

namespace Service.Service
{
    public class HtmlReportRenderService : IHtmlReportRenderService
    {
        private readonly ILogger<HtmlReportRenderService> _logger;
        private readonly IReportDesignerService _reportDesignerService;
        private readonly IDynamicDbContextService _dynamicDbContextService;

        public HtmlReportRenderService(
            ILogger<HtmlReportRenderService> logger,
            IReportDesignerService reportDesignerService,
            IDynamicDbContextService dynamicDbContextService)
        {
            _logger = logger;
            _reportDesignerService = reportDesignerService;
            _dynamicDbContextService = dynamicDbContextService;
        }

        public async Task<ReportRenderResultDto> RenderReport(ReportRenderRequestDto request)
        {
            try
            {
                var reportResult = await _reportDesignerService.Get(request.ReportId);
                if (!reportResult.Success || reportResult.Data == null)
                {
                    return new ReportRenderResultDto
                    {
                        Success = false,
                        Message = "Report not found"
                    };
                }

                var design = (ReportDesignDto)reportResult.Data;
                var html = await GenerateHtml(design, request.Parameters, request.Standalone, request.IncludeStyles);

                return new ReportRenderResultDto
                {
                    Success = true,
                    Content = html,
                    ContentType = "text/html",
                    Metadata = new Dictionary<string, object>
                    {
                        { "ReportName", design.Name ?? "Untitled" },
                        { "GeneratedAt", DateTime.UtcNow }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering report {ReportId}", request.ReportId);
                return new ReportRenderResultDto
                {
                    Success = false,
                    Message = $"Error rendering report: {ex.Message}"
                };
            }
        }

        public async Task<ReportRenderResultDto> RenderReportById(int reportId, Dictionary<string, object>? parameters = null)
        {
            return await RenderReport(new ReportRenderRequestDto
            {
                ReportId = reportId,
                Parameters = parameters,
                OutputFormat = "HTML",
                Standalone = true,
                IncludeStyles = true
            });
        }

        public async Task<ReportRenderResultDto> PreviewReport(int reportId)
        {
            return await RenderReportById(reportId, new Dictionary<string, object>());
        }

        private async Task<string> GenerateHtml(ReportDesignDto design, Dictionary<string, object>? parameters, bool standalone, bool includeStyles)
        {
            var html = new StringBuilder();

            if (standalone)
            {
                html.AppendLine("<!DOCTYPE html>");
                html.AppendLine("<html lang=\"en\">");
                html.AppendLine("<head>");
                html.AppendLine($"    <meta charset=\"UTF-8\">");
                html.AppendLine($"    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                html.AppendLine($"    <title>{design.Name ?? "Report"}</title>");
                
                if (includeStyles)
                {
                    html.AppendLine("    <style>");
                    html.AppendLine(GenerateStyles(design));
                    html.AppendLine("    </style>");
                }
                
                html.AppendLine("</head>");
                html.AppendLine("<body>");
            }

            // Get data from data sources
            var dataSets = await LoadDataSources(design, parameters);

            // Generate report container
            html.AppendLine($"<div class=\"report-container\" style=\"width: {design.PageSettings.Width}mm; margin: 0 auto;\">");

            // Render sections in order
            await RenderSection(html, design, "PageHeader", dataSets, parameters);
            await RenderSection(html, design, "ReportHeader", dataSets, parameters);
            await RenderDetailsSection(html, design, dataSets, parameters);
            await RenderSection(html, design, "ReportFooter", dataSets, parameters);
            await RenderSection(html, design, "PageFooter", dataSets, parameters);

            html.AppendLine("</div>");

            if (standalone)
            {
                html.AppendLine("</body>");
                html.AppendLine("</html>");
            }

            return html.ToString();
        }

        private async Task RenderSection(StringBuilder html, ReportDesignDto design, string sectionName, 
            Dictionary<string, DataTable> dataSets, Dictionary<string, object>? parameters)
        {
            var elements = design.Elements
                .Where(e => e.Section == sectionName)
                .OrderBy(e => e.ZIndex)
                .ToList();

            if (!elements.Any())
                return;

            html.AppendLine($"    <div class=\"report-section section-{sectionName.ToLower()}\">");
            
            foreach (var element in elements)
            {
                var dataRow = dataSets.Values.FirstOrDefault()?.Rows.Count > 0 
                    ? dataSets.Values.First().Rows[0] 
                    : null;
                html.AppendLine(RenderElement(element, dataRow, parameters));
            }
            
            html.AppendLine("    </div>");
        }

        private async Task RenderDetailsSection(StringBuilder html, ReportDesignDto design, 
            Dictionary<string, DataTable> dataSets, Dictionary<string, object>? parameters)
        {
            var elements = design.Elements
                .Where(e => e.Section == "Details")
                .OrderBy(e => e.ZIndex)
                .ToList();

            if (!elements.Any())
                return;

            html.AppendLine($"    <div class=\"report-section section-details\">");

            // Get primary data source
            var primaryDataSet = dataSets.Values.FirstOrDefault();
            if (primaryDataSet != null && primaryDataSet.Rows.Count > 0)
            {
                // Apply grouping if configured
                if (design.Groups.Any())
                {
                    await RenderGroupedDetails(html, design, elements, primaryDataSet, parameters);
                }
                else
                {
                    // Render each row
                    foreach (DataRow row in primaryDataSet.Rows)
                    {
                        html.AppendLine("        <div class=\"detail-row\">");
                        foreach (var element in elements)
                        {
                            html.AppendLine(RenderElement(element, row, parameters));
                        }
                        html.AppendLine("        </div>");
                    }
                }
            }

            html.AppendLine("    </div>");
        }

        private async Task RenderGroupedDetails(StringBuilder html, ReportDesignDto design, 
            List<ReportElementDto> elements, DataTable dataTable, Dictionary<string, object>? parameters)
        {
            var groups = design.Groups.OrderBy(g => g.Level).ToList();
            var groupedData = GroupData(dataTable, groups);

            RenderGroupRecursive(html, design, elements, groupedData, 0, groups, parameters);
        }

        private void RenderGroupRecursive(StringBuilder html, ReportDesignDto design, 
            List<ReportElementDto> elements, Dictionary<object, List<DataRow>> groupedData, 
            int level, List<GroupingDefinitionDto> groups, Dictionary<string, object>? parameters)
        {
            foreach (var group in groupedData)
            {
                // Render group header
                if (groups[level].ShowHeader)
                {
                    html.AppendLine($"        <div class=\"group-header group-level-{level}\">");
                    html.AppendLine($"            <strong>{groups[level].GroupName}: {group.Key}</strong>");
                    html.AppendLine("        </div>");
                }

                // Render detail rows in this group
                foreach (var row in group.Value)
                {
                    html.AppendLine("        <div class=\"detail-row\">");
                    foreach (var element in elements)
                    {
                        html.AppendLine(RenderElement(element, row, parameters));
                    }
                    html.AppendLine("        </div>");
                }

                // Render group footer
                if (groups[level].ShowFooter)
                {
                    html.AppendLine($"        <div class=\"group-footer group-level-{level}\">");
                    html.AppendLine($"            <em>End of {groups[level].GroupName}: {group.Key}</em>");
                    html.AppendLine("        </div>");
                }
            }
        }

        public string RenderElement(ReportElementDto element, DataRow? dataRow = null, Dictionary<string, object>? parameters = null)
        {
            var html = new StringBuilder();
            var style = BuildInlineStyle(element);

            switch (element.ElementType.ToLower())
            {
                case "textbox":
                    html.Append(RenderTextBox(element, dataRow, parameters, style));
                    break;
                case "table":
                    html.Append(RenderTable(element, dataRow, parameters, style));
                    break;
                case "image":
                    html.Append(RenderImage(element, dataRow, parameters, style));
                    break;
                case "line":
                    html.Append(RenderLine(element, style));
                    break;
                case "rectangle":
                    html.Append(RenderRectangle(element, style));
                    break;
                default:
                    html.Append($"<!-- Unknown element type: {element.ElementType} -->");
                    break;
            }

            return html.ToString();
        }

        private string RenderTextBox(ReportElementDto element, DataRow? dataRow, Dictionary<string, object>? parameters, string style)
        {
            var props = element.Properties != null 
                ? JsonConvert.DeserializeObject<TextBoxPropertiesDto>(element.Properties.ToString()!) 
                : new TextBoxPropertiesDto();

            string text = GetElementValue(element, dataRow, parameters) ?? props?.StaticText ?? "";

            var html = new StringBuilder();
            var tag = !string.IsNullOrEmpty(props?.HyperlinkUrl) ? "a" : "div";
            var href = !string.IsNullOrEmpty(props?.HyperlinkUrl) ? $" href=\"{props.HyperlinkUrl}\"" : "";

            html.Append($"<{tag} class=\"element element-textbox\"{href} style=\"{style}\">");
            html.Append(System.Net.WebUtility.HtmlEncode(text));
            html.Append($"</{tag}>");

            return html.ToString();
        }

        private string RenderTable(ReportElementDto element, DataRow? dataRow, Dictionary<string, object>? parameters, string style)
        {
            var props = element.Properties != null 
                ? JsonConvert.DeserializeObject<TablePropertiesDto>(element.Properties.ToString()!) 
                : new TablePropertiesDto();

            var html = new StringBuilder();
            html.Append($"<table class=\"element element-table\" style=\"{style}\">");

            // Render header
            if (props.ShowHeader && props.Columns.Any())
            {
                html.Append("<thead><tr>");
                foreach (var col in props.Columns)
                {
                    html.Append($"<th style=\"width: {col.Width}px; text-align: {col.Alignment};\">{col.Header}</th>");
                }
                html.Append("</tr></thead>");
            }

            // Render body (placeholder - would need actual data iteration)
            html.Append("<tbody>");
            html.Append("<tr>");
            foreach (var col in props.Columns)
            {
                var value = dataRow != null && !string.IsNullOrEmpty(col.Field) && dataRow.Table.Columns.Contains(col.Field)
                    ? dataRow[col.Field]?.ToString() ?? ""
                    : "";
                html.Append($"<td style=\"text-align: {col.Alignment};\">{System.Net.WebUtility.HtmlEncode(value)}</td>");
            }
            html.Append("</tr>");
            html.Append("</tbody>");

            html.Append("</table>");

            return html.ToString();
        }

        private string RenderImage(ReportElementDto element, DataRow? dataRow, Dictionary<string, object>? parameters, string style)
        {
            var props = element.Properties != null 
                ? JsonConvert.DeserializeObject<ImagePropertiesDto>(element.Properties.ToString()!) 
                : new ImagePropertiesDto();

            string src = props?.Source ?? "";
            string alt = props?.AlternateText ?? "";

            return $"<img class=\"element element-image\" src=\"{src}\" alt=\"{alt}\" style=\"{style}\" />";
        }

        private string RenderLine(ReportElementDto element, string style)
        {
            var props = element.Properties != null 
                ? JsonConvert.DeserializeObject<LinePropertiesDto>(element.Properties.ToString()!) 
                : new LinePropertiesDto();

            return $"<hr class=\"element element-line\" style=\"{style} border-style: {props?.LineStyle ?? "solid"};\" />";
        }

        private string RenderRectangle(ReportElementDto element, string style)
        {
            return $"<div class=\"element element-rectangle\" style=\"{style}\"></div>";
        }

        private string BuildInlineStyle(ReportElementDto element)
        {
            var styles = new List<string>();
            var layout = element.Layout;
            var style = element.Style;

            // Position and size
            styles.Add($"position: absolute");
            styles.Add($"left: {layout.X}mm");
            styles.Add($"top: {layout.Y}mm");
            styles.Add($"width: {layout.Width}mm");
            styles.Add($"height: {layout.Height}mm");

            // Font
            if (!string.IsNullOrEmpty(style.FontFamily))
                styles.Add($"font-family: {style.FontFamily}");
            if (style.FontSize.HasValue)
                styles.Add($"font-size: {style.FontSize}pt");
            if (!string.IsNullOrEmpty(style.FontWeight))
                styles.Add($"font-weight: {style.FontWeight}");
            if (!string.IsNullOrEmpty(style.FontStyle))
                styles.Add($"font-style: {style.FontStyle}");
            if (!string.IsNullOrEmpty(style.TextDecoration))
                styles.Add($"text-decoration: {style.TextDecoration}");

            // Alignment
            if (!string.IsNullOrEmpty(style.TextAlign))
                styles.Add($"text-align: {style.TextAlign}");
            if (!string.IsNullOrEmpty(style.VerticalAlign))
                styles.Add($"vertical-align: {style.VerticalAlign}");

            // Colors
            if (!string.IsNullOrEmpty(style.Color))
                styles.Add($"color: {style.Color}");
            if (!string.IsNullOrEmpty(style.BackgroundColor))
                styles.Add($"background-color: {style.BackgroundColor}");

            // Border
            if (style.Border != null)
            {
                var border = style.Border;
                if (border.Style != "none")
                {
                    styles.Add($"border-width: {border.Width ?? 1}px");
                    styles.Add($"border-style: {border.Style}");
                    styles.Add($"border-color: {border.Color ?? "#000000"}");
                    if (border.Radius.HasValue && border.Radius > 0)
                        styles.Add($"border-radius: {border.Radius}px");
                }
            }

            // Padding
            if (style.Padding != null)
            {
                var p = style.Padding;
                styles.Add($"padding: {p.Top}px {p.Right}px {p.Bottom}px {p.Left}px");
            }

            // Other
            if (style.Opacity.HasValue)
                styles.Add($"opacity: {style.Opacity}");

            return string.Join("; ", styles);
        }

        private string? GetElementValue(ReportElementDto element, DataRow? dataRow, Dictionary<string, object>? parameters)
        {
            if (element.DataBinding == null)
                return null;

            // Check for formula first
            if (!string.IsNullOrEmpty(element.DataBinding.Formula))
            {
                return EvaluateFormula(element.DataBinding.Formula, dataRow, parameters);
            }

            // Get field value
            if (!string.IsNullOrEmpty(element.DataBinding.Field) && dataRow != null)
            {
                if (dataRow.Table.Columns.Contains(element.DataBinding.Field))
                {
                    var value = dataRow[element.DataBinding.Field];
                    
                    // Apply formatting
                    if (!string.IsNullOrEmpty(element.DataBinding.Format) && value != null && value != DBNull.Value)
                    {
                        return string.Format($"{{0:{element.DataBinding.Format}}}", value);
                    }
                    
                    return value?.ToString();
                }
            }

            return null;
        }

        private string? EvaluateFormula(string formula, DataRow? dataRow, Dictionary<string, object>? parameters)
        {
            // Simple formula evaluation - can be enhanced with expression evaluator
            try
            {
                // Replace parameters like {ParamName}
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        formula = formula.Replace($"{{{param.Key}}}", param.Value?.ToString() ?? "");
                    }
                }

                // Replace field references like [FieldName]
                if (dataRow != null)
                {
                    foreach (DataColumn col in dataRow.Table.Columns)
                    {
                        formula = formula.Replace($"[{col.ColumnName}]", dataRow[col]?.ToString() ?? "");
                    }
                }

                return formula;
            }
            catch
            {
                return formula;
            }
        }

        public string GenerateStyles(ReportDesignDto design)
        {
            var css = new StringBuilder();

            css.AppendLine("* { box-sizing: border-box; margin: 0; padding: 0; }");
            css.AppendLine("body { font-family: Arial, sans-serif; font-size: 12pt; }");
            css.AppendLine(".report-container { background: white; position: relative; }");
            css.AppendLine(".report-section { position: relative; page-break-inside: avoid; }");
            css.AppendLine(".element { display: block; }");
            css.AppendLine(".element-table { border-collapse: collapse; width: 100%; }");
            css.AppendLine(".element-table th, .element-table td { border: 1px solid #ddd; padding: 8px; }");
            css.AppendLine(".element-table th { background-color: #f4f4f4; font-weight: bold; }");
            css.AppendLine(".detail-row { position: relative; min-height: 20px; }");
            css.AppendLine(".group-header { background-color: #e8e8e8; padding: 8px; font-weight: bold; margin-top: 10px; }");
            css.AppendLine(".group-footer { background-color: #f4f4f4; padding: 8px; font-style: italic; margin-bottom: 10px; }");
            css.AppendLine("@media print { .report-section { page-break-inside: avoid; } }");

            return css.ToString();
        }

        private async Task<Dictionary<string, DataTable>> LoadDataSources(ReportDesignDto design, Dictionary<string, object>? parameters)
        {
            var dataSets = new Dictionary<string, DataTable>();

            foreach (var dataSource in design.DataSources)
            {
                try
                {
                    DataTable? dataTable = null;

                    if (dataSource.SourceType == "Table")
                    {
                        dataTable = await LoadFromTable(dataSource, parameters);
                    }
                    else if (dataSource.SourceType == "StoredProcedure")
                    {
                        dataTable = await LoadFromStoredProcedure(dataSource, parameters);
                    }
                    else if (dataSource.SourceType == "Query")
                    {
                        dataTable = await LoadFromQuery(dataSource, parameters);
                    }

                    if (dataTable != null)
                    {
                        dataSets[dataSource.Name] = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading data source {DataSourceName}", dataSource.Name);
                }
            }

            return dataSets;
        }

        private async Task<DataTable?> LoadFromTable(ReportDataSourceDto dataSource, Dictionary<string, object>? parameters)
        {
            if (string.IsNullOrEmpty(dataSource.ConnectionString) || string.IsNullOrEmpty(dataSource.SourceDefinition))
                return null;

            using var connection = new SqlConnection(dataSource.ConnectionString + ";TrustServerCertificate=True");
            await connection.OpenAsync();

            var command = new SqlCommand($"SELECT * FROM {dataSource.SourceDefinition}", connection);
            var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }

        private async Task<DataTable?> LoadFromStoredProcedure(ReportDataSourceDto dataSource, Dictionary<string, object>? parameters)
        {
            if (string.IsNullOrEmpty(dataSource.ConnectionString) || string.IsNullOrEmpty(dataSource.SourceDefinition))
                return null;

            using var connection = new SqlConnection(dataSource.ConnectionString + ";TrustServerCertificate=True");
            await connection.OpenAsync();

            var command = new SqlCommand(dataSource.SourceDefinition, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add parameters
            if (dataSource.Parameters != null)
            {
                foreach (var param in dataSource.Parameters)
                {
                    var value = param.UseReportParameter && parameters != null && parameters.ContainsKey(param.ReportParameterName!)
                        ? parameters[param.ReportParameterName!]
                        : param.Value;

                    command.Parameters.AddWithValue($"@{param.Name}", value ?? DBNull.Value);
                }
            }

            var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }

        private async Task<DataTable?> LoadFromQuery(ReportDataSourceDto dataSource, Dictionary<string, object>? parameters)
        {
            if (string.IsNullOrEmpty(dataSource.ConnectionString) || string.IsNullOrEmpty(dataSource.SourceDefinition))
                return null;

            using var connection = new SqlConnection(dataSource.ConnectionString + ";TrustServerCertificate=True");
            await connection.OpenAsync();

            // Replace parameters in query
            var query = dataSource.SourceDefinition;
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    query = query.Replace($"@{param.Key}", $"'{param.Value}'");
                }
            }

            var command = new SqlCommand(query, connection);
            var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }

        private Dictionary<object, List<DataRow>> GroupData(DataTable dataTable, List<GroupingDefinitionDto> groups)
        {
            var grouped = new Dictionary<object, List<DataRow>>();
            
            if (!groups.Any() || !dataTable.Columns.Contains(groups[0].Field))
                return grouped;

            foreach (DataRow row in dataTable.Rows)
            {
                var key = row[groups[0].Field!];
                if (!grouped.ContainsKey(key))
                {
                    grouped[key] = new List<DataRow>();
                }
                grouped[key].Add(row);
            }

            return grouped;
        }
    }
}
