using Core.Data.DTOs.ReportDesigner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.IService;
using System.Data;
using System.Text;

namespace Service.Service
{
    /// <summary>
    /// Enhanced HTML report rendering with advanced grouping and formula support
    /// </summary>
    public class EnhancedHtmlReportRenderService : IHtmlReportRenderService
    {
        private readonly ILogger<EnhancedHtmlReportRenderService> _logger;
        private readonly IReportDesignerService _reportDesignerService;
        private readonly IDynamicDbContextService _dynamicDbContextService;
        private readonly IFormulaEvaluationService _formulaEvaluationService;

        public EnhancedHtmlReportRenderService(
            ILogger<EnhancedHtmlReportRenderService> logger,
            IReportDesignerService reportDesignerService,
            IDynamicDbContextService dynamicDbContextService,
            IFormulaEvaluationService formulaEvaluationService)
        {
            _logger = logger;
            _reportDesignerService = reportDesignerService;
            _dynamicDbContextService = dynamicDbContextService;
            _formulaEvaluationService = formulaEvaluationService;
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
                        { "GeneratedAt", DateTime.UtcNow },
                        { "GroupCount", design.Groups.Count },
                        { "FormulaFieldCount", design.FormulaFields.Count }
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
            var primaryDataSet = dataSets.Values.FirstOrDefault();

            if (primaryDataSet != null)
            {
                // Calculate formula fields
                var calculatedFields = CalculateFormulaFields(design, primaryDataSet, parameters);

                // Initialize running totals
                var runningTotals = InitializeRunningTotals(design);

                // Generate report container
                html.AppendLine($"<div class=\"report-container\" style=\"width: {design.PageSettings.Width}mm; margin: 0 auto;\">");

                // Render sections (names match the frontend: "Page Header", "Report Header", etc.)
                await RenderSection(html, design, "Page Header", primaryDataSet, null, parameters, calculatedFields);
                await RenderSection(html, design, "Report Header", primaryDataSet, null, parameters, calculatedFields);
                
                // Render details with grouping support
                if (design.Groups.Any())
                {
                    await RenderGroupedDetails(html, design, primaryDataSet, parameters, calculatedFields, runningTotals);
                }
                else
                {
                    await RenderDetailsSection(html, design, primaryDataSet, parameters, calculatedFields, runningTotals);
                }
                
                await RenderSection(html, design, "Report Footer", primaryDataSet, null, parameters, calculatedFields);
                await RenderSection(html, design, "Page Footer", primaryDataSet, null, parameters, calculatedFields);

                html.AppendLine("</div>");
            }

            if (standalone)
            {
                html.AppendLine("</body>");
                html.AppendLine("</html>");
            }

            return html.ToString();
        }

        private Dictionary<string, object> CalculateFormulaFields(ReportDesignDto design, DataTable dataTable, Dictionary<string, object>? parameters)
        {
            var calculatedFields = new Dictionary<string, object>();

            foreach (var formulaField in design.FormulaFields.Where(f => f.EvaluationContext == FormulaEvaluationContext.Report))
            {
                var context = new FormulaContext
                {
                    AllData = dataTable,
                    Parameters = parameters,
                    CalculatedFields = calculatedFields
                };

                var result = _formulaEvaluationService.Evaluate(formulaField.Formula, context);
                if (result != null)
                {
                    calculatedFields[formulaField.Name] = result;
                }
            }

            return calculatedFields;
        }

        private Dictionary<string, double> InitializeRunningTotals(ReportDesignDto design)
        {
            var runningTotals = new Dictionary<string, double>();
            foreach (var rt in design.RunningTotals)
            {
                runningTotals[rt.Name] = 0;
            }
            return runningTotals;
        }

        private async Task RenderGroupedDetails(StringBuilder html, ReportDesignDto design, 
            DataTable dataTable, Dictionary<string, object>? parameters, 
            Dictionary<string, object> calculatedFields, Dictionary<string, double> runningTotals)
        {
            html.AppendLine($"    <div class=\"report-section section-details\">");

            // Build hierarchical groups
            var groupedData = BuildGroupHierarchy(dataTable, design.Groups, parameters, calculatedFields);
            
            // Render groups recursively
            RenderGroupLevel(html, design, groupedData, 0, dataTable, parameters, calculatedFields, runningTotals);

            html.AppendLine("    </div>");
        }

