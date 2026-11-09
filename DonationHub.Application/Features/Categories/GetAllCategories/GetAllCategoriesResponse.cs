using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.GetAllCategories
{
    public class GetAllCategoriesResponse
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
    }
}
