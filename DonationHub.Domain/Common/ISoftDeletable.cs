namespace DonationHub.Domain.Common
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        public void Delete(DateTime deletedAt) 
        {
            IsDeleted = true;
            DeletedAt = deletedAt;
        }
        public void UndoDelete()
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}