        private GroupNode BuildGroupHierarchy(DataTable dataTable, List<GroupingDefinitionDto> groups, 
            Dictionary<string, object>? parameters, Dictionary<string, object> calculatedFields)
        {
            var rootNode = new GroupNode { Level = 0, GroupValue = "Root" };

            if (!groups.Any())
            {
                rootNode.Rows = dataTable.AsEnumerable().ToList();
                return rootNode;
            }

            // Build hierarchy level by level
            var currentLevelNodes = new List<GroupNode> { rootNode };

            foreach (var group in groups.OrderBy(g => g.Level))
            {
                var nextLevelNodes = new List<GroupNode>();

                foreach (var parentNode in currentLevelNodes)
                {
                    var rowsToGroup = parentNode.Rows ?? dataTable.AsEnumerable().ToList();
                    var grouped = GroupRows(rowsToGroup, group, parameters, calculatedFields);

                    foreach (var kvp in grouped)
                    {
                        var childNode = new GroupNode
                        {
                            Level = group.Level,
                            GroupDefinition = group,
                            GroupValue = kvp.Key,
                            Rows = kvp.Value,
                            Parent = parentNode
                        };
                        parentNode.Children.Add(childNode);
                        nextLevelNodes.Add(childNode);
                    }
                }

                currentLevelNodes = nextLevelNodes;
            }

            return rootNode;
        }

