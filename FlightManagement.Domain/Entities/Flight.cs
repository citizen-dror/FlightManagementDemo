using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Domain.Entities
{
    public class Flight
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string DepartureAirport { get; set; } = string.Empty;
        public string ArrivalAirport { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";  // Currency of the flight price

        // Many-to-many relationship with alerts
        public ICollection<PriceAlert> MatchedAlerts { get; set; } = new List<PriceAlert>();
        // Many-to-many flights associated with a user
        public ICollection<UserFlight> UserFlights { get; set; } = new List<UserFlight>();
    }
}
