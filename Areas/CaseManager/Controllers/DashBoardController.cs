using caseManageMentSystem.Areas.Admin.Models.ViewModels;
using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.ViewModels;
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
        public async Task<IActionResult> Index(Status? caseStatus, string? search, int page = 1)
        {
            int pageSize = 4;
  
          
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }
            
            var allCases = _context.Cases
                .Where(c => c.CaseManagerId == userId);

            var cases = allCases;

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
            {
                cases = cases.Where(c => c.Status == caseStatus.Value);
            }

            var loggedInUser = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync();


            var pagedResult = new PagedResult<Case>
            {
                Items = [.. cases.Skip((page - 1) * pageSize).Take(pageSize)],
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = await cases.CountAsync()
            };
            var viewModel = new CasesViewModel
            {
                Cases = await cases
                    .Include(c => c.Client)
                    .ToListAsync(),

                TotalCases = await allCases.CountAsync(),
                ActiveCases = await allCases.CountAsync(c => c.Status == Status.active),
                ClosedCases = await allCases.CountAsync(c => c.Status == Status.closed),
                WaitingCases = await allCases.CountAsync(c => c.Status == Status.waiting),
                DelayedCases = await allCases.CountAsync(c => c.Status == Status.delayed),

                FullName = loggedInUser ?? string.Empty,
                CaseStatus = caseStatus,
                Search = search ?? string.Empty,
                PagedResult = pagedResult,
               
            };
            return  View(viewModel);
        }
    }
}
