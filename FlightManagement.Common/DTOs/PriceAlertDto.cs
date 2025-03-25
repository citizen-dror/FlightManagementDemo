using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Common.DTOs
{
    public class PriceAlertDto
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Origin { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Destination { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Target price must be between 1 and 10,000.")]
        public decimal TargetPrice { get; set; }

        [Required]
        [StringLength(10)]
        public string Currency { get; set; } = "USD";

        public bool IsFlexibleDate { get; set; }

        [Range(1, 30, ErrorMessage = "Flexible days must be between 1 and 30.")]
        public int? FlexibleDays { get; set; }

        public bool OneWay { get; set; }
        public bool IsActive { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}
