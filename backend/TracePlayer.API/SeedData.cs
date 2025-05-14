using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TracePlayer.BL.Helpers;
using TracePlayer.DB.Models;

namespace TracePlayer.API
{
    public static class SeedData
    {
        public static string[] roleNames = { "Admin" };

        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
        }

        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider, string adminSteamId64)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var adminUser = await userManager.Users.FirstOrDefaultAsync(u => u.SteamId64 == adminSteamId64);
            var adminSteamId = SteamIdConverter.ConvertSteamId64ToSteamId(adminSteamId64);

            if (adminUser is null)
            {
                var user = new User
                {
                    UserName = $"steam_{adminSteamId64}",
                    SteamId64 = adminSteamId64,
                    SteamId = adminSteamId,
                    Email = $"{adminSteamId64}@steam.local"
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(user, roleNames);
                }
            }
        }
    }
}
