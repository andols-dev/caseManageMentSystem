using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Data;
using caseManageMentSystem.Enums;
using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public class CaseHistoryService(ApplicationDbContext context) : ICaseHistoryService
{
    public async Task CreateCaseHistory(int caseId, ApplicationUser user)
    {
        var newCaseHistory = new CaseHistory
        {
            CaseId = caseId,
            UserId = user.Id,
            Type = CaseHistoryType.CaseCreated,
            CreatedDate = DateTime.UtcNow,
        };

        context.CaseHistories.Add(newCaseHistory);
        await context.SaveChangesAsync();
    }
}