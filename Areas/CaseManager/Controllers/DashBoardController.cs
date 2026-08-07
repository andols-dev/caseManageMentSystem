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
        public async Task<IActionResult> Index()
        {

            var loggedInUser = await _context.Users
                .Include(u => u.ManagedCases)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            return View(loggedInUser);
        }
    }
}
