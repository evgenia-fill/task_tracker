using Data.DataContext;
using Data.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class TelegramLoginData
        {
            public long Id { get; set; }
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Username { get; set; } = "";
            public long AuthDate { get; set; }
            public string Hash { get; set; } = "";
        }

        [HttpPost("api/auth/telegram")]
        public async Task<IActionResult> LoginWithTelegram([FromBody] TelegramLoginData data)
        {
            if (data == null)
            {
                Console.WriteLine("No data received from Telegram!");
                return BadRequest("No data received");
            }

            Console.WriteLine($"Received TelegramId: {data.Id}, Username: {data.Username}");

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.TelegramId == data.Id);

            if (existingUser != null)
            {
                Console.WriteLine($"User found: {existingUser.Id}");
                return Ok(existingUser);
            }

            var user = new User
            {
                TelegramId = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                UserName = string.IsNullOrWhiteSpace(data.Username) ? $"user_{data.Id}" : data.Username,
                Hash = Guid.NewGuid().ToString()
            };

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"New user created: {user.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user: {ex}");
                return StatusCode(500, "Error saving user");
            }

            return Ok(user);
        }
    }
}