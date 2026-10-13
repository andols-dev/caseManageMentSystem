using caseManageMentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public class PagedResultServiceCaseManagerDaschboard : IPagedResultServiceCaseManagerDaschboard
{
    public async Task<PagedResult<Case>> CreatePagedResult(IQueryable<Case> cases, int page, int pageSize)
    {
        var totalItems = await cases.CountAsync();
        var items = await cases
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return new PagedResult<Case>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalItems =  totalItems,
        };
    }
}