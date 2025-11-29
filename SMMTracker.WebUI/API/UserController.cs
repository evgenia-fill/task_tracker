using Microsoft.AspNetCore.Mvc;
using SMMTracker.Infrastructure.Data.DataContext;

namespace BlazorApp1.API;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext context;

    public UserController(ApplicationDbContext context)
    {
        this.context = context;
    }
    
    [HttpGet]
    public IActionResult GetHello()
    {
        return Ok("GET");
    }
    
    
}