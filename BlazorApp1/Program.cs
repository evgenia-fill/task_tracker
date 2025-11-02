using BlazorApp1.Services;
using Data;
using Data.DataContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// var dbPath = Path.Combine(AppContext.BaseDirectory, "DataBase.db");
var dbPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
    @"../../../../SharedDatabase/DataBase.db"));
var connectionString = $"Data Source={dbPath}";

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite(connectionString,
        b => b.MigrationsAssembly("Data")));
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<UserStateService>();

// builder.Services.AddSingleton(new UserManager(connectionString));
// builder.Services.AddScoped<UserStateService>();

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