using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Domain.Entities
{
    public class UserFlight
    {
        public Guid UserId { get; set; }
        public Guid FlightId { get; set; }

        public User User { get; set; }
        public Flight Flight { get; set; }
    }
}
