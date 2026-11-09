using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Authentication.RefreshToken
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
