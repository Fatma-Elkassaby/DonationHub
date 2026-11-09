using DonationHub.Application.Abstractions.Services;
using DonationHub.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TimeProvider _timeProvider;

        public IdentityService(UserManager<ApplicationUser> userManager, TimeProvider timeProvider)
        {
            _userManager = userManager;
            _timeProvider = timeProvider;

        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) 
                return false;
            return true;
        }

        public async Task<(Guid? UserId, IReadOnlyList<string> Errors)> CreateUserAsync(string email,string fullName,string password,
            string preferredLanguage)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                PreferredLanguage = preferredLanguage,
                IsActive = true,
                CreatedAt = _timeProvider.GetUtcNow().UtcDateTime
            };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(error => error.Code).ToArray();
                return (null, errors);
            }

            return (user.Id, Array.Empty<string>());
        }

        public async Task<bool> CheckPasswordAsync(string email,string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Guid?> GetUserIdByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;
            return user.Id;
        }

        public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return Array.Empty<string>();
            return (await _userManager.GetRolesAsync(user)).ToArray();
        }

        public async Task<bool> IsActiveAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return false;
            if (!user.IsActive)
                return false;
            return true;
        }

        public async Task<bool> AddToRoleAsync(Guid userId,UserRole role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return false;
            var roleName = role.ToString();
            if (await _userManager.IsInRoleAsync(user, roleName))
                return true;
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<(string Email, string FullName, string PreferredLanguage, bool IsActive)?>
            GetUserProfileAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return null;
            return (
                user.Email ?? string.Empty,
                user.FullName,
                user.PreferredLanguage,
                user.IsActive
            );
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return false;
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }

}
