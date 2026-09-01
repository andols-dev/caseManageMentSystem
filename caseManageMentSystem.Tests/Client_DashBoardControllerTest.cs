using System.Security.Claims;
using System.Security.Cryptography;
using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.Client.Controllers;
using caseManageMentSystem.Areas.Client.Services;
using caseManageMentSystem.Areas.Client.ViewModels;
using caseManageMentSystem.Models;
using caseManageMentSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace caseManageMentSystem.Tests;

public class Client_DashBoardControllerTest
{
    [Fact]
    public async Task Index_Get_LoggedInUserAndCases()
    {
        // create test-user
        var testUser1 = new ApplicationUser()
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "John",
            LastName = "Doe",
        };
        
        // moq user manager
        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();

        var mockUserManager = new Mock<UserManager<ApplicationUser>>(
            mockUserStore.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(testUser1);

        // Create a list of cases
        List<CaseListItemViewModel> cases = [];
        cases.Add(new CaseListItemViewModel(CaseNumberGenerator.Generate(), "The best case ever", "test description",
            Status.active, new DateTime(2026, 8, 30, 10, 0, 0)));
        cases.Add(new CaseListItemViewModel(CaseNumberGenerator.Generate(), "A case", "a test description", Status.closed,new DateTime(2026, 8, 30, 10, 0, 0)));


        // moq case service

        var moqICaseService = new Mock<ICaseService>();
        
        // setup

        moqICaseService.Setup(c => c.GetCasesForClient(testUser1.Id)).ReturnsAsync(cases);
        
        // create controller
        
        var controller = new DashBoardController(mockUserManager.Object, moqICaseService.Object);
        
        // create result

        var result = await controller.Index();
        
        // Assert viewresult
        
        var viewResult = Assert.IsType<ViewResult>(result);
        
        // Assert model
        var model = Assert.IsType<DashBoardViewModel>(viewResult.Model);
        
        // Assert Fullname
        
        Assert.Equal(testUser1.FullName, model.FullName);
        
        // Assert that there is two cases
        
        Assert.Equal(2, model.Cases.Count);
        
        // Assert correct data
        Assert.Equal(
            cases[0].Title,
            model.Cases[0].Title
        );
        
        // Assert , verify user
        moqICaseService.Verify(c => c.GetCasesForClient(testUser1.Id), Times.Once);
        

    }

    [Fact]
    public async Task Index_Get_LoggedInUserNoCases()
    {
        // create test-user
        var testUser1 = new ApplicationUser()
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "John",
            LastName = "Doe",
        };
        
        // moq user manager
        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();

        var mockUserManager = new Mock<UserManager<ApplicationUser>>(
            mockUserStore.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        // get user
        mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(testUser1);
        
        // Create a list of cases, empty
        List<CaseListItemViewModel> emptyCaseList = [];
        
        // moq case service

        var moqICaseService = new Mock<ICaseService>();
        
        moqICaseService.Setup(c => c.GetCasesForClient(testUser1.Id)).ReturnsAsync(emptyCaseList);
        
        // create controller
        
        var controller = new DashBoardController(mockUserManager.Object, moqICaseService.Object);
        
        // create result

        var result = await controller.Index();
        
        // Assert viewresult
        
        var viewResult = Assert.IsType<ViewResult>(result);
        
        // Assert model
        var model = Assert.IsType<DashBoardViewModel>(viewResult.Model);
        
        // Assert Fullname
        
        Assert.Equal(testUser1.FullName, model.FullName);
        
        // Assert empty list
        Assert.Empty(model.Cases);
        
        moqICaseService.Verify(c => c.GetCasesForClient(testUser1.Id), Times.Once);
    }
    
  
    [Fact]
    public async Task Index_Returns_NotFound_When_NoLoggedInUser()
    {
        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(
            mockUserStore.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
        
        // check if user is null
        mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((ApplicationUser)null);
        
        // create mock caseService
        var caseService = new Mock<ICaseService>();
        
        // create controller
        
        var controller = new DashBoardController(mockUserManager.Object, caseService.Object);
        
        // result
        
        var result = await controller.Index();
        
        // Assert NotFound
        
        Assert.IsType<NotFoundResult>(result);
        
        // Verify
        
        caseService.Verify(
            c => c.GetCasesForClient(It.IsAny<string>()),
            Times.Never
        );

    }
    
    
}