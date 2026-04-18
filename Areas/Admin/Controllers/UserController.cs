using caseManageMentSystem.Areas.Admin.Models.ViewModels;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace caseManageMentSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        // GET: UserController
        public ActionResult Index()
        {
            //TODO: Get all users and their roles and pass to the view
            var users = _userManager.Users;
            // get roles for each user and pass to the view
            var userRolesViewModel = new List<UserVM>();
            foreach (var user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
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