        private Dictionary<object, List<DataRow>> GroupRows(List<DataRow> rows, GroupingDefinitionDto group, 
            Dictionary<string, object>? parameters, Dictionary<string, object> calculatedFields)
        {
            var grouped = new Dictionary<object, List<DataRow>>();

            foreach (var row in rows)
            {
                object groupValue;

                if (!string.IsNullOrEmpty(group.Expression))
                {
                    // Evaluate group expression
                    var context = new FormulaContext
                    {
                        CurrentRow = row,
                        Parameters = parameters,
                        CalculatedFields = calculatedFields
                    };
                    groupValue = _formulaEvaluationService.Evaluate(group.Expression, context) ?? "NULL";
                }
                else if (!string.IsNullOrEmpty(group.Field))
                {
                    // Simple field grouping
                    groupValue = row[group.Field] ?? DBNull.Value;
                }
                else
                {
                    groupValue = "Ungrouped";
                }

                if (!grouped.ContainsKey(groupValue))
                {
                    grouped[groupValue] = new List<DataRow>();
                }
                grouped[groupValue].Add(row);
            }

            // Sort groups
            var sortedGroups = group.SortDirection == "Desc" 
                ? grouped.OrderByDescending(kvp => kvp.Key) 
                : grouped.OrderBy(kvp => kvp.Key);

            return sortedGroups.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private void RenderGroupLevel(StringBuilder html, ReportDesignDto design, GroupNode node, 
            int currentLevel, DataTable allData, Dictionary<string, object>? parameters, 
            Dictionary<string, object> calculatedFields, Dictionary<string, double> runningTotals)
        {
            if (node.Children.Any())
            {
                foreach (var childNode in node.Children)
                {
                    RenderGroupNode(html, design, childNode, allData, parameters, calculatedFields, runningTotals);
                }
            }
            else if (node.Rows != null && node.Rows.Any())
            {
                var elements = design.Elements
                    .Where(e => e.Section == "Details")
                    .OrderBy(e => e.ZIndex)
                    .ToList();

                int recordNumber = 1;
                foreach (var row in node.Rows)
                {
                    // Update running totals BEFORE rendering so the current row's value is included
                    UpdateRunningTotals(design, row, runningTotals);

                    // Merge running totals into calculatedFields so @RunningTotalName resolves
                    var mergedFields = new Dictionary<string, object>(calculatedFields);
                    foreach (var kvp in runningTotals)
                        mergedFields[kvp.Key] = kvp.Value;

                    var context = new FormulaContext
                    {
                        CurrentRow = row,
                        AllData = allData,
                        Parameters = parameters,
                        CalculatedFields = mergedFields,
                        RunningTotals = runningTotals.ToDictionary(k => k.Key, v => (object)v.Value),
                        RecordNumber = recordNumber++
                    };

                    html.AppendLine("        <div class=\"detail-row\">");

                    foreach (var element in elements)
                    {
                        var appliedElement = ApplyConditionalFormatting(element, design.ConditionalFormats, context);
                        html.AppendLine(RenderElement(appliedElement, row, parameters, mergedFields, context));
                    }

                    html.AppendLine("        </div>");
                }
            }
        }

        private void RenderGroupNode(StringBuilder html, ReportDesignDto design, GroupNode node, 
            DataTable allData, Dictionary<string, object>? parameters, 
            Dictionary<string, object> calculatedFields, Dictionary<string, double> runningTotals)
        {
            var group = node.GroupDefinition!;

            // Merge group summaries into calculatedFields so elements can reference them via @SummaryName
            var groupSummaries = CalculateGroupSummaries(group, node.Rows!, new FormulaContext
            {
                GroupRows = node.Rows,
                AllData = allData,
                Parameters = parameters,
                CalculatedFields = calculatedFields
            });
            var mergedFields = new Dictionary<string, object>(calculatedFields);
            foreach (var kvp in groupSummaries)
                mergedFields[kvp.Key] = kvp.Value;

            var groupContext = new FormulaContext
            {
                CurrentRow = node.Rows?.FirstOrDefault(),
                GroupRows = node.Rows,
                AllData = allData,
                Parameters = parameters,
                CalculatedFields = mergedFields,
                RunningTotals = runningTotals.ToDictionary(k => k.Key, v => (object)v.Value),
                GroupValues = new Dictionary<string, object> { { group.GroupName, node.GroupValue } }
            };

            // Reset running totals that are configured to reset on this group
            ResetRunningTotals(design, group.GroupName, runningTotals);

            // Render group header
            if (group.ShowHeader)
            {
                var headerSectionName = $"Group Header {group.Level}";
                var headerElements = design.Elements
                    .Where(e => e.Section == headerSectionName)
                    .OrderBy(e => e.ZIndex)
                    .ToList();

                html.AppendLine($"        <div class=\"group-header group-level-{group.Level}\">");

                if (headerElements.Any())
                {
                    foreach (var element in headerElements)
                    {
                        html.AppendLine(RenderElement(element, groupContext.CurrentRow, parameters, mergedFields, groupContext));
                    }
                }
                else if (!string.IsNullOrEmpty(group.HeaderTemplate))
                {
                    var headerText = _formulaEvaluationService.EvaluateAsString(group.HeaderTemplate, groupContext);
                    html.AppendLine($"            <strong>{headerText}</strong>");
                }
                else
                {
                    html.AppendLine($"            <strong>{group.GroupName}: {node.GroupValue}</strong>");
                }

                if (group.Summaries != null)
                {
                    foreach (var summary in group.Summaries.Where(s => s.ShowInHeader))
                    {
                        if (groupSummaries.TryGetValue(summary.Name, out var value))
                        {
                            var formatted = !string.IsNullOrEmpty(summary.Format)
                                ? string.Format($"{{0:{summary.Format}}}", value)
                                : value.ToString();
                            html.AppendLine($"            <span class=\"group-summary\">{summary.Label ?? summary.Name}: {formatted}</span>");
                        }
                    }
                }

                html.AppendLine("        </div>");
            }

            // Render child groups or detail rows
            RenderGroupLevel(html, design, node, node.Level, allData, parameters, calculatedFields, runningTotals);

            // Render group footer
            if (group.ShowFooter)
            {
                // Refresh mergedFields after detail rows (running totals may have changed)
                mergedFields = new Dictionary<string, object>(calculatedFields);
                foreach (var kvp in groupSummaries)
                    mergedFields[kvp.Key] = kvp.Value;
                foreach (var kvp in runningTotals)
                    mergedFields[kvp.Key] = kvp.Value;

                groupContext.CalculatedFields = mergedFields;
                groupContext.RunningTotals = runningTotals.ToDictionary(k => k.Key, v => (object)v.Value);

                var footerSectionName = $"Group Footer {group.Level}";
                var footerElements = design.Elements
                    .Where(e => e.Section == footerSectionName)
                    .OrderBy(e => e.ZIndex)
                    .ToList();

                html.AppendLine($"        <div class=\"group-footer group-level-{group.Level}\">");

                if (footerElements.Any())
                {
                    foreach (var element in footerElements)
                    {
                        html.AppendLine(RenderElement(element, groupContext.CurrentRow, parameters, mergedFields, groupContext));
                    }
                }
                else if (!string.IsNullOrEmpty(group.FooterTemplate))
                {
                    var footerText = _formulaEvaluationService.EvaluateAsString(group.FooterTemplate, groupContext);
                    html.AppendLine($"            {footerText}");
                }

                if (group.Summaries != null)
                {
                    foreach (var summary in group.Summaries.Where(s => s.ShowInFooter))
                    {
                        if (groupSummaries.TryGetValue(summary.Name, out var value))
                        {
                            var formatted = !string.IsNullOrEmpty(summary.Format)
                                ? string.Format($"{{0:{summary.Format}}}", value)
                                : value.ToString();
                            html.AppendLine($"            <div class=\"group-summary\"><strong>{summary.Label ?? summary.Name}:</strong> {formatted}</div>");
                        }
                    }
                }

                html.AppendLine("        </div>");
            }
        }

        private Dictionary<string, object> CalculateGroupSummaries(GroupingDefinitionDto group, 
            List<DataRow> groupRows, FormulaContext context)
        {
            var summaries = new Dictionary<string, object>();

            if (group.Summaries == null || !group.Summaries.Any())
                return summaries;

            foreach (var summary in group.Summaries)
            {
                try
                {
                    var values = groupRows
                        .Where(r => r[summary.Field] != DBNull.Value)
                        .Select(r => Convert.ToDouble(r[summary.Field]))
                        .ToList();

                    object result = summary.Function switch
                    {
                        AggregateFunction.Sum => values.Sum(),
                        AggregateFunction.Average => values.Any() ? values.Average() : 0,
                        AggregateFunction.Count => values.Count,
                        AggregateFunction.Min => values.Any() ? values.Min() : 0,
                        AggregateFunction.Max => values.Any() ? values.Max() : 0,
                        AggregateFunction.DistinctCount => values.Distinct().Count(),
                        AggregateFunction.StdDev => CalculateStdDev(values),
                        AggregateFunction.Variance => CalculateVariance(values),
                        AggregateFunction.First => groupRows.First()[summary.Field],
                        AggregateFunction.Last => groupRows.Last()[summary.Field],
                        _ => 0
                    };

                    summaries[summary.Name] = result;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error calculating group summary {SummaryName}", summary.Name);
                    summaries[summary.Name] = 0;
                }
            }

            return summaries;
        }

        private void UpdateRunningTotals(ReportDesignDto design, DataRow row, Dictionary<string, double> runningTotals)
        {
            foreach (var rt in design.RunningTotals)
            {
                if (!row.Table.Columns.Contains(rt.Field) || row[rt.Field] == DBNull.Value)
                {
                    if (rt.Function == AggregateFunction.Count)
                        runningTotals[rt.Name]++;
                    continue;
                }

                var value = Convert.ToDouble(row[rt.Field]);

                switch (rt.Function)
                {
                    case AggregateFunction.Sum:
                        runningTotals[rt.Name] += value;
                        break;
                    case AggregateFunction.Count:
                        runningTotals[rt.Name]++;
                        break;
                    case AggregateFunction.Average:
                        var countKey = $"__{rt.Name}_count";
                        var sumKey = $"__{rt.Name}_sum";
                        if (!runningTotals.ContainsKey(countKey)) runningTotals[countKey] = 0;
                        if (!runningTotals.ContainsKey(sumKey)) runningTotals[sumKey] = 0;
                        runningTotals[countKey]++;
                        runningTotals[sumKey] += value;
                        runningTotals[rt.Name] = runningTotals[sumKey] / runningTotals[countKey];
                        break;
                    case AggregateFunction.Min:
                        runningTotals[rt.Name] = runningTotals[rt.Name] == 0
                            ? value
                            : Math.Min(runningTotals[rt.Name], value);
                        break;
                    case AggregateFunction.Max:
                        runningTotals[rt.Name] = Math.Max(runningTotals[rt.Name], value);
                        break;
                }
            }
        }

        private void ResetRunningTotals(ReportDesignDto design, string groupName, Dictionary<string, double> runningTotals)
        {
            foreach (var rt in design.RunningTotals)
            {
                if (rt.ResetTime == "OnChangeOfGroup" &&
                    string.Equals(rt.ResetOn, groupName, StringComparison.OrdinalIgnoreCase))
                {
                    runningTotals[rt.Name] = 0;

                    var countKey = $"__{rt.Name}_count";
                    var sumKey = $"__{rt.Name}_sum";
                    if (runningTotals.ContainsKey(countKey)) runningTotals[countKey] = 0;
                    if (runningTotals.ContainsKey(sumKey)) runningTotals[sumKey] = 0;
                }
            }
        }

        private ReportElementDto ApplyConditionalFormatting(ReportElementDto element, 
            List<ConditionalFormatDto> formats, FormulaContext context)
        {
            if (!formats.Any())
                return element;

            var applicableFormats = formats
                .Where(f => EvaluateCondition(f.Condition, context))
                .OrderBy(f => f.Priority)
                .ToList();

            if (!applicableFormats.Any())
                return element;

            // Clone element and apply conditional styles
            var enhancedElement = element;
            foreach (var format in applicableFormats)
            {
                if (format.Style != null)
                {
                    ApplyStyleOverride(enhancedElement.Style, format.Style);
                }
            }

            return enhancedElement;
        }

        private bool EvaluateCondition(string condition, FormulaContext context)
        {
            try
            {
                var result = _formulaEvaluationService.Evaluate(condition, context);
                return Convert.ToBoolean(result);
            }
            catch
            {
                return false;
            }
        }

        private void ApplyStyleOverride(StyleDto target, StyleDto source)
        {
            if (source.Color != null) target.Color = source.Color;
            if (source.BackgroundColor != null) target.BackgroundColor = source.BackgroundColor;
            if (source.FontWeight != null) target.FontWeight = source.FontWeight;
            if (source.FontSize.HasValue) target.FontSize = source.FontSize;
            // Apply other style properties as needed
        }

        private double CalculateStdDev(List<double> values)
        {
            if (!values.Any()) return 0;
            var avg = values.Average();
            var sumOfSquares = values.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sumOfSquares / values.Count);
        }

