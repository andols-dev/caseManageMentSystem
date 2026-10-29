using System.Net;
using caseManageMentSystem.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace caseManageMentSystem.Tests.Integration;

public class ApplicationIntegrationTests
{
    [Fact]
    public async Task LoginPage_ReturnsExpectedResponse()
    {
        await using var factory = new CustomWebApplicationFactory();

        using  var client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        var response = await client.GetAsync("/Account/Login");
        
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Redirect,
            $"Unexpected HTTP-status code: {(int)response.StatusCode}");
    }
    [Fact]
    public void ApplicationDbContext_UsesSqlite()
    {
        using var factory = new CustomWebApplicationFactory();
        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        Assert.Equal(
            "Microsoft.EntityFrameworkCore.Sqlite",
            db.Database.ProviderName);
    }

    [Fact]
    public async Task Database_CanCreateSchema()
    {
       await using var factory = new CustomWebApplicationFactory();

       await factory.InitializeDatabaseAsync();
       
       using var  scope = factory.Services.CreateScope();
       var db = scope.ServiceProvider
           .GetRequiredService<ApplicationDbContext>();
       
       Assert.True(await db.Database.CanConnectAsync());
       Assert.True(await db.Cases.AnyAsync() == false);
    }
}