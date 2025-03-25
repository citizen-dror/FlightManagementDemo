using FlightManagement.AlertService;
using FlightManagement.AlertService.Mappings;
using FlightManagement.Common.Logging;
using FlightManagement.Domain.Interfaces;
using FlightManagement.Infrastructure.Persistence;
using FlightManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load configuration 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing.");
}

// Add services to DI
builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Database Context (Ensure it's before repositories)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register repositories
builder.Services.AddScoped<IPriceAlertRepository, PriceAlertRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(PriceAlertProfile));

// Register services
builder.Services.AddScoped<IPriceAlertService, PriceAlertService>();

try
{
    var app = builder.Build();

    // Configure middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseMiddleware<HttpLoggerMiddleware>(); // Custom middleware
    app.UseHttpLogging(); // logging for headers, query params, etc.

    app.UseHttpsRedirection();
    app.UseAuthorization();
    // Map Controllers (Ensure routes work)
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup error: {ex}");
    if (ex.InnerException != null)
        Console.WriteLine($"Inner exception: {ex.InnerException}");
}
