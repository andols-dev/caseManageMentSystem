using caseManageMentSystem.Areas.Admin.Models.ViewModels;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace caseManageMentSystem.Areas.Admin.Controllers
{
    [Authorize]// add role based authorization
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

    }
}
