using Data.DataContext;
using Data.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1;

public class AuthController: ControllerBase
{
    private readonly ApplicationDbContext context;

    public AuthController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public class TelegramLoginData
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public long AuthDate { get; set; }
        public string Hash { get; set; }
    }

    [HttpPost("api/auth/telegram")]
    public async Task<IActionResult> LoginWithTelegram([FromBody] TelegramLoginData data)
    {
        if (data == null)
            return BadRequest("No data received");
        
        var existing = await context.Users
            .FirstOrDefaultAsync(u => u.TelegramId == data.Id);

        if (existing != null)
        {
            return Ok(existing);
        }
        
        var user = new User
        {
            TelegramId = data.Id,
            FirstName = data.FirstName,
            LastName = data.LastName,
            UserName = data.Username ?? ("user_" + data.Id),
            Hash = Guid.NewGuid().ToString()
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Ok(user);
    }
}