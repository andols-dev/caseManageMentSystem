using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public interface IClientService
{
    Task<ApplicationUser?> GetClientWithCases(string clientId);
}