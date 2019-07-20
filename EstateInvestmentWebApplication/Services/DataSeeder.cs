using EstateInvestmentWebApplication.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace EstateInvestmentWebApplication.Services
{
    public class DataSeeder
    {
        private const string USER_NAME = "adminbatdongsan";
        private const string EMAIL = "adminbatdongsan@gmail.com";
        private const string PASSWORD = "Adminbatdongsan@123";

        public static async Task Initialize(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {

            bool isAdminRoleExist = await roleManager.RoleExistsAsync("Admin");
            IdentityResult roleResult;
            if (!isAdminRoleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            else
            {
                roleResult = IdentityResult.Success;
            }

            bool isUserExist = userManager.Users.Any(x => x.Email == EMAIL);

            if (!isUserExist)
            {
                var user = new IdentityUser { UserName = USER_NAME, Email = EMAIL };

                var userResult = await userManager.CreateAsync(user, PASSWORD);

                if (roleResult.Succeeded && userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            

        }
    }
}
