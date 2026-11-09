using DonationHub.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Identity
{
    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedRolesAsync()
        {
            foreach (var role in Enum.GetNames<UserRole>())
            {
                bool roleExists = await _roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    var result = await _roleManager.CreateAsync(
                        new IdentityRole<Guid>(role));

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException($"Failed to create role: {role} !");
                    }
                }
            }
        }
    }
}
