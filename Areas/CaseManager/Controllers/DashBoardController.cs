using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace caseManageMentSystem.Areas.CaseManager.Controllers
{
    [Area("CaseManager")]
    [Authorize(Roles = "caseManager")]
    public class DashBoardController : Controller
    {
        // get logged in user save in var
        private readonly UserManager<ApplicationUser> _userManager;

        public DashBoardController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {

            var loggedInUser = await _userManager.GetUserAsync(User);

            return View(loggedInUser);
        }
    }
}
