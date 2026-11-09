using DonationHub.Application.Features.Authentication.CreateStaff;
using DonationHub.Application.Features.Authentication.GetCurrentUser;
using DonationHub.Application.Features.Authentication.Login;
using DonationHub.Application.Features.Authentication.Logout;
using DonationHub.Application.Features.Authentication.RefreshToken;
using DonationHub.Application.Features.Authentication.Register;
using DonationHub.Application.Features.Categories.CreateCategory;
using DonationHub.Application.Features.Categories.DeleteCategory;
using DonationHub.Application.Features.Categories.GetAllCategories;
using DonationHub.Application.Features.Categories.GetCategoryById;
using DonationHub.Application.Features.Categories.RemoveCategoryDescription;
using DonationHub.Application.Features.Categories.SetCategoryDescription;
using DonationHub.Application.Features.Categories.UpdateCategory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<RegisterService>();
            services.AddScoped<LoginService>();
            services.AddScoped<RefreshTokenService>();
            services.AddScoped<LogoutService>();
            services.AddScoped<GetCurrentUserService>();
            services.AddScoped<CreateStaffService>();

            services.AddScoped<GetAllCategoriesService>();
            services.AddScoped<GetCategoryByIdService>();
            services.AddScoped<CreateCategoryService>();
            services.AddScoped<UpdateCategoryService>();
            services.AddScoped<SetCategoryDescriptionService>();
            services.AddScoped<RemoveCategoryDescriptionService>();
            services.AddScoped<DeleteCategoryService>();
            return services;
        }
    }
}
