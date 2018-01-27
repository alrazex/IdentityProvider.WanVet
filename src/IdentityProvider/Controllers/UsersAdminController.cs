using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityProvider.Models;
using IdentityProvider.Models.AccountViewModels;
using IdentityProvider.Models.UsersAdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersAdminController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //
        // GET: /Users/
        public async Task<IActionResult> Index()
        {
            return View(await _userManager.Users.ToListAsync());
        }

        //
        // GET: /Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            ViewBag.RoleNames = await _userManager.GetRolesAsync(user);

            return View(user);
        }

        //
        // GET: /Users/Create
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");

            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel registerViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = registerViewModel.Email,
                    Email = registerViewModel.Email,
                    Gender = registerViewModel.Gender,
                    Firstname = registerViewModel.Firstname,
                    IsActive = true,
                    Lastname = registerViewModel.Lastname,
                    PhoneNumber = registerViewModel.PhoneNumber,
                    PhoneNumberConfirmed = true,                    
                };

                var createUserResult = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (createUserResult.Succeeded)
                {
                    if (selectedRoles == null) return RedirectToAction("Index");

                    var result = await _userManager.AddToRolesAsync(user, selectedRoles);

                    if (result.Succeeded) return RedirectToAction("Index");

                    ModelState.AddModelError("", result.Errors.First().Description);
                    ViewBag.RoleId = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", createUserResult.Errors.First().Description);
                    ViewBag.RoleId = new SelectList(_roleManager.Roles, "Name", "Name");
                    return View();

                }
            }

            ViewBag.RoleId = new SelectList(_roleManager.Roles, "Name", "Name");

            return View();
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var userRoles = await _userManager.GetRolesAsync(user);

            return View(new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = _roleManager.Roles.ToList().Select(x => new SelectListItem
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(new []{ "Email", "Id" } )] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(editUser.Id);

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await _userManager.GetRolesAsync(user);

                selectedRole = selectedRole ?? new string[] { };

                var result = await _userManager.AddToRolesAsync(user, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().Description);
                    return View();
                }
                result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRole).ToArray<string>());

                if (result.Succeeded) return RedirectToAction("Index");

                ModelState.AddModelError("", result.Errors.First().Description);
                return View();
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);      
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (!ModelState.IsValid) return View("~/Views/UsersAdmin/Delete.cshtml");

            var user = await _userManager.FindByIdAsync(id);

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded) return RedirectToAction("Index");

            ModelState.AddModelError("", result.Errors.First().Description);
            return View("~/Views/UsersAdmin/Delete.cshtml");
        }
    }
}
