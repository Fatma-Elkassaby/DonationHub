using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public string TokenHash { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public Guid UserId { get; private set; }
        public Guid? ReplacedByTokenId { get; private set; }
        private RefreshToken() { }
        public static RefreshToken Create(string tokenHash,DateTime createdAt,DateTime expiresAt,Guid userId) 
        {
            if(expiresAt <= createdAt)
                throw new ArgumentException("Expires date must be greater than created date!",nameof(expiresAt));
            if (string.IsNullOrWhiteSpace(tokenHash))
                throw new ArgumentException("TokenHash con not be epmty!",nameof(tokenHash));
            if (userId == Guid.Empty)
                throw new ArgumentException("User Id can not be empty!",nameof(userId));
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                TokenHash = tokenHash,
                CreatedAt = createdAt,
                ExpiresAt = expiresAt,
                UserId = userId
            };
        }
        public bool IsExpired(DateTime time) 
        {
        return time >= ExpiresAt;
        }
        public void Revoke(DateTime revokedAt) 
        {
            if (RevokedAt.HasValue)
                return;
            RevokedAt = revokedAt;
        }
        public void ReplaceWith(RefreshToken replacementToken, DateTime revokedAt) 
        {
            if (replacementToken is null)
                throw new ArgumentNullException(nameof(replacementToken));
            if (replacementToken.Id == Id)
                throw new InvalidOperationException("Refrsh token can not replace itself!");
            if (replacementToken.UserId != UserId)
                throw new InvalidOperationException("Refrsh token must belong to the same user!");
            if (RevokedAt.HasValue)
                throw new InvalidOperationException("Revoked refrsh token can not be replaced!");
            if (ReplacedByTokenId.HasValue)
                throw new InvalidOperationException("Refrsh token has already been replaced!");
            ReplacedByTokenId = replacementToken.Id;
            Revoke(revokedAt);
        }
    }
}
