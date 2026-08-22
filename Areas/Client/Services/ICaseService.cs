using caseManageMentSystem.Areas.Client.ViewModels;

namespace caseManageMentSystem.Areas.Client.Services;

public interface ICaseService
{
    Task<IReadOnlyList<CaseListItemViewModel>> GetCasesForClient(string clientId);
}