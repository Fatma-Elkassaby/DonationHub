using DonationHub.Application.Abstractions.Repositories;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(DonationHubDbContext context)
            : base(context){}
        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _dbSet.FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash);
        }
    }
}
