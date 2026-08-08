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
        public async Task<IActionResult> Index(Status? caseStatus = null)
        {
            // visa cases med rätt casestatus

            var userId = _userManager.GetUserId(User);

            var loggedInUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            var cases = await _context.Cases
                    .Where(c => c.CaseManagerId == userId)
                    .Include(c => c.Client)
                    .ToListAsync();

            var viewModel = new CasesViewModel()
            {
                Cases = cases,
                FullName = loggedInUser.FullName,
                CaseStatus = caseStatus,
            };

            //var loggedInUser = await _context.Users
            //    .Include(u => u.ManagedCases)
            //    .ThenInclude(c => c.Client)
            //    .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            //return View(loggedInUser);

            return  View(viewModel);
        }
    }
}
