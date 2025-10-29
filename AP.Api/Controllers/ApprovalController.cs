using Microsoft.AspNetCore.Mvc;
using AP.Core.BusinessLogic;
using AP.Models.DTOs;
using TaskModel = AP.Data.Models.Task;

namespace AP.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApprovalController : ControllerBase
{
    private readonly IApprovalBusiness _approvalBusiness;

    public ApprovalController(IApprovalBusiness approvalBusiness)
    {
        _approvalBusiness = approvalBusiness;
    }

    // GET: api/Approval/all
    /// <summary>
    /// Obtiene todas las tareas ordenadas (null, aprobadas, denegadas)
    /// Solo para Managers/Admin
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllForApproval()
    {
        var tasks = await _approvalBusiness.GetAllTasksForApprovalAsync();
        return Ok(tasks);
    }

    // GET: api/Approval/approved-denied
    /// <summary>
    /// Obtiene solo tareas aprobadas o denegadas (para usuarios normales)
    /// </summary>
    [HttpGet("approved-denied")]
    public async Task<IActionResult> GetApprovedOrDenied()
    {
        var tasks = await _approvalBusiness.GetApprovedOrDeniedTasksAsync();
        return Ok(tasks);
    }

    // POST: api/Approval/approve
    /// <summary>
    /// Aprueba una tarea
    /// </summary>
    [HttpPost("approve")]
    public async Task<IActionResult> ApproveTask([FromBody] ApproveTaskDTO dto)
    {
        var result = await _approvalBusiness.ApproveTaskAsync(dto.TaskId, dto.ApprovedBy);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }

    // POST: api/Approval/deny
    /// <summary>
    /// Deniega una tarea
    /// </summary>
    [HttpPost("deny")]
    public async Task<IActionResult> DenyTask([FromBody] ApproveTaskDTO dto)
    {
        var result = await _approvalBusiness.DenyTaskAsync(dto.TaskId, dto.ApprovedBy);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }

    // GET: api/Approval/can-approve/{taskId}
    /// <summary>
    /// Verifica si una tarea puede ser aprobada (validación 24hrs)
    /// </summary>
    [HttpGet("can-approve/{taskId}")]
    public async Task<IActionResult> CanBeApproved(int taskId)
    {
        var canApprove = await _approvalBusiness.CanBeApprovedAfterDenialAsync(taskId);
        return Ok(new { canApprove });
    }
}