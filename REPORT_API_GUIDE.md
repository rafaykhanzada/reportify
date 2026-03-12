# Dynamic HTML Report Generator API - Complete Guide

## Overview

This API provides a complete Crystal Reports-like experience for creating, editing, and rendering dynamic HTML reports. Reports are built using a drag-and-drop designer approach with full support for:

- **Visual Elements**: TextBoxes, Tables, Charts, Images, Lines, Rectangles
- **Dynamic Data**: Database tables, stored procedures, SQL queries, APIs
- **Parameters**: Query string parameters for flexible report generation
- **Sections**: Page headers/footers, report headers/footers, details, grouping
- **Formatting**: Complete styling control, borders, fonts, colors
- **Output**: Dynamic HTML with optional PDF and Excel export

## Key Features

### 100% Crystal Reports-Like Experience
- Drag-and-drop report designer
- Visual elements with precise positioning
- Data binding and formulas
- Grouping, sorting, and aggregation
- Conditional formatting
- Sub-reports support

### Dynamic HTML Output
- Pure HTML/CSS output (no static images)
- Responsive and printable
- Standalone HTML documents or fragments
- Embedded or external styles

### Query Parameter Support
All report parameters can be passed via query strings for automation:
```
GET /api/ReportRender/5?startDate=2024-01-01&endDate=2024-12-31&customerId=123
```

## API Endpoints

### Report Designer Endpoints

#### 1. Create or Update Report Design
```http
POST /api/ReportDesigner
Content-Type: application/json

{
  "name": "Sales Report",
  "description": "Monthly sales analysis",
  "pageSettings": {
    "paperSize": "A4",
    "orientation": "Portrait",
    "width": 210,
    "height": 297,
    "margins": {
      "top": 20,
      "right": 20,
      "bottom": 20,
      "left": 20
    }
  },
  "elements": [
    {
      "elementType": "TextBox",
      "name": "Title",
      "section": "ReportHeader",
      "layout": {
        "x": 10,
        "y": 10,
        "width": 190,
        "height": 15
      },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 24,
        "fontWeight": "bold",
        "textAlign": "center",
        "color": "#000000"
      },
      "properties": {
        "staticText": "Monthly Sales Report"
      }
    },
    {
      "elementType": "Table",
      "name": "SalesTable",
      "section": "Details",
      "layout": {
        "x": 10,
        "y": 40,
        "width": 190,
        "height": 100
      },
      "dataBinding": {
        "dataSource": "SalesData",
        "field": null
      },
      "properties": {
        "columns": [
          {
            "header": "Product",
            "field": "ProductName",
            "width": 100,
            "alignment": "left"
          },
          {
            "header": "Quantity",
            "field": "Quantity",
            "width": 45,
            "alignment": "right"
          },
          {
            "header": "Total",
            "field": "Total",
            "width": 45,
            "alignment": "right",
            "format": "C2"
          }
        ],
        "showHeader": true,
        "alternateRowColors": true,
        "showBorders": true
      }
    }
  ],
  "dataSources": [
    {
      "name": "SalesData",
      "sourceType": "StoredProcedure",
      "connectionString": "Server=localhost;Database=MyDB;Trusted_Connection=True;",
      "sourceDefinition": "sp_GetSalesReport",
      "parameters": [
        {
          "name": "StartDate",
          "dataType": "Date",
          "useReportParameter": true,
          "reportParameterName": "startDate"
        },
        {
          "name": "EndDate",
          "dataType": "Date",
          "useReportParameter": true,
          "reportParameterName": "endDate"
        }
      ],
      "isPrimary": true
    }
  ],
  "parameters": [
    {
      "name": "startDate",
      "label": "Start Date",
      "dataType": "Date",
      "isRequired": true,
      "defaultValue": "2024-01-01",
      "displayOrder": 1
    },
    {
      "name": "endDate",
      "label": "End Date",
      "dataType": "Date",
      "isRequired": true,
      "defaultValue": "2024-12-31",
      "displayOrder": 2
    }
  ]
}
```

#### 2. Get Report Design
```http
GET /api/ReportDesigner/{id}
```

#### 3. Get All Report Designs
```http
GET /api/ReportDesigner?pageNo=0&pageSize=50
```

#### 4. Delete Report Design
```http
DELETE /api/ReportDesigner/{id}
```

#### 5. Duplicate Report Design
```http
POST /api/ReportDesigner/{id}/duplicate?newName=Sales%20Report%20Copy
```

#### 6. Save Element to Report
```http
POST /api/ReportDesigner/{reportId}/elements
Content-Type: application/json

{
  "elementType": "TextBox",
  "name": "CompanyName",
  "section": "PageHeader",
  "layout": { "x": 10, "y": 5, "width": 100, "height": 10 },
  "style": { "fontSize": 16, "fontWeight": "bold" },
  "properties": { "staticText": "ACME Corporation" }
}
```

