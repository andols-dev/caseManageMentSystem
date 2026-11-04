using System.Net;
using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Areas.CaseManager.Services;
using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Areas.Client.Services;
using caseManageMentSystem.Data;
using caseManageMentSystem.Enums;
using caseManageMentSystem.Models;
using caseManageMentSystem.Services;
using HtmlAgilityPack;
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
        await factory.InitializeDatabaseAsync();
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

    [Fact]
    public async Task CreatingCase_SavesCaseAndHistory()
    {
        await using var  factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        var caseService = scope.ServiceProvider
            .GetRequiredService<ICaseCaseManagerService>();
        var historyService = scope.ServiceProvider
            .GetRequiredService<ICaseHistoryService>();

        var client = new ApplicationUser
        {
            UserName = "client@test.local",
            Email = "client@test.local",
            FirstName = "Test",
            LastName = "Client",
            EmailConfirmed = true,
        };
        var manager = new ApplicationUser
        {
            UserName = "manager@test.local",
            Email = "manager@test.local",
            FirstName = "Test",
            LastName = "Manager",
            EmailConfirmed = true,
        };
        db.Users.AddRange(client, manager);
        await db.SaveChangesAsync();
        var viewModel = new CreateCaseViewModel
        {
            ClientId = client.Id,
            Title = "Integration Test case",
            Description = "Testing case creation and history",
        };
        var createdCase = await caseService.CreateCase(viewModel, manager);
        var createdHistory =  historyService.CreateCaseHistory(createdCase.Id, manager);
        var savedCase = await db.Cases.SingleAsync(c => c.Id == createdCase.Id);
        var caseHistory = await db.CaseHistories.SingleAsync(c => c.Id == createdCase.Id);
        
        Assert.Equal("Integration Test case", savedCase.Title);
        Assert.Equal(client.Id, savedCase.ClientId);
        Assert.Equal(manager.Id, savedCase.CaseManagerId);
        Assert.Equal(createdCase.Id, caseHistory.CaseId);
    }

    [Fact]
    public async Task CreatingCase_ThroughHttp_ReturnsResponse()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var response = await client.GetAsync($"CaseManager/ClientsList/Create?clientId=test-client");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreatingCase_ThroughHttp_CreatesCaseAndHistory()
    {
        await using var factory = new CustomWebApplicationFactory();
        await factory.InitializeDatabaseAsync();
        string clientId;
        using var scope = factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
            
        var client = new ApplicationUser
        {
            UserName = "client@test.local",
            Email = "client@test.local",
            FirstName = "Test",
            LastName = "Client",
            EmailConfirmed = true
        };
            
        var manager = new ApplicationUser
        {
            UserName = "manager@test.local",
            Email = "manager@test.local",
            FirstName = "Test",
            LastName = "Manager",
            EmailConfirmed = true
        };
        db.Users.AddRange(client, manager);
        await db.SaveChangesAsync();
        var managerId = manager.Id;

        clientId = client.Id;
        using var httpClient = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        httpClient.DefaultRequestHeaders.Add(
            "X-Test-UserId",
            managerId
        );
        var response = await httpClient.GetAsync(
            $"/CaseManager/ClientsList/Create?clientId={clientId}"
        );
        var formData = new Dictionary<string, string>
        {
            { "ClientId", clientId },
            { "Title", "Första testärendet" },
            { "Description", "Detta är ett integrationstest." }
        };
        var html = await response.Content.ReadAsStringAsync();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var token = htmlDoc.DocumentNode
            .SelectSingleNode("//input[@name='__RequestVerificationToken']")
            ?.GetAttributeValue("value", null);

        Assert.NotNull(token);
        var postContent = new FormUrlEncodedContent(formData);
        postContent.Headers.Add(
            "RequestVerificationToken",
            token
            );
        var postResponse = await httpClient.PostAsync(
            "/CaseManager/ClientsList/Create",
            postContent
            );
        Assert.Equal(
            HttpStatusCode.Redirect,
            postResponse.StatusCode
        );
        using var verificationScope = factory.Services.CreateScope();
        
        var verificationDb = verificationScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var createdCase = await verificationDb.Cases.FirstOrDefaultAsync(c => c.ClientId == clientId);
        Assert.NotNull(createdCase);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
    }
}