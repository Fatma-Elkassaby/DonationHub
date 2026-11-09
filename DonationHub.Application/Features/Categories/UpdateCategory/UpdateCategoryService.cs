using DonationHub.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.UpdateCategory
{
    public class UpdateCategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;
        public UpdateCategoryService(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<string?> UpdateAsync(Guid id,UpdateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);

            if (category is null)
                return "Category.NotFound";
            if (request.NameAr is null && request.NameEn is null)
                return "Category.NoChanges";
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            if (request.NameAr is not null)
            {
                if (await _categoryRepository.ExistsByNameArAsync(request.NameAr.Trim(), id, cancellationToken))
                    return "Category.NameArAlreadyExists";
                category.ChangeNameAr(request.NameAr, now);
            }

            if (request.NameEn is not null)
            {
                if (await _categoryRepository.ExistsByNameEnAsync(request.NameEn.Trim(), id, cancellationToken))
                    return "Category.NameEnAlreadyExists";
                category.ChangeNameEn(request.NameEn, now);
            }
            _categoryRepository.Update(category);
            await _unitOfWork.CompleteAsync(cancellationToken);

            return null;
        }
    }
}
