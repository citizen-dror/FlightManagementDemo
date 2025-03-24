using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Domain.Entities
{
    public class PriceAlert
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        // Alert criteria
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal TargetPrice { get; set; }
        public string Currency { get; set; } = "USD";  // Currency to track in

        // Flexibility options
        public bool IsFlexibleDate { get; set; }
        public int? FlexibleDays { get; set; } // Only used if IsFlexibleDate = true
        public bool OneWay { get; set; }

        // Status and expiration
        public bool IsActive { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Link to the user and matched flights
        public User User { get; set; }
        public ICollection<Flight> MatchedFlights { get; set; } = new List<Flight>();
    }
}
