# Quick Test Guide

This guide will help you test the Dynamic HTML Report Generator API immediately after setup.

## Prerequisites

1. SQL Server running locally or accessible
2. Application is running (`dotnet run`)
3. Connection string is configured in `appsettings.json`

## Step 1: Create a Test Database

Run this SQL script to create a simple test database:

```sql
-- Create test database
CREATE DATABASE ReportifyTest;
GO

USE ReportifyTest;
GO

-- Create Customers table
CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    CustomerName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    City NVARCHAR(50),
    State NVARCHAR(50),
    CreatedDate DATE DEFAULT GETDATE()
);

-- Insert sample data
INSERT INTO Customers (CustomerName, Email, Phone, City, State) VALUES
('John Doe', 'john.doe@email.com', '555-0101', 'New York', 'NY'),
('Jane Smith', 'jane.smith@email.com', '555-0102', 'Los Angeles', 'CA'),
('Bob Johnson', 'bob.johnson@email.com', '555-0103', 'Chicago', 'IL'),
('Alice Williams', 'alice.williams@email.com', '555-0104', 'Houston', 'TX'),
('Charlie Brown', 'charlie.brown@email.com', '555-0105', 'Phoenix', 'AZ'),
('Diana Davis', 'diana.davis@email.com', '555-0106', 'Philadelphia', 'PA'),
('Edward Miller', 'edward.miller@email.com', '555-0107', 'San Antonio', 'TX'),
('Fiona Wilson', 'fiona.wilson@email.com', '555-0108', 'San Diego', 'CA'),
('George Moore', 'george.moore@email.com', '555-0109', 'Dallas', 'TX'),
('Helen Taylor', 'helen.taylor@email.com', '555-0110', 'San Jose', 'CA');

-- Create Orders table
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId),
    OrderDate DATE NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Pending'
);

-- Insert sample orders
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount, Status) VALUES
(1, '2024-01-15', 1250.00, 'Completed'),
(2, '2024-01-16', 750.50, 'Completed'),
(1, '2024-01-20', 2100.75, 'Pending'),
(3, '2024-01-22', 450.00, 'Completed'),
(4, '2024-01-25', 890.25, 'Shipped'),
(5, '2024-02-01', 1520.00, 'Completed'),
(2, '2024-02-03', 675.50, 'Pending'),
(6, '2024-02-05', 3250.00, 'Completed'),
(7, '2024-02-10', 925.75, 'Shipped'),
(8, '2024-02-15', 1100.00, 'Completed');

-- Create a stored procedure for report
CREATE PROCEDURE sp_GetCustomerOrders
    @State NVARCHAR(50) = NULL,
    @StartDate DATE = NULL,
    @EndDate DATE = NULL
AS
BEGIN
    SELECT 
        c.CustomerName,
        c.Email,
        c.City,
        c.State,
        o.OrderDate,
        o.TotalAmount,
        o.Status
    FROM Customers c
    LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
    WHERE 
        (@State IS NULL OR c.State = @State)
        AND (@StartDate IS NULL OR o.OrderDate >= @StartDate)
        AND (@EndDate IS NULL OR o.OrderDate <= @EndDate)
    ORDER BY c.CustomerName, o.OrderDate;
END
GO
```

## Step 2: Update Connection String

Update your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ReportifyTest;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Step 3: Create Your First Report

Save this as `create-report.json`:

