using FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Domain.Interfaces
{
    public interface IUserFlightRepository
    {
        Task AddAsync(UserFlight userFlight);  // Add a user-flight association
        Task<IEnumerable<UserFlight>> GetByUserIdAsync(Guid userId);  // Get all user-flight associations for a given user
        Task<IEnumerable<UserFlight>> GetByFlightIdAsync(Guid flightId);  // Get all user-flight associations for a given flight
        Task DeleteAsync(Guid userId, Guid flightId);  // Delete a user-flight association
    }
}
