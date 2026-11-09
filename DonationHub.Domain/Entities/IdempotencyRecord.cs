using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class IdempotencyRecord
    {
        public string Key { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public string RequestHash { get; private set; } = string.Empty;
        public int ResponseStatusCode { get; private set; }
        public string ResponseBody { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        private IdempotencyRecord() { }
        public static IdempotencyRecord Create(string key,Guid userId,string requestHash,int responseStatusCode,
            string responseBody,DateTime createdAt,DateTime expiresAt) 
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Idempotency Key is required!", nameof(key));
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID is required!", nameof(userId));
            if (string.IsNullOrWhiteSpace(requestHash))
                throw new ArgumentException("Request Hash is required!", nameof(requestHash));
            if (expiresAt <= createdAt)
                throw new ArgumentException("Expiration date must be after creation date!", nameof(expiresAt));
            return new IdempotencyRecord
            {
                Key = key.Trim(),
                UserId = userId,
                RequestHash = requestHash.Trim(),
                ResponseStatusCode = responseStatusCode,
                ResponseBody = responseBody,
                CreatedAt = createdAt,
                ExpiresAt = expiresAt
            };
        }
    }
}
