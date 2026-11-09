using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Authentication.GetCurrentUser
{
    public class GetCurrentUserResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PreferredLanguage { get; set; } = "en";
        public bool IsActive { get; set; }
    }
}
