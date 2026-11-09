using DonationHub.Application.Abstractions.Services;
using DonationHub.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DonationHub.Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly TimeProvider _timeProvider;

        public TokenService(IOptions<JwtOptions> jwtOptions,TimeProvider timeProvider)
        {
            _jwtOptions = jwtOptions.Value;
            _timeProvider = timeProvider;
        }
        public (string AccessToken, DateTime ExpiresAtUtc) GenerateAccessToken(
            Guid userId,string email,IReadOnlyList<string> roles)
        {
            var now = _timeProvider.GetUtcNow();
            var expiresAt = now.AddMinutes(_jwtOptions.ExpiryMinutes);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return (accessToken, expiresAt.UtcDateTime);
        }

        public DateTime GetRefreshTokenExpiry(DateTimeOffset createdAt)
        {
            return createdAt
                .AddDays(_jwtOptions.RefreshTokenExpiryDays)
                .UtcDateTime;
        }
    }
}
