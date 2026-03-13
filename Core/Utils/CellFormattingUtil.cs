using Core.Data.DTOs;
using System.Globalization;

namespace Core.Utils;

/// <summary>
/// Applies column format configuration (Currency, Date, DateTime, Time, Number) to cell values.
/// Supports .NET format strings (e.g. C2, F3, MMM dd yyyy) and custom currency symbol override.
/// </summary>
public static class CellFormattingUtil
{
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Formats a value using the specified format configuration.
    /// Handles nulls, DBNull, and invalid values safely.
    /// </summary>
    /// <param name="value">Raw value from DB (e.g. decimal, DateTime, string).</param>
    /// <param name="header">Column header with format config (Format or legacy format/template).</param>
    /// <returns>Formatted string, or original value as string if no format or invalid.</returns>
    public static string FormatValue(object? value, TabledataHeaders? header)
    {
        if (value == null || value is DBNull)
            return string.Empty;

        var config = GetFormatConfig(header);
        if (config == null)
            return value.ToString() ?? string.Empty;

        var type = config.type?.Trim();
        var template = config.template?.Trim();
        var symbol = config.symbol?.Trim();

        try
        {
            return type?.ToUpperInvariant() switch
            {
                "CURRENCY" => FormatCurrency(value, template, symbol),
                "DATE" => FormatDate(value, template),
                "DATETIME" => FormatDateTime(value, template),
                "TIME" => FormatTime(value, template),
                "NUMBER" => FormatNumber(value, template),
                _ => string.IsNullOrEmpty(template)
                    ? value.ToString() ?? string.Empty
                    : FormatWithTemplate(value, template)
            };
        }
        catch
        {
            return value.ToString() ?? string.Empty;
        }
    }

    private static FormatConfig? GetFormatConfig(TabledataHeaders? header)
    {
        if (header == null)
            return null;

        var fc = header.format;
        if (fc == null)
            return null;

        if (!string.IsNullOrEmpty(fc.type) || !string.IsNullOrEmpty(fc.template))
            return new FormatConfig(fc.type, fc.template, fc.symbol ?? header.symbol);

        return null;
    }

    private static string FormatCurrency(object value, string? template, string? symbol)
    {
        decimal? num = value switch
        {
            decimal d => d,
            double dbl => (decimal)dbl,
            float f => (decimal)f,
            int i => i,
            long l => l,
            string s when decimal.TryParse(s, InvariantCulture, out var parsed) => parsed,
            _ => null
        };

        if (num == null)
            return value.ToString() ?? string.Empty;

        var format = !string.IsNullOrEmpty(template) ? template : "C2";

        if (!string.IsNullOrEmpty(symbol))
        {
            var nfi = (NumberFormatInfo)InvariantCulture.NumberFormat.Clone();
            nfi.CurrencySymbol = symbol;
            return ((decimal)num).ToString(format, nfi);
        }

        return ((decimal)num).ToString(format, InvariantCulture);
    }

    private static string FormatDate(object value, string? template)
    {
        DateTime? dt = value switch
        {
            DateTime d => d,
            DateOnly d => d.ToDateTime(TimeOnly.MinValue),
            string s when DateTime.TryParse(s, InvariantCulture, DateTimeStyles.None, out var parsed) => parsed,
            _ => null
        };

        if (dt == null)
            return value.ToString() ?? string.Empty;

        var format = !string.IsNullOrEmpty(template) ? template : "d";
        return ((DateTime)dt).ToString(format, InvariantCulture);
    }

    private static string FormatDateTime(object value, string? template)
    {
        DateTime? dt = value switch
        {
            DateTime d => d,
            DateOnly d => d.ToDateTime(TimeOnly.MinValue),
            string s when DateTime.TryParse(s, InvariantCulture, DateTimeStyles.None, out var parsed) => parsed,
            _ => null
        };

        if (dt == null)
            return value.ToString() ?? string.Empty;

        var format = !string.IsNullOrEmpty(template) ? template : "g";
        return ((DateTime)dt).ToString(format, InvariantCulture);
    }

    private static string FormatTime(object value, string? template)
    {
        TimeSpan? time = value switch
        {
            TimeSpan t => t,
            DateTime dt => dt.TimeOfDay,
            TimeOnly t => t.ToTimeSpan(),
            string s when TimeSpan.TryParse(s, InvariantCulture, out var parsed) => parsed,
            string s when DateTime.TryParse(s, InvariantCulture, DateTimeStyles.None, out var parsed) => parsed.TimeOfDay,
            _ => null
        };

        if (time == null)
            return value.ToString() ?? string.Empty;

        var format = !string.IsNullOrEmpty(template) ? template : "c";
        return ((TimeSpan)time).ToString(format, InvariantCulture);
    }

    private static string FormatNumber(object value, string? template)
    {
        decimal? num = value switch
        {
            decimal d => d,
            double dbl => (decimal)dbl,
            float f => (decimal)f,
            int i => i,
            long l => l,
            string s when decimal.TryParse(s, InvariantCulture, out var parsed) => parsed,
            _ => null
        };

        if (num == null)
            return value.ToString() ?? string.Empty;

        var format = !string.IsNullOrEmpty(template) ? template : "N2";
        return ((decimal)num).ToString(format, InvariantCulture);
    }

    private static string FormatWithTemplate(object value, string template)
    {
        var formattable = value as IFormattable;
        if (formattable != null)
            return formattable.ToString(template, InvariantCulture);

        return string.Format(InvariantCulture, $"{{0:{template}}}", value);
    }

    private sealed record FormatConfig(string? type, string? template, string? symbol);
}
