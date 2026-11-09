using DonationHub.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.SetCategoryDescription
{
    public class SetCategoryDescriptionService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public SetCategoryDescriptionService(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<string?> SetAsync(Guid id,SetCategoryDescriptionRequest request,
            CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category is null)
                return "Category.NotFound";

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            switch (request.Language.Trim().ToLowerInvariant())
            {
                case "ar":
                    category.SetDescriptionAr(request.Description, now);
                    break;
                case "en":
                    category.SetDescriptionEn(request.Description, now);
                    break;
                default:
                    return "Category.InvalidLanguage";
            }
            _categoryRepository.Update(category);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return null;
        }
    }
}
