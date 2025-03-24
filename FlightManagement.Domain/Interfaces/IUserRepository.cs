using FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid userId, bool includePriceAlerts = false, bool includeUserFlights = false);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid userId);

        // Get users with specific price alerts or user flights
        Task<IEnumerable<User>> GetUsersWithPriceAlertsAsync();
        Task<IEnumerable<User>> GetUsersWithUserFlightsAsync();
    }
}
