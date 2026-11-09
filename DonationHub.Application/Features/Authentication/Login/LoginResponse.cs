using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Authentication.Login
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime AccessTokenExpiresAtUtc { get; set; }

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime RefreshTokenExpiresAtUtc { get; set; }
    }
}
