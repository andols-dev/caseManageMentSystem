using System.Security.Claims;
using caseManageMentSystem.Areas.CaseManager.Controllers;
using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.Services;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace caseManageMentSystem.Tests;

public class CaseManagerDashBoardTests
{
    [Fact] // testing the service. should return logged-in users list of cases
    public async Task GetAllCases_ReturnAllCasesForLoggedInUser()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);
        
        var caseList = new List<Case>()
        {
            new Case
            {
                Id = 1,
                CaseManagerId = "2",
                Status = Status.active
            },
            new Case
            {
                Id = 2,
                CaseManagerId = "2",
                Status = Status.active
            },
            new Case
            {
                Id = 3,
                CaseManagerId = "1",
                Status = Status.active
            }
        }.AsQueryable();

        context.AddRange(caseList);
        await context.SaveChangesAsync();
        var service = new CaseServiceCaseManagerDashboard(context);
        // Act
        var result = service.GetAllCases("2");
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
    [Fact] // testing the service. should return even when user have no cases
    public async Task GetAllCases_ReturnEmptyList_WhenUserHasNoCases()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);

        var service = new CaseServiceCaseManagerDashboard(context);

        // Act
        var result = service.GetAllCases("2");

        // Assert
        Assert.Empty(result);
    }
}