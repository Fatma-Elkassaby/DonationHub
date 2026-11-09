using DonationHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Abstractions.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> ExistsByNameArAsync(string nameAr,Guid? excludeCategoryId = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameEnAsync(string nameEn,Guid? excludeCategoryId = null,
            CancellationToken cancellationToken = default);

        Task<bool> HasCampaignsAsync(Guid categoryId,CancellationToken cancellationToken = default);
    }
}
