using DonationHub.Domain.ValueObjects;
using DonationHub.Domain.Enums;
using DonationHub.Domain.Common;

namespace DonationHub.Domain.Entities
{
    public class Campaign : ISoftDeletable
    {
        public Guid Id { get; private set; }

        public string TitleAr { get; private set; } = string.Empty;
        public string TitleEn { get; private set; } = string.Empty;
        public string DescriptionAr { get; private set; } = string.Empty;
        public string DescriptionEn { get; private set; } = string.Empty;
        public Money GoalAmount { get; private set; } = null!;
        public Money RaisedAmount { get; private set; } = null!;
        public DateTime EndDate { get; private set; }
        public Guid CategoryId { get; private set; }
        public string? BannerFileName { get; private set; }
        public CampaignStatus Status { get; private set; }
        public Guid CreatedByUserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public byte[] RowVersion { get; private set; } = [];
        private Campaign() { }
        public static Campaign Create(string titleAr,string titleEn,string descriptionAr,string descriptionEn,Money 
            goalAmount,DateTime endDate,Guid categoryId,string? bannerFileName,
            Guid createdByUserId,DateTime createdAt
            ) 
        {
            if (string.IsNullOrWhiteSpace(titleAr))
                throw new ArgumentException("Arabic title is required!", nameof(titleAr));
            if (string.IsNullOrWhiteSpace(titleEn))
                throw new ArgumentException("English title is required!", nameof(titleEn));
            if (string.IsNullOrWhiteSpace(descriptionAr))
                throw new ArgumentException("Arabic description is required!", nameof(descriptionAr));
            if (string.IsNullOrWhiteSpace(descriptionEn))
                throw new ArgumentException("English description is required!", nameof(descriptionEn));
            if (goalAmount is null)
                throw new ArgumentNullException(nameof(goalAmount));
            if (goalAmount.Amount <= 0)
                throw new ArgumentException("Goal amount must be greater than zero!",nameof(goalAmount));
            if (endDate <= createdAt)
                throw new ArgumentException("Campaign end date must be in the future.",nameof(endDate));
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category ID cannot be empty!",nameof(categoryId));
            if (createdByUserId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty!",nameof(createdByUserId));
            if (!string.IsNullOrWhiteSpace(bannerFileName))
                bannerFileName = bannerFileName.Trim();
            else
                bannerFileName = null;
            return new Campaign
            {
                Id = Guid.NewGuid(),
                TitleAr = titleAr.Trim(),
                TitleEn = titleEn.Trim(),
                DescriptionAr = descriptionAr.Trim(),
                DescriptionEn = descriptionEn.Trim(),
                GoalAmount = goalAmount,
                RaisedAmount = new Money(0, goalAmount.Currency),
                EndDate = endDate,
                CategoryId = categoryId,
                BannerFileName = bannerFileName,
                Status = CampaignStatus.Active,
                CreatedByUserId = createdByUserId,
                CreatedAt = createdAt
            };
        }
        private void EnsureCampaignIsActive()
        {
            if (Status != CampaignStatus.Active)
                throw new InvalidOperationException("Only active campaigns can perform this operation!");
        }
        public void SetBannerFileName(string bannerFileName, DateTime updatedAt)
        {
            if (string.IsNullOrWhiteSpace(bannerFileName))
                throw new ArgumentException("Banner file name is required!",nameof(bannerFileName));
            bannerFileName = bannerFileName.Trim();
            if (BannerFileName == bannerFileName)
                return;
            BannerFileName = bannerFileName;
            UpdatedAt = updatedAt;
        }

        public void RemoveBannerFileName(DateTime updatedAt)
        {
            if (BannerFileName is null)
                return;
            BannerFileName = null;
            UpdatedAt = updatedAt;
        }
        public void ChangeTitleAr(string titleAr, DateTime updatedAt)
        {
            if (string.IsNullOrWhiteSpace(titleAr))
                throw new ArgumentException("Arabic title is required!", nameof(titleAr));
            titleAr = titleAr.Trim();
            if (TitleAr == titleAr)
                return;
            TitleAr = titleAr;
            UpdatedAt = updatedAt;
        }
        public void ChangeTitleEn(string titleEn, DateTime updatedAt)
        {
            if (string.IsNullOrWhiteSpace(titleEn))
                throw new ArgumentException("English title is required!", nameof(titleEn));
            titleEn = titleEn.Trim();
            if (TitleEn == titleEn)
                return;
            TitleEn = titleEn;
            UpdatedAt = updatedAt;
        }
        public void ChangeDescriptionAr(string descriptionAr, DateTime updatedAt)
        {
            if (string.IsNullOrWhiteSpace(descriptionAr))
                throw new ArgumentException("Arabic description is required!", nameof(descriptionAr));
            descriptionAr = descriptionAr.Trim();
            if (DescriptionAr == descriptionAr)
                return;
            DescriptionAr = descriptionAr;
            UpdatedAt = updatedAt;
        }
        public void ChangeDescriptionEn(string descriptionEn, DateTime updatedAt)
        {
            if (string.IsNullOrWhiteSpace(descriptionEn))
                throw new ArgumentException("English description is required!", nameof(descriptionEn));
            descriptionEn = descriptionEn.Trim();
            if (DescriptionEn == descriptionEn)
                return;
            DescriptionEn = descriptionEn;
            UpdatedAt = updatedAt;
        }
        public void ChangeEndDate(DateTime endDate, DateTime updatedAt)
        {
            EnsureCampaignIsActive();
            if (endDate <= updatedAt)
                throw new ArgumentException("Campaign end date must be in the future!",nameof(endDate));
            if (EndDate == endDate)
                return;
            EndDate = endDate;
            UpdatedAt = updatedAt;
        }
        public void ChangeGoalAmount(Money goalAmount, DateTime updatedAt)
        {
            EnsureCampaignIsActive();
            if (goalAmount is null)
                throw new ArgumentNullException(nameof(goalAmount));
            if (goalAmount.Amount <= 0)
                throw new ArgumentException("Goal amount must be greater than zero!",nameof(goalAmount));
            if (goalAmount.Currency != GoalAmount.Currency)
                throw new ArgumentException("Goal amount currency must match the current currency!",nameof(goalAmount));

            if (goalAmount.Amount < RaisedAmount.Amount)
                throw new InvalidOperationException("Goal amount cannot be less than the raised amount!");
            if (GoalAmount.Equals(goalAmount))
                return;
            GoalAmount = goalAmount;
            UpdatedAt = updatedAt;
        }
        public void MarkAsExpired(DateTime updatedAt)
        {
            if (Status == CampaignStatus.Expired)
                return;
            if (Status == CampaignStatus.Completed)
                throw new InvalidOperationException("Completed campaign cannot be expired!");
            if (EndDate > updatedAt)
                throw new InvalidOperationException("Campaign cannot expire before its end date!");
            Status = CampaignStatus.Expired;
            UpdatedAt = updatedAt;
        }
        private void MarkAsCompleted(DateTime updatedAt
            )
        {
            Status = CampaignStatus.Completed;
            UpdatedAt = updatedAt;
        }
        public void AddSuccessfulDonation(Donation donation, DateTime updatedAt) 
        {
            if (donation is null)
                throw new ArgumentNullException(nameof(donation));
            if (donation.Status != DonationStatus.Succeeded)
                throw new InvalidOperationException("Only succeeded donations can be added to the campaign!");
            if (donation.CampaignId != Id)
                throw new InvalidOperationException("Donation does not belong to this campaign!");
            if (donation.Amount.Currency != RaisedAmount.Currency)
                throw new ArgumentException("Donation currency must match campaign currency!",nameof(donation));
            if (Status != CampaignStatus.Active)
                throw new InvalidOperationException("Only active campaigns can receive donations!");
            if (EndDate <= updatedAt)
                throw new InvalidOperationException("Campaign has already ended!");
            Money newRaisedAmount = new(
                RaisedAmount.Amount + donation.Amount.Amount,
                RaisedAmount.Currency);
            RaisedAmount = newRaisedAmount;
            if (RaisedAmount.Amount >= GoalAmount.Amount)
                MarkAsCompleted(updatedAt);
        }
    }
}
