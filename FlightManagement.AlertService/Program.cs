using FlightManagement.AlertService;
using FlightManagement.AlertService.Mappings;
using FlightManagement.Common.Logging;
using FlightManagement.Domain.Interfaces;
using FlightManagement.Infrastructure.Persistence;
using FlightManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
// Logging
builder.Logging.AddConsole();
// Load configuration 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing.");
}

// Add services to DI
builder.Services.AddControllers();
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

    app.UseHttpsRedirection();
    // Register the ApiLogger middleware
    app.UseMiddleware<HttpLoggerMiddleware>();
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