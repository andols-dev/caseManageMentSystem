namespace caseManageMentSystem.Areas.CaseManager.Services;

public interface IUserServiceCaseManagerDaschboard
{
   Task<string?> GetUserFullName(string userId);
}