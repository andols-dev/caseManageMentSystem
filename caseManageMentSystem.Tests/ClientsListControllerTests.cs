using caseManageMentSystem.Areas.Admin.Controllers;
using caseManageMentSystem.Areas.CaseManager.Controllers;
using caseManageMentSystem.Areas.CaseManager.Services;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace caseManageMentSystem.Tests
{
    public class ClientsListControllerTests
    {
        [Fact]
        public async Task Index_ReturnsClientsView()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!
            );

            var clients = new List<ApplicationUser>
            { 
                new ApplicationUser
                {
                    Id = "client-1"
                },
                new ApplicationUser
                {
                    Id = "client-2"
                }

            };

            userManagerMock
                .Setup(x => x.GetUsersInRoleAsync("client"))
                .ReturnsAsync( clients );


            var iNoteService = new Mock<INoteService>();
            var iCaseService = new Mock<ICaseCaseManagerService>();
            var iCaseHistory = new Mock<ICaseHistoryService>();
            var iClientService = new Mock<IClientService>();
            var controller = new ClientsListController(
                userManagerMock.Object,
                iNoteService.Object,
                iCaseService.Object,
                iCaseHistory.Object,
                iClientService.Object
            );

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<IEnumerable<ApplicationUser>>(
                viewResult.Model
            );

            Assert.Equal(2, model.Count());

            Assert.Contains(model, client => client.Id == "client-1");
            Assert.Contains(model, client => client.Id == "client-2");
        }

        [Fact]
        public async Task Index_WhenThereAreNoCLients_ReturnsEmptyList()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!
            );

            var clients = new List<ApplicationUser>();

            userManagerMock
                .Setup(x => x.GetUsersInRoleAsync("client"))
                .ReturnsAsync(clients);

            var iNoteService = new Mock<INoteService>();
            var iCaseService = new Mock<ICaseCaseManagerService>();
            var iCaseHistory = new Mock<ICaseHistoryService>();
            var iClientService = new Mock<IClientService>();
            var controller = new ClientsListController(
                userManagerMock.Object,
                iNoteService.Object,
                iCaseService.Object,
                iCaseHistory.Object,
                iClientService.Object
            );

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<IEnumerable<ApplicationUser>>(
                 viewResult.Model
             );

            Assert.Empty( model );
        }
        [Fact]
        public async Task Details_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            await using var context = new ApplicationDbContext(options);

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
                null
            );

            var iNoteService = new Mock<INoteService>();
            var iCaseService = new Mock<ICaseCaseManagerService>();
            var iCaseHistory = new Mock<ICaseHistoryService>();
            var iClientService = new Mock<IClientService>();
            var controller = new ClientsListController(
                mockUserManager.Object,
                iNoteService.Object,
                iCaseService.Object,
                iCaseHistory.Object,
                iClientService.Object
            );
            var result = await controller.Details("123");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Return_Client_When_ClientHasNoCases()
        {
            // Arrange

            var client = new ApplicationUser
            {
                Id = "client-1",
                UserName = "client@test.com",
                FirstName = "Test",
                LastName = "Client"
            };


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
            
            var iNoteService = new Mock<INoteService>();
            var iCaseService = new Mock<ICaseCaseManagerService>();
            var iCaseHistory = new Mock<ICaseHistoryService>();
            var iClientService = new Mock<IClientService>();

            iClientService
                .Setup(s => s.GetClientWithCases(client.Id))
                .ReturnsAsync(client);
            
            var controller = new ClientsListController(
                mockUserManager.Object,
                iNoteService.Object,
                iCaseService.Object,
                iCaseHistory.Object,
                iClientService.Object
                );

            // Act
            var result = await controller.Details(client.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApplicationUser>(viewResult.Model);

            Assert.Equal(client.Id, model.Id);
            Assert.Empty(model.ClientCases);
        }

        [Fact]
        public async Task Details_Return_ClientWithCasesAndCaseManagers()
        {
            // Arrange
            var client = new ApplicationUser
            {
                Id = "client-1",
                UserName = "client@test.com",
                FirstName = "Test",
                LastName = "Client"
            };

            var caseManager = new ApplicationUser
            {
                Id = "manager-1",
                UserName = "manager@test.com",
                FirstName = "Test",
                LastName = "Manager"
            };

            var case1 = new Case
            {
                Id = 1,
                ClientId = client.Id,
                CaseManagerId = caseManager.Id,
                CaseNumber = "CASE-001",
                Title = "First case",
                CaseManager = caseManager
            };

            var case2 = new Case
            {
                Id = 2,
                ClientId = client.Id,
                CaseManagerId = caseManager.Id,
                CaseNumber = "CASE-002",
                Title = "Second case",
                CaseManager = caseManager
            };
            client.ClientCases = new List<Case>
            {
                case1,
                case2
            };
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
            
            var iNoteService = new Mock<INoteService>();
            var iCaseService = new Mock<ICaseCaseManagerService>();
            var iCaseHistory = new Mock<ICaseHistoryService>();
            var iClientService = new Mock<IClientService>();
            iClientService
                .Setup(s => s.GetClientWithCases(client.Id))
                .ReturnsAsync(client);
                
            var controller = new ClientsListController(
                mockUserManager.Object,
                iNoteService.Object,
                iCaseService.Object,
                iCaseHistory.Object,
                iClientService.Object
                );

            // Act
            var result = await controller.Details(client.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApplicationUser>(viewResult.Model);
            iClientService.Verify(
                s => s.GetClientWithCases(client.Id),
                Times.Once);
            Assert.Equal(client.Id, model.Id);

            Assert.Equal(2, model.ClientCases.Count);

            Assert.Contains(model.ClientCases, c => c.CaseNumber == "CASE-001");
            Assert.Contains(model.ClientCases, c => c.CaseNumber == "CASE-002");

            Assert.All(
                model.ClientCases,
                c => Assert.NotNull(c.CaseManager));

            Assert.All(
                model.ClientCases,
                c => Assert.Equal("manager-1", c.CaseManager.Id));
        }
    }
}
