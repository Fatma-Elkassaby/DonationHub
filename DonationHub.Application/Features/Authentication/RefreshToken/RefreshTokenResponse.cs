using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Authentication.RefreshToken
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAtUtc { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAtUtc { get; set; }
    }
}
