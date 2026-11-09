using DonationHub.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DonationHub.Application.Features.Authentication.Logout
{
    public class LogoutService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        public LogoutService(
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<string?> LogoutAsync(Guid userId,LogoutRequest request,CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return "Auth.RefreshTokenInvalid";
            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken)));
            var refreshToken = await _refreshTokenRepository
                .GetByTokenHashAsync(tokenHash);
            if (refreshToken is null || refreshToken.UserId != userId)
                return "Auth.RefreshTokenInvalid";
            refreshToken.Revoke(_timeProvider.GetUtcNow().UtcDateTime);
            _refreshTokenRepository.Update(refreshToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return null;
        }
    }
}
