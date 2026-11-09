using DonationHub.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.GetAllCategories
{
    public class GetAllCategoriesService
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetAllCategoriesService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IReadOnlyList<GetAllCategoriesResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            return categories.Select(category => new GetAllCategoriesResponse
            {
                Id = category.Id,
                NameAr = category.NameAr,
                NameEn = category.NameEn,
                DescriptionAr = category.DescriptionAr,
                DescriptionEn = category.DescriptionEn
            }).ToList();
        }
    }
}
