using DonationHub.Domain.Common;

namespace DonationHub.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string NameAr { get; private set; } = string.Empty;
        public string NameEn { get; private set; } = string.Empty;
        public string? DescriptionAr { get; private set; }
        public string? DescriptionEn { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private Category() { }
        public static Category Create(string nameAr,string nameEn,string? descriptionAr,string? descriptionEn,DateTime createdAt) 
        {
            nameAr = ValidateName(nameAr, "Arabic");
            nameEn = ValidateName(nameEn, "English");
            if (descriptionAr is not null)
                descriptionAr = ValidateDescription(descriptionAr,"Arabic");
            if (descriptionEn is not null)
                descriptionEn = ValidateDescription(descriptionEn, "English");
            Category category = new Category
            {
                Id = Guid.NewGuid(),
                NameAr = nameAr,
                NameEn = nameEn,
                DescriptionAr = descriptionAr,
                DescriptionEn = descriptionEn,
                CreatedAt = createdAt
            };
            return category;
        }
        private static string ValidateName(string name, string language) 
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"{language} category name is required! ", nameof(name));
            }
            name = name.Trim();
            if (name.Length > 100)
            {
                throw new ArgumentException($"{language} category name can not exceed 100 characters!", nameof(name));
            }
            return name;
        }
        private static string ValidateDescription(string description , string language)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException($"{language} description can not be empty! ", nameof(description));
            }
            description = description.Trim();       
            return description;
        }
        public void ChangeNameAr(string nameAr,DateTime updatedAt) 
        {
            string validatedNameAr = ValidateName(nameAr,"Arabic");
            if (NameAr == validatedNameAr)
                return;
            NameAr = validatedNameAr;
            UpdatedAt = updatedAt;
        }
        public void ChangeNameEn(string nameEn, DateTime updatedAt)
        {
            string validatedNameEn = ValidateName(nameEn, "English");
            if (NameEn == validatedNameEn)
                return;
            NameEn = validatedNameEn;
            UpdatedAt = updatedAt;
        }
        public void SetDescriptionAr(string descriptionAr,DateTime updatedAt)
        {
            string validatedDescriptionAr = ValidateDescription(descriptionAr,"Arabic");
            if (DescriptionAr == validatedDescriptionAr)
                return;
            DescriptionAr = validatedDescriptionAr;
            UpdatedAt = updatedAt;
        }
        public void SetDescriptionEn(string descriptionEn, DateTime updatedAt)
        {
            string validatedDescriptionEn = ValidateDescription(descriptionEn, "English");
            if (DescriptionEn == validatedDescriptionEn)
                return;
            DescriptionEn = validatedDescriptionEn;
            UpdatedAt = updatedAt;
        }
        public void RemoveDescriptionAr(DateTime updatedAt) 
        {
            if (DescriptionAr is null)
                return;
            DescriptionAr = null;
            UpdatedAt = updatedAt;
        }
        public void RemoveDescriptionEn(DateTime updatedAt)
        {
            if (DescriptionEn is null)
                return;
            DescriptionEn = null;
            UpdatedAt = updatedAt;
        }
    }
}
