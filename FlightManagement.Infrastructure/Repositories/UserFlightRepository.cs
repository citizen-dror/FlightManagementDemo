using FlightManagement.Domain.Entities;
using FlightManagement.Domain.Interfaces;
using FlightManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace FlightManagement.Infrastructure.Repositories
{
    public class UserFlightRepository : IUserFlightRepository
    {
        private readonly ApplicationDbContext _context;

        public UserFlightRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserFlight userFlight)
        {
            await _context.UserFlights.AddAsync(userFlight);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserFlight>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserFlights
                .Where(uf => uf.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserFlight>> GetByFlightIdAsync(Guid flightId)
        {
            return await _context.UserFlights
                .Where(uf => uf.FlightId == flightId)
                .ToListAsync();
        }

        public async Task DeleteAsync(Guid userId, Guid flightId)
        {
            var userFlight = await _context.UserFlights
                .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.FlightId == flightId);

            if (userFlight != null)
            {
                _context.UserFlights.Remove(userFlight);
                await _context.SaveChangesAsync();
            }
        }
    }
}
