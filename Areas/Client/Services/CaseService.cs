using caseManageMentSystem.Areas.Client.ViewModels;
using caseManageMentSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace caseManageMentSystem.Areas.Client.Services;

public class CaseService(ApplicationDbContext context) : ICaseService
{
    public async Task<IReadOnlyList<CaseListItemViewModel>> GetCasesForClient(string clientId)
    {
        return await context.Cases
            .AsNoTracking()
            .Where(c => c.ClientId == clientId)
            .Select(c => new CaseListItemViewModel(
                c.CaseNumber,
                c.Title,
                c.Description,
                c.Status,
                c.CreatedDate
            ))
            .ToListAsync();
    }
}