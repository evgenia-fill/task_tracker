using Microsoft.AspNetCore.Mvc;
using SMMTracker.Infrastructure.Data.DataContext;

namespace SMMTracker.WebUI.API;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IActionResult GetHello()
    {
        return Ok("GET");
    }
    
    
}