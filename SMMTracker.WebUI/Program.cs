using Microsoft.AspNetCore.Authentication.Cookies;
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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite(connectionString));

// Сервисы приложения
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<UserStateService>();
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TeamService>();
builder.Services.AddScoped<CalendarService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddEndpointsApiExplorer(); // Эта строка нужна для Swagger
builder.Services.AddSwaggerGen(); // А эта его добавляет

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

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
    app.UseSwagger();
    app.UseSwaggerUI(); // Эта строка добавляет веб-интерфейс
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();

Console.WriteLine("Web application started: [http://localhost:5002](http://localhost:5002)");

app.Run();