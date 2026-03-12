using Core.Data.DTOs.ReportDesigner;
using Microsoft.Extensions.Logging;
using Service.IService;
using System.Data;
using System.Text.RegularExpressions;

namespace Service.Service
{
    /// <summary>
    /// Advanced formula evaluation service for dynamic calculations
    /// Supports Crystal Reports-like formula syntax
    /// </summary>
    public class FormulaEvaluationService : IFormulaEvaluationService
    {
        private readonly ILogger<FormulaEvaluationService> _logger;

        public FormulaEvaluationService(ILogger<FormulaEvaluationService> logger)
        {
            _logger = logger;
        }

        public object? Evaluate(string formula, FormulaContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(formula))
                    return null;

                // Replace field references [FieldName]
                formula = ReplaceFieldReferences(formula, context);

                // Replace parameter references {ParamName}
                formula = ReplaceParameterReferences(formula, context);

                // Replace formula field references @FormulaName
                formula = ReplaceFormulaReferences(formula, context);

                // Replace function calls
                formula = ReplaceFunctions(formula, context);

                // Evaluate the expression
                return EvaluateExpression(formula, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating formula: {Formula}", formula);
                return $"#ERROR: {ex.Message}";
            }
        }

        public string? EvaluateAsString(string formula, FormulaContext context, string? format = null)
        {
            var result = Evaluate(formula, context);
            
            if (result == null)
                return null;

            if (result is string str)
                return str;

            // Apply formatting if specified
            if (!string.IsNullOrEmpty(format))
            {
                try
                {
                    return string.Format($"{{0:{format}}}", result);
                }
                catch
                {
                    return result.ToString();
                }
            }

            return result.ToString();
        }