```json
{
  "name": "Customer Orders Report",
  "description": "List of customer orders with filtering",
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
  "parameters": [
    {
      "name": "state",
      "label": "State",
      "dataType": "String",
      "isRequired": false,
      "displayOrder": 1
    },
    {
      "name": "startDate",
      "label": "Start Date",
      "dataType": "Date",
      "isRequired": false,
      "defaultValue": "2024-01-01",
      "displayOrder": 2
    },
    {
      "name": "endDate",
      "label": "End Date",
      "dataType": "Date",
      "isRequired": false,
      "defaultValue": "2024-12-31",
      "displayOrder": 3
    }
  ],
  "dataSources": [
    {
      "name": "CustomerOrders",
      "sourceType": "StoredProcedure",
      "connectionString": "Server=localhost;Database=ReportifyTest;Trusted_Connection=True;",
      "sourceDefinition": "sp_GetCustomerOrders",
      "parameters": [
        {
          "name": "State",
          "dataType": "String",
          "useReportParameter": true,
          "reportParameterName": "state"
        },
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
  "elements": [
    {
      "elementType": "TextBox",
      "name": "ReportTitle",
      "section": "ReportHeader",
      "zIndex": 1,
      "layout": {
        "x": 20,
        "y": 20,
        "width": 170,
        "height": 15
      },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 24,
        "fontWeight": "bold",
        "textAlign": "center",
        "color": "#2c3e50"
      },
      "properties": {
        "staticText": "Customer Orders Report"
      }
    },
    {
      "elementType": "Line",
      "name": "TitleLine",
      "section": "ReportHeader",
      "zIndex": 2,
      "layout": {
        "x": 20,
        "y": 38,
        "width": 170,
        "height": 1
      },
      "style": {
        "backgroundColor": "#3498db"
      },
      "properties": {
        "thickness": 2,
        "lineStyle": "solid"
      }
    },
    {
      "elementType": "Table",
      "name": "OrdersTable",
      "section": "Details",
      "zIndex": 3,
      "layout": {
        "x": 20,
        "y": 45,
        "width": 170,
        "height": 200
      },
      "dataBinding": {
        "dataSource": "CustomerOrders"
      },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 10
      },
      "properties": {
        "columns": [
          {
            "header": "Customer",
            "field": "CustomerName",
            "width": 60,
            "alignment": "left"
          },
          {
            "header": "City",
            "field": "City",
            "width": 40,
            "alignment": "left"
          },
          {
            "header": "State",
            "field": "State",
            "width": 20,
            "alignment": "center"
          },
          {
            "header": "Order Date",
            "field": "OrderDate",
            "width": 30,
            "alignment": "center",
            "format": "d"
          },
          {
            "header": "Amount",
            "field": "TotalAmount",
            "width": 30,
            "alignment": "right",
            "format": "C2"
          }
        ],
        "showHeader": true,
        "alternateRowColors": true,
        "alternateRowColor": "#f8f9fa",
        "showBorders": true,
        "rowHeight": 20
      }
    },
    {
      "elementType": "TextBox",
      "name": "Footer",
      "section": "PageFooter",
      "zIndex": 4,
      "layout": {
        "x": 20,
        "y": 270,
        "width": 170,
        "height": 8
      },
      "style": {
        "fontFamily": "Arial",
        "fontSize": 9,
        "color": "#7f8c8d",
        "textAlign": "center"
      },
      "properties": {
        "staticText": "Generated by Reportify - Dynamic Report Generator"
      }
    }
  ],
  "groups": [],
  "sorting": [
    {
      "field": "CustomerName",
      "direction": "Asc",
      "order": 1
    }
  ]
}
```

## Step 4: Test the API

### Using PowerShell (Windows)

```powershell
# 1. Create the report
$json = Get-Content -Path "create-report.json" -Raw
$response = Invoke-RestMethod -Uri "https://localhost:5008/api/ReportDesigner" `
    -Method Post `
    -ContentType "application/json" `
    -Body $json

$reportId = $response.data.id
Write-Host "Created report with ID: $reportId"

# 2. Preview the report
Invoke-WebRequest -Uri "https://localhost:5008/api/ReportRender/$reportId/preview" `
    -OutFile "preview.html"
Write-Host "Preview saved to preview.html"

# 3. Render with parameters - All orders
Invoke-WebRequest -Uri "https://localhost:5008/api/ReportRender/$reportId" `
    -OutFile "all-orders.html"
Write-Host "All orders report saved to all-orders.html"

# 4. Render with state filter - Texas only
Invoke-WebRequest -Uri "https://localhost:5008/api/ReportRender/$reportId?state=TX" `
    -OutFile "texas-orders.html"
Write-Host "Texas orders report saved to texas-orders.html"

# 5. Render with date range
Invoke-WebRequest -Uri "https://localhost:5008/api/ReportRender/$reportId?startDate=2024-02-01&endDate=2024-02-29" `
    -OutFile "february-orders.html"
Write-Host "February orders report saved to february-orders.html"

# Open the preview in browser
Start-Process "preview.html"
```

### Using curl (Linux/Mac/Windows with curl)

```bash
# 1. Create the report
curl -X POST https://localhost:5008/api/ReportDesigner \
  -H "Content-Type: application/json" \
  -d @create-report.json \
  -k | jq .

