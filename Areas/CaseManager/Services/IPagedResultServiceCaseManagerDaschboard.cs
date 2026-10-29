using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public interface IPagedResultServiceCaseManagerDaschboard
{
    Task<PagedResult<Case>> CreatePagedResult(IQueryable<Case> cases, int page, int pageSize);
}