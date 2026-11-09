using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Infrastructure.Options
{
    public class DonationOptions
    {
        [Range(typeof(decimal), "1.00", "10000.00")]
        public decimal MinAmount { get; set; } = 1.00m;

        [Range(typeof(decimal), "1.00", "10000.00")]
        public decimal MaxAmount { get; set; } = 10000.00m;

        [Range(1, int.MaxValue)]
        public int PendingTimeoutMinutes { get; set; } = 30;

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string DefaultCurrency { get; set; } = "USD";
    }
}
