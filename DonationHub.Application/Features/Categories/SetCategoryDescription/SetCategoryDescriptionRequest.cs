using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.SetCategoryDescription
{
    public class SetCategoryDescriptionRequest
    {
        public string Language { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
