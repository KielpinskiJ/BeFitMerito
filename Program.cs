using BeFit.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    {
        // Disable email confirmation for development
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
        
        // Password settings (relaxed for development)
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
        
        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
        
        // User settings
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdmin(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

/// <summary>
/// Seeds the Admin role and creates an admin user if they don't exist.
/// </summary>
static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

    // Create Admin role if it doesn't exist
    const string adminRole = "Admin";
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // Create admin user if it doesn't exist
    const string adminEmail = "kielpinski@admin.pl";
    const string adminPassword = "Haslo123!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
    else
    {
        // Ensure the admin user is in the Admin role
        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }

    // Seed exercise types if none exist
    if (!context.ExerciseTypes.Any())
    {
        var exerciseTypes = new[]
        {
            new BeFit.Models.ExerciseType { Name = "Przysiad ze sztangą", Description = "Ćwiczenie na nogi z wykorzystaniem sztangi" },
            new BeFit.Models.ExerciseType { Name = "Wyciskanie sztangi leżąc", Description = "Ćwiczenie na klatkę piersiową" },
            new BeFit.Models.ExerciseType { Name = "Martwy ciąg", Description = "Ćwiczenie angażujące całe ciało" },
            new BeFit.Models.ExerciseType { Name = "Wiosłowanie sztangą", Description = "Ćwiczenie na plecy" },
            new BeFit.Models.ExerciseType { Name = "Wyciskanie żołnierskie", Description = "Ćwiczenie na barki" },
            new BeFit.Models.ExerciseType { Name = "Podciąganie na drążku", Description = "Ćwiczenie na plecy z własną masą ciała" },
            new BeFit.Models.ExerciseType { Name = "Pompki", Description = "Ćwiczenie na klatkę piersiową z własną masą ciała" },
            new BeFit.Models.ExerciseType { Name = "Brzuszki", Description = "Ćwiczenie na mięśnie brzucha" },
            new BeFit.Models.ExerciseType { Name = "Wykroki", Description = "Ćwiczenie na nogi" },
            new BeFit.Models.ExerciseType { Name = "Uginanie ramion ze sztangą", Description = "Ćwiczenie na biceps" }
        };

        context.ExerciseTypes.AddRange(exerciseTypes);
        await context.SaveChangesAsync();
    }
}
