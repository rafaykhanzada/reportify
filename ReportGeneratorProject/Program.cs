using Core.Data.Context;
using Microsoft.EntityFrameworkCore;
using Service.IService;
using Service.Service;
using UnitOfWork;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//builder.Services.AddControllers().AddNewtonsoftJson(options =>
//{
//    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
//    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore; // Optional, ignore null values
//});
// Add CORS to allow all origins
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()  // Allow all origins
               .AllowAnyMethod()  // Allow any HTTP method (GET, POST, etc.)
               .AllowAnyHeader(); // Allow any HTTP headers
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           
);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUnitOfWork,UnitOfwork>();
builder.Services.AddScoped<IWidgetService, WidgetService>();
builder.Services.AddScoped<IWidgetSettingsService, WidgetSettingsService>();
builder.Services.AddScoped<IWidgetPropertyService, WidgetPropertyService>();
builder.Services.AddScoped<IWidgetPropertyDataService, WidgetPropertyDataService>();
builder.Services.AddScoped<IWidgetReportService, WidgetReportService>();
builder.Services.AddScoped<IWidgetSaveDataService,WidgetSaveDataService>();
builder.Services.AddScoped<IDynamicDbContextService, DynamicDbContextService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportDesignerService, ReportDesignerService>();
builder.Services.AddScoped<IFormulaEvaluationService, FormulaEvaluationService>();
builder.Services.AddScoped<IHtmlReportRenderService, EnhancedHtmlReportRenderService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("https://192.168.19.86:5008/");
//builder.WebHost.UseUrls("https://192.168.26.104:5008/");
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run(); 

