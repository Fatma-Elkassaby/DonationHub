using DonationHub.Application.Abstractions.Repositories;
using DonationHub.Application.Abstractions.Services;
using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Application.Features.Authentication.RefreshToken
{
    public class RefreshTokenService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _TokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public RefreshTokenService(
            IIdentityService identityService,
            ITokenService TokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _identityService = identityService;
            _TokenService = TokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<(RefreshTokenResponse? Response, string? ErrorCode)>
            RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return (null, "Auth.RefreshTokenInvalid");
            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));
            var oldToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
            if (oldToken is null)
                return (null, "Auth.RefreshTokenInvalid");
            var now = _timeProvider.GetUtcNow();
            if (oldToken.IsExpired(now.UtcDateTime) ||
                oldToken.RevokedAt.HasValue ||
                oldToken.ReplacedByTokenId.HasValue)
            {
                return (null, "Auth.RefreshTokenInvalid");
            }
            var isActive = await _identityService.IsActiveAsync(oldToken.UserId);
            if (!isActive)
                return (null, "Auth.AccountDeactivated");
            var profile = await _identityService.GetUserProfileAsync(oldToken.UserId);
            if (profile is null)
                return (null, "Auth.RefreshTokenInvalid");
            var roles = await _identityService.GetRolesAsync(oldToken.UserId);
            var accessTokenResult = _TokenService.GenerateAccessToken(
                oldToken.UserId,
                profile.Value.Email,
                roles);
            var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var newRefreshTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(newRefreshToken)));
            var refreshTokenExpiresAt = _TokenService.GetRefreshTokenExpiry(now);
            var newTokenEntity = DonationHub.Domain.Entities.RefreshToken.Create(
                newRefreshTokenHash,
                now.UtcDateTime,
                refreshTokenExpiresAt,
                oldToken.UserId);
            oldToken.ReplaceWith(newTokenEntity, now.UtcDateTime);
            await _refreshTokenRepository.AddAsync(
                newTokenEntity,
                cancellationToken);
            _refreshTokenRepository.Update(oldToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            var response = new RefreshTokenResponse
            {
                AccessToken = accessTokenResult.AccessToken,
                AccessTokenExpiresAtUtc = accessTokenResult.ExpiresAtUtc,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiresAtUtc = refreshTokenExpiresAt
            };
            return (response, null);
        }
     }
    }
