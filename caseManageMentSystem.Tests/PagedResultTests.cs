using caseManageMentSystem.Models;

namespace caseManageMentSystem.Tests;

public class PagedResultTests
{
    [Theory]
    [InlineData(5, 22, 5)]
    [InlineData(10, 30, 3)]
    public void PagedResult_HasCorrectNumberOfPages(int pageSize, int totalItems, int expectedTotalPages)
    {
        // Arrange
        var pagedResult = new PagedResult<Case>();
        // Act
        pagedResult.PageSize =  pageSize;
        pagedResult.TotalItems =  totalItems;
        
        // Assert
        Assert.Equal(expectedTotalPages, pagedResult.TotalPages);
    }
    [Fact]
    public void HasPreviousPage_ShouldReturnTrueWhenNotOnFirstPage()
    {
        var pagedResult = new PagedResult<Case>();
        pagedResult.PageNumber = 2;
        
        Assert.True(pagedResult.HasPreviousPage);
    }

    [Fact]
    public void HasPreviousPage_ShouldReturnFalseOnFirstPage()
    {
        var pagedResult = new PagedResult<Case>();
        pagedResult.PageNumber = 1;
        Assert.False(pagedResult.HasPreviousPage);
    }

    [Fact]
    public void HasNextPage_ShouldReturnTrueWhenNotOnLastPage()
    {
        var pagedResult = new PagedResult<Case>();
        pagedResult.PageNumber = 2;
        pagedResult.PageSize = 10;
        pagedResult.TotalItems = 30;
        Assert.True(pagedResult.HasNextPage);
    }
    
    [Fact]
    public void HasNextPage_ShouldReturnFalseWhenOnLastPage()
    {
        var pagedResult = new PagedResult<Case>();
        pagedResult.PageNumber = 3;
        pagedResult.PageSize = 10;
        pagedResult.TotalItems = 30;
        Assert.False(pagedResult.HasNextPage);
    }
}