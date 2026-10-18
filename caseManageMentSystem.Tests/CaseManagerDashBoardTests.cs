using System.Runtime.InteropServices;
using System.Security.Claims;
using caseManageMentSystem.Areas.CaseManager.Controllers;
using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.Services;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using caseManageMentSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    // testing the service, GetCasesBySearchTerm
    [Fact]
    public async Task GetCasesBySearchTerm_ReturnEmpty_WhenSearchTermDontMatch()
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
                Status = Status.active,
                Title =  "Test",
                CaseNumber = CaseNumberGenerator.Generate(),
                Client = new ApplicationUser()
                {
                    FirstName = "Anna",
                    LastName = "Andersson"
                }
            },
        }.AsQueryable();
       
        var service = new CaseServiceCaseManagerDashboard(context);
        // Act
        var result = service.GetCasesBySearchTerm(caseList, "test1");
        // Assert
        Assert.Empty(result);
    }
    
    [Theory]
    [InlineData(3,"Test")]
    [InlineData(0,"Test Case")]
    [InlineData(1,"first")]
    public async Task GetCasesBySearchTerm_SearchByTitle_ReturnsMatchingCase(int expected, string searchTerm)
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
                Status = Status.active,
                Title =  "Test",
                CaseNumber = CaseNumberGenerator.Generate(),
                Client = new ApplicationUser()
                {
                    FirstName = "Anna",
                    LastName = "Andersson"
                }
            },
            new Case
            {
                Id = 2,
                CaseManagerId = "3",
                Status = Status.closed,
                Title =  "Test",
                CaseNumber = CaseNumberGenerator.Generate(),
                Client = new ApplicationUser()
                {
                    FirstName = "Boris",
                    LastName = "Jönsson"
                }
            },
            new Case
            {
                Id = 3,
                CaseManagerId = "2",
                Status = Status.active,
                Title =  "Test",
                CaseNumber = CaseNumberGenerator.Generate(),
                Client = new ApplicationUser()
                {
                    FirstName = "David",
                    LastName = "Andersson"
                }
            },
            new Case
            {
                Id = 4,
                CaseManagerId = "2",
                Status = Status.active,
                Title =  "The first case",
                CaseNumber = CaseNumberGenerator.Generate(),
                Client = new ApplicationUser()
                {
                    FirstName = "David",
                    LastName = "Andersson"
                }
            },
        }.AsQueryable();
       
        var service = new CaseServiceCaseManagerDashboard(context);
        // Act
        var result = service.GetCasesBySearchTerm(caseList, searchTerm);
        // Assert
        Assert.Equal(expected, result.Count());
    }
    [Theory]
    [InlineData(1,"David")]
    [InlineData(1,"David")]
    [InlineData(1,"David Andersson")]
    [InlineData(1,"Anna")]
    [InlineData(1,"Dav")]
    [InlineData(2, "Andersson")]
    [InlineData(0, "Kalle")]
    public async Task GetCasesBySearchTerm_SearchByClientName_ReturnsMatchingCase(int expected, string searchTerm)
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
                Status = Status.active,
                Title =  "Test",
                CaseNumber = CaseNumberGenerator.Generate(),
                Client = new ApplicationUser()
                {
                    FirstName = "Anna",
                    LastName = "Andersson"
                }
            },
            new Case
            {
                Id = 1,
                CaseManagerId = "3",
                Status = Status.closed,
                Title =  "Test",
                CaseNumber = CaseNumberGenerator.Generate(),
                Client = new ApplicationUser()
                {
                    FirstName = "Boris",
                    LastName = "Jönsson"
                }
            },
            new Case
            {
                Id = 3,
                CaseManagerId = "2",
                Status = Status.active,
                Title =  "Test",
                CaseNumber = CaseNumberGenerator.Generate(),
                Client = new ApplicationUser()
                {
                    FirstName = "David",
                    LastName = "Andersson"
                }
            },
        }.AsQueryable();
       
        var service = new CaseServiceCaseManagerDashboard(context);
        // Act
        var result = service.GetCasesBySearchTerm(caseList, searchTerm);
        // Assert
        Assert.Equal(expected, result.Count());
    }
    
    [Theory]
    [InlineData(1, "2026-65893215")]
    public async Task GetCasesBySearchTerm_SearchByCaseNumber_ReturnsMatchingCase(int expected, string searchTerm)
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
                Status = Status.active,
                Title =  "Test",
                CaseNumber = "2026-65893215",
                Client = new ApplicationUser()
                {
                    FirstName = "Anna",
                    LastName = "Andersson"
                }
            },
            new Case
            {
                Id = 1,
                CaseManagerId = "3",
                Status = Status.closed,
                Title =  "Test",
                CaseNumber = CaseNumberGenerator.Generate(),
                Client = new ApplicationUser()
                {
                    FirstName = "Boris",
                    LastName = "Jönsson"
                }
            },
            new Case
            {
                Id = 3,
                CaseManagerId = "2",
                Status = Status.active,
                Title =  "Test",
                CaseNumber = CaseNumberGenerator.Generate(),
                Client = new ApplicationUser()
                {
                    FirstName = "David",
                    LastName = "Andersson"
                }
            },
        }.AsQueryable();
       
        var service = new CaseServiceCaseManagerDashboard(context);
        // Act
        var result = service.GetCasesBySearchTerm(caseList, searchTerm);
        // Assert
        Assert.Equal(expected, result.Count());
    }
    
    
    // test the controller
    [Fact]
    public async Task Index_ReturnCasesThatBelongsToUser()
    {
        // Arrange
        var caseService = new Mock<ICaseServiceCaseManagerDashboard>();
        var userService = new Mock<IUserServiceCaseManagerDaschboard>();
        var pagedResultService = new Mock<IPagedResultServiceCaseManagerDaschboard>();
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
        
        userManager
            .Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns("123");
        // create a new controller. What is needed: UserManager<ApplicationUser> userManager, ICaseServiceCaseManagerDashboard caseService, IUserServiceCaseManagerDaschboard userService, IPagedResultServiceCaseManagerDaschboard pagedResultService
        var controller = new DashBoardController(userManager.Object, caseService.Object, userService.Object, pagedResultService.Object );

        caseService
            .Setup(c => c.GetAllCases("123"))
            .Returns(new List<Case>()
                {
                    new Case
                    {
                        Id = 1,
                    }
                }.AsQueryable()
            );
        
        // Act
        var result = await controller.Index(null, null); 
        
        // check that result is viewresult
        Assert.IsType(typeof(ViewResult), result);
        
        caseService.Verify(c => c.GetAllCases("123"), Times.Once);    
        
    }

    [Fact]
    public async Task Index_NoUserId_ReturnsUnauthorizedResult()
    {
        // Arrange
        var caseService = new Mock<ICaseServiceCaseManagerDashboard>();
        var userService = new Mock<IUserServiceCaseManagerDaschboard>();
        var pagedResultService = new Mock<IPagedResultServiceCaseManagerDaschboard>();
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        var controller = new DashBoardController(userManager.Object, caseService.Object, userService.Object, pagedResultService.Object );
        // Act
        var result = await controller.Index(null, null); 
        
        // Assert
        Assert.IsType(typeof(UnauthorizedResult), result);
        
    }
}