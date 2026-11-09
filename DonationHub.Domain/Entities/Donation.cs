using DonationHub.Domain.Enums;
using DonationHub.Domain.ValueObjects;

namespace DonationHub.Domain.Entities
{
    public class Donation
    {
        public Guid Id { get; private set; } 
        public Guid CampaignId { get; private set; }
        public Guid DonorUserId { get; private set; }
        public Money Amount { get; private set; } = null!;
        public DonationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public string? FailureReason { get; private set; }
        public string? ReceiptNumber { get; private set; }
        public string? ReceiptFileName { get; private set; }

        private Donation() { }
        public static Donation Create(Guid campaignId,Guid donorUserId,Money amount,DateTime createdAt)
        {
            if (campaignId == Guid.Empty)
                throw new ArgumentException("Campaign ID cannot be empty!",nameof(campaignId));
            if (donorUserId == Guid.Empty)
                throw new ArgumentException("Donor ID cannot be empty!",nameof(donorUserId));
            if (amount is null)
                throw new ArgumentNullException(nameof(amount));
            if (amount.Amount <= 0)
                throw new ArgumentException("Donation amount must be greater than zero!",nameof(amount));

            return new Donation
            {
                Id = Guid.NewGuid(),
                CampaignId = campaignId,
                DonorUserId = donorUserId,
                Amount = amount,
                Status = DonationStatus.Pending,
                CreatedAt = createdAt
            };
        }
        public void MarkAsSucceeded(DateTime completedAt)
        {
            if (Status != DonationStatus.Pending)
                throw new InvalidOperationException("Only pending donations can be marked as succeeded!");
            Status = DonationStatus.Succeeded;
            CompletedAt = completedAt;
        }
        public void MarkAsFailed(string failureReason)
        {
            if (Status != DonationStatus.Pending)
                throw new InvalidOperationException("Only pending donations can be marked as failed!");
            if (string.IsNullOrWhiteSpace(failureReason))
                throw new ArgumentException("Failure reason is required!",nameof(failureReason));
            Status = DonationStatus.Failed;
            FailureReason = failureReason.Trim();
        }
        public void SetReceipt(string receiptNumber, string receiptFileName)
        {
            if (Status != DonationStatus.Succeeded)
                throw new InvalidOperationException("Receipt can only be assigned to a succeeded donation!");
            if (ReceiptNumber is not null || ReceiptFileName is not null)
                throw new InvalidOperationException("Receipt has already been assigned!");
            if (string.IsNullOrWhiteSpace(receiptNumber))
                throw new ArgumentException("Receipt number is required!", nameof(receiptNumber));
            if (string.IsNullOrWhiteSpace(receiptFileName))
                throw new ArgumentException("Receipt file name is required!", nameof(receiptFileName));
            ReceiptNumber = receiptNumber.Trim();
            ReceiptFileName = receiptFileName.Trim();
        }
    }
}
