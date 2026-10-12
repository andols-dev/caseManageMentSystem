using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public class CaseServiceCaseManagerDashboard : ICaseServiceCaseManagerDashboard
{
    public IQueryable<Case> GetAllCases(string userId)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Case> GetCasesBySearchTerm(IQueryable<Case> cases, string searchTerm)
    {
        throw new NotImplementedException();
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