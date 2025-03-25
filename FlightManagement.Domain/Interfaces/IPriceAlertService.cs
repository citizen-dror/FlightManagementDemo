using FlightManagement.Common.DTOs;

namespace FlightManagement.Domain.Interfaces
{
    public interface IPriceAlertService
    {
        Task<IEnumerable<PriceAlertDto>> GetAlertsByUserIdAsync(Guid userId);
        Task<PriceAlertDto> CreateAlertAsync(PriceAlertDto alertDto);
        Task<bool> DeleteAlertAsync(Guid alertId);
        Task<int> CheckAlertsAsync(IEnumerable<FlightPriceDto> flightPrices);
    }
}
