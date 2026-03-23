using Library.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Microsoft.AspNetCore.Diagnostics;
using Library.MVC.SeedData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Library.MVC")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();


var app = builder.Build();

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    string[] roles = { "Admin", "Inspector", "Viewer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminEmail = "admin@test.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    var inspectorEmail = "inspector@test.com";
    var inspectorUser = await userManager.FindByEmailAsync(inspectorEmail);
    if (inspectorUser == null)
    {
        inspectorUser = new IdentityUser
        {
            UserName = inspectorEmail,
            Email = inspectorEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(inspectorUser, "Inspector123!");
        await userManager.AddToRoleAsync(inspectorUser, "Inspector");
    }

    var viewerEmail = "viewer@test.com";
    var viewerUser = await userManager.FindByEmailAsync(viewerEmail);
    if (viewerUser == null)
    {
        viewerUser = new IdentityUser
        {
            UserName = viewerEmail,
            Email = viewerEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(viewerUser, "Viewer123!");
        await userManager.AddToRoleAsync(viewerUser, "Viewer");
    }

    DbInitializer.Seed(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error != null)
            {
                Log.Error(exceptionHandlerPathFeature.Error,
                    "Unhandled exception occurred while processing request for {Path}",
                    exceptionHandlerPathFeature.Path);
            }

            context.Response.Redirect("/Home/Error");
            await Task.CompletedTask;
        });
    });

    app.UseHsts();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature?.Error != null)
            {
                Log.Error(exceptionHandlerPathFeature.Error,
                    "Unhandled exception occurred while processing request for {Path}",
                    exceptionHandlerPathFeature.Path);
            }

            context.Response.Redirect("/Home/Error");
            await Task.CompletedTask;
        });
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
builder.Services.AddHttpContextAccessor();
app.UseAuthentication();
app.Use(async (context, next) =>
{
    var userName = context.User?.Identity?.IsAuthenticated == true
        ? context.User.Identity.Name
        : "Anonymous";

    using (LogContext.PushProperty("UserName", userName))
    {
        await next();
    }
});
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();