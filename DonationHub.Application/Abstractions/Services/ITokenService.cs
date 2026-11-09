using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Abstractions.Services
{
    public interface ITokenService
    {
        (string AccessToken, DateTime ExpiresAtUtc) GenerateAccessToken(Guid userId, string email,IReadOnlyList<string> roles);
        DateTime GetRefreshTokenExpiry(DateTimeOffset createdAt);
    }
}
