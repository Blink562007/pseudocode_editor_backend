using Microsoft.EntityFrameworkCore;
using PseudocodeEditorAPI.Data;
using PseudocodeEditorAPI.Data.Repositories;
using PseudocodeEditorAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Data layer (SQLite)
var sqliteConnectionString = builder.Configuration.GetConnectionString("PseudocodeDb");
if (string.IsNullOrWhiteSpace(sqliteConnectionString))
{
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, "pseudocode.db");
    sqliteConnectionString = $"Data Source={dbPath}";
}

builder.Services.AddDbContext<PseudocodeDbContext>(options =>
    options.UseSqlite(sqliteConnectionString));

builder.Services.AddScoped<IPseudocodeDocumentRepository, PseudocodeDocumentRepository>();

// Register business layer services
builder.Services.AddScoped<IPseudocodeService, PseudocodeService>();
builder.Services.AddScoped<IPseudocodeValidationService, PseudocodeValidationService>();
builder.Services.AddScoped<IPseudocodeFormattingService, PseudocodeFormattingService>();
builder.Services.AddScoped<IPseudocodeExecutionService, PseudocodeExecutionService>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply EF Core migrations (creates local SQLite db if missing)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PseudocodeDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enable CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

app.Run();