        private double CalculateVariance(List<double> values)
        {
            if (!values.Any()) return 0;
            var avg = values.Average();
            return values.Sum(v => Math.Pow(v - avg, 2)) / values.Count;
        }

        private async Task RenderSection(StringBuilder html, ReportDesignDto design, string sectionName, 
            DataTable dataTable, DataRow? row, Dictionary<string, object>? parameters, 
            Dictionary<string, object> calculatedFields)
        {
            var elements = design.Elements
                .Where(e => e.Section == sectionName)
                .OrderBy(e => e.ZIndex)
                .ToList();

            if (!elements.Any())
                return;

            var cssClass = sectionName.ToLower().Replace(" ", "-");
            html.AppendLine($"    <div class=\"report-section section-{cssClass}\">");
            
            var dataRow = row ?? (dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null);
            var context = new FormulaContext
            {
                CurrentRow = dataRow,
                AllData = dataTable,
                Parameters = parameters,
                CalculatedFields = calculatedFields
            };

            foreach (var element in elements)
            {
                html.AppendLine(RenderElement(element, dataRow, parameters, calculatedFields, context));
            }
            
            html.AppendLine("    </div>");
        }

        private async Task RenderDetailsSection(StringBuilder html, ReportDesignDto design, 
            DataTable dataTable, Dictionary<string, object>? parameters, 
            Dictionary<string, object> calculatedFields, Dictionary<string, double> runningTotals)
        {
            html.AppendLine($"    <div class=\"report-section section-details\">");

            var elements = design.Elements
                .Where(e => e.Section == "Details")
                .OrderBy(e => e.ZIndex)
                .ToList();

            int recordNumber = 1;
            foreach (DataRow row in dataTable.Rows)
            {
                UpdateRunningTotals(design, row, runningTotals);

                var mergedFields = new Dictionary<string, object>(calculatedFields);
                foreach (var kvp in runningTotals)
                    mergedFields[kvp.Key] = kvp.Value;

                var context = new FormulaContext
                {
                    CurrentRow = row,
                    AllData = dataTable,
                    Parameters = parameters,
                    CalculatedFields = mergedFields,
                    RunningTotals = runningTotals.ToDictionary(k => k.Key, v => (object)v.Value),
                    RecordNumber = recordNumber++
                };

                html.AppendLine("        <div class=\"detail-row\">");

                foreach (var element in elements)
                {
                    var appliedElement = ApplyConditionalFormatting(element, design.ConditionalFormats, context);
                    html.AppendLine(RenderElement(appliedElement, row, parameters, mergedFields, context));
                }

                html.AppendLine("        </div>");
            }

            html.AppendLine("    </div>");
        }

