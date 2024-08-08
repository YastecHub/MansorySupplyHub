using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification;
using MansorySupplyHub.Data;
using MansorySupplyHub.Implementation.Interface;
using MansorySupplyHub.Implementation.Services;
using MansorySupplyHub.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using MansorySupplyHub.Dto;
using MansorySupplyHub.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
});

builder.Services.AddFluentEmail(builder.Configuration);

builder.Services.Configure<SMTPConfig>(builder.Configuration.GetSection("SMTPConfig"));
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    });

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IApplicationTypeService, ApplicationTypeService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages(); 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();





//// Ensure roles are created
//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//    //await SeedRolesAsync(roleManager);
//    //await SeedAdminUserAsync(userManager, roleManager);
//}


////async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
////{
////    string[] roleNames = { WC.AdminRole, WC.CustomerRole };
////    IdentityResult roleResult;

////    foreach (var roleName in roleNames)
////    {
////        var roleExist = await roleManager.RoleExistsAsync(roleName);
////        if (!roleExist)
////        {
////            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
////        }
////    }
////}

////async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
////{
////    string adminEmail = "admin@gmail.com";
////    string adminPassword = "Admin@123";

////    if (await userManager.FindByEmailAsync(adminEmail) == null)
////    {
////        var adminUser = new ApplicationUser
////        {
////            UserName = adminEmail,
////            Email = adminEmail
////        };

////        var result = await userManager.CreateAsync(adminUser, adminPassword);
////        if (result.Succeeded)
////        {
////            await userManager.AddToRoleAsync(adminUser, WC.AdminRole);
////        }
////    }
////}
