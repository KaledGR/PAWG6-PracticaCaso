using Microsoft.AspNetCore.Mvc;
using AP.Core.BusinessLogic;
using AP.Data.Models;
using TaskModel = AP.Data.Models.Task;

namespace PAW3.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskApiController(ITaskBusiness taskBusiness) : ControllerBase
{
    // GET: api/TaskApi
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var task = await taskBusiness.GetTask(id: null);
        return Ok(task);
    }

    // GET: api/TaskApi/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var task = await taskBusiness.GetTask(id);
        if (task == null || !task.Any())
            return NotFound();
        return Ok(task.First());
    }

    // POST: api/TaskApi
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TaskModel task)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await taskBusiness.SaveTaskAsync(task);
        return result ? Ok(task) : BadRequest("Failed to create task");
    }

    // PUT: api/TaskApi/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] TaskModel task)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        task.Id = id;
        var result = await taskBusiness.SaveTaskAsync(task);
        return result ? Ok(task) : BadRequest("Failed to update task");
    }

    // DELETE: api/TaskApi/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await taskBusiness.DeleteTaskAsync(id);
        return result ? Ok() : NotFound();
    }
}