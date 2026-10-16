using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.Services;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace caseManageMentSystem.Areas.CaseManager.Controllers
{
    [Area("CaseManager")]
    [Authorize(Roles = "caseManager")]
    public class DashBoardController(UserManager<ApplicationUser> userManager, ICaseServiceCaseManagerDashboard caseService, IUserServiceCaseManagerDaschboard userService, IPagedResultServiceCaseManagerDaschboard pagedResultService ) : Controller
    {
        public async Task<IActionResult> Index(Status? caseStatus, string? search, int page = 1)
        {
            var pageSize = 4;
            var userId = userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized();
            
            // Get all cases that belongs to logged-in user: Iqueryable
            var loggedInUserCases = caseService.GetAllCases(userId);

            var cases = loggedInUserCases ?? throw new ArgumentNullException(nameof(loggedInUserCases));

            if (!string.IsNullOrEmpty(search))
                cases = caseService.GetCasesBySearchTerm(cases, search);
                
            if (caseStatus.HasValue)
                cases = cases.Where(c => c.Status == caseStatus.Value);

            // Get logged-in user full name
            var loggedInUser = await userService.GetUserFullName(userId);
            cases = cases.OrderBy(c => c.CaseNumber)
                .ThenBy(c => c.Id);
            var pagedResult = await pagedResultService.CreatePagedResult(cases, page, pageSize);
            var viewModel = await caseService.CreateCaseViewModel(pagedResult, loggedInUserCases, caseStatus, search, loggedInUser);
            return View(viewModel);
        }
    }
}
