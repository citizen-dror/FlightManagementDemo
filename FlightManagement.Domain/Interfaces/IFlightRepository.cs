using FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Domain.Interfaces
{
    public interface IFlightRepository
    {
        Task<Flight> GetByIdAsync(Guid flightId);  // Get a flight by its unique Id
        Task<IEnumerable<Flight>> GetAllAsync();  // Get all flights
        Task<IEnumerable<Flight>> GetByOriginAndDestinationAsync(string origin, string destination);  // Get flights by origin and destination
        Task<IEnumerable<Flight>> GetByUserIdAsync(Guid userId);  // Get flights associated with a user via UserFlights
        Task AddAsync(Flight flight);  // Add a new flight
        Task UpdateAsync(Flight flight);  // Update an existing flight
        Task DeleteAsync(Guid flightId);  // Delete a flight by its Id
    }
}
