using DonationHub.Application.Abstractions.Services;
using DonationHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Authentication.Register
{
    public class RegisterService
    {
        private readonly IIdentityService _identityService;
        public RegisterService(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<(RegisterResponse? Response, string? ErrorCode,
            IReadOnlyList<string> Errors)> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return (null, "Auth.InvalidRegistration",new[] { "RequiredFieldsMissing" });
            }

            var preferredLanguage = string.IsNullOrWhiteSpace(request.PreferredLanguage)
                ? "en"
                : request.PreferredLanguage.Trim().ToLowerInvariant();

            if (preferredLanguage != "en" && preferredLanguage != "ar")
            {
                return (null, "Auth.InvalidRegistration", new[] { "InvalidPreferredLanguage" });
            }

            if (await _identityService.EmailExistsAsync(request.Email))
            {
                return (null, "Auth.EmailAlreadyExists",Array.Empty<string>());
            }

            var result = await _identityService.CreateUserAsync(
                request.Email,
                request.FullName,
                request.Password,
                preferredLanguage);

            if (result.UserId is null)
            {
                if (result.Errors.Contains("DuplicateEmail"))
                {
                    return (null, "Auth.EmailAlreadyExists", result.Errors);
                }

                return (null, "Auth.InvalidRegistration", result.Errors);
            }

            var roleAdded = await _identityService.AddToRoleAsync(
                result.UserId.Value,
                UserRole.Donor);

            if (!roleAdded)
            {
                return (null, "Auth.RoleAssignmentFailed",Array.Empty<string>());
            }

            var response = new RegisterResponse
            {
                UserId = result.UserId.Value,
                Email = request.Email,
                FullName = request.FullName,
                PreferredLanguage = preferredLanguage
            };
            return (response, null, Array.Empty<string>());
        }
    }
}
