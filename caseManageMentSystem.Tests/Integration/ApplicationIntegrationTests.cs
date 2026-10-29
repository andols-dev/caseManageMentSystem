using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace caseManageMentSystem.Tests.Integration;

public class ApplicationIntegrationTests
{
    [Fact]
    public async Task LoginPage_ReturnsExpectedResponse()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });

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
}