using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public interface ICaseHistoryService
{
    Task CreateCaseHistory(int CaseId, ApplicationUser user);
}