using DonationHub.Application.Abstractions;
using DonationHub.Application.Abstractions.Repositories;
using DonationHub.Application.Abstractions.Services;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Options;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Persistence.Interceptors;
using DonationHub.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace DonationHub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddDbContext<DonationHubDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).AddInterceptors(new SoftDeleteInterceptor(TimeProvider.System)));

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<DonationHubDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<RoleSeeder>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddOptions<JwtOptions>()
                .BindConfiguration("Jwt")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<SeedAdminOptions>()
                .BindConfiguration("SeedAdmin")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<DonationOptions>()
                .BindConfiguration("Donation")
                .ValidateDataAnnotations()
                .ValidateOnStart();
            return services;
        }
    }
}
