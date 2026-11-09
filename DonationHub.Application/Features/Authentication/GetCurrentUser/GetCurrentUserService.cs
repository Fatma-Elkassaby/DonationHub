using DonationHub.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Authentication.GetCurrentUser
{
    public class GetCurrentUserService
    {
        private readonly IIdentityService _identityService;
        public GetCurrentUserService(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<GetCurrentUserResponse?> GetCurrentUserAsync(Guid userId)
        {
            var profile = await _identityService.GetUserProfileAsync(userId);
            if (profile is null)
                return null;
            return new GetCurrentUserResponse
            {
                UserId = userId,
                Email = profile.Value.Email,
                FullName = profile.Value.FullName,
                PreferredLanguage = profile.Value.PreferredLanguage,
                IsActive = profile.Value.IsActive
            };
        }
    }
}
