namespace Core.Data.DTOs.ReportDesigner
{
    /// <summary>
    /// Represents a custom formula field that calculates values dynamically
    /// Similar to Crystal Reports Formula Fields
    /// </summary>
    public class FormulaFieldDto
    {
        public int? Id { get; set; }
        
        /// <summary>
        /// Unique name for this formula field
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Display label
        /// </summary>
        public string? Label { get; set; }
        
        /// <summary>
        /// Formula expression
        /// </summary>
        public string Formula { get; set; }
        
        /// <summary>
        /// Result data type: String, Number, Date, Boolean
        /// </summary>
        public string ResultType { get; set; } = "String";
        
        /// <summary>
        /// Format string for the result (e.g., "C2", "N2", "d")
        /// </summary>
        public string? Format { get; set; }
        
        /// <summary>
        /// Evaluation context: Record, Group, Report
        /// </summary>
        public FormulaEvaluationContext EvaluationContext { get; set; } = FormulaEvaluationContext.Record;
        
        /// <summary>
        /// Description of what this formula does
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Category for organization
        /// </summary>
        public string? Category { get; set; }
    }

    /// <summary>
    /// When the formula should be evaluated
    /// </summary>
    public enum FormulaEvaluationContext
    {
        /// <summary>
        /// Evaluated for each data record
        /// </summary>
        Record,
        
        /// <summary>
        /// Evaluated at group level (once per group)
        /// </summary>
        Group,
        
        /// <summary>
        /// Evaluated at report level (once for entire report)
        /// </summary>
        Report
    }

    /// <summary>
    /// Enhanced grouping with expression support
    /// </summary>
    public class GroupingDefinitionDto
    {
        public int? Id { get; set; }
        
        /// <summary>
        /// Group name for display
        /// </summary>
        public string GroupName { get; set; }
        
        /// <summary>
        /// Field name to group by
        /// </summary>
        public string? Field { get; set; }
        
        /// <summary>
        /// Custom expression for grouping (alternative to Field)
        /// Example: "LEFT([CustomerName], 1)" for grouping by first letter
        /// </summary>
        public string? Expression { get; set; }
        
        /// <summary>
        /// Group level (1, 2, 3, etc. for nested groups)
        /// </summary>
        public int Level { get; set; } = 1;
        
        /// <summary>
        /// Sort direction within group: Asc, Desc
        /// </summary>
        public string SortDirection { get; set; } = "Asc";
        
        /// <summary>
        /// Show group header section
        /// </summary>
        public bool ShowHeader { get; set; } = true;
        
        /// <summary>
        /// Show group footer section
        /// </summary>
        public bool ShowFooter { get; set; } = true;
        
        /// <summary>
        /// Keep group together on same page
        /// </summary>
        public bool KeepTogether { get; set; } = false;
        
        /// <summary>
        /// Repeat group header on each page
        /// </summary>
        public bool RepeatHeaderOnNewPage { get; set; } = false;
        
        /// <summary>
        /// Custom header template (formula)
        /// </summary>
        public string? HeaderTemplate { get; set; }
        
        /// <summary>
        /// Custom footer template (formula)
        /// </summary>
        public string? FooterTemplate { get; set; }
        
        /// <summary>
        /// Summary fields to calculate for this group
        /// </summary>
        public List<GroupSummaryDto>? Summaries { get; set; }
    }

    /// <summary>
    /// Summary/aggregation field for a group
    /// </summary>
    public class GroupSummaryDto
    {
        public string Name { get; set; }
        public string Field { get; set; }
        public AggregateFunction Function { get; set; }
        public string? Format { get; set; }
        public string? Label { get; set; }
        public bool ShowInHeader { get; set; } = false;
        public bool ShowInFooter { get; set; } = true;
    }

    /// <summary>
    /// Aggregate functions for summaries
    /// </summary>
    public enum AggregateFunction
    {
        Sum,
        Average,
        Count,
        Min,
        Max,
        DistinctCount,
        StdDev,
        Variance,
        First,
        Last
    }

    /// <summary>
    /// Running total configuration
    /// </summary>
    public class RunningTotalDto
    {
        public string Name { get; set; }
        public string Field { get; set; }
        public AggregateFunction Function { get; set; } = AggregateFunction.Sum;
        public string? Format { get; set; }
        
        /// <summary>
        /// When to evaluate: OnChangeOfGroup, OnChangeOfField, ForEachRecord
        /// </summary>
        public string EvaluationTime { get; set; } = "ForEachRecord";
        
        /// <summary>
        /// When to reset: Never, OnChangeOfGroup, OnChangeOfField
        /// </summary>
        public string ResetTime { get; set; } = "Never";
        
        /// <summary>
        /// Group name or field name for evaluation/reset
        /// </summary>
        public string? ResetOn { get; set; }
    }

    /// <summary>
    /// Conditional formatting rule
    /// </summary>
    public class ConditionalFormatDto
    {
        public string Name { get; set; }
        public string Condition { get; set; } // Formula expression
        public StyleDto? Style { get; set; }
        public int Priority { get; set; } = 1;
    }
}
