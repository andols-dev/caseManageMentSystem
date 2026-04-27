using caseManageMentSystem.Areas.Admin.Models;
using caseManageMentSystem.Areas.Admin.Models.ViewModels;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace caseManageMentSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]// add role based authorization
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
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
    }
}
