using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public interface ICaseServiceCaseManagerDashboard
{
    IQueryable<Case> GetAllCases(string userId);
    
    IQueryable<Case> GetCasesBySearchTerm(IQueryable<Case> cases,string searchTerm);
    
    CasesViewModel CreateCaseViewModel(
        List<Case> cases,
        int totalCases,
        int activeCases,
        int closedCases,
        int waitingCases,
        int delayedCases,
        string fullName,
        Status? caseStatus,
        string search,
        PagedResult<Case> pagedResult);
}