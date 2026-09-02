using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using caseManageMentSystem.Services;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public class CaseCaseManagerService(ApplicationDbContext context) : ICaseCaseManagerService
{
    public async Task<Case> CreateCase(CreateCaseViewModel caseItem, ApplicationUser user)
    {
        var newCase = new Case
        {
            ClientId = caseItem.ClientId,
            Title = caseItem.Title,
            Description = caseItem.Description,
            Status = Enums.Status.active,
            CaseNumber = CaseNumberGenerator.Generate(),
            CreatedDate = DateTime.Now,
            CaseManagerId = user.Id,
        };

        context.Cases.Add(newCase);
        await context.SaveChangesAsync();
        
        return newCase;
    }
}