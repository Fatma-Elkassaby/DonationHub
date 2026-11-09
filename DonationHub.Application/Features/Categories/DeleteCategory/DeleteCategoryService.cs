using DonationHub.Application.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Features.Categories.DeleteCategory
{
    public class DeleteCategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryService(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string?> DeleteAsync(Guid id,CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category is null)
                return "Category.NotFound";
            if (await _categoryRepository.HasCampaignsAsync(
                id, cancellationToken))
            {
                return "Category.HasCampaigns";
            }
            _categoryRepository.Delete(category);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return null;
        }
    }
}
