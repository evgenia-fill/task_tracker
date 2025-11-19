using Microsoft.EntityFrameworkCore;
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Настройка HTTP
builder.WebHost.UseUrls("http://localhost:5000", "http://0.0.0.0:5000");

// Базовые сервисы ASP.NET Core
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

// Бд
var dbDir = Path.Combine(AppContext.BaseDirectory, "SharedDatabase");
Directory.CreateDirectory(dbDir);
var dbPath = Path.Combine(dbDir, "DataBase.db");
var connectionString = $"Data Source={dbPath}";

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite(connectionString));

// Сервисы
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<UserStateService>();

var app = builder.Build();

// Миграции
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();

Console.WriteLine("Web application started: http://localhost:5000");

app.Run();