        public FormulaValidationResult ValidateFormula(string formula)
        {
            var result = new FormulaValidationResult
            {
                IsValid = true,
                ReferencedFields = new List<string>(),
                ReferencedParameters = new List<string>()
            };

            try
            {
                // Extract field references
                var fieldMatches = Regex.Matches(formula, @"\[([^\]]+)\]");
                foreach (Match match in fieldMatches)
                {
                    result.ReferencedFields.Add(match.Groups[1].Value);
                }

                // Extract parameter references
                var paramMatches = Regex.Matches(formula, @"\{([^\}]+)\}");
                foreach (Match match in paramMatches)
                {
                    result.ReferencedParameters.Add(match.Groups[1].Value);
                }

                // Basic syntax validation
                if (formula.Contains("[") && !formula.Contains("]"))
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Unclosed field reference bracket";
                }
                else if (formula.Contains("{") && !formula.Contains("}"))
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Unclosed parameter reference brace";
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public Dictionary<string, string> GetAvailableFunctions()
        {
            return new Dictionary<string, string>
            {
                // String functions
                { "UPPER(text)", "Converts text to uppercase" },
                { "LOWER(text)", "Converts text to lowercase" },
                { "LEFT(text, length)", "Returns leftmost characters" },
                { "RIGHT(text, length)", "Returns rightmost characters" },
                { "MID(text, start, length)", "Returns substring" },
                { "TRIM(text)", "Removes leading/trailing spaces" },
                { "LEN(text)", "Returns text length" },
                { "CONCAT(text1, text2, ...)", "Concatenates strings" },
                
                // Math functions
                { "ABS(number)", "Absolute value" },
                { "ROUND(number, decimals)", "Rounds number" },
                { "CEILING(number)", "Rounds up" },
                { "FLOOR(number)", "Rounds down" },
                { "SQRT(number)", "Square root" },
                { "POWER(base, exponent)", "Power function" },
                
                // Date functions
                { "TODAY()", "Current date" },
                { "NOW()", "Current date and time" },
                { "YEAR(date)", "Extract year" },
                { "MONTH(date)", "Extract month" },
                { "DAY(date)", "Extract day" },
                { "DATEDIFF(date1, date2)", "Days between dates" },
                
                // Aggregate functions (for groups)
                { "SUM(field)", "Sum of values" },
                { "AVG(field)", "Average of values" },
                { "COUNT(field)", "Count of values" },
                { "MIN(field)", "Minimum value" },
                { "MAX(field)", "Maximum value" },
                
                // Conditional functions
                { "IF(condition, trueValue, falseValue)", "Conditional logic" },
                { "ISNULL(value, defaultValue)", "Replace null values" },
                
                // Special functions
                { "RECORDNUMBER()", "Current record number" },
                { "PAGENUMBER()", "Current page number" },
                { "TOTALPAGES()", "Total page count" }
            };
        }

        #region Private Methods

        private string ReplaceFieldReferences(string formula, FormulaContext context)
        {
            if (context.CurrentRow == null)
                return formula;

            var matches = Regex.Matches(formula, @"\[([^\]]+)\]");
            foreach (Match match in matches)
            {
                var fieldName = match.Groups[1].Value;
                if (context.CurrentRow.Table.Columns.Contains(fieldName))
                {
                    var value = context.CurrentRow[fieldName];
                    var replacement = FormatValueForFormula(value);
                    formula = formula.Replace(match.Value, replacement);
                }
            }

            return formula;
        }

        private string ReplaceParameterReferences(string formula, FormulaContext context)
        {
            if (context.Parameters == null)
                return formula;

            var matches = Regex.Matches(formula, @"\{([^\}]+)\}");
            foreach (Match match in matches)
            {
                var paramName = match.Groups[1].Value;
                if (context.Parameters.ContainsKey(paramName))
                {
                    var value = context.Parameters[paramName];
                    var replacement = FormatValueForFormula(value);
                    formula = formula.Replace(match.Value, replacement);
                }
            }

            return formula;
        }

        private string ReplaceFormulaReferences(string formula, FormulaContext context)
        {
            if (context.CalculatedFields == null)
                return formula;

            var matches = Regex.Matches(formula, @"@([a-zA-Z0-9_]+)");
            foreach (Match match in matches)
            {
                var fieldName = match.Groups[1].Value;
                if (context.CalculatedFields.ContainsKey(fieldName))
                {
                    var value = context.CalculatedFields[fieldName];
                    var replacement = FormatValueForFormula(value);
                    formula = formula.Replace(match.Value, replacement);
                }
            }

            return formula;
        }

        private string ReplaceFunctions(string formula, FormulaContext context)
        {
            // String functions
            formula = ReplaceFunctionCall(formula, "UPPER", args => args[0].ToUpper());
            formula = ReplaceFunctionCall(formula, "LOWER", args => args[0].ToLower());
            formula = ReplaceFunctionCall(formula, "TRIM", args => args[0].Trim());
            formula = ReplaceFunctionCall(formula, "LEN", args => args[0].Length.ToString());
            
            // Special functions
            formula = formula.Replace("TODAY()", $"\"{DateTime.Today:yyyy-MM-dd}\"");
            formula = formula.Replace("NOW()", $"\"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\"");
            formula = formula.Replace("RECORDNUMBER()", context.RecordNumber.ToString());
            formula = formula.Replace("PAGENUMBER()", context.PageNumber.ToString());
            formula = formula.Replace("TOTALPAGES()", context.TotalPages.ToString());

            // Math functions
            formula = ReplaceFunctionCall(formula, "ABS", args => 
                Math.Abs(Convert.ToDouble(args[0])).ToString());
            formula = ReplaceFunctionCall(formula, "SQRT", args => 
                Math.Sqrt(Convert.ToDouble(args[0])).ToString());

            // Aggregate functions (for group context)
            if (context.GroupRows != null && context.GroupRows.Any())
            {
                formula = ReplaceAggregateFunction(formula, "SUM", context, 
                    (rows, field) => rows.Sum(r => Convert.ToDouble(r[field])));
                formula = ReplaceAggregateFunction(formula, "AVG", context, 
                    (rows, field) => rows.Average(r => Convert.ToDouble(r[field])));
                formula = ReplaceAggregateFunction(formula, "COUNT", context, 
                    (rows, field) => rows.Count);
                formula = ReplaceAggregateFunction(formula, "MIN", context, 
                    (rows, field) => rows.Min(r => Convert.ToDouble(r[field])));
                formula = ReplaceAggregateFunction(formula, "MAX", context, 
                    (rows, field) => rows.Max(r => Convert.ToDouble(r[field])));
            }

            return formula;
        }

        private string ReplaceFunctionCall(string formula, string functionName, Func<string[], string> evaluator)
        {
            var pattern = $@"{functionName}\(([^\)]+)\)";
            var matches = Regex.Matches(formula, pattern, RegexOptions.IgnoreCase);
            
            foreach (Match match in matches)
            {
                try
                {
                    var args = match.Groups[1].Value.Split(',')
                        .Select(a => a.Trim().Trim('"'))
                        .ToArray();
                    var result = evaluator(args);
                    formula = formula.Replace(match.Value, result);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error evaluating function {Function}", functionName);
                }
            }

            return formula;
        }

        private string ReplaceAggregateFunction(string formula, string functionName, 
            FormulaContext context, Func<List<DataRow>, string, double> aggregator)
        {
            var pattern = $@"{functionName}\(\[([^\]]+)\]\)";
            var matches = Regex.Matches(formula, pattern, RegexOptions.IgnoreCase);
            
            foreach (Match match in matches)
            {
                try
                {
                    var fieldName = match.Groups[1].Value;
                    if (context.GroupRows != null && context.GroupRows.Any())
                    {
                        var result = aggregator(context.GroupRows, fieldName);
                        formula = formula.Replace(match.Value, result.ToString());
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error evaluating aggregate {Function}", functionName);
                }
            }

            return formula;
        }

        private object? EvaluateExpression(string expression, FormulaContext context)
        {
            try
            {
                // Handle simple string concatenation
                if (expression.Contains("+") && expression.Contains("\""))
                {
                    return EvaluateStringExpression(expression);
                }

                // Handle numeric expressions
                if (IsNumericExpression(expression))
                {
                    return EvaluateNumericExpression(expression);
                }

                // Handle IF statements
                if (expression.Trim().StartsWith("IF(", StringComparison.OrdinalIgnoreCase))
                {
                    return EvaluateIfStatement(expression, context);
                }

                // Return as-is if it's a simple value
                if (expression.StartsWith("\"") && expression.EndsWith("\""))
                {
                    return expression.Trim('"');
                }

                if (double.TryParse(expression, out double numValue))
                {
                    return numValue;
                }

                return expression;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating expression: {Expression}", expression);
                return expression;
            }
        }

        private string EvaluateStringExpression(string expression)
        {
            // Simple string concatenation with +
            var parts = expression.Split('+')
                .Select(p => p.Trim())
                .Select(p => p.Trim('"'))
                .ToArray();
            return string.Join("", parts);
        }

        private double EvaluateNumericExpression(string expression)
        {
            // Simple math expression evaluation
            // For production, consider using a proper expression evaluator library
            try
            {
                var dataTable = new DataTable();
                var result = dataTable.Compute(expression, "");
                return Convert.ToDouble(result);
            }
            catch
            {
                return 0;
            }
        }

        private object? EvaluateIfStatement(string expression, FormulaContext context)
        {
            // Extract IF(condition, trueValue, falseValue)
            var match = Regex.Match(expression, @"IF\s*\(([^,]+),([^,]+),([^\)]+)\)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var condition = match.Groups[1].Value.Trim();
                var trueValue = match.Groups[2].Value.Trim();
                var falseValue = match.Groups[3].Value.Trim();

                var conditionResult = EvaluateCondition(condition);
                return conditionResult 
                    ? Evaluate(trueValue, context) 
                    : Evaluate(falseValue, context);
            }

            return expression;
        }

        private bool EvaluateCondition(string condition)
        {
            try
            {
                var dataTable = new DataTable();
                var result = dataTable.Compute(condition, "");
                return Convert.ToBoolean(result);
            }
            catch
            {
                return false;
            }
        }

        private bool IsNumericExpression(string expression)
        {
            return Regex.IsMatch(expression, @"^[\d\+\-\*\/\(\)\.\s]+$");
        }

        private string FormatValueForFormula(object? value)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";

            if (value is string str)
                return $"\"{str}\"";

            if (value is DateTime dt)
                return $"\"{dt:yyyy-MM-dd HH:mm:ss}\"";

            if (value is bool b)
                return b ? "1" : "0";

            return value.ToString() ?? "NULL";
        }

        #endregion
    }
}
