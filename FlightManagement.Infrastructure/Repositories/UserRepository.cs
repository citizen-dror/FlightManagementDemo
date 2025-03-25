using FlightManagement.Domain.Entities;
using FlightManagement.Domain.Interfaces;
using FlightManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid userId, bool includePriceAlerts = false, bool includeUserFlights = false)
        {
            var query = _context.Set<User>().AsQueryable();

            if (includePriceAlerts)
            {
                query = query.Include(u => u.PriceAlerts);  // Eager load PriceAlerts
            }

            if (includeUserFlights)
            {
                query = query.Include(u => u.UserFlights);  // Eager load UserFlights
            }

            return await query
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>().ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Set<User>().AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Set<User>().Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            if (user != null)
            {
                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetUsersWithPriceAlertsAsync()
        {
            return await _context.Set<User>()
                .Where(u => u.PriceAlerts.Any())
                .Include(u => u.PriceAlerts)  // Eager load PriceAlerts
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithUserFlightsAsync()
        {
            return await _context.Set<User>()
                .Where(u => u.UserFlights.Any())
                .Include(u => u.UserFlights)  // Eager load UserFlights
                .ToListAsync();
        }
    }

}
