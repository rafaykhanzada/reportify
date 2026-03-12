using Core.Data.DTOs.ReportDesigner;
using System.Data;

namespace Service.IService
{
    /// <summary>
    /// Service for evaluating formulas and expressions in reports
    /// </summary>
    public interface IFormulaEvaluationService
    {
        /// <summary>
        /// Evaluate a formula expression
        /// </summary>
        object? Evaluate(string formula, FormulaContext context);
        
        /// <summary>
        /// Evaluate a formula and return as string
        /// </summary>
        string? EvaluateAsString(string formula, FormulaContext context, string? format = null);
        
        /// <summary>
        /// Validate a formula expression
        /// </summary>
        FormulaValidationResult ValidateFormula(string formula);
        
        /// <summary>
        /// Get available functions and their descriptions
        /// </summary>
        Dictionary<string, string> GetAvailableFunctions();
    }

    /// <summary>
    /// Context for formula evaluation
    /// </summary>
    public class FormulaContext
    {
        /// <summary>
        /// Current data row
        /// </summary>
        public DataRow? CurrentRow { get; set; }
        
        /// <summary>
        /// All rows in current group
        /// </summary>
        public List<DataRow>? GroupRows { get; set; }
        
        /// <summary>
        /// All rows in report
        /// </summary>
        public DataTable? AllData { get; set; }
        
        /// <summary>
        /// Report parameters
        /// </summary>
        public Dictionary<string, object>? Parameters { get; set; }
        
        /// <summary>
        /// Formula fields already calculated
        /// </summary>
        public Dictionary<string, object>? CalculatedFields { get; set; }
        
        /// <summary>
        /// Current group values
        /// </summary>
        public Dictionary<string, object>? GroupValues { get; set; }
        
        /// <summary>
        /// Running totals
        /// </summary>
        public Dictionary<string, object>? RunningTotals { get; set; }
        
        /// <summary>
        /// Current record number
        /// </summary>
        public int RecordNumber { get; set; }
        
        /// <summary>
        /// Current page number
        /// </summary>
        public int PageNumber { get; set; } = 1;
        
        /// <summary>
        /// Total page count
        /// </summary>
        public int TotalPages { get; set; } = 1;
    }

    /// <summary>
    /// Result of formula validation
    /// </summary>
    public class FormulaValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string>? ReferencedFields { get; set; }
        public List<string>? ReferencedParameters { get; set; }
        public string? ResultType { get; set; }
    }
}
