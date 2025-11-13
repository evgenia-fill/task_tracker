using BlazorApp1.Services;
using Data;
using Data.DataContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Создаём папку для базы, если её нет
var dbDir = Path.Combine(AppContext.BaseDirectory, "SharedDatabase");
Directory.CreateDirectory(dbDir);

// Полный путь к файлу базы
var dbPath = Path.Combine(dbDir, "DataBase.db");
var connectionString = $"Data Source={dbPath}";

// Регистрируем DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite(connectionString,
        b => b.MigrationsAssembly("Data")));

builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<UserStateService>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();