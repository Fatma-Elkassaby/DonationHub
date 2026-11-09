using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class PaymentAttempt
    {
        public Guid Id { get; private set; }
        public Guid DonationId { get; private set; }
        public string StripeCheckoutSessionId { get; private set; } = string.Empty;
        public string? StripePaymentIntentId { get; private set; }
        public Money Amount { get; private set; } = null!;
        public PaymentAttemptStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? FailureCode { get; private set; }
        private PaymentAttempt() { }
        public static PaymentAttempt Create(Guid donationId,string stripeCheckoutSessionId,Money amount,DateTime createdAt) 
        {
            if (donationId == Guid.Empty)
                throw new ArgumentException("Donation ID is required!", nameof(donationId));
            if (string.IsNullOrWhiteSpace(stripeCheckoutSessionId))
                throw new ArgumentException("Stripe Checkout Session ID is required!", nameof(stripeCheckoutSessionId));
            if (amount is null)
                throw new ArgumentNullException(nameof(amount));
            if (amount.Amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));
            return new PaymentAttempt 
            {

                Id = Guid.NewGuid(),
                DonationId = donationId,
                StripeCheckoutSessionId = stripeCheckoutSessionId.Trim(),
                Amount = amount,
                Status = PaymentAttemptStatus.Created,
                CreatedAt = createdAt
            };
        }
        public void MarkAsSucceeded(string? stripePaymentIntentId, DateTime updatedAt)
        {
            if (Status != PaymentAttemptStatus.Created)
                throw new InvalidOperationException("Only created payment attempts can be succeeded!");
            Status = PaymentAttemptStatus.Succeeded;
            StripePaymentIntentId = stripePaymentIntentId;
            UpdatedAt = updatedAt;
        }
        public void MarkAsFailed(string? failureCode, DateTime updatedAt)
        {
            if (Status != PaymentAttemptStatus.Created)
                throw new InvalidOperationException("Only created payment attempts can be failed!");
            Status = PaymentAttemptStatus.Failed;
            FailureCode = failureCode;
            UpdatedAt = updatedAt;
        }
        public void MarkAsExpired(DateTime updatedAt)
        {
            if (Status != PaymentAttemptStatus.Created)
                throw new InvalidOperationException("Only created payment attempts can be expired!");
            Status = PaymentAttemptStatus.Expired;
            UpdatedAt = updatedAt;
        }
    }
}
