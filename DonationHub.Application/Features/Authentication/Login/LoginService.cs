using DonationHub.Application.Abstractions.Repositories;
using DonationHub.Application.Abstractions.Services;
using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Application.Features.Authentication.Login
{
    public class LoginService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _TokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public LoginService(
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

        public async Task<(LoginResponse? Response, string? ErrorCode)> LoginAsync(
            LoginRequest request,CancellationToken cancellationToken = default)
        {
            var userId = await _identityService.GetUserIdByEmailAsync(request.Email);
            if (userId is null)
                return (null, "Auth.InvalidCredentials");
            var isPasswordValid = await _identityService.CheckPasswordAsync(request.Email,request.Password);
            if (!isPasswordValid)
                return (null, "Auth.InvalidCredentials");
            var isActive = await _identityService.IsActiveAsync(userId.Value);
            if (!isActive)
                return (null, "Auth.AccountDeactivated");
            var roles = await _identityService.GetRolesAsync(userId.Value);
            var accessTokenResult = _TokenService.GenerateAccessToken(
                userId.Value,
                request.Email,
                roles);
            var refreshToken = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));
            var refreshTokenHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
            var now = _timeProvider.GetUtcNow();
            var refreshTokenExpiresAt = _TokenService.GetRefreshTokenExpiry(now);
            var refreshTokenEntity = DonationHub.Domain.Entities.RefreshToken.Create(
                refreshTokenHash,
                now.UtcDateTime,
                refreshTokenExpiresAt,
                userId.Value);
            await _refreshTokenRepository.AddAsync(
                refreshTokenEntity,cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            var response = new LoginResponse
            {
                AccessToken = accessTokenResult.AccessToken,
                AccessTokenExpiresAtUtc = accessTokenResult.ExpiresAtUtc,
                RefreshToken = refreshToken,
                RefreshTokenExpiresAtUtc = refreshTokenExpiresAt
            };
            return (response, null);
        }
    }
}
