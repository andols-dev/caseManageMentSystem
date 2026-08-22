using caseManageMentSystem.Areas.Client.Services;
using caseManageMentSystem.Areas.Client.ViewModels;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace caseManageMentSystem.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize(Roles ="client")]
    public class DashBoardController(UserManager<ApplicationUser> userManager, ICaseService caseService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            // get logged-in user
            var user = await userManager.GetUserAsync(User);
            // if there is no user then return notfound
            if (user == null)
            {
                return NotFound();
            }
            // get the fullname of the user
            var fullName = user.FullName;
            
            // get the cases that belongs to the logged-in user
            var cases = await caseService.GetCasesForClient(user.Id);

            var viewModel = new DashBoardViewModel(
                user.FullName,
                cases
            );

            return View(viewModel);
        }
    }
}
