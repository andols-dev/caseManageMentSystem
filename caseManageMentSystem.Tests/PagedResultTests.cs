using caseManageMentSystem.Models;

namespace caseManageMentSystem.Tests;

public class PagedResultTests
{
    [Fact]
    public void PagedResult_HasCorrectNumberOfPages()
    {
        // Arrange
        var pagedResult = new PagedResult<Case>();
        // Act
        pagedResult.PageSize = 5;
        pagedResult.TotalItems = 22;
        
        // Assert
        Assert.Equal(5, pagedResult.TotalPages);
    }
    [Fact]
    public void HasPreviousPage_ShouldBeGreaterThen1()
    {
        var pagedResult = new PagedResult<Case>();
        pagedResult.PageNumber = 2;
        
        Assert.True(pagedResult.HasPreviousPage);
    }
}