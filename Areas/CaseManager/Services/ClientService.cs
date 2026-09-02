using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public class ClientService(ApplicationDbContext context) : IClientService
{
    public Task<ApplicationUser?> GetClientWithCases(string clientId)
    {
        return context.Users
            .Include(c => c.ClientCases)
            .ThenInclude(c => c.CaseManager)
            .FirstOrDefaultAsync(u => u.Id == clientId);
    }
}