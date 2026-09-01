using caseManageMentSystem.Areas.Admin.Controllers;
using caseManageMentSystem.Areas.CaseManager.Controllers;
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

            var controller = new ClientsListController(
                null!,
                userManagerMock.Object
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

            var controller = new ClientsListController(
                null!,
                userManagerMock.Object
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

            var controller = new ClientsListController(
                context,
                mockUserManager.Object
            );
            var result = await controller.Details("123");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_Return_Client_When_ClientHasNoCases()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new ApplicationDbContext(options);

            var client = new ApplicationUser
            {
                Id = "client-1",
                UserName = "client@test.com",
                FirstName = "Test",
                LastName = "Client"
            };

            context.Users.Add(client);
            await context.SaveChangesAsync();

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

            var controller = new ClientsListController(
                context,
                mockUserManager.Object);

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
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            await using var context = new ApplicationDbContext(options);

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
                Title = "First case"
            };

            var case2 = new Case
            {
                Id = 2,
                ClientId = client.Id,
                CaseManagerId = caseManager.Id,
                CaseNumber = "CASE-002",
                Title = "Second case"
            };

            client.ClientCases.Add(case1);
            client.ClientCases.Add(case2);

            context.Users.Add(client);
            context.Users.Add(caseManager);
            context.Cases.AddRange(case1, case2);

            await context.SaveChangesAsync();

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

            var controller = new ClientsListController(
                context,
                mockUserManager.Object);

            // Act
            var result = await controller.Details(client.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApplicationUser>(viewResult.Model);

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
