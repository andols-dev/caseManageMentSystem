using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public interface ICaseCaseManagerService
{
    Task<Case> CreateCase(CreateCaseViewModel caseItem, ApplicationUser user);
    Task<Case?> GetCaseDetails(int caseId);
}