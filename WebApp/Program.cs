using BusinessLogic.Factories;
using BusinessLogic.Services;
using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IProjectsRepository, ProjectsRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<ProjectService>();


builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
});

var app = builder.Build();

app.UseHsts();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var profileService = scope.ServiceProvider.GetRequiredService<ProfileService>();

    var roleExists = await roleManager.RoleExistsAsync("Admin");
    if (!roleExists)
    {
        var role = new AppRole { Name = "Admin" };
        await roleManager.CreateAsync(role);
    }

    var user = new AppUser { UserName = "admin@alpha.com", Email = "admin@alpha.com" };
    var userExists = await userManager.FindByNameAsync(user.UserName);
    if(userExists == null)
    {
        var result = await userManager.CreateAsync(user, "BytMig123!");
        if(result.Succeeded)
        {
            var profile = ProfileFactory.NewProfileEntity();
            profile.UserId = user.Id;
            await profileService.CreateProfile(profile);

            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
};


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Dashboard}/{id?}")
    .WithStaticAssets();


app.Run();
