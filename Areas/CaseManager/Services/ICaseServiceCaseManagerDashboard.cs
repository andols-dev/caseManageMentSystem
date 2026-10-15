using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public interface ICaseServiceCaseManagerDashboard
{
    IQueryable<Case> GetAllCases(string userId);
    
    IQueryable<Case> GetCasesBySearchTerm(IQueryable<Case> cases,string searchTerm);
    
    Task<CasesViewModel> CreateCaseViewModel(
        PagedResult<Case> pagedResult,
        IQueryable<Case> loggedInUserCases,
        Status? caseStatus,
        string search,
        string loggedInUser
);
}