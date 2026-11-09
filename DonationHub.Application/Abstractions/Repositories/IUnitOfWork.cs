using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Abstractions.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
