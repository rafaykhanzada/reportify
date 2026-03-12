# Reportify - Dynamic HTML Report Generator API

A comprehensive ASP.NET Core API for creating, editing, and rendering dynamic HTML reports with a 100% Crystal Reports-like experience.

## 🚀 Features

### Crystal Reports-Like Experience
- **Drag-and-Drop Designer**: Visual report builder with precise element positioning via API
- **Rich Element Library**: TextBoxes, Tables, Charts, Images, Lines, Rectangles
- **Data Binding**: Connect to databases, stored procedures, SQL queries, and APIs
- **Advanced Formula Engine**: 30+ functions (String, Math, Date, Aggregate, Conditional)
- **Expression-Based Grouping**: Group by fields OR custom expressions
- **Multi-Level Hierarchical Grouping**: Unlimited nesting with summaries
- **Group Summaries**: Sum, Average, Count, Min, Max, StdDev, and more
- **Running Totals**: Cumulative calculations with smart reset logic
- **Conditional Formatting**: Dynamic styling based on data and formulas
- **Three Evaluation Contexts**: Record, Group, and Report-level calculations

### Dynamic HTML Output
- **Pure HTML/CSS**: No static images, fully customizable
- **Responsive Design**: Works on desktop, tablet, and mobile
- **Print-Ready**: Optimized for printing and PDF generation
- **Embedded or Standalone**: Generate complete documents or HTML fragments

### Query Parameter Support
- **Flexible Automation**: Pass parameters via query strings
- **Type Detection**: Automatic parsing of integers, decimals, dates, booleans
- **RESTful Design**: Clean, intuitive API endpoints
- **Batch Processing**: Generate multiple reports with different parameters

### Professional Architecture
- **Clean Architecture**: Separation of concerns with Repository, Service, and API layers
- **Unit of Work Pattern**: Transaction management
- **AutoMapper**: DTO mapping
- **Entity Framework Core**: Database access
- **Swagger/OpenAPI**: Auto-generated API documentation

## 📋 Requirements

- .NET 8.0 SDK
- SQL Server (or any EF Core supported database)
- Visual Studio 2022 or VS Code

## 🔧 Installation

### 1. Clone the Repository
```bash
git clone https://github.com/yourcompany/reportify.git
cd reportify
```

### 2. Update Connection String
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ReportifyDB;Trusted_Connection=True;"
  }
}
```

### 3. Run Database Migrations
```bash
cd ReportGeneratorProject
dotnet ef database update
```

### 4. Build and Run
```bash
dotnet build
dotnet run
```

The API will be available at `https://localhost:5008` (or the configured URL).

### 5. Access Swagger Documentation
Navigate to `https://localhost:5008/swagger` to explore the API endpoints.

## 📖 Quick Start

### Create Your First Report

```bash
curl -X POST https://localhost:5008/api/ReportDesigner \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Customer List",
    "description": "Simple customer listing",
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
```

### Render the Report

```bash
# View in browser
curl https://localhost:5008/api/ReportRender/1 > report.html

# With parameters
curl "https://localhost:5008/api/ReportRender/1?startDate=2024-01-01&endDate=2024-12-31"

# Download as PDF
curl "https://localhost:5008/api/ReportRender/1/download?format=pdf" -o report.pdf
```

## 🎯 API Endpoints

### Report Designer
- `GET /api/ReportDesigner` - List all reports
- `GET /api/ReportDesigner/{id}` - Get report design
- `POST /api/ReportDesigner` - Create/update report
- `PUT /api/ReportDesigner/{id}` - Update report
- `DELETE /api/ReportDesigner/{id}` - Delete report
- `POST /api/ReportDesigner/{id}/duplicate` - Duplicate report
- `GET /api/ReportDesigner/{reportId}/elements` - Get report elements
- `POST /api/ReportDesigner/{reportId}/elements` - Save element

### Report Rendering
- `GET /api/ReportRender/{reportId}` - Render with query parameters
- `POST /api/ReportRender/{reportId}` - Render with body parameters
- `GET /api/ReportRender/{reportId}/preview` - Preview report
- `GET /api/ReportRender/{reportId}/download` - Download report
- `GET /api/ReportRender/{reportId}/metadata` - Get report metadata

## 📚 Documentation

