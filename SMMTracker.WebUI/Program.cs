using Microsoft.EntityFrameworkCore;
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Infrastructure.Services;
using SMMTracker.Application.Interfaces;
using SMMTracker.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Настройка HTTP
builder.WebHost.UseUrls("http://localhost:5002", "http://0.0.0.0:5002");

// Базовые сервисы ASP.NET Core
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

// База данных
var dbDir = Path.Combine(AppContext.BaseDirectory, "SharedDatabase");
Directory.CreateDirectory(dbDir);
var dbPath = Path.Combine(dbDir, "DataBase.db");
var connectionString = $"Data Source={dbPath}";

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite(connectionString));

// Сервисы приложения
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<UserStateService>();
builder.Services.AddScoped<IApplicationDbContext>(provider => 
    provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<TaskService>();

var app = builder.Build();

// Применяем миграции
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}

// Middleware
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

Console.WriteLine("Web application started: [http://localhost:5002](http://localhost:5002)");

app.Run();