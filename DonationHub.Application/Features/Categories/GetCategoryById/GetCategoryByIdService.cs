using DonationHub.Application.Abstractions.Repositories;
using DonationHub.Application.Features.Categories.GetAllCategories;
using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.GetCategoryById
{
    public class GetCategoryByIdService
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryByIdService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<GetAllCategoriesResponse?> GetByIdAsync(GetCategoryByIdRequest request,CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category is null)
                return null;
            return new GetAllCategoriesResponse
            {
                Id = category.Id,
                NameAr = category.NameAr,
                NameEn = category.NameEn,
                DescriptionAr = category.DescriptionAr,
                DescriptionEn = category.DescriptionEn
            };
        }
    }
}
