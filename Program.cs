using caseManageMentSystem.Areas.CaseManager.Services;
using caseManageMentSystem.Areas.Client.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 12;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});
builder.Services.AddScoped<ICaseService, CaseService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<ICaseCaseManagerService, CaseCaseManagerService>();
builder.Services.AddScoped<ICaseHistoryService, CaseHistoryService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InitializeAsync(scope.ServiceProvider);
}


app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/HandleErrorCode/500");
    app.UseStatusCodePagesWithReExecute("/Error/HandleErrorCode/{0}");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=User}/{action=Index}/{id?}"
    );
app.MapAreaControllerRoute(
    name: "Client",
    areaName: "Client",
    pattern: "Client/{controller=DashBoard}/{action=Index}/{id?}"
    );
app.MapAreaControllerRoute(
    name: "CaseManager",
    areaName: "CaseManager",
    pattern: "CaseManager/{controller=DashBoard}/{action=Index}/{id?}"
    );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");



app.Run();
