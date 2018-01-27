using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using IdentityProvider.Models;
using IdentityProvider.Models.RolesAdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProvider.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesAdminController(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //
        // GET: /Roles/
        public IActionResult Index()
        {
            return View(_roleManager.Roles.ToList());
        }

        //
        // GET: /Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            // Get the list of Users in this Role
            var users = new List<ApplicationUser>();

            // Get the list of Users in this Role
            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();

            return View(role);
        }

        //
        // GET: /Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel roleViewModel)
        {
            if (!ModelState.IsValid) return View();

            var role = new ApplicationRole
            {
                Description = roleViewModel.Description,
                Name = roleViewModel.Name
            };

            // Save the new Description property:
            var createRoleResult = await _roleManager.CreateAsync(role);

            if (createRoleResult.Succeeded) return RedirectToAction("Index");

            ModelState.AddModelError("", createRoleResult.Errors.First().Description);

            return View();
        }

        //
        // GET: /Roles/Edit/Admin
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var roleModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };

            // Update the new Description property for the ViewModel:
            return View(roleModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind(new []{ "Name", "Id", "Description" })] RoleViewModel roleModel)
        {
            if (!ModelState.IsValid) return View();

            var role = await _roleManager.FindByIdAsync(roleModel.Id);
            role.Name = roleModel.Name;

            // Update the new Description property:
            role.Description = roleModel.Description;
            await _roleManager.UpdateAsync(role);

            return RedirectToAction("Index");
        }

        //
        // GET: /Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (!ModelState.IsValid) return View("~/Views/RolesAdmin/Delete.cshtml");

            var role = await _roleManager.FindByIdAsync(id);
            IdentityResult result;

            if (deleteUser != null)
            {
                result = await _roleManager.DeleteAsync(role);
            }
            else
            {
                result = await _roleManager.DeleteAsync(role);
            }

            if (result.Succeeded) return RedirectToAction("Index");

            ModelState.AddModelError("", result.Errors.First().Description);

            return View("~/Views/RolesAdmin/Delete.cshtml");
        }
    }
}