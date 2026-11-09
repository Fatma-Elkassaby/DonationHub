using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Authentication.CreateStaff
{
    public class CreateStaffResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string TemporaryPassword { get; set; } = string.Empty;
    }
}
