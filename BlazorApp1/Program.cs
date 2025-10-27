// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Components.Web;
// using BlazorApp1.Data;
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
// builder.Services.AddRazorPages();
// builder.Services.AddServerSideBlazor();
// builder.Services.AddSingleton<WeatherForecastService>();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }
//
// app.UseHttpsRedirection();
//
// app.UseStaticFiles();
//
// app.UseRouting();
//
// app.MapBlazorHub();
// app.MapFallbackToPage("/_Host");
//
// app.Run();

using BlazorApp1.Services;
using Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// var dbPath = Path.Combine(AppContext.BaseDirectory, "DataBase.db");
var dbPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
    @"../../../../SharedDatabase/DataBase.db"));
var connectionString = $"Data Source={dbPath}";

builder.Services.AddSingleton(new UserManager(connectionString));
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