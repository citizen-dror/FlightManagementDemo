using FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Domain.Interfaces
{
    public interface IPriceAlertRepository
    {
        Task<PriceAlert> GetByIdAsync(Guid alertId);  // Get a price alert by its Id
        Task<IEnumerable<PriceAlert>> GetByUserIdAsync(Guid userId);  // Get all price alerts for a user
        Task<IEnumerable<PriceAlert>> GetActiveAlertsByUserIdAsync(Guid userId);  // Get active alerts for a user
        Task AddAsync(PriceAlert priceAlert);  // Add a new price alert
        Task UpdateAsync(PriceAlert priceAlert);  // Update an existing price alert
        Task<bool> DeleteAsync(Guid alertId);  // Delete a price alert by its Id
    }
}
