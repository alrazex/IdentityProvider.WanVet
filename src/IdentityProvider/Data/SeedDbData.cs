using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityProvider.Enums;
using IdentityProvider.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityProvider.Data
{
    public class SeedDbData
    {
        readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public SeedDbData(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task EnsureSeedDataAsync()
        {
            await CreateRoles(); // Add roles
            await CreateUsers(); // Add users
        }

        private async Task CreateRoles()
        {
            var rolesToAdd = new List<ApplicationRole>
            {
                new ApplicationRole {Name = "Admin", Description = "Admin - Identity Provider"},
                new ApplicationRole {Name = "wanvet.admin", Description = "This role will have full rights to the website. (WanVet)"},
                new ApplicationRole {Name = "wanvet.user", Description = "This role will have limited rights to the website. (WanVet)"},
                new ApplicationRole {Name = "wanvet.staff", Description = "This role will have limited rights to the website. Appointment & inventory management. (WanVet)"},
                new ApplicationRole {Name = "wanvet.doctor", Description = "This role will have limited rights to the website. It doesn't have the possibility to adjust user roles. (WanVet)"}
            };

            foreach (var role in rolesToAdd)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    await _roleManager.CreateAsync(role);
                }
            }
        }
        private async Task CreateUsers()
        {
            var users = new List<ApplicationUser>()
            {
                new ApplicationUser
                {
                    UserName = "admin@admin.com",  Email = "admin@admin.com", EmailConfirmed = true, Firstname = "Razvan", Lastname = "Dumitru",
                    PhoneNumber = "+40754621692", PhoneNumberConfirmed = true, IsActive = true, Gender = (int)Gender.Male
                },
            };

            if (await _userManager.FindByEmailAsync(users[0].Email) == null)
            {
                await _userManager.CreateAsync(users[0], "P@ssw0rd!");
                var currentUser = await _userManager.FindByEmailAsync(users[0].Email);
                var addAdminRoleResult = await _userManager.AddToRoleAsync(currentUser, "Admin");
                var addWVAdminResult = await _userManager.AddToRoleAsync(currentUser, "wanvet.admin");
            }

        }

    }
}
