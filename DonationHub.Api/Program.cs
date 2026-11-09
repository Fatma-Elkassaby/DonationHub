using DonationHub.Application;
using DonationHub.Domain.Enums;
using DonationHub.Infrastructure;
using DonationHub.Infrastructure.Identity;
using DonationHub.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using DonationHub.Domain.Entities;
using Microsoft.OpenApi;
using DonationHub.Application.Abstractions.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration
            .GetSection("Jwt")
            .Get<JwtOptions>()!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
        };
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleSeeder = services.GetRequiredService<RoleSeeder>();

    await roleSeeder.SeedRolesAsync();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var seedAdminOptions = services
        .GetRequiredService<IOptions<SeedAdminOptions>>().Value;

    var timeProvider = services.GetRequiredService<TimeProvider>();
    var admin = await userManager.FindByEmailAsync(seedAdminOptions.Email);

    if (admin is null)
    {
        admin = new ApplicationUser
        {
            UserName = seedAdminOptions.Email,
            Email = seedAdminOptions.Email,
            FullName = seedAdminOptions.FullName,
            PreferredLanguage = "en",
            IsActive = true,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime
        };
        var createResult = await userManager.CreateAsync(admin, seedAdminOptions.Password);
        if (!createResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ", createResult.Errors.Select(e => e.Description)));
        }
    }
    if (!await userManager.IsInRoleAsync(admin, UserRole.Admin.ToString()))
    {
        var roleResult = await userManager.AddToRoleAsync(
            admin,
            UserRole.Admin.ToString());

        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join("; ", roleResult.Errors.Select(e => e.Description)));
        }
    }
    if (app.Environment.IsDevelopment())
    {
        async Task SeedDevelopmentUserAsync(string sectionName, UserRole role)
        {
            var userSection = builder.Configuration.GetSection($"DevelopmentSeedUsers:{sectionName}");
            var email = userSection["Email"];
            var password = userSection["Password"];
            var fullName = userSection["FullName"];
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fullName))
            {
                throw new InvalidOperationException( $"Development seed configuration for {sectionName} is missing.");
            }
            var user = await userManager.FindByEmailAsync(email);
            bool isNewUser = user is null;
            if (isNewUser)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    PreferredLanguage = "en",
                    IsActive = true,
                    CreatedAt = timeProvider.GetUtcNow().UtcDateTime
                };
                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", createResult.Errors.Select(e => e.Description)));
                }
            }
            if (!await userManager.IsInRoleAsync(user!, role.ToString()))
            {
                var roleResult = await userManager.AddToRoleAsync(user!, role.ToString());

                if (!roleResult.Succeeded)
                {
                    if (isNewUser)
                        await userManager.DeleteAsync(user!);

                    throw new InvalidOperationException(
                        string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                }
            }
        }

        await SeedDevelopmentUserAsync("CampaignManager", UserRole.CampaignManager);
        await SeedDevelopmentUserAsync("Donor", UserRole.Donor);
    }
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var categoryRepository = services.GetRequiredService<ICategoryRepository>();
    var unitOfWork = services.GetRequiredService<IUnitOfWork>();
    var timeProvider = services.GetRequiredService<TimeProvider>();

    var categories = new[]
    {
        ("التعليم", "Education"),
        ("الصحة", "Healthcare"),
        ("الغذاء", "Food"),
        ("السكن", "Housing"),
        ("البيئة", "Environment")
    };

    foreach (var (nameAr, nameEn) in categories)
    {
        var existsAr = await categoryRepository.ExistsByNameArAsync(nameAr);
        var existsEn = await categoryRepository.ExistsByNameEnAsync(nameEn);

        if (existsAr || existsEn)
            continue;

        var category = Category.Create(
            nameAr,
            nameEn,
            null,
            null,
            timeProvider.GetUtcNow().UtcDateTime);

        await categoryRepository.AddAsync(category);
    }

    await unitOfWork.CompleteAsync();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
