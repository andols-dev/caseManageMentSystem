using System.Net;
using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using caseManageMentSystem.Services;
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

    [Fact]
    public async Task Case_CanBeSavedAndRetrieved()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        var client = new ApplicationUser
        {
            UserName = "client@test.local",
            Email = "client@test.local",
            FirstName = "Test",
            LastName = "Client",
            EmailConfirmed = true,
        };
        var caseManager = new ApplicationUser
        {
            UserName = "manager@test.local",
            Email = "manager@test.local",
            FirstName = "Test",
            LastName = "Manager",
            EmailConfirmed = true,
        };
        db.Users.AddRange(client, caseManager);
        await db.SaveChangesAsync();

        var testCase = new Case
        {
            ClientId = client.Id,
            CaseManagerId = caseManager.Id,
            CaseNumber = "TEST-001",
            Title = "Integration Test",
            Description = "Testing SQLite persistence",
            Status = Status.active,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };
        db.Cases.Add(testCase);
        await db.SaveChangesAsync();
        
        var savedCase = await db.Cases.SingleAsync(c => c.CaseNumber == "TEST-001");
        Assert.Equal("Integration Test", savedCase.Title);
        Assert.Equal(client.Id, savedCase.ClientId);
        Assert.Equal(caseManager.Id, savedCase.CaseManagerId);
    }
}