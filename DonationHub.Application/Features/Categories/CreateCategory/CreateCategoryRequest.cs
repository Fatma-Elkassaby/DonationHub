using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.CreateCategory
{
    public class CreateCategoryRequest
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
    }
}
