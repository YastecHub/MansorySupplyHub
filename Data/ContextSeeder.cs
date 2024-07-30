using MansorySupplyHub.Entities;
using MansorySupplyHub.Enum;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace MansorySupplyHub.Data
{
    public class ContextSeeder
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Role.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.User.ToString()));
        }
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                Id = new Guid().ToString(),
                UserName = "Admin",
                FullName = "Yas Tech",
                Email = "admin@gmail.com",
                PhoneNumber = "09168913009",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            //  if (userManager.Members.All(u => u.Id != defaultUser.Id))
            //  {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Admin@123");
                await userManager.AddToRoleAsync(defaultUser, Role.Admin.ToString());
                await userManager.AddToRoleAsync(defaultUser, Role.User.ToString());
            }
            //}
        }
    }
}
