using Microsoft.AspNetCore.Mvc;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;

namespace SMMTracker.WebUI.API;

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

    [HttpPost("{taskId}/move-to-review")]
    public async Task<IActionResult> MoveTaskToReview(int taskId)
    {
        try
        {
            await _taskService.MoveTaskToReviewAsync(taskId);
            return Ok();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{taskId}/move-to-done")]
    public async Task<IActionResult> MoveTaskToDone(int taskId)
    {
        try
        {
            await _taskService.MoveTaskToDoneAsync(taskId);
            return Ok();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
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