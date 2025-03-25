using AutoMapper;
using Moq;
using Xunit;
using FluentAssertions;
using FlightManagement.AlertService;
using FlightManagement.Domain.Entities;
using FlightManagement.Common.DTOs;
using FlightManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlightManagement.Tests.AlertService.Services
{
    public class PriceAlertServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPriceAlertRepository> _priceAlertRepositoryMock;
        private readonly Mock<ILogger<PriceAlertService>> _loggerMock;
        private readonly IMapper _mapper;
        private readonly PriceAlertService _priceAlertService;

        public PriceAlertServiceTests()
        {
            _priceAlertRepositoryMock = new Mock<IPriceAlertRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<PriceAlertService>>();

            // Setup AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PriceAlertDto, PriceAlert>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
                cfg.CreateMap<PriceAlert, PriceAlertDto>();
            });
            _mapper = mapperConfig.CreateMapper();

            _priceAlertService = new PriceAlertService(
                _priceAlertRepositoryMock.Object,
                _userRepositoryMock.Object,
                _mapper,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAlertsByUserIdAsync_ShouldReturnMappedAlerts()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var alerts = new List<PriceAlert>
        {
            new PriceAlert { Id = Guid.NewGuid(), UserId = userId, Origin = "NYC", Destination = "LAX", TargetPrice = 300 },
            new PriceAlert { Id = Guid.NewGuid(), UserId = userId, Origin = "LAX", Destination = "SFO", TargetPrice = 150 }
        };

            _priceAlertRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(alerts);

            // Act
            var result = await _priceAlertService.GetAlertsByUserIdAsync(userId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(x => x.Origin == "NYC" && x.Destination == "LAX");
            result.Should().Contain(x => x.Origin == "LAX" && x.Destination == "SFO");
        }

        [Fact]
        public async Task CreateAlertAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var alertDto = new PriceAlertDto { UserId = Guid.NewGuid(), Origin = "NYC", Destination = "LAX", TargetPrice = 300 };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(alertDto.UserId, false, false))
                .ReturnsAsync((User)null); // Simulate user not found

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _priceAlertService.CreateAlertAsync(alertDto));
        }

        [Fact]
        public async Task CreateAlertAsync_ShouldCreateAndReturnAlert()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var alertDto = new PriceAlertDto { UserId = userId, Origin = "NYC", Destination = "LAX", TargetPrice = 300 };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId, false, false))
                .ReturnsAsync(new User { Id = userId });

            _priceAlertRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<PriceAlert>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _priceAlertService.CreateAlertAsync(alertDto);

            // Assert
            result.Should().NotBeNull();
            result.Origin.Should().Be("NYC");
            result.Destination.Should().Be("LAX");
        }

        [Fact]
        public async Task CheckAlertsAsync_WhenMatchingAlertsExist_ShouldReturnCorrectAlertCount()
        {
            // Arrange
            var flightPrices = new List<FlightPriceDto>
        {
            new FlightPriceDto
            {
                FlightNumber = "BA456",
                Airline = "British Airways",
                DepartureAirport = "LHR",
                ArrivalAirport = "JFK",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(7),
                Price = 450.00m,
                Currency = "USD"
            }
        };

            var matchingAlerts = new List<PriceAlert>
        {
            new PriceAlert { UserId = Guid.NewGuid(), Origin = "LHR", Destination = "JFK", TargetPrice = 500.00m },
            new PriceAlert { UserId = Guid.NewGuid(), Origin = "LHR", Destination = "JFK", TargetPrice = 480.00m }
        };

            _priceAlertRepositoryMock
                .Setup(repo => repo.GetMatchingAlertsAsync("LHR", "JFK", 450.00m))
                .ReturnsAsync(matchingAlerts);

            // Act
            int alertCount = await _priceAlertService.CheckAlertsAsync(flightPrices);

            // Assert
            Assert.Equal(2, alertCount);
            _priceAlertRepositoryMock.Verify(repo => repo.GetMatchingAlertsAsync("LHR", "JFK", 450.00m), Times.Once);
        }

        [Fact]
        public async Task CheckAlertsAsync_WhenNoMatchingAlertsExist_ShouldReturnZero()
        {
            // Arrange
            var flightPrices = new List<FlightPriceDto>
        {
            new FlightPriceDto
            {
                FlightNumber = "BA456",
                Airline = "British Airways",
                DepartureAirport = "LHR",
                ArrivalAirport = "JFK",
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(7),
                Price = 450.00m,
                Currency = "USD"
            }
        };

            _priceAlertRepositoryMock
                .Setup(repo => repo.GetMatchingAlertsAsync("LHR", "JFK", 450.00m))
                .ReturnsAsync(new List<PriceAlert>());

            // Act
            int alertCount = await _priceAlertService.CheckAlertsAsync(flightPrices);

            // Assert
            Assert.Equal(0, alertCount);
            _priceAlertRepositoryMock.Verify(repo => repo.GetMatchingAlertsAsync("LHR", "JFK", 450.00m), Times.Once);
        }
    }
}