        public string RenderElement(ReportElementDto element, DataRow? dataRow = null, 
            Dictionary<string, object>? parameters = null)
        {
            return RenderElement(element, dataRow, parameters, new Dictionary<string, object>(), new FormulaContext());
        }

        private string RenderElement(ReportElementDto element, DataRow? dataRow, 
            Dictionary<string, object>? parameters, Dictionary<string, object> calculatedFields, 
            FormulaContext context)
        {
            var elementType = (element.ElementType ?? "textbox").ToLower();
            var style = BuildInlineStyle(element);
            var value = GetElementValue(element, dataRow, parameters, calculatedFields, context);
            var encoded = System.Net.WebUtility.HtmlEncode(value ?? "");

            return elementType switch
            {
                "textbox" => $"<div class=\"element element-textbox\" style=\"{style}\">{encoded}</div>",
                "line" => $"<hr class=\"element element-line\" style=\"{style}\" />",
                "rectangle" => $"<div class=\"element element-rectangle\" style=\"{style}\"></div>",
                "image" => RenderImageElement(element, style),
                _ => $"<div class=\"element element-{elementType}\" style=\"{style}\">{encoded}</div>"
            };
        }

        private string RenderImageElement(ReportElementDto element, string style)
        {
            var props = DeserializeProperties<ImagePropertiesDto>(element.Properties);
            var src = props?.Source ?? "";
            var alt = props?.AlternateText ?? "";
            return $"<img class=\"element element-image\" src=\"{src}\" alt=\"{alt}\" style=\"{style}\" />";
        }

