using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DonationHub.Infrastructure.Options
{
    public class JwtOptions
    {
        [Required]
        [MinLength(32)]
        public string SecretKey { get; set; } = string.Empty;

        [Required]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        public string Audience { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ExpiryMinutes { get; set; }

        [Range(1, int.MaxValue)]
        public int RefreshTokenExpiryDays { get; set; }
    }
}
