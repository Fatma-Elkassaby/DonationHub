using DonationHub.Application.Abstractions.Repositories;
using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.CreateCategory
{
    public class CreateCategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        public CreateCategoryService(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<string?> CreateAsync( CreateCategoryRequest request,CancellationToken cancellationToken = default)
        {
            if (await _categoryRepository.ExistsByNameArAsync(
                request.NameAr.Trim(),
                cancellationToken: cancellationToken))
            {
                return "Category.NameArAlreadyExists";
            }

            if (await _categoryRepository.ExistsByNameEnAsync(
                request.NameEn.Trim(),
                cancellationToken: cancellationToken))
            {
                return "Category.NameEnAlreadyExists";
            }

            var category = Category.Create(
                request.NameAr,
                request.NameEn,
                request.DescriptionAr,
                request.DescriptionEn,
                _timeProvider.GetUtcNow().UtcDateTime);
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return null;
        }
    }
}
