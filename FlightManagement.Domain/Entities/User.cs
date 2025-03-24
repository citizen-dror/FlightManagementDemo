using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string DeviceToken { get; set; }
        public string MobileOS { get; set; }

        // One-to-many relationship with price alerts
        public ICollection<PriceAlert> PriceAlerts { get; set; } = new List<PriceAlert>();
        public ICollection<UserFlight> UserFlights { get; set; } = new List<UserFlight>();
    }
}
