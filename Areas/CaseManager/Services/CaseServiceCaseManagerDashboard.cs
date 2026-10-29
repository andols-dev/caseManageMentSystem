using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public class CaseServiceCaseManagerDashboard(ApplicationDbContext context) : ICaseServiceCaseManagerDashboard
{
    public IQueryable<Case> GetAllCases(string userId)
    {
        return context.Cases
            .Include(c => c.Client)
            .Where(c => c.CaseManagerId == userId);
    }

    public IQueryable<Case> GetCasesBySearchTerm(IQueryable<Case> cases, string searchTerm)
    {
        searchTerm = searchTerm.Trim();
        cases = cases.Where(c => 
            c.CaseNumber.Contains(searchTerm) || 
            c.Title.Contains(searchTerm) ||
            c.Client.FirstName.Contains(searchTerm) ||
            c.Client.LastName.Contains(searchTerm) ||
            (c.Client.FirstName + " " + c.Client.LastName).Contains(searchTerm));
        return cases;
    }

    public async Task<CasesViewModel> CreateCaseViewModel(PagedResult<Case> pagedResult, IQueryable<Case> loggedInUserCases, Status? caseStatus,
        string search, string loggedInUser)
    {
        return new CasesViewModel
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
    }
}