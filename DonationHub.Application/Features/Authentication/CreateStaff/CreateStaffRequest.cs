using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Authentication.CreateStaff
{
    public class CreateStaffRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
