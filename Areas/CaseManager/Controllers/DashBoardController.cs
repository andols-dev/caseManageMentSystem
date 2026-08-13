using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caseManageMentSystem.Areas.CaseManager.Controllers
{
    [Area("CaseManager")]
    [Authorize(Roles = "caseManager")]
    public class DashBoardController : Controller
    {
        // get logged in user save in var
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DashBoardController(UserManager<ApplicationUser> userManager, ApplicationDbContext context )
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index(Status? caseStatus)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            var cases = _context.Cases
                .Where(c => c.CaseManagerId == userId);

            if (caseStatus.HasValue)
            {
                cases = cases.Where(c => c.Status == caseStatus.Value);
            }

            var loggedInUser = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync();

            var viewModel = new CasesViewModel
            {
                Cases = await cases
                    .Include(c => c.Client)
                    .ToListAsync(),

                FullName = loggedInUser ?? string.Empty,
                CaseStatus = caseStatus
            };


            return  View(viewModel);
        }
    }
}
