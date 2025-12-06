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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();  
app.MapControllers();

Console.WriteLine("Web application started: [http://localhost:5002](http://localhost:5002)");

app.Run();