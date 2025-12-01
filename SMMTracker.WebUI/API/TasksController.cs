using Microsoft.AspNetCore.Mvc;
using SMMTracker.Application.Services;
using SMMTracker.Application.Dtos;

namespace BlazorApp1.API;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        var taskId = await _taskService.CreateTaskAsync(createTaskDto);
        return Ok(new { TaskId = taskId });
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> RemoveTask(int taskId)
    {
        try
        {
            await _taskService.RemoveTaskAsync(taskId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}