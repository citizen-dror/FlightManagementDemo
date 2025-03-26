using FlightManagement.Domain.Entities;
using FlightManagement.Domain.Interfaces;
using FlightManagement.Common.DTOs;
using AutoMapper;
using FlightManagement.Infrastructure.RabbitMQ;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;

namespace FlightManagement.AlertService
{
    public class PriceAlertService : IPriceAlertService
    {
        private readonly IPriceAlertRepository _priceAlertRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly RabbitConnectionFactory _connectionFactory;
        private readonly RabbitSender _sender;
        private readonly ILogger<PriceAlertService> _logger;

        public PriceAlertService(IPriceAlertRepository alertRepository, 
            IUserRepository userRepository, IMapper mapper,
            RabbitConnectionFactory connectionFactory,
            RabbitSender sender,
            ILogger<PriceAlertService> logger)
        {
            _priceAlertRepository = alertRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _connectionFactory = connectionFactory;
            _sender = sender;
            _logger = logger;
        }

        public async Task<IEnumerable<PriceAlertDto>> GetAlertsByUserIdAsync(Guid userId)
        {
            var alerts = await _priceAlertRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<PriceAlertDto>>(alerts);
        }

        public async Task<PriceAlertDto> CreateAlertAsync(PriceAlertDto alertDto)
        {
            var user = await _userRepository.GetByIdAsync(alertDto.UserId);
            if (user == null)
                throw new Exception("User not found");

            var alert = _mapper.Map<PriceAlert>(alertDto);
            alert.Id = Guid.NewGuid(); // Ensure a new GUID is assigned
            alert.CreatedAt = DateTime.UtcNow; // Set creation timestamp

            await _priceAlertRepository.AddAsync(alert);
            return _mapper.Map<PriceAlertDto>(alert);
        }
        public async Task<bool> DeleteAlertAsync(Guid alertId)
        {
            return await _priceAlertRepository.DeleteAsync(alertId);
        }

        public async Task<int> CheckAlertsAsync(IEnumerable<FlightPriceDto> flightPrices)
        {
            int alertCount = 0;
            // Initialize logger 
            //var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<RabbitSender>();

            // Open connection and channel outside the loop to avoid repetitive creation
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                foreach (var flight in flightPrices)
                {
                    // Query matching alerts from the database with the same origin, destination, and higher price
                    var matchingAlerts = await _priceAlertRepository.GetMatchingAlertsAsync(flight.DepartureAirport, flight.ArrivalAirport, flight.Price);

                    foreach (var alert in matchingAlerts)
                    {
                        _logger.LogInformation($"Price alert triggered for User {alert.UserId}: Flight {flight.FlightNumber} ({flight.Airline}) from {flight.DepartureAirport} to {flight.ArrivalAirport} at {flight.Price} {flight.Currency}");
                        alertCount++; // Increment count for each triggered alert

                        // Create message with flight and alert details in JSON format
                        var message = new
                        {
                            AlertType = "Price Alert",
                            FlightNumber = flight.FlightNumber,
                            Airline = flight.Airline,
                            DepartureAirport = flight.DepartureAirport,
                            ArrivalAirport = flight.ArrivalAirport,
                            Price = flight.Price,
                            Currency = flight.Currency,
                            UserId = alert.UserId,
                            AlertPrice = alert.TargetPrice,
                            TriggeredAt = DateTime.UtcNow
                        };

                        // Convert the message to JSON format
                        var jsonMessage = JsonConvert.SerializeObject(message);

                        // Send the message to RabbitMQ
                        await _sender.SendMessageAsync("FlightsNotificationQueue", jsonMessage);
                    }
                }
            }
            _logger.LogInformation($"Total price alerts triggered: {alertCount}");
            return alertCount;
        }
    }
}
