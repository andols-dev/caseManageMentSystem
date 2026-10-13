using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.Services;
using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            {
                search = search.Trim();
                cases = cases.Where(c => 
                    c.CaseNumber.Contains(search) || 
                    c.Title.Contains(search) ||
                    c.Client.FirstName.Contains(search) ||
                    c.Client.LastName.Contains(search) ||
                    (c.Client.FirstName + " " + c.Client.LastName).Contains(search));
            }
            if (caseStatus.HasValue)
                cases = cases.Where(c => c.Status == caseStatus.Value);

            // Get logged-in user full name
            var loggedInUser = await userService.GetUserFullName(userId);
            cases = cases.OrderBy(c => c.CaseNumber)
                .ThenBy(c => c.Id);
            var pagedResult = await pagedResultService.CreatePagedResult(cases, page, pageSize);
            
            var viewModel = new CasesViewModel
            {
                Cases = pagedResult.Items.ToList(),
                TotalCases = await loggedInUserCases.CountAsync(),
                ActiveCases = await loggedInUserCases.CountAsync(c => c.Status == Status.active),
                ClosedCases = await loggedInUserCases.CountAsync(c => c.Status == Status.closed),
                WaitingCases = await loggedInUserCases.CountAsync(c => c.Status == Status.waiting),
                DelayedCases = await loggedInUserCases.CountAsync(c => c.Status == Status.delayed),
                FullName = loggedInUser ?? string.Empty,
                CaseStatus = caseStatus,
                Search = search ?? string.Empty,
                PagedResult = pagedResult,
            };
            return  View(viewModel);
        }
    }
}
