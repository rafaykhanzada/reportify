# Advanced Grouping and Formula Fields Guide

This guide explains how to use the enhanced grouping and formula field features in the Dynamic HTML Report Generator API.

## Table of Contents

1. [Grouping Overview](#grouping-overview)
2. [Formula Fields](#formula-fields)
3. [Group Summaries](#group-summaries)
4. [Running Totals](#running-totals)
5. [Conditional Formatting](#conditional-formatting)
6. [Complete Examples](#complete-examples)
7. [Formula Reference](#formula-reference)

---

## Grouping Overview

### What is Grouping?

Grouping allows you to organize report data into logical sections based on common values or expressions. Similar to Crystal Reports, you can create multi-level hierarchical groups with headers, footers, and summary calculations.

### Basic Group Configuration

```json
{
  "groups": [
    {
      "groupName": "By Customer",
      "field": "CustomerName",
      "level": 1,
      "sortDirection": "Asc",
      "showHeader": true,
      "showFooter": true
    }
  ]
}
```

### Expression-Based Grouping

Group by custom expressions instead of simple fields:

```json
{
  "groups": [
    {
      "groupName": "By First Letter",
      "expression": "LEFT([CustomerName], 1)",
      "level": 1,
      "sortDirection": "Asc",
      "showHeader": true,
      "showFooter": true,
      "headerTemplate": "Customers starting with: [expression_value]"
    }
  ]
}
```

### Multi-Level Grouping

Create nested groups with parent-child relationships:

```json
{
  "groups": [
    {
      "groupName": "By Region",
      "field": "Region",
      "level": 1,
      "sortDirection": "Asc",
      "showHeader": true,
      "showFooter": true
    },
    {
      "groupName": "By State",
      "field": "State",
      "level": 2,
      "sortDirection": "Asc",
      "showHeader": true,
      "showFooter": true
    },
    {
      "groupName": "By City",
      "field": "City",
      "level": 3,
      "sortDirection": "Asc",
      "showHeader": true,
      "showFooter": false
    }
  ]
}
```

### Group Options

| Property | Type | Description |
|----------|------|-------------|
| `groupName` | string | Display name for the group |
| `field` | string | Field name to group by (alternative to expression) |
| `expression` | string | Custom formula for grouping |
| `level` | int | Group level (1, 2, 3, etc.) |
| `sortDirection` | string | "Asc" or "Desc" |
| `showHeader` | bool | Show group header section |
| `showFooter` | bool | Show group footer section |
| `keepTogether` | bool | Keep group on same page |
| `repeatHeaderOnNewPage` | bool | Repeat header on page breaks |
| `headerTemplate` | string | Custom header formula |
| `footerTemplate` | string | Custom footer formula |

---

## Formula Fields

### What are Formula Fields?

Formula fields are calculated fields that dynamically compute values based on data, parameters, and other formulas. They provide Crystal Reports-like formula capabilities.

### Creating Formula Fields

```json
{
  "formulaFields": [
    {
      "name": "FullName",
      "label": "Full Name",
      "formula": "[FirstName] + \" \" + [LastName]",
      "resultType": "String",
      "evaluationContext": "Record"
    },
    {
      "name": "Discount",
      "label": "Discount Amount",
      "formula": "[Subtotal] * 0.10",
      "resultType": "Number",
      "format": "C2",
      "evaluationContext": "Record"
    },
    {
      "name": "GrandTotal",
      "label": "Grand Total",
      "formula": "SUM([Total])",
      "resultType": "Number",
      "format": "C2",
      "evaluationContext": "Report"
    }
  ]
}
```

### Evaluation Contexts

Formula fields can be evaluated at different levels:

#### 1. **Record Level** (Most Common)
Evaluated for each data record.

```json
{
  "name": "LineTotal",
  "formula": "[Quantity] * [UnitPrice]",
  "evaluationContext": "Record"
}
```

#### 2. **Group Level**
Evaluated once per group.

```json
{
  "name": "GroupAverage",
  "formula": "AVG([Sales])",
  "evaluationContext": "Group"
}
```

#### 3. **Report Level**
Evaluated once for entire report.

```json
{
  "name": "ReportTotal",
  "formula": "SUM([OrderTotal])",
  "evaluationContext": "Report"
}
```

### Using Formula Fields in Elements

Reference formula fields in text boxes or other elements:

```json
{
  "elementType": "TextBox",
  "name": "CustomerFullName",
  "section": "Details",
  "dataBinding": {
    "formula": "@FullName"
  }
}
```

---

## Group Summaries

### What are Group Summaries?

Group summaries calculate aggregate values for each group (sum, average, count, etc.).

### Configuring Group Summaries

```json
{
  "groups": [
    {
      "groupName": "By Category",
      "field": "Category",
      "level": 1,
      "showHeader": true,
      "showFooter": true,
      "summaries": [
        {
          "name": "TotalSales",
          "field": "SalesAmount",
          "function": "Sum",
          "format": "C2",
          "label": "Category Total",
          "showInHeader": false,
          "showInFooter": true
        },
        {
          "name": "AveragePrice",
          "field": "UnitPrice",
          "function": "Average",
          "format": "C2",
          "label": "Avg Price",
          "showInFooter": true
        },
        {
          "name": "ItemCount",
          "field": "ProductID",
          "function": "Count",
          "label": "Items",
          "showInHeader": true,
          "showInFooter": true
        }
      ]
    }
  ]
}
```

### Available Aggregate Functions

| Function | Description |
|----------|-------------|
| `Sum` | Sum of values |
| `Average` | Average of values |
| `Count` | Count of records |
| `Min` | Minimum value |
| `Max` | Maximum value |
| `DistinctCount` | Count of unique values |
| `StdDev` | Standard deviation |
| `Variance` | Statistical variance |
| `First` | First value in group |
| `Last` | Last value in group |

---

## Running Totals

### What are Running Totals?

Running totals calculate cumulative values as the report processes records.

### Creating Running Totals

```json
{
  "runningTotals": [
    {
      "name": "CumulativeSales",
      "field": "SalesAmount",
      "function": "Sum",
      "format": "C2",
      "evaluationTime": "ForEachRecord",
      "resetTime": "Never"
    },
    {
      "name": "CategoryRunningTotal",
      "field": "SalesAmount",
      "function": "Sum",
      "format": "C2",
      "evaluationTime": "ForEachRecord",
      "resetTime": "OnChangeOfGroup",
      "resetOn": "Category"
    }
  ]
}
```

### Running Total Options

| Property | Values | Description |
|----------|--------|-------------|
| `evaluationTime` | `ForEachRecord`, `OnChangeOfGroup`, `OnChangeOfField` | When to calculate |
| `resetTime` | `Never`, `OnChangeOfGroup`, `OnChangeOfField` | When to reset to zero |
| `resetOn` | Group name or field name | What triggers reset |

### Using Running Totals

Reference running totals in formulas:

```json
{
  "dataBinding": {
    "formula": "Running Total: @CumulativeSales"
  }
}
```

---

## Conditional Formatting

### What is Conditional Formatting?

Apply different styles to elements based on conditions.

### Creating Conditional Formats

```json
{
  "conditionalFormats": [
    {
      "name": "HighlightLargeSales",
      "condition": "[SalesAmount] > 1000",
      "priority": 1,
      "style": {
        "backgroundColor": "#90EE90",
        "fontWeight": "bold"
      }
    },
    {
      "name": "WarningLowStock",
      "condition": "[StockLevel] < 10",
      "priority": 2,
      "style": {
        "color": "#FF0000",
        "fontWeight": "bold"
      }
    },
    {
      "name": "AlternateRowColor",
      "condition": "RECORDNUMBER() % 2 = 0",
      "priority": 3,
      "style": {
        "backgroundColor": "#F5F5F5"
      }
    }
  ]
}
```

### Condition Examples

```javascript
// Numeric comparisons
"[Price] > 100"
"[Quantity] >= 50"
"[Discount] = 0"

// String comparisons
"[Status] = \"Pending\""
"[Category] <> \"Electronics\""

// Range checks
"[Total] > 1000 AND [Total] < 5000"

// NULL checks
"[ShippedDate] IS NULL"

// Date comparisons
"[OrderDate] > {startDate}"

// Complex logic
"([Priority] = \"High\" OR [Amount] > 10000) AND [Status] = \"Active\""
```

---

## Complete Examples

### Example 1: Sales Report with Grouping and Summaries

```json
{
  "name": "Sales by Region Report",
  "description": "Detailed sales with regional grouping and summaries",
  "pageSettings": {
    "paperSize": "A4",
    "orientation": "Portrait"
  },
  "parameters": [
    {
      "name": "startDate",
      "dataType": "Date",
      "isRequired": true
    },
    {
      "name": "endDate",
      "dataType": "Date",
      "isRequired": true
    }
  ],
  "dataSources": [
    {
      "name": "SalesData",
      "sourceType": "StoredProcedure",
      "connectionString": "YOUR_CONNECTION_STRING",
      "sourceDefinition": "sp_GetSalesReport",
      "parameters": [
        {
          "name": "StartDate",
          "useReportParameter": true,
          "reportParameterName": "startDate"
        },
        {
          "name": "EndDate",
          "useReportParameter": true,
          "reportParameterName": "endDate"
        }
      ],
      "isPrimary": true
    }
  ],
  "groups": [
    {
      "groupName": "Region",
      "field": "Region",
      "level": 1,
      "sortDirection": "Asc",
      "showHeader": true,
      "showFooter": true,
      "headerTemplate": "Region: [Region] - {startDate} to {endDate}",
      "summaries": [
        {
          "name": "RegionTotal",
          "field": "SalesAmount",
          "function": "Sum",
          "format": "C2",
          "label": "Region Total Sales",
          "showInFooter": true
        },
        {
          "name": "OrderCount",
          "field": "OrderID",
          "function": "Count",
          "label": "Total Orders",
          "showInFooter": true
        },
        {
          "name": "AvgOrder",
          "field": "SalesAmount",
          "function": "Average",
          "format": "C2",
          "label": "Average Order Value",
          "showInFooter": true
        }
      ]
    },
    {
      "groupName": "Salesperson",
      "field": "SalespersonName",
      "level": 2,
      "sortDirection": "Asc",
      "showHeader": true,
      "showFooter": true,
      "summaries": [
        {
          "name": "PersonTotal",
          "field": "SalesAmount",
          "function": "Sum",
          "format": "C2",
          "label": "Salesperson Total",
          "showInFooter": true
        }
      ]
    }
  ],
  "formulaFields": [
    {
      "name": "Commission",
      "formula": "[SalesAmount] * 0.05",
      "resultType": "Number",
      "format": "C2",
      "evaluationContext": "Record",
      "description": "5% commission on sales"
    },
    {
      "name": "PerformanceRating",
      "formula": "IF([SalesAmount] > 5000, \"Excellent\", IF([SalesAmount] > 2000, \"Good\", \"Fair\"))",
      "resultType": "String",
      "evaluationContext": "Record"
    }
  ],
  "runningTotals": [
    {
      "name": "CumulativeSales",
      "field": "SalesAmount",
      "function": "Sum",
      "format": "C2",
      "evaluationTime": "ForEachRecord",
      "resetTime": "OnChangeOfGroup",
      "resetOn": "Region"
    }
  ],
  "conditionalFormats": [
    {
      "name": "HighPerformance",
      "condition": "[SalesAmount] > 5000",
      "priority": 1,
      "style": {
        "backgroundColor": "#90EE90",
        "fontWeight": "bold"
      }
    },
    {
      "name": "LowPerformance",
      "condition": "[SalesAmount] < 1000",
      "priority": 2,
      "style": {
        "backgroundColor": "#FFB6C1",
        "color": "#8B0000"
      }
    }
  ],
  "elements": [
    {
      "elementType": "TextBox",
      "name": "ReportTitle",
      "section": "ReportHeader",
      "layout": {"x": 20, "y": 20, "width": 170, "height": 15},
      "style": {
        "fontSize": 24,
        "fontWeight": "bold",
        "textAlign": "center"
      },
      "properties": {
        "staticText": "Sales Report by Region"
      }
    },
    {
      "elementType": "TextBox",
      "name": "OrderInfo",
      "section": "Details",
      "layout": {"x": 20, "y": 50, "width": 170, "height": 8},
      "dataBinding": {
        "formula": "[CustomerName] - Order #[OrderID]: [SalesAmount]"
      }
    },
    {
      "elementType": "TextBox",
      "name": "CommissionAmount",
      "section": "Details",
      "layout": {"x": 150, "y": 50, "width": 40, "height": 8},
      "dataBinding": {
        "formula": "@Commission"
      }
    }
  ]
}
```

**Usage:**
```bash
curl "https://localhost:5008/api/ReportRender/1?startDate=2024-01-01&endDate=2024-12-31"
```

### Example 2: Product Inventory with Expression Grouping

```json
{
  "name": "Product Inventory by Category",
  "groups": [
    {
      "groupName": "By Price Range",
      "expression": "IF([UnitPrice] < 50, \"Budget\", IF([UnitPrice] < 200, \"Mid-Range\", \"Premium\"))",
      "level": 1,
      "showHeader": true,
      "showFooter": true,
      "headerTemplate": "Price Category: [expression_value]",
      "summaries": [
        {
          "name": "CategoryCount",
          "field": "ProductID",
          "function": "Count",
          "label": "Products in Category",
          "showInFooter": true
        },
        {
          "name": "TotalValue",
          "field": "StockValue",
          "function": "Sum",
          "format": "C2",
          "label": "Total Inventory Value",
          "showInFooter": true
        }
      ]
    }
  ],
  "formulaFields": [
    {
      "name": "StockValue",
      "formula": "[UnitsInStock] * [UnitPrice]",
      "resultType": "Number",
      "format": "C2",
      "evaluationContext": "Record"
    },
    {
      "name": "StockStatus",
      "formula": "IF([UnitsInStock] = 0, \"OUT OF STOCK\", IF([UnitsInStock] < 10, \"LOW\", \"OK\"))",
      "resultType": "String",
      "evaluationContext": "Record"
    }
  ],
  "conditionalFormats": [
    {
      "name": "OutOfStock",
      "condition": "[UnitsInStock] = 0",
      "style": {
        "backgroundColor": "#FF6B6B",
        "color": "#FFFFFF",
        "fontWeight": "bold"
      }
    },
    {
      "name": "LowStock",
      "condition": "[UnitsInStock] > 0 AND [UnitsInStock] < 10",
      "style": {
        "backgroundColor": "#FFF3CD",
        "color": "#856404"
      }
    }
  ]
}
```

---

## Formula Reference

### Field References

```
[FieldName]         - Reference database field
{ParamName}         - Reference report parameter
@FormulaName        - Reference formula field
```

### String Functions

```
UPPER(text)                    - Convert to uppercase
LOWER(text)                    - Convert to lowercase
LEFT(text, length)             - Get leftmost characters
RIGHT(text, length)            - Get rightmost characters
MID(text, start, length)       - Get substring
TRIM(text)                     - Remove spaces
LEN(text)                      - Get length
CONCAT(text1, text2, ...)      - Concatenate strings
```

### Math Functions

```
ABS(number)                    - Absolute value
ROUND(number, decimals)        - Round number
CEILING(number)                - Round up
FLOOR(number)                  - Round down
SQRT(number)                   - Square root
POWER(base, exponent)          - Power
```

### Date Functions

```
TODAY()                        - Current date
NOW()                          - Current date/time
YEAR(date)                     - Extract year
MONTH(date)                    - Extract month
DAY(date)                      - Extract day
DATEDIFF(date1, date2)         - Days between dates
```

### Aggregate Functions (Group Context)

```
SUM([field])                   - Sum of values
AVG([field])                   - Average
COUNT([field])                 - Count records
MIN([field])                   - Minimum value
MAX([field])                   - Maximum value
```

### Conditional Functions

```
IF(condition, trueValue, falseValue)    - Conditional logic
ISNULL(value, defaultValue)             - Replace NULL
```

### Special Functions

```
RECORDNUMBER()                 - Current record number
PAGENUMBER()                   - Current page number
TOTALPAGES()                   - Total pages
```

### Operators

```
Arithmetic:  +, -, *, /, %
Comparison:  =, <>, <, >, <=, >=
Logical:     AND, OR, NOT
String:      + (concatenation)
```

### Example Formulas

```javascript
// Simple calculation
"[Quantity] * [UnitPrice]"

// With formatting
"\"Total: \" + [Total]"

// Conditional
"IF([Status] = \"Active\", \"✓\", \"✗\")"

// Date calculation
"\"Days until delivery: \" + DATEDIFF([DeliveryDate], TODAY())"

// Complex logic
"IF([Quantity] > 100, [UnitPrice] * 0.9, [UnitPrice])"

// Using parameters
"\"Report Period: \" + {startDate} + \" to \" + {endDate}"

// Record number
"\"Row \" + RECORDNUMBER() + \" of \" + TOTALRECORDS()"
```

---

## Best Practices

### 1. **Organize Groups Logically**
- Use ascending levels (1, 2, 3)
- Most general group at level 1
- Most specific at highest level

### 2. **Optimize Formula Performance**
- Calculate complex formulas at report level when possible
- Use record-level only when values change per row
- Cache frequently used calculations

### 3. **Clear Naming**
- Use descriptive names for formulas and summaries
- Include units in labels ("Total Sales (USD)")
- Document complex expressions

### 4. **Test Incrementally**
- Start with simple grouping
- Add one formula at a time
- Test with sample data first

### 5. **Handle NULL Values**
- Use ISNULL() in formulas
- Consider empty groups
- Provide default values

---

## Troubleshooting

### Common Issues

**Issue**: Group not showing
- **Solution**: Verify field name matches database column
- Check `showHeader` and `showFooter` settings

**Issue**: Formula returns #ERROR
- **Solution**: Validate formula syntax
- Check field references exist
- Ensure data types are compatible

**Issue**: Summary shows incorrect value
- **Solution**: Verify aggregate function
- Check if field contains NULL values
- Ensure correct group level

**Issue**: Conditional format not applying
- **Solution**: Check condition syntax
- Verify priority order
- Test condition independently

---

## Next Steps

1. **Start Simple**: Begin with basic field grouping
2. **Add Summaries**: Calculate totals for groups
3. **Create Formulas**: Build calculated fields
4. **Apply Formatting**: Add conditional styles
5. **Test Thoroughly**: Verify with various data sets

For more examples, see `SAMPLE_REPORTS.md` and `REPORT_API_GUIDE.md`.
