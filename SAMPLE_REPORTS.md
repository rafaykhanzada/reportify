# Sample Report Templates

This document contains ready-to-use report templates that demonstrate various features of the Dynamic HTML Report Generator.

## Sample 1: Invoice Report

A professional invoice layout with company header, line items table, and totals.

```json
{
  "name": "Invoice Template",
  "description": "Professional invoice with company branding",
  "pageSettings": {
    "paperSize": "A4",
    "orientation": "Portrait",
    "width": 210,
    "height": 297,
    "margins": {
      "top": 15,
      "right": 15,
      "bottom": 15,
      "left": 15
    }
  },
  "parameters": [
    {
      "name": "invoiceId",
      "label": "Invoice ID",
      "dataType": "Integer",
      "isRequired": true,
      "displayOrder": 1
    }
  ],
  "dataSources": [
    {
      "name": "InvoiceHeader",
      "sourceType": "StoredProcedure",
      "connectionString": "YOUR_CONNECTION_STRING",
      "sourceDefinition": "sp_GetInvoiceHeader",
      "parameters": [
        {
          "name": "InvoiceId",
          "dataType": "Integer",
          "useReportParameter": true,
          "reportParameterName": "invoiceId"
        }
      ],
      "isPrimary": true
    },
    {
      "name": "InvoiceItems",
      "sourceType": "StoredProcedure",
      "connectionString": "YOUR_CONNECTION_STRING",
      "sourceDefinition": "sp_GetInvoiceItems",
      "parameters": [
        {
          "name": "InvoiceId",
          "dataType": "Integer",
          "useReportParameter": true,
          "reportParameterName": "invoiceId"
        }
      ]
    }
  ],
  "elements": [
    {
      "elementType": "Image",
      "name": "CompanyLogo",
      "section": "ReportHeader",
      "zIndex": 1,
      "layout": { "x": 15, "y": 15, "width": 50, "height": 25 },
      "properties": {
        "source": "https://yourcompany.com/logo.png",
        "sourceType": "URL",
        "sizing": "Fit"
      }
    },
    {
      "elementType": "TextBox",
      "name": "CompanyName",
      "section": "ReportHeader",
      "zIndex": 2,
      "layout": { "x": 70, "y": 15, "width": 125, "height": 10 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 18,
        "fontWeight": "bold",
        "color": "#2c3e50",
        "textAlign": "left"
      },
      "properties": {
        "staticText": "ACME Corporation"
      }
    },
    {
      "elementType": "TextBox",
      "name": "CompanyAddress",
      "section": "ReportHeader",
      "zIndex": 3,
      "layout": { "x": 70, "y": 27, "width": 125, "height": 15 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 10,
        "color": "#7f8c8d",
        "textAlign": "left"
      },
      "properties": {
        "staticText": "123 Business Street\\nCity, State 12345\\nPhone: (555) 123-4567"
      }
    },
    {
      "elementType": "Line",
      "name": "HeaderLine",
      "section": "ReportHeader",
      "zIndex": 4,
      "layout": { "x": 15, "y": 45, "width": 180, "height": 1 },
      "style": {
        "backgroundColor": "#3498db"
      },
      "properties": {
        "thickness": 2,
        "lineStyle": "solid"
      }
    },
    {
      "elementType": "TextBox",
      "name": "InvoiceTitle",
      "section": "ReportHeader",
      "zIndex": 5,
      "layout": { "x": 15, "y": 52, "width": 180, "height": 12 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 24,
        "fontWeight": "bold",
        "color": "#2c3e50",
        "textAlign": "center"
      },
      "properties": {
        "staticText": "INVOICE"
      }
    },
    {
      "elementType": "TextBox",
      "name": "InvoiceNumber",
      "section": "ReportHeader",
      "zIndex": 6,
      "layout": { "x": 15, "y": 70, "width": 85, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 11,
        "fontWeight": "bold"
      },
      "dataBinding": {
        "dataSource": "InvoiceHeader",
        "field": "InvoiceNumber",
        "formula": "Invoice #: [InvoiceNumber]"
      }
    },
    {
      "elementType": "TextBox",
      "name": "InvoiceDate",
      "section": "ReportHeader",
      "zIndex": 7,
      "layout": { "x": 110, "y": 70, "width": 85, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 11,
        "textAlign": "right"
      },
      "dataBinding": {
        "dataSource": "InvoiceHeader",
        "field": "InvoiceDate",
        "format": "d",
        "formula": "Date: [InvoiceDate]"
      }
    },
    {
      "elementType": "TextBox",
      "name": "BillToLabel",
      "section": "ReportHeader",
      "zIndex": 8,
      "layout": { "x": 15, "y": 85, "width": 30, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 11,
        "fontWeight": "bold",
        "color": "#3498db"
      },
      "properties": {
        "staticText": "BILL TO:"
      }
    },
    {
      "elementType": "TextBox",
      "name": "CustomerInfo",
      "section": "ReportHeader",
      "zIndex": 9,
      "layout": { "x": 15, "y": 95, "width": 85, "height": 25 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 10
      },
      "dataBinding": {
        "dataSource": "InvoiceHeader",
        "formula": "[CustomerName]\\n[CustomerAddress]\\n[CustomerCity], [CustomerState] [CustomerZip]"
      },
      "properties": {
        "canGrow": true
      }
    },
    {
      "elementType": "Table",
      "name": "InvoiceItemsTable",
      "section": "Details",
      "zIndex": 10,
      "layout": { "x": 15, "y": 130, "width": 180, "height": 80 },
      "dataBinding": {
        "dataSource": "InvoiceItems"
      },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 10
      },
      "properties": {
        "columns": [
          {
            "header": "Description",
            "field": "Description",
            "width": 90,
            "alignment": "left"
          },
          {
            "header": "Qty",
            "field": "Quantity",
            "width": 20,
            "alignment": "center"
          },
          {
            "header": "Unit Price",
            "field": "UnitPrice",
            "width": 35,
            "alignment": "right",
            "format": "C2"
          },
          {
            "header": "Total",
            "field": "LineTotal",
            "width": 35,
            "alignment": "right",
            "format": "C2"
          }
        ],
        "showHeader": true,
        "alternateRowColors": true,
        "alternateRowColor": "#ecf0f1",
        "showBorders": true,
        "rowHeight": 20
      }
    },
    {
      "elementType": "Line",
      "name": "TotalLine",
      "section": "ReportFooter",
      "zIndex": 11,
      "layout": { "x": 125, "y": 5, "width": 70, "height": 1 },
      "style": {
        "backgroundColor": "#2c3e50"
      },
      "properties": {
        "thickness": 1,
        "lineStyle": "solid"
      }
    },
    {
      "elementType": "TextBox",
      "name": "SubtotalLabel",
      "section": "ReportFooter",
      "zIndex": 12,
      "layout": { "x": 125, "y": 10, "width": 35, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 11,
        "fontWeight": "bold",
        "textAlign": "right"
      },
      "properties": {
        "staticText": "Subtotal:"
      }
    },
    {
      "elementType": "TextBox",
      "name": "SubtotalAmount",
      "section": "ReportFooter",
      "zIndex": 13,
      "layout": { "x": 160, "y": 10, "width": 35, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 11,
        "textAlign": "right"
      },
      "dataBinding": {
        "dataSource": "InvoiceHeader",
        "field": "Subtotal",
        "format": "C2"
      }
    },
    {
      "elementType": "TextBox",
      "name": "TaxLabel",
      "section": "ReportFooter",
      "zIndex": 14,
      "layout": { "x": 125, "y": 20, "width": 35, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 11,
        "fontWeight": "bold",
        "textAlign": "right"
      },
      "properties": {
        "staticText": "Tax:"
      }
    },
    {
      "elementType": "TextBox",
      "name": "TaxAmount",
      "section": "ReportFooter",
      "zIndex": 15,
      "layout": { "x": 160, "y": 20, "width": 35, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 11,
        "textAlign": "right"
      },
      "dataBinding": {
        "dataSource": "InvoiceHeader",
        "field": "Tax",
        "format": "C2"
      }
    },
    {
      "elementType": "Rectangle",
      "name": "TotalBackground",
      "section": "ReportFooter",
      "zIndex": 16,
      "layout": { "x": 125, "y": 32, "width": 70, "height": 10 },
      "style": {
        "backgroundColor": "#3498db",
        "opacity": 1
      },
      "properties": {
        "filled": true
      }
    },
    {
      "elementType": "TextBox",
      "name": "TotalLabel",
      "section": "ReportFooter",
      "zIndex": 17,
      "layout": { "x": 125, "y": 33, "width": 35, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 14,
        "fontWeight": "bold",
        "color": "#ffffff",
        "textAlign": "right"
      },
      "properties": {
        "staticText": "TOTAL:"
      }
    },
    {
      "elementType": "TextBox",
      "name": "TotalAmount",
      "section": "ReportFooter",
      "zIndex": 18,
      "layout": { "x": 160, "y": 33, "width": 35, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 14,
        "fontWeight": "bold",
        "color": "#ffffff",
        "textAlign": "right"
      },
      "dataBinding": {
        "dataSource": "InvoiceHeader",
        "field": "Total",
        "format": "C2"
      }
    },
    {
      "elementType": "TextBox",
      "name": "ThankYou",
      "section": "ReportFooter",
      "zIndex": 19,
      "layout": { "x": 15, "y": 50, "width": 180, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 11,
        "fontStyle": "italic",
        "color": "#7f8c8d",
        "textAlign": "center"
      },
      "properties": {
        "staticText": "Thank you for your business!"
      }
    },
    {
      "elementType": "TextBox",
      "name": "PageNumber",
      "section": "PageFooter",
      "zIndex": 20,
      "layout": { "x": 15, "y": 280, "width": 180, "height": 6 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 9,
        "color": "#95a5a6",
        "textAlign": "center"
      },
      "properties": {
        "staticText": "Page 1"
      }
    }
  ]
}
```

**Usage:**
```bash
# Render invoice for order ID 12345
curl "https://yourserver/api/ReportRender/1?invoiceId=12345"

# Download as PDF
curl "https://yourserver/api/ReportRender/1/download?format=pdf&invoiceId=12345" -o invoice.pdf
```

## Sample 2: Sales Dashboard Report

A summary report with grouping and aggregations.

```json
{
  "name": "Sales Dashboard",
  "description": "Sales summary by region and product category",
  "pageSettings": {
    "paperSize": "A4",
    "orientation": "Landscape",
    "width": 297,
    "height": 210
  },
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
    },
    {
      "name": "region",
      "label": "Region",
      "dataType": "String",
      "isRequired": false,
      "displayOrder": 3,
      "allowedValues": [
        { "label": "All Regions", "value": "" },
        { "label": "North", "value": "North" },
        { "label": "South", "value": "South" },
        { "label": "East", "value": "East" },
        { "label": "West", "value": "West" }
      ]
    }
  ],
  "dataSources": [
    {
      "name": "SalesData",
      "sourceType": "StoredProcedure",
      "connectionString": "YOUR_CONNECTION_STRING",
      "sourceDefinition": "sp_GetSalesSummary",
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
        },
        {
          "name": "Region",
          "dataType": "String",
          "useReportParameter": true,
          "reportParameterName": "region"
        }
      ],
      "isPrimary": true
    }
  ],
  "groups": [
    {
      "field": "Region",
      "groupName": "Region",
      "showHeader": true,
      "showFooter": true,
      "level": 1
    }
  ],
  "sorting": [
    {
      "field": "Region",
      "direction": "Asc",
      "order": 1
    },
    {
      "field": "TotalSales",
      "direction": "Desc",
      "order": 2
    }
  ],
  "elements": [
    {
      "elementType": "TextBox",
      "name": "ReportTitle",
      "section": "ReportHeader",
      "layout": { "x": 20, "y": 10, "width": 257, "height": 12 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 20,
        "fontWeight": "bold",
        "textAlign": "center",
        "color": "#2c3e50"
      },
      "properties": {
        "staticText": "Sales Dashboard"
      }
    },
    {
      "elementType": "TextBox",
      "name": "DateRange",
      "section": "ReportHeader",
      "layout": { "x": 20, "y": 25, "width": 257, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 12,
        "textAlign": "center",
        "color": "#7f8c8d"
      },
      "dataBinding": {
        "formula": "Period: {startDate} to {endDate}"
      }
    },
    {
      "elementType": "Table",
      "name": "SalesTable",
      "section": "Details",
      "layout": { "x": 20, "y": 45, "width": 257, "height": 120 },
      "dataBinding": {
        "dataSource": "SalesData"
      },
      "properties": {
        "columns": [
          {
            "header": "Product Category",
            "field": "Category",
            "width": 80,
            "alignment": "left"
          },
          {
            "header": "Units Sold",
            "field": "UnitsSold",
            "width": 50,
            "alignment": "right",
            "aggregateFunction": "Sum"
          },
          {
            "header": "Total Sales",
            "field": "TotalSales",
            "width": 60,
            "alignment": "right",
            "format": "C2",
            "aggregateFunction": "Sum"
          },
          {
            "header": "Avg Price",
            "field": "AveragePrice",
            "width": 50,
            "alignment": "right",
            "format": "C2",
            "aggregateFunction": "Avg"
          }
        ],
        "showHeader": true,
        "alternateRowColors": true,
        "showBorders": true
      }
    }
  ]
}
```

**Usage:**
```bash
# All regions for Q1 2024
curl "https://yourserver/api/ReportRender/2?startDate=2024-01-01&endDate=2024-03-31"

# North region only for full year
curl "https://yourserver/api/ReportRender/2?startDate=2024-01-01&endDate=2024-12-31&region=North"
```

## Sample 3: Employee Directory

Simple directory listing with photos.

```json
{
  "name": "Employee Directory",
  "description": "Company employee contact information",
  "pageSettings": {
    "paperSize": "A4",
    "orientation": "Portrait"
  },
  "dataSources": [
    {
      "name": "Employees",
      "sourceType": "Table",
      "connectionString": "YOUR_CONNECTION_STRING",
      "sourceDefinition": "Employees",
      "isPrimary": true
    }
  ],
  "sorting": [
    {
      "field": "Department",
      "direction": "Asc",
      "order": 1
    },
    {
      "field": "LastName",
      "direction": "Asc",
      "order": 2
    }
  ],
  "elements": [
    {
      "elementType": "TextBox",
      "name": "Title",
      "section": "ReportHeader",
      "layout": { "x": 15, "y": 15, "width": 180, "height": 12 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 22,
        "fontWeight": "bold",
        "textAlign": "center"
      },
      "properties": {
        "staticText": "Employee Directory"
      }
    },
    {
      "elementType": "Image",
      "name": "EmployeePhoto",
      "section": "Details",
      "layout": { "x": 15, "y": 35, "width": 25, "height": 30 },
      "dataBinding": {
        "dataSource": "Employees",
        "field": "PhotoUrl"
      },
      "properties": {
        "sourceType": "Field",
        "sizing": "Fit"
      }
    },
    {
      "elementType": "TextBox",
      "name": "EmployeeName",
      "section": "Details",
      "layout": { "x": 45, "y": 35, "width": 70, "height": 8 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 14,
        "fontWeight": "bold"
      },
      "dataBinding": {
        "dataSource": "Employees",
        "formula": "[FirstName] [LastName]"
      }
    },
    {
      "elementType": "TextBox",
      "name": "Department",
      "section": "Details",
      "layout": { "x": 45, "y": 45, "width": 70, "height": 6 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 11,
        "color": "#7f8c8d"
      },
      "dataBinding": {
        "dataSource": "Employees",
        "field": "Department"
      }
    },
    {
      "elementType": "TextBox",
      "name": "Email",
      "section": "Details",
      "layout": { "x": 45, "y": 53, "width": 70, "height": 6 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 10
      },
      "dataBinding": {
        "dataSource": "Employees",
        "field": "Email"
      },
      "properties": {
        "hyperlinkUrl": "mailto:[Email]"
      }
    },
    {
      "elementType": "TextBox",
      "name": "Phone",
      "section": "Details",
      "layout": { "x": 45, "y": 60, "width": 70, "height": 6 },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 10
      },
      "dataBinding": {
        "dataSource": "Employees",
        "field": "Phone"
      }
    }
  ]
}
```

## Testing Your Reports

### 1. Create a Report
```bash
curl -X POST https://yourserver/api/ReportDesigner \
  -H "Content-Type: application/json" \
  -d @invoice-report.json
```

### 2. Preview the Report
```bash
curl https://yourserver/api/ReportRender/1/preview > preview.html
```

### 3. Render with Parameters
```bash
curl "https://yourserver/api/ReportRender/1?invoiceId=12345&format=html" > invoice.html
```

### 4. Download as PDF
```bash
curl "https://yourserver/api/ReportRender/1/download?format=pdf&invoiceId=12345" -o invoice.pdf
```

## Customization Tips

1. **Adjust Colors**: Modify `style.color` and `style.backgroundColor` for branding
2. **Change Fonts**: Update `style.fontFamily` and `style.fontSize`
3. **Add Borders**: Use `style.border` properties for custom borders
4. **Dynamic Images**: Use database fields for logos and product images
5. **Conditional Formatting**: Use `visibilityFormula` to show/hide elements
6. **Custom Formulas**: Create calculated fields with `dataBinding.formula`

Happy reporting!
