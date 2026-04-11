using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = true;
});
var app = builder.Build();
// lowercase urls support


app.UseStaticFiles();

app.MapDefaultControllerRoute();
app.Run();
