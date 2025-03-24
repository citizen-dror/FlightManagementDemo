using FlightManagement.Domain.Entities;
using FlightManagement.Domain.Interfaces;
using FlightManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace FlightManagement.Infrastructure.Repositories
{
    public class PriceAlertRepository : IPriceAlertRepository
    {
        private readonly ApplicationDbContext _context;

        public PriceAlertRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PriceAlert> GetByIdAsync(Guid alertId)
        {
            return await _context.PriceAlerts.FindAsync(alertId);
        }

        public async Task<IEnumerable<PriceAlert>> GetByUserIdAsync(Guid userId)
        {
            return await _context.PriceAlerts
                .Where(pa => pa.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PriceAlert>> GetActiveAlertsByUserIdAsync(Guid userId)
        {
            return await _context.PriceAlerts
                .Where(pa => pa.UserId == userId && pa.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(PriceAlert priceAlert)
        {
            await _context.PriceAlerts.AddAsync(priceAlert);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PriceAlert priceAlert)
        {
            _context.PriceAlerts.Update(priceAlert);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid alertId)
        {
            var priceAlert = await _context.PriceAlerts.FindAsync(alertId);
            if (priceAlert != null)
            {
                _context.PriceAlerts.Remove(priceAlert);
                await _context.SaveChangesAsync();
            }
        }
    }
}
