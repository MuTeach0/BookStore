﻿using BookShoppingCartMvcUI.Constants;
using Microsoft.AspNetCore.Identity;

namespace BookShoppingCartMvcUI.Data;

public class DbSeeder
{
    public static async Task SeedDefaultData(IServiceProvider service)
    {
        var userMgr = service.GetService<UserManager<IdentityUser>>();
        var roleMgr = service.GetService<RoleManager<IdentityRole>>();

        // Adding some roles to db
        await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
        await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

        // Create Admin User
        var admin = new IdentityUser
        {
            UserName = "admin@gmail.com",
            Email = "admin@gmail.com",
            EmailConfirmed = true
        };

        var userInDb = await userMgr.FindByEmailAsync(admin.Email);

        if (userInDb is null)
        {
            await userMgr.CreateAsync(admin, "Admin@123");
            await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
        }
    }
}