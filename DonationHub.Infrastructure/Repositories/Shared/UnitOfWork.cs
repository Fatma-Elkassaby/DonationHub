using DonationHub.Application.Abstractions;
using DonationHub.Application.Abstractions.Repositories;
using DonationHub.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DonationHubDbContext _context;

        public UnitOfWork(DonationHubDbContext context)
        {
            _context = context;
        }

        public async Task<int> CompleteAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
