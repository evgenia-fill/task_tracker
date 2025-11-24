using BlazorApp1.Services;
using Data;
using Data.DataContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var solutionDir = Directory.GetParent(AppContext.BaseDirectory)
                      ?.Parent?.Parent?.Parent?.Parent?.FullName 
                  ?? throw new InvalidOperationException("Cannot find solution directory");

var dataProjectDir = Path.Combine(solutionDir, "Data");
if (!Directory.Exists(dataProjectDir))
    Directory.CreateDirectory(dataProjectDir);

var dbPath = Path.Combine(dataProjectDir, "DataBase.db");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<UserStateService>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();