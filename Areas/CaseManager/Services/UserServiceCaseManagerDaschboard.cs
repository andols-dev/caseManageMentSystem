using caseManageMentSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public class UserServiceCaseManagerDaschboard(ApplicationDbContext context) : IUserServiceCaseManagerDaschboard
{
    public async Task<string?> GetUserFullName(string userId)
    {
        return await context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.FullName)
            .FirstOrDefaultAsync();
    }
}