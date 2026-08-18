using caseManageMentSystem.Areas.CaseManager.Controllers;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    }
}
