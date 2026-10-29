using caseManageMentSystem.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace caseManageMentSystem.Tests.Integration;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _databaseName =
        Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<
                IDbContextOptionsConfiguration<ApplicationDbContext>>();

            services.RemoveAll<
                DbContextOptions<ApplicationDbContext>>();

            services.RemoveAll<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlite(
                    $"Data Source={_databaseName}.db"));
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var  scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync();
    }
    
    
}