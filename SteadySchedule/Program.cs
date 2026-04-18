using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SteadySchedule.Components;
using SteadySchedule.Data;
using SteadySchedule.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddScoped<ScheduleService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var scheduleService = services.GetRequiredService<ScheduleService>();
    await scheduleService.SeedPositionsIfEmptyAsync();
    await scheduleService.SeedEmployeesIfEmptyAsync();

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    var testUsers = new List<(string Email, int CompanyId)>
    {
        ("test@test.com", 1),
        ("user2@test.com", 2)
    };

    foreach (var (email, companyId) in testUsers)
    {
        var normalizedEmail = email.ToLower();

        var existingUser = await userManager.FindByEmailAsync(normalizedEmail);

        if (existingUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                CompanyId = companyId
            };

            var result = await userManager.CreateAsync(user, "Password123!");

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user {email}: {errors}");
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();