### Report Rendering Endpoints

#### 1. Render Report with Query Parameters
The most powerful endpoint - pass all parameters via query string for complete automation:

```http
GET /api/ReportRender/{reportId}?startDate=2024-01-01&endDate=2024-12-31&customerId=123&region=North

Query Parameters:
- startDate, endDate, customerId, region: Report-specific parameters
- format: html (default), pdf, excel
- standalone: true (default) - generates complete HTML document
- includeStyles: true (default) - includes CSS in output
```

Example URLs:
```
# Basic rendering with parameters
https://yourserver/api/ReportRender/5?startDate=2024-01-01&endDate=2024-12-31

# With customer filter
https://yourserver/api/ReportRender/5?customerId=123&year=2024

# HTML fragment without styles (for embedding)
https://yourserver/api/ReportRender/5?standalone=false&includeStyles=false&orderId=789
```

#### 2. Render Report with POST Body
```http
POST /api/ReportRender/{reportId}
Content-Type: application/json

{
  "startDate": "2024-01-01",
  "endDate": "2024-12-31",
  "customerId": 123
}
```

#### 3. Preview Report
```http
GET /api/ReportRender/{reportId}/preview
```

#### 4. Download Report
```http
GET /api/ReportRender/{reportId}/download?format=pdf&startDate=2024-01-01&endDate=2024-12-31
```

## Report Elements

### TextBox
Displays static or dynamic text.

```json
{
  "elementType": "TextBox",
  "name": "CustomerName",
  "section": "Details",
  "layout": { "x": 10, "y": 10, "width": 80, "height": 10 },
  "style": {
    "fontFamily": "Arial",
    "fontSize": 12,
    "color": "#000000",
    "textAlign": "left"
  },
  "dataBinding": {
    "dataSource": "Customers",
    "field": "CustomerName",
    "format": null
  },
  "properties": {
    "canGrow": true,
    "canShrink": false,
    "hyperlinkUrl": null
  }
}
```

### Table
Displays tabular data with multiple columns.

```json
{
  "elementType": "Table",
  "name": "ProductTable",
  "section": "Details",
  "layout": { "x": 10, "y": 30, "width": 190, "height": 100 },
  "dataBinding": {
    "dataSource": "Products"
  },
  "properties": {
    "columns": [
      {
        "header": "Product Name",
        "field": "ProductName",
        "width": 100,
        "alignment": "left"
      },
      {
        "header": "Price",
        "field": "Price",
        "width": 45,
        "format": "C2",
        "alignment": "right"
      },
      {
        "header": "Stock",
        "field": "Stock",
        "width": 45,
        "alignment": "right",
        "aggregateFunction": "Sum"
      }
    ],
    "showHeader": true,
    "alternateRowColors": true,
    "alternateRowColor": "#f5f5f5",
    "showBorders": true
  }
}
```

### Image
Displays images from URL, base64, or database field.

```json
{
  "elementType": "Image",
  "name": "CompanyLogo",
  "section": "PageHeader",
  "layout": { "x": 10, "y": 5, "width": 40, "height": 20 },
  "properties": {
    "source": "https://example.com/logo.png",
    "sourceType": "URL",
    "sizing": "Fit",
    "alternateText": "Company Logo"
  }
}
```

### Line
Draws horizontal or vertical lines.

```json
{
  "elementType": "Line",
  "name": "Separator",
  "section": "ReportHeader",
  "layout": { "x": 10, "y": 30, "width": 190, "height": 1 },
  "style": {
    "color": "#000000"
  },
  "properties": {
    "thickness": 2,
    "lineStyle": "solid"
  }
}
```

### Rectangle
Draws rectangles and shapes.

```json
{
  "elementType": "Rectangle",
  "name": "HeaderBackground",
  "section": "PageHeader",
  "layout": { "x": 0, "y": 0, "width": 210, "height": 25 },
  "style": {
    "backgroundColor": "#4a90e2",
    "opacity": 0.2
  },
  "properties": {
    "filled": true,
    "cornerRadius": 0
  }
}
```

## Data Sources

### Table Data Source
```json
{
  "name": "Customers",
  "sourceType": "Table",
  "connectionString": "Server=localhost;Database=MyDB;Trusted_Connection=True;",
  "sourceDefinition": "Customers",
  "isPrimary": true
}
```

### Stored Procedure Data Source
```json
{
  "name": "SalesData",
  "sourceType": "StoredProcedure",
  "connectionString": "Server=localhost;Database=MyDB;Trusted_Connection=True;",
  "sourceDefinition": "sp_GetSalesReport",
  "parameters": [
    {
      "name": "StartDate",
      "dataType": "Date",
      "useReportParameter": true,
      "reportParameterName": "startDate"
    },
    {
      "name": "CustomerId",
      "dataType": "Integer",
      "useReportParameter": true,
      "reportParameterName": "customerId"
    }
  ],
  "isPrimary": true
}
```

### SQL Query Data Source
```json
{
  "name": "FilteredOrders",
  "sourceType": "Query",
  "connectionString": "Server=localhost;Database=MyDB;Trusted_Connection=True;",
  "sourceDefinition": "SELECT * FROM Orders WHERE OrderDate BETWEEN @StartDate AND @EndDate",
  "isPrimary": true
}
```

## Report Sections

- **PageHeader**: Appears at the top of every page
- **ReportHeader**: Appears once at the start of the report
- **Details**: Main content section (repeats for each data row)
- **ReportFooter**: Appears once at the end of the report
- **PageFooter**: Appears at the bottom of every page
- **GroupHeader1-N**: Appears when a new group starts
- **GroupFooter1-N**: Appears when a group ends

## Grouping and Sorting

### Grouping
```json
{
  "groups": [
    {
      "field": "Category",
      "groupName": "Product Category",
      "showHeader": true,
      "showFooter": true,
      "level": 1
    }
  ]
}
```

### Sorting
```json
{
  "sorting": [
    {
      "field": "OrderDate",
      "direction": "Desc",
      "order": 1
    },
    {
      "field": "CustomerName",
      "direction": "Asc",
      "order": 2
    }
  ]
}
```

## Formulas and Expressions

Use formulas in data binding:

```json
{
  "dataBinding": {
    "formula": "[Quantity] * [UnitPrice]",
    "format": "C2"
  }
}
```

Reference report parameters:
```json
{
  "dataBinding": {
    "formula": "Report generated for: {startDate} to {endDate}"
  }
}
```

## Usage Examples

### Example 1: Simple Customer List
```bash
# Create the report
curl -X POST https://yourserver/api/ReportDesigner \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Customer List",
    "dataSources": [{
      "name": "Customers",
      "sourceType": "Table",
      "connectionString": "Server=localhost;Database=MyDB;Trusted_Connection=True;",
      "sourceDefinition": "Customers",
      "isPrimary": true
    }],
    "elements": [{
      "elementType": "Table",
      "section": "Details",
      "layout": {"x": 10, "y": 10, "width": 190, "height": 200},
      "dataBinding": {"dataSource": "Customers"},
      "properties": {
        "columns": [
          {"header": "Name", "field": "CustomerName", "width": 100},
          {"header": "Email", "field": "Email", "width": 90}
        ]
      }
    }]
  }'

# Render the report (assuming reportId = 1)
curl https://yourserver/api/ReportRender/1
```

### Example 2: Sales Report with Parameters
```bash
# Render with query parameters
curl "https://yourserver/api/ReportRender/5?startDate=2024-01-01&endDate=2024-12-31&region=North"

# Download as PDF
curl "https://yourserver/api/ReportRender/5/download?format=pdf&startDate=2024-01-01&endDate=2024-12-31" \
  --output sales-report.pdf
```

### Example 3: Embedding in Web Application
```html
<!-- Embed report as HTML fragment -->
<div id="report-container">
  <iframe src="https://yourserver/api/ReportRender/5?customerId=123&year=2024" 
          width="100%" 
          height="600px">
  </iframe>
</div>
```

### Example 4: Automated Report Generation
```javascript
// Generate reports programmatically
async function generateMonthlyReport(year, month) {
  const response = await fetch(
    `https://yourserver/api/ReportRender/10?year=${year}&month=${month}&format=pdf`,
    { method: 'GET' }
  );
  
  const blob = await response.blob();
  const url = window.URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = `monthly-report-${year}-${month}.pdf`;
  a.click();
}
```

## Best Practices

### 1. Performance
- Use stored procedures for complex queries
- Add appropriate database indexes
- Limit result sets with parameters
- Cache frequently accessed reports

### 2. Security
- Validate all input parameters
- Use parameterized queries
- Encrypt connection strings
- Implement authentication/authorization

### 3. Design
- Use consistent styling across elements
- Set appropriate margins and spacing
- Test with different data volumes
- Consider page breaks for printing

### 4. Parameter Design
- Provide sensible defaults
- Use clear, descriptive parameter names
- Validate parameter types and ranges
- Document required vs. optional parameters

## Troubleshooting

### Report Not Rendering
- Check data source connection strings
- Verify stored procedure parameters
- Ensure report has at least one data source
- Check browser console for errors

### Missing Data
- Verify data binding field names match database columns
- Check parameter values being passed
- Test data source query independently
- Review report filters and conditions

### Styling Issues
- Ensure element positions don't overlap
- Check z-index for layering
- Verify CSS is included (includeStyles=true)
- Test in different browsers

## Next Steps

1. **Create your first report** using the Report Designer API
2. **Test rendering** with sample parameters
3. **Integrate** into your application
4. **Add authentication** and authorization
5. **Implement caching** for better performance
6. **Add PDF export** for production use

## Support

For issues, questions, or contributions, please refer to the project documentation or contact the development team.
