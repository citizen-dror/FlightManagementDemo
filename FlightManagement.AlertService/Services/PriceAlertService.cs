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

        public PriceAlertService(IPriceAlertRepository alertRepository, IUserRepository userRepository, IMapper mapper)
        {
            _priceAlertRepository = alertRepository;
            _userRepository = userRepository;
            _mapper = mapper;
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
    }
}
