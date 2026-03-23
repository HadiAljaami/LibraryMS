using LibraryMS.Infrastructure.Identity;
using LibraryMS.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace LibraryMS.Web.Extensions;

public static class WebExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db          = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        await DbSeeder.SeedAsync(db, userManager, roleManager);
    }
}