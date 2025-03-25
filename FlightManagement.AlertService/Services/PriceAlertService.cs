using FlightManagement.Domain.Entities;
using FlightManagement.Domain.Interfaces;
using FlightManagement.Common.DTOs;
using AutoMapper;

namespace FlightManagement.AlertService
{
    public class PriceAlertService : IPriceAlertService
    {
        private readonly IPriceAlertRepository _priceAlertRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PriceAlertService> _logger;

        public PriceAlertService(IPriceAlertRepository alertRepository, IUserRepository userRepository, IMapper mapper, ILogger<PriceAlertService> logger)
        {
            _priceAlertRepository = alertRepository;
            _userRepository = userRepository;
            _mapper = mapper;
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

            foreach (var flight in flightPrices)
            {
                // Query matching alerts from the database with same origin, destination, and higher price
                var matchingAlerts = await _priceAlertRepository.GetMatchingAlertsAsync(flight.DepartureAirport, flight.ArrivalAirport, flight.Price);

                foreach (var alert in matchingAlerts)
                {
                    _logger.LogInformation($"Price alert triggered for User {alert.UserId}: Flight {flight.FlightNumber} ({flight.Airline}) from {flight.DepartureAirport} to {flight.ArrivalAirport} at {flight.Price} {flight.Currency}");

                    alertCount++; // Increment count for each triggered alert

                    // In the future, send this event to RabbitMQ
                }
            }
            _logger.LogInformation($"Total price alerts triggered: {alertCount}");
            return alertCount;
        }
    }
}
