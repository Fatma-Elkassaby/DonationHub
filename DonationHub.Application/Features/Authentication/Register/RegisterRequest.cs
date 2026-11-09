using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Authentication.Register
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PreferredLanguage { get; set; } = "en";
    }

}
