using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Core.Utils
{
    /// <summary>
    /// Parses formula strings with placeholders (e.g. {sp_GetInvoiceData.Trade_Price}),
    /// detects operators (+, -, *, /), and either calculates a result when values are provided
    /// or outputs a dynamic expression in a target language (Python, Excel, SQL).
    /// </summary>
    /// <remarks>
    /// <para><b>Generic formula format</b> (use with <see cref="TryGenericCalculate"/> / <see cref="GenericCalculate"/>):</para>
    /// <list type="bullet">
    /// <item>Placeholders: <c>{Name}</c> or <c>{DataSource.Column}</c> — replaced by values when calculating.</item>
    /// <item>Operators: <c>+</c> <c>-</c> <c>*</c> <c>/</c> with standard precedence (e.g. * before +).</item>
    /// <item>Parentheses: <c>(</c> <c>)</c> for grouping.</item>
    /// <item>Numeric literals: e.g. <c>0.15</c>, <c>100</c>.</item>
    /// </list>
    /// Examples: <c>{Price} * {Qty}</c>, <c>{Total} * (1 + {TaxRate})</c>, <c>({A} + {B}) / {Count}</c>.
    /// </remarks>
    public static class FormulaExpressionParser
    {
        private static readonly Regex PlaceholderRegex = new(@"\{([^}]+)\}", RegexOptions.Compiled);
        private static readonly char[] SupportedOperators = { '+', '-', '*', '/' };

        /// <summary>
        /// Target language for emitting placeholder expressions.
        /// </summary>
        public enum FormulaTargetLanguage
        {
            /// <summary>Identifier style: sp_GetInvoiceData.Trade_Price</summary>
            Python,
            /// <summary>Bracket column names: [Trade_Price]</summary>
            Excel,
            /// <summary>Bracket column names for SQL: [Trade_Price]</summary>
            Sql
        }

        /// <summary>
        /// Detects the first supported operator (+, -, *, /) in the formula.
        /// </summary>
        /// <returns>The operator character, or null if none found.</returns>
        public static char? DetectOperator(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
                return null;

            foreach (var op in SupportedOperators)
            {
                if (formula.IndexOf(op) >= 0)
                    return op;
            }
            return null;
        }

        /// <summary>
        /// Extracts all placeholder names from the formula (without braces).
        /// </summary>
        public static IReadOnlyList<string> GetPlaceholders(string formula)
        {
            if (string.IsNullOrWhiteSpace(formula))
                return Array.Empty<string>();

            var list = new List<string>();
            foreach (Match m in PlaceholderRegex.Matches(formula))
            {
                if (m.Success && m.Groups.Count > 1)
                {
                    var name = m.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(name) && !list.Contains(name))
                        list.Add(name);
                }
            }
            return list;
        }

        /// <summary>
        /// Evaluates the formula when placeholder values are provided.
        /// Placeholder keys should match names without braces (e.g. "sp_GetInvoiceData.Trade_Price").
        /// </summary>
        /// <param name="formula">Formula with placeholders, e.g. "{sp_GetInvoiceData.Trade_Price} + {sp_GetInvoiceData.Qty}"</param>
        /// <param name="placeholderValues">Map of placeholder name (no braces) to value.</param>
        /// <param name="result">The calculated value if evaluation succeeded.</param>
        /// <returns>True if all placeholders had values and the expression was evaluated successfully.</returns>
        public static bool TryEvaluate(
            string formula,
            IReadOnlyDictionary<string, object?> placeholderValues,
            out object? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(formula))
                return false;

            var op = DetectOperator(formula);
            if (op == null)
            {
                // No operator: single placeholder or literal
                var single = SubstitutePlaceholders(formula, placeholderValues, FormulaTargetLanguage.Python);
                if (TryParseNumber(single, out var num))
                {
                    result = num;
                    return true;
                }
                result = single;
                return true;
            }

            var parts = SplitByOperator(formula, op.Value);
            if (parts.Count < 2)
                return false;

            var resolved = new List<decimal>();
            foreach (var part in parts)
            {
                var substituted = SubstitutePlaceholders(part.Trim(), placeholderValues, FormulaTargetLanguage.Python);
                if (!TryParseNumber(substituted, out var num))
                    return false;
                resolved.Add(num);
            }

            decimal acc = resolved[0];
            for (int i = 1; i < resolved.Count; i++)
            {
                switch (op.Value)
                {
                    case '+': acc += resolved[i]; break;
                    case '-': acc -= resolved[i]; break;
                    case '*': acc *= resolved[i]; break;
                    case '/': acc = resolved[i] == 0 ? 0 : acc / resolved[i]; break;
                }
            }

            result = acc;
            return true;
        }

        /// <summary>
        /// Evaluates the formula when placeholder values are provided.
        /// Returns the calculated value, or null if evaluation fails.
        /// </summary>
        public static object? Evaluate(string formula, IReadOnlyDictionary<string, object?> placeholderValues)
        {
            return TryEvaluate(formula, placeholderValues, out var result) ? result : null;
        }

        /// <summary>
        /// Converts the formula to a dynamic expression in the target language:
        /// placeholders are replaced by the appropriate syntax (e.g. Python: no braces, Excel/SQL: bracket column names).
        /// </summary>
        public static string ToExpression(string formula, FormulaTargetLanguage target)
        {
            if (string.IsNullOrWhiteSpace(formula))
                return formula;

            return SubstitutePlaceholders(formula, null, target);
        }

        /// <summary>
        /// Generic calculator: substitute placeholders with values, then evaluate the resulting expression.
        /// Supports any arithmetic formula: +, -, *, /, parentheses, and numeric literals (e.g. "{Price} * (1 + 0.05)").
        /// Placeholder keys = names without braces. Missing or null values are treated as 0.
        /// </summary>
        /// <param name="formula">Formula in generic format, e.g. "{sp_GetInvoiceData.Trade_Price} + {sp_GetInvoiceData.Qty}" or "{Total} * (1 + {TaxRate})".</param>
        /// <param name="placeholderValues">Map of placeholder name (no braces) to value (number or convertible to number).</param>
        /// <param name="result">The calculated value (numeric or string if expression could not be evaluated).</param>
        /// <returns>True if the expression was evaluated successfully.</returns>
        public static bool TryGenericCalculate(
            string formula,
            IReadOnlyDictionary<string, object?> placeholderValues,
            out object? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(formula))
                return false;

            var expression = SubstitutePlaceholdersForGeneric(formula, placeholderValues);
            if (string.IsNullOrWhiteSpace(expression))
                return false;

            try
            {
                var dt = new DataTable();
                var computed = dt.Compute(expression, null);
                if (computed is DBNull)
                {
                    result = null;
                    return false;
                }
                result = computed;
                return true;
            }
            catch
            {
                result = expression;
                return false;
            }
        }

        /// <summary>
        /// Generic calculator: substitute placeholders and evaluate. Returns the result or null if evaluation fails.
        /// Use for any formula with +, -, *, /, (), and {Placeholder} — one method for all such expressions.
        /// </summary>
        public static object? GenericCalculate(string formula, IReadOnlyDictionary<string, object?> placeholderValues)
        {
            return TryGenericCalculate(formula, placeholderValues, out var result) ? result : null;
        }

        /// <summary>
        /// Substitutes placeholders with values for generic evaluation. Values are formatted as numbers (InvariantCulture);
        /// null/DBNull/missing → 0. Non-numeric values are converted to 0.
        /// </summary>
        private static string SubstitutePlaceholdersForGeneric(
            string formula,
            IReadOnlyDictionary<string, object?>? placeholderValues)
        {
            return PlaceholderRegex.Replace(formula, match =>
            {
                var name = match.Groups[1].Value.Trim();
                if (placeholderValues != null && placeholderValues.TryGetValue(name, out var value))
                    return FormatValueForGeneric(value);
                return "0";
            });
        }

        private static string FormatValueForGeneric(object? value)
        {
            if (value == null || value is DBNull)
                return "0";
            if (value is decimal d) return d.ToString(CultureInfo.InvariantCulture);
            if (value is double dbl) return dbl.ToString(CultureInfo.InvariantCulture);
            if (value is float f) return f.ToString(CultureInfo.InvariantCulture);
            if (value is int i) return i.ToString(CultureInfo.InvariantCulture);
            if (value is long l) return l.ToString(CultureInfo.InvariantCulture);
            if (value is string s && decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
                return parsed.ToString(CultureInfo.InvariantCulture);
            return "0";
        }

        private static string SubstitutePlaceholders(
            string formula,
            IReadOnlyDictionary<string, object?>? placeholderValues,
            FormulaTargetLanguage target)
        {
            return PlaceholderRegex.Replace(formula, match =>
            {
                var name = match.Groups[1].Value.Trim();
                if (placeholderValues != null &&
                    placeholderValues.TryGetValue(name, out var value) &&
                    value != null && value is not DBNull)
                {
                    return FormatValue(value);
                }

                return FormatPlaceholderForTarget(name, target);
            });
        }

        private static string FormatPlaceholderForTarget(string placeholderName, FormulaTargetLanguage target)
        {
            switch (target)
            {
                case FormulaTargetLanguage.Python:
                    return placeholderName;
                case FormulaTargetLanguage.Excel:
                case FormulaTargetLanguage.Sql:
                    var columnName = placeholderName.Contains('.')
                        ? placeholderName.Substring(placeholderName.LastIndexOf('.') + 1)
                        : placeholderName;
                    return $"[{columnName}]";
                default:
                    return placeholderName;
            }
        }

        private static string FormatValue(object value)
        {
            if (value is decimal d) return d.ToString(CultureInfo.InvariantCulture);
            if (value is double dbl) return dbl.ToString(CultureInfo.InvariantCulture);
            if (value is float f) return f.ToString(CultureInfo.InvariantCulture);
            if (value is int i) return i.ToString(CultureInfo.InvariantCulture);
            if (value is long l) return l.ToString(CultureInfo.InvariantCulture);
            var s = value.ToString();
            return TryParseNumber(s, out _) ? s : $"\"{s?.Replace("\"", "\\\"")}\"";
        }

        private static bool TryParseNumber(string? s, out decimal number)
        {
            number = 0;
            if (string.IsNullOrWhiteSpace(s))
                return false;
            s = s.Trim().Replace("\"", "");
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out number);
        }

        private static IReadOnlyList<string> SplitByOperator(string formula, char op)
        {
            var list = new List<string>();
            var start = 0;
            for (int i = 0; i < formula.Length; i++)
            {
                var c = formula[i];
                if (c == op)
                {
                    list.Add(formula.Substring(start, i - start));
                    start = i + 1;
                }
            }
            if (start <= formula.Length)
                list.Add(formula.Substring(start));
            return list;
        }
    }
}