- **[Complete API Guide](REPORT_API_GUIDE.md)** - Comprehensive API documentation
- **[Grouping & Formula Guide](GROUPING_AND_FORMULAS_GUIDE.md)** - Advanced grouping and formulas ⭐ NEW
- **[Sample Reports](SAMPLE_REPORTS.md)** - Ready-to-use report templates
- **[Advanced Example](ADVANCED_EXAMPLE_SALES_REPORT.json)** - Complete working example ⭐ NEW
- **[Quick Test Guide](QUICK_TEST.md)** - Step-by-step testing instructions
- **[Swagger UI](https://localhost:5008/swagger)** - Interactive API documentation

## 🎨 Report Elements

### TextBox
Display static or dynamic text with full formatting control.

### Table
Multi-column data tables with headers, aggregations, and alternating row colors.

### Image
Display images from URLs, base64, or database fields.

### Chart
Bar, line, pie, and area charts (coming soon).

### Line
Horizontal and vertical lines for visual separation.

### Rectangle
Filled or outlined rectangles for backgrounds and emphasis.

## 🔌 Data Sources

### Supported Types
- **SQL Tables**: Direct table access
- **Stored Procedures**: With parameter mapping
- **SQL Queries**: Custom SQL with parameters
- **APIs**: RESTful API integration (coming soon)

### Connection Management
- Encrypted connection strings
- Connection pooling
- Multiple data sources per report
- Parent-child relationships for sub-reports

## 📊 Parameters

### Parameter Types
- String
- Integer
- Decimal
- Date
- Boolean

### Parameter Features
- Default values
- Required/optional
- Validation rules
- Dropdown lists (allowed values)
- Dynamic parameter binding to data sources

## 🎭 Sections

Reports are organized into sections:

- **PageHeader**: Top of every page
- **ReportHeader**: Start of report (once)
- **Details**: Main content (repeats for each row)
- **GroupHeader**: Start of each group
- **GroupFooter**: End of each group
- **ReportFooter**: End of report (once)
- **PageFooter**: Bottom of every page

## 📈 Advanced Grouping & Formulas

### Expression-Based Grouping ⭐ NEW
Group by custom expressions, not just fields:
```javascript
"expression": "IF([Price] < 100, \"Budget\", \"Premium\")"
```

### Multi-Level Hierarchical Grouping
- Unlimited nesting levels
- Independent configuration per level
- Custom header/footer templates
- Keep together on same page
- Repeat headers on new pages

### Group Summaries ⭐ NEW
Automatic aggregate calculations per group:
- Sum, Average, Count, Min, Max
- DistinctCount, StdDev, Variance
- First, Last values
- Display in headers or footers

### Formula Fields ⭐ NEW
Dynamic calculated fields with 30+ functions:
```javascript
// String Functions
"UPPER([Name])", "LEFT([Code], 3)", "CONCAT([First], \" \", [Last])"

// Math Functions
"[Quantity] * [Price]", "ROUND([Total], 2)", "ABS([Variance])"

// Date Functions
"DATEDIFF([DueDate], TODAY())", "YEAR([OrderDate])"

// Conditional Logic
"IF([Status] = \"Active\", \"✓\", \"✗\")"
"IF([Amount] > 1000, [Amount] * 0.9, [Amount])"

// Aggregate Functions (in groups)
"SUM([Sales])", "AVG([Price])", "COUNT([Orders])"

// Special Functions
"RECORDNUMBER()", "PAGENUMBER()", "TOTALPAGES()"
```

### Running Totals ⭐ NEW
Cumulative calculations with smart reset logic:
- Reset on group changes
- Reset on field changes
- Never reset (grand totals)

### Conditional Formatting ⭐ NEW
Apply styles dynamically based on conditions:
```javascript
"condition": "[SalesAmount] > 5000",
"style": { "backgroundColor": "#90EE90", "fontWeight": "bold" }
```

## 🔐 Security Considerations

### Recommended Security Measures
1. **Authentication**: Add JWT or OAuth authentication
2. **Authorization**: Implement role-based access control
3. **Input Validation**: Validate all report parameters
4. **SQL Injection Prevention**: Use parameterized queries
5. **Connection String Encryption**: Encrypt sensitive data
6. **Rate Limiting**: Prevent abuse with rate limiting
7. **CORS**: Configure appropriate CORS policies

## 🚀 Deployment

### Docker
```bash
docker build -t reportify .
docker run -p 5008:5008 reportify
```

### IIS
1. Publish the application: `dotnet publish -c Release`
2. Create IIS site pointing to publish folder
3. Configure application pool for .NET Core
4. Set appropriate permissions

### Azure App Service
1. Create App Service in Azure Portal
2. Configure connection strings in Application Settings
3. Deploy using Visual Studio, VS Code, or Azure CLI

## 🛠️ Development

### Project Structure
```
reportify/
├── Core/                          # Domain models, DTOs, utilities
│   ├── Data/
│   │   ├── Models/               # Entity models
│   │   ├── DTOs/                 # Data transfer objects
│   │   └── Context/              # Database context
│   └── Utils/                    # Helper utilities
├── Repository/                    # Data access layer
│   ├── Repository/               # Repository implementations
│   └── IRepository/              # Repository interfaces
├── Service/                       # Business logic layer
│   ├── Service/                  # Service implementations
│   └── IService/                 # Service interfaces
├── UnitOfWork/                    # Unit of Work pattern
├── ReportGeneratorProject/        # Web API project
│   ├── Controllers/              # API controllers
│   └── Program.cs                # Application startup
└── README.md
```

### Adding New Features
1. Add models to `Core/Data/Models`
2. Add DTOs to `Core/Data/DTOs`
3. Create repository interface and implementation
4. Create service interface and implementation
5. Add controller endpoint
6. Update AutoMapper profile
7. Run migrations if database changes

### Running Tests
```bash
dotnet test
```

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit your changes: `git commit -am 'Add new feature'`
4. Push to the branch: `git push origin feature/my-feature`
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For support, questions, or feedback:
- Create an issue in the GitHub repository
- Email: support@yourcompany.com
- Documentation: [Complete API Guide](REPORT_API_GUIDE.md)

## 🗺️ Roadmap

### Version 1.1
- [ ] PDF export functionality
- [ ] Excel export functionality
- [ ] Chart elements (Bar, Line, Pie)
- [ ] Advanced formula engine

### Version 1.2
- [ ] Sub-reports support
- [ ] Cross-tab reports
- [ ] Report scheduler
- [ ] Email delivery

### Version 2.0
- [ ] Web-based report designer UI
- [ ] Report sharing and collaboration
- [ ] Report versioning
- [ ] Template marketplace

## 🙏 Acknowledgments

- Inspired by Crystal Reports and BoldReports
- Built with ASP.NET Core
- Uses Entity Framework Core for data access
- Swagger/OpenAPI for documentation

---

**Built with ❤️ by your development team**
