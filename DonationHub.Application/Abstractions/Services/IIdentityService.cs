using System;
using System.Collections.Generic;
using System.Text;
using DonationHub.Domain.Enums;
namespace DonationHub.Application.Abstractions.Services
{
    public interface IIdentityService
    {
        Task<bool> EmailExistsAsync(string email);
        Task<(Guid? UserId, IReadOnlyList<string> Errors)> CreateUserAsync(string email,string fullName,string password,string preferredLanguage);
        Task<bool> CheckPasswordAsync(string email,string password);
        Task<Guid?> GetUserIdByEmailAsync(string email);
        Task<IReadOnlyList<string>> GetRolesAsync(Guid userId);
        Task<bool> IsActiveAsync(Guid userId);
        Task<bool> AddToRoleAsync(Guid userId,UserRole role);
        Task<(string Email, string FullName, string PreferredLanguage, bool IsActive)?>GetUserProfileAsync(Guid userId);
        Task<bool> DeleteUserAsync(Guid userId);
    }
}
