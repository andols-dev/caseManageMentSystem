using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public class CaseServiceCaseManagerDashboard(ApplicationDbContext context) : ICaseServiceCaseManagerDashboard
{
    public IQueryable<Case> GetAllCases(string userId)
    {
        return context.Cases
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

    public CasesViewModel CreateCaseViewModel(List<Case> cases, int totalCases, int activeCases, int closedCases, int waitingCases,
        int delayedCases, string fullName, Status? caseStatus, string search, PagedResult<Case> pagedResult)
    {
        return new CasesViewModel(Cases = cases,
            TotalCases = totalCases,
            ActiveCases = activeCases,
            ClosedCases = closedCases,
            WaitingCases = waitingCases,
            DelayedCases = delayedCases,
            FullName = fullName,
            CaseStatus = caseStatus,
            Search = search,
            PagedResult = pagedResult);
    }
}