        private string? GetElementValue(ReportElementDto element, DataRow? dataRow, 
            Dictionary<string, object>? parameters, Dictionary<string, object> calculatedFields, 
            FormulaContext context)
        {
            if (element.DataBinding != null)
            {
                if (!string.IsNullOrEmpty(element.DataBinding.Formula))
                {
                    context.CalculatedFields = calculatedFields;
                    return _formulaEvaluationService.EvaluateAsString(element.DataBinding.Formula, context, element.DataBinding.Format);
                }

                if (!string.IsNullOrEmpty(element.DataBinding.Field) && dataRow != null 
                    && dataRow.Table.Columns.Contains(element.DataBinding.Field))
                {
                    var value = dataRow[element.DataBinding.Field];
                    if (!string.IsNullOrEmpty(element.DataBinding.Format) && value != null && value != DBNull.Value)
                    {
                        return string.Format($"{{0:{element.DataBinding.Format}}}", value);
                    }
                    return value?.ToString();
                }
            }

            // Fall back to static text from Properties
            var props = DeserializeProperties<TextBoxPropertiesDto>(element.Properties);
            return props?.StaticText;
        }

        private T? DeserializeProperties<T>(object? properties) where T : class
        {
            if (properties == null) return null;
            if (properties is T typed) return typed;

            try
            {
                return JsonConvert.DeserializeObject<T>(properties.ToString()!);
            }
            catch
            {
                return null;
            }
        }

        private string BuildInlineStyle(ReportElementDto element)
        {
            var styles = new List<string>();
            var layout = element.Layout;
            var s = element.Style;

            styles.Add("position: absolute");
            styles.Add($"left: {layout.X.ToString(System.Globalization.CultureInfo.InvariantCulture)}px");
            styles.Add($"top: {layout.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)}px");
            styles.Add($"width: {layout.Width.ToString(System.Globalization.CultureInfo.InvariantCulture)}px");
            styles.Add($"height: {layout.Height.ToString(System.Globalization.CultureInfo.InvariantCulture)}px");

            if (!string.IsNullOrEmpty(s.FontFamily))
                styles.Add($"font-family: {s.FontFamily}");
            if (s.FontSize.HasValue)
                styles.Add($"font-size: {s.FontSize}px");
            if (!string.IsNullOrEmpty(s.FontWeight) && s.FontWeight != "normal")
                styles.Add($"font-weight: {s.FontWeight}");
            if (!string.IsNullOrEmpty(s.FontStyle) && s.FontStyle != "normal")
                styles.Add($"font-style: {s.FontStyle}");
            if (!string.IsNullOrEmpty(s.TextDecoration) && s.TextDecoration != "none")
                styles.Add($"text-decoration: {s.TextDecoration}");
            if (!string.IsNullOrEmpty(s.TextAlign) && s.TextAlign != "left")
                styles.Add($"text-align: {s.TextAlign}");
            if (!string.IsNullOrEmpty(s.Color))
                styles.Add($"color: {s.Color}");
            if (!string.IsNullOrEmpty(s.BackgroundColor))
                styles.Add($"background-color: {s.BackgroundColor}");

            if (s.Border != null && s.Border.Style != "none")
            {
                styles.Add($"border: {s.Border.Width ?? 1}px {s.Border.Style} {s.Border.Color ?? "#000"}");
                if (s.Border.Radius.HasValue && s.Border.Radius > 0)
                    styles.Add($"border-radius: {s.Border.Radius}px");
            }

            if (s.Padding != null)
                styles.Add($"padding: {s.Padding.Top}px {s.Padding.Right}px {s.Padding.Bottom}px {s.Padding.Left}px");

            if (s.Opacity.HasValue && s.Opacity < 1.0)
                styles.Add($"opacity: {s.Opacity}");

            styles.Add("overflow: hidden");

            return string.Join("; ", styles);
        }

        public string GenerateStyles(ReportDesignDto design)
        {
            var css = new StringBuilder();
            css.AppendLine("* { box-sizing: border-box; margin: 0; padding: 0; }");
            css.AppendLine("body { font-family: Arial, sans-serif; font-size: 12pt; }");
            css.AppendLine(".report-container { background: white; position: relative; }");
            css.AppendLine(".report-section { position: relative; page-break-inside: avoid; }");
            css.AppendLine(".element { display: block; }");
            css.AppendLine(".detail-row { position: relative; min-height: 20px; }");
            css.AppendLine(".group-header { background-color: #e8e8e8; padding: 10px; font-weight: bold; margin-top: 10px; border-left: 4px solid #3498db; }");
            css.AppendLine(".group-footer { background-color: #f4f4f4; padding: 10px; margin-bottom: 10px; border-top: 2px solid #bdc3c7; }");
            css.AppendLine(".group-summary { margin-left: 20px; padding: 5px 10px; display: inline-block; }");
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
                    DataTable? dataTable = await LoadDataSource(dataSource, parameters);
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

        private async Task<DataTable?> LoadDataSource(ReportDataSourceDto dataSource, Dictionary<string, object>? parameters)
        {
            if (string.IsNullOrEmpty(dataSource.ConnectionString) || string.IsNullOrEmpty(dataSource.SourceDefinition))
                return null;

            using var connection = new SqlConnection(dataSource.ConnectionString + ";TrustServerCertificate=True");
            await connection.OpenAsync();

            SqlCommand command;
            
            if (dataSource.SourceType == "StoredProcedure")
            {
                command = new SqlCommand(dataSource.SourceDefinition, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

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
            }
            else
            {
                command = new SqlCommand(dataSource.SourceDefinition, connection);
            }

            var adapter = new SqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }

        // Helper class for group hierarchy
        private class GroupNode
        {
            public int Level { get; set; }
            public GroupingDefinitionDto? GroupDefinition { get; set; }
            public object? GroupValue { get; set; }
            public List<DataRow>? Rows { get; set; }
            public List<GroupNode> Children { get; set; } = new();
            public GroupNode? Parent { get; set; }
        }
    }
}
