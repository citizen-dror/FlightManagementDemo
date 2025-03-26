using FlightManagement.AlertService;
using FlightManagement.AlertService.Mappings;
using FlightManagement.Common.Configs;
using FlightManagement.Common.Logging;
using FlightManagement.Domain.Interfaces;
using FlightManagement.Infrastructure.Persistence;
using FlightManagement.Infrastructure.RabbitMQ;
using FlightManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
ConfigureLogging(builder);

// Load Configuration
var connectionString = GetConnectionString(builder);
var rabbitMqConfig = ConfigureRabbitMQ(builder);

// Register Services
ConfigureServices(builder.Services, connectionString, rabbitMqConfig);

var app = builder.Build();

try
{
    // Configure Middleware
    ConfigureMiddleware(app);

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application startup error: {ex}");
    if (ex.InnerException != null)
        Console.WriteLine($"Inner exception: {ex.InnerException}");
}

#region Helper Functions

void ConfigureLogging(WebApplicationBuilder builder)
{
    builder.Logging.AddConsole();
}

string GetConnectionString(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Database connection string is missing.");
    }
    return connectionString;
}

RabbitMQConfig ConfigureRabbitMQ(WebApplicationBuilder builder)
{
    var config = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>();
    if (config == null)
    {
        throw new InvalidOperationException("RabbitMQ configuration is missing.");
    }
    return config;
}

void ConfigureServices(IServiceCollection services, string connectionString, RabbitMQConfig rabbitMqConfig)
{
    // Add controllers
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    // Add Database Context
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    // Register repositories
    services.AddScoped<IPriceAlertRepository, PriceAlertRepository>();
    services.AddScoped<IUserRepository, UserRepository>();

    // Register AutoMapper
    services.AddAutoMapper(typeof(PriceAlertProfile));

    // Register services
    services.AddScoped<IPriceAlertService, PriceAlertService>();

    // Register RabbitMQ dependencies
    services.AddSingleton(rabbitMqConfig); // Register RabbitMQConfig as a singleton
    services.AddSingleton<RabbitConnectionFactory>(); // Register RabbitConnectionFactory as a singleton
    services.AddScoped<RabbitSender>(); // Register RabbitSender as a scoped dependency
}

void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseMiddleware<HttpLoggerMiddleware>();
    app.UseAuthorization();
    app.MapControllers();
}

#endregion
