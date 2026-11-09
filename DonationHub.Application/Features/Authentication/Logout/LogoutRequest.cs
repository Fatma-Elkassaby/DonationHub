using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Authentication.Logout
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
