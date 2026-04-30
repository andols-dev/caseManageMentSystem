using caseManageMentSystem.Areas.Admin.Models;
using caseManageMentSystem.Areas.Admin.Models.ViewModels;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace caseManageMentSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]// add role based authorization
    [Area("Admin")]
    public class UserController : Controller
    {


        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
        }
        // GET: UserController
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users;

            var userRolesViewModel = new List<UserVM>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserVM
                {
                    User = user,
                    Roles = roles
                });
            }
            return View(userRolesViewModel);
        }

        public async Task<IActionResult> Create(CreateUserVM createUser)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = createUser.FirstName,
                    LastName = createUser.LastName,
                    UserName = createUser.Email,
                    Email = createUser.Email,
                };

                var result = await _userManager.CreateAsync(user, createUser.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, createUser.Role);
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please correct the errors and try again.");
            }
            return View(createUser);
        }



        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index));

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            var userVM = new EditUserVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = await _userManager.GetRolesAsync(user).ContinueWith(t => t.Result.FirstOrDefault() ?? string.Empty)
            };
            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserVM appUser)
        {
            var user = await _userManager.FindByIdAsync(appUser.Id);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                user.FirstName = appUser.FirstName;
                user.LastName = appUser.LastName;
                user.Email = appUser.Email;
                user.UserName = appUser.Email;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(appUser);
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                if (!currentRoles.Contains(appUser.Role))
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRoleAsync(user, appUser.Role);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(appUser);
        }
    }
}
