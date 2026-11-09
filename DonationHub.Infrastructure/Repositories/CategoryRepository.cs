using DonationHub.Application.Abstractions.Repositories;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DonationHubDbContext context) : base(context)
        {}
        public async Task<bool> ExistsByNameArAsync(string nameAr,Guid? excludeCategoryId = null,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(
                c => c.NameAr == nameAr &&(excludeCategoryId == null || c.Id != excludeCategoryId),
                cancellationToken);
        }

        public async Task<bool> ExistsByNameEnAsync(string nameEn,Guid? excludeCategoryId = null,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(
                c => c.NameEn == nameEn &&(excludeCategoryId == null || c.Id != excludeCategoryId),
                cancellationToken);
        }

        public async Task<bool> HasCampaignsAsync(Guid categoryId,CancellationToken cancellationToken = default)
        {
            return await _context.Campaigns.AnyAsync(campaign => campaign.CategoryId == categoryId,cancellationToken);
        }
    }
}
