using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Services;
using SMMTracker.Domain.IRepositories;
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Infrastructure.Repositories;

namespace SMMTracker.WebUI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Secrets.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        ConfigureServices(builder);
        
        var app = builder.Build();
        await ConfigureMiddlewareAsync(app);
        
        await app.RunAsync();
    }
    
    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;
        
        // бд
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite(connectionString));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        
        // основные сервисы
        services.AddRazorPages();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHttpClient();
        
        // репозитории
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ICalendarRepository, CalendarRepository>();
        services.AddScoped<IUserTeamRepository, UserTeamRepository>();
        services.AddScoped<IUserTaskRepository, UserTaskRepository>();
        
        // сервисы
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<IEventService, EventService>();
        
        // шттп для razor pages
        services.AddScoped<HttpClient>(_ => 
            new HttpClient { BaseAddress = new Uri("https://kindly-rapid-margay.cloudpub.ru") }); // тут должны быть ваши url или localhost
        
        // cors
        services.AddCors(options => options.AddPolicy("CorsPolicy",
            policy => policy.WithOrigins("https://kindly-rapid-margay.cloudpub.ru")
                           .AllowAnyHeader()
                           .AllowAnyMethod()));
        
        // Authentication
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Login"; // Путь к странице логина
                options.AccessDeniedPath = "/Login"; // При отказе в доступе тоже на логин
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
        
                // Для API запросов
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    }
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });
        
        services.AddAuthorization();
    }
    
    private static async Task ConfigureMiddlewareAsync(WebApplication app)
{
    // миграции
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    
    // конфиг pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors("CorsPolicy");
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.MapRazorPages();
    app.MapControllers();
}
}