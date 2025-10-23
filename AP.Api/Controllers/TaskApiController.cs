using Microsoft.AspNetCore.Mvc;
using AP.Core.BusinessLogic;
using AP.Data.Models;
using TaskModel = AP.Data.Models.Task;

namespace AP.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskApiController : ControllerBase
{
    private readonly ITaskBusiness _taskBusiness;

    public TaskApiController(ITaskBusiness taskBusiness)
    {
        _taskBusiness = taskBusiness;
    }

    // POST: api/TaskApi
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TaskModel task)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // ✅ IMPORTANTE: Crear siempre con Approved = null
        task.Approved = null;
        task.CreatedAt = DateTime.UtcNow;

        var result = await _taskBusiness.SaveTaskAsync(task);
        return result ? Ok(task) : BadRequest("Failed to create task");
    }

    // PUT: api/TaskApi/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] TaskModel task)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        task.Id = id;
        var result = await _taskBusiness.SaveTaskAsync(task);
        return result ? Ok(task) : BadRequest("Failed to update task");
    }

    // DELETE: api/TaskApi/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _taskBusiness.DeleteTaskAsync(id);
        return result ? Ok() : NotFound();
    }
}