# Note the reportId from the response, then:

# 2. Preview the report (replace {reportId} with actual ID)
curl https://localhost:5008/api/ReportRender/1/preview -k > preview.html

# 3. Render with parameters - All orders
curl "https://localhost:5008/api/ReportRender/1" -k > all-orders.html

# 4. Render with state filter - California only
curl "https://localhost:5008/api/ReportRender/1?state=CA" -k > california-orders.html

# 5. Render with date range
curl "https://localhost:5008/api/ReportRender/1?startDate=2024-02-01&endDate=2024-02-29" -k > february-orders.html

# 6. Download as PDF (once PDF export is implemented)
# curl "https://localhost:5008/api/ReportRender/1/download?format=pdf&state=TX" -k > texas-orders.pdf
```

### Using Swagger UI

1. Navigate to `https://localhost:5008/swagger`
2. Click on **POST /api/ReportDesigner**
3. Click "Try it out"
4. Paste the JSON from `create-report.json`
5. Click "Execute"
6. Note the `id` from the response
7. Scroll to **GET /api/ReportRender/{reportId}**
8. Click "Try it out"
9. Enter the report ID
10. Click "Execute"
11. Copy the response and save as HTML file

## Step 5: Verify the Output

Open the generated HTML files in a web browser. You should see:

- **preview.html**: Full report with all data
- **all-orders.html**: Same as preview
- **texas-orders.html**: Only orders from customers in Texas
- **february-orders.html**: Only orders from February 2024

## Expected Results

### Preview/All Orders
You should see a table with all 10 customers and their orders, nicely formatted with:
- Professional header with title
- Blue line separator
- Table with alternating row colors
- Properly formatted dates and currency
- Footer text

### Texas Orders
Should show only:
- Alice Williams (Houston, TX)
- Edward Miller (San Antonio, TX)
- George Moore (Dallas, TX)

### February Orders
Should show only orders placed in February 2024.

## Common Issues

### Issue: "Report not found"
**Solution**: Make sure the report was created successfully. Check the response from the POST request for the ID.

### Issue: "Cannot connect to database"
**Solution**: 
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure database and tables exist
- Test connection string with SQL Server Management Studio

### Issue: "No data displayed"
**Solution**:
- Verify stored procedure exists: `SELECT * FROM sys.procedures WHERE name = 'sp_GetCustomerOrders'`
- Test stored procedure directly: `EXEC sp_GetCustomerOrders`
- Check if sample data was inserted: `SELECT * FROM Customers; SELECT * FROM Orders;`

### Issue: "HTML looks plain/unstyled"
**Solution**:
- Ensure `includeStyles=true` (default)
- Check that the HTML file opened correctly in the browser
- View page source to verify CSS is included

## Next Steps

Once you've verified the basic functionality:

1. **Experiment with Parameters**: Try different state values and date ranges
2. **Modify the Report**: Update the report design to add/remove columns
3. **Create New Reports**: Design reports for other scenarios
4. **Add Grouping**: Try grouping by State
5. **Implement Export**: Add PDF export functionality (see roadmap)
6. **Build UI**: Create a web-based report designer

## Performance Testing

Test with larger datasets:

```sql
-- Generate more test data
DECLARE @i INT = 11;
WHILE @i <= 1000
BEGIN
    INSERT INTO Customers (CustomerName, Email, Phone, City, State)
    VALUES (
        'Customer ' + CAST(@i AS VARCHAR),
        'customer' + CAST(@i AS VARCHAR) + '@email.com',
        '555-' + RIGHT('0000' + CAST(@i AS VARCHAR), 4),
        'City' + CAST(@i % 50 AS VARCHAR),
        CASE @i % 5 
            WHEN 0 THEN 'CA'
            WHEN 1 THEN 'TX'
            WHEN 2 THEN 'NY'
            WHEN 3 THEN 'FL'
            ELSE 'IL'
        END
    );
    SET @i = @i + 1;
END

-- Re-render the report and check performance
```

## Support

If you encounter issues:
1. Check the application logs
2. Review the Swagger documentation
3. Consult the [Complete API Guide](REPORT_API_GUIDE.md)
4. Check the [Sample Reports](SAMPLE_REPORTS.md) for more examples

Happy reporting! 🎉
