using BLOOM.DataAccess.Data;
using BLOOM.Models;
using BLOOM.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLOOM.DataAccess.DbInitilizer
{
    public class DbInitilizer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitilizer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if((await _db.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _db.Database.MigrateAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }


            if (!await _roleManager.RoleExistsAsync(SD.RoleCustomer))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleCustomer));
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleAdmin));
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleEmployee));
            }

            ApplicationUser user = await _userManager.FindByEmailAsync("admin@bloom.com");
            if(user == null)
            {
                var result = await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@bloom.com",
                    Email = "admin@bloom.com",
                    EmailConfirmed = true,
                    Name = "Bloom",
                    PhoneNumber = "1111111111",
                    StreetAddress = "abcd",
                    State = "abcd",
                    PostalCode = "1234",
                    City = "abcd",
                }, "Admin123");

                if (result.Succeeded)
                {
                    user = await _userManager.FindByEmailAsync("admin@bloom.com");
                    await _userManager.AddToRoleAsync(user, SD.RoleAdmin);
                }
            }

        }
    }
}
