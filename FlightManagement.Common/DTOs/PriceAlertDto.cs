using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Common.DTOs
{
    public class PriceAlertDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal TargetPrice { get; set; }
        public string Currency { get; set; } = "USD";
        public bool IsFlexibleDate { get; set; }
        public int? FlexibleDays { get; set; }
        public bool OneWay { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
