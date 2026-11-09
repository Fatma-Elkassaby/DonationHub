using DonationHub.Application.Abstractions.Services;
using DonationHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Application.Features.Authentication.CreateStaff
{
    public class CreateStaffService
    {
        private readonly IIdentityService _identityService;
        public CreateStaffService(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<(CreateStaffResponse? Response,string? ErrorCode,IReadOnlyList<string> Errors)> CreateStaffAsync(
                CreateStaffRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Role))
            {
                return (null, "Auth.InvalidStaffData",Array.Empty<string>());
            }
            UserRole role;
            switch (request.Role.Trim())
            {
                case "Admin":
                    role = UserRole.Admin;
                    break;
                case "CampaignManager":
                    role = UserRole.CampaignManager;
                    break;
                default:
                    return (null, "Auth.InvalidStaffRole",Array.Empty<string>());
            }
            if (await _identityService.EmailExistsAsync(request.Email))
            {
                return (null, "Auth.EmailAlreadyExists",Array.Empty<string>());
            }
            var temporaryPassword = GenerateTemporaryPassword();
            var result = await _identityService.CreateUserAsync(
                request.Email,
                request.FullName,
                temporaryPassword,
                "en");
            if (result.UserId is null)
            {
                if (result.Errors.Contains("DuplicateEmail"))
                {
                    return (null, "Auth.EmailAlreadyExists",
                        result.Errors);
                }
                return (null, "Auth.StaffCreationFailed",
                    result.Errors);
            }
            var roleAdded = await _identityService.AddToRoleAsync(result.UserId.Value, role);

            if (!roleAdded)
            {
                var userDeleted = await _identityService.DeleteUserAsync(result.UserId.Value);

                if (!userDeleted)
                {
                    return (null, "Auth.RoleAssignmentAndCleanupFailed", Array.Empty<string>());
                }

                return (null, "Auth.RoleAssignmentFailed", Array.Empty<string>());
            }
            var response = new CreateStaffResponse
            {
                UserId = result.UserId.Value,
                Email = request.Email,
                FullName = request.FullName,
                Role = role.ToString(),
                TemporaryPassword = temporaryPassword
            };
            return (response, null, Array.Empty<string>());
        }

        private static string GenerateTemporaryPassword()
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lower = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "123456789";

            const string allCharacters =
                "ABCDEFGHJKLMNPQRSTUVWXYZ" +
                "abcdefghijkmnopqrstuvwxyz" +
                "123456789";
            var password = new List<char>
            {
                upper[RandomNumberGenerator.GetInt32(upper.Length)],
                lower[RandomNumberGenerator.GetInt32(lower.Length)],
                digits[RandomNumberGenerator.GetInt32(digits.Length)]
            };
            for (int i = 0; i < 9; i++)
            {
                password.Add(allCharacters[RandomNumberGenerator.GetInt32(allCharacters.Length)]);
            }
            for (int i = password.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (password[i], password[j]) = (password[j], password[i]);
            }
            return new string(password.ToArray());
        }
    }
}
