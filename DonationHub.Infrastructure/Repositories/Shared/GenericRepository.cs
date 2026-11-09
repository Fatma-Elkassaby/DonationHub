using DonationHub.Application.Abstractions;
using DonationHub.Application.Abstractions.Repositories;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
            where T : class
    {
        protected readonly DonationHubDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DonationHubDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(
                new object?[] { id },
                cancellationToken);
        }

        public async Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }
    }
}
