using Microsoft.EntityFrameworkCore;
using SteadySchedule.Components;
using SteadySchedule.Data;
using SteadySchedule.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ScheduleService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var scheduleService = scope.ServiceProvider.GetRequiredService<ScheduleService>();
    await scheduleService.SeedPositionsIfEmptyAsync();
    await scheduleService.SeedEmployeesIfEmptyAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
