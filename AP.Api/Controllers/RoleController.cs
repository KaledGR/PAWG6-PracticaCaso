using AP.Core.BusinessLogic;
using AP.Data.Models;
using AP.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AP.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleBusiness _roleBusiness;
    private readonly IUserBusiness _userBusiness;

    public RoleController(IRoleBusiness roleBusiness, IUserBusiness userBusiness)
    {
        _roleBusiness = roleBusiness;
        _userBusiness = userBusiness;
    }

    // GET: api/Role
    /// <summary>
    /// Obtiene todos los roles disponibles
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleBusiness.GetAllRolesAsync();
        var roleDtos = roles.Select(r => new RoleDTO
        {
            RoleId = r.RoleId,
            RoleName = r.RoleName,
            Description = r.Description
        });
        return Ok(roleDtos);
    }

    // GET: api/Role/users-with-roles
    /// <summary>
    /// Obtiene todos los usuarios con sus roles asignados
    /// </summary>
    [HttpGet("users-with-roles")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var users = await _userBusiness.GetAllUsersWithRolesAsync();

        var userDtos = users.Select(u => new UserDTO
        {
            UserId = u.UserId,
            Username = u.Username,
            Email = u.Email,
            FullName = u.FullName,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            LastLogin = u.LastLogin,
            RoleName = u.UserRoles.FirstOrDefault()?.Role?.RoleName,
            RoleId = u.UserRoles.FirstOrDefault()?.RoleId
        });

        return Ok(userDtos);
    }

    // POST: api/Role/assign
    /// <summary>
    /// Asigna un rol a un usuario (solo si no tiene rol)
    /// </summary>
    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _roleBusiness.AssignRoleToUserAsync(dto.UserId, dto.RoleId, dto.Description);

        if (!result)
        {
            return BadRequest(new { message = "No se pudo asignar el rol. El usuario ya tiene un rol asignado o no existe." });
        }

        return Ok(new { message = "Rol asignado exitosamente" });
    }

    // PUT: api/Role/update
    /// <summary>
    /// Actualiza el rol de un usuario (cambia de un rol a otro)
    /// </summary>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateRole([FromBody] AssignRoleDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _roleBusiness.UpdateUserRoleAsync(dto.UserId, dto.RoleId, dto.Description);

        if (!result)
        {
            return BadRequest(new { message = "No se pudo actualizar el rol" });
        }

        return Ok(new { message = "Rol actualizado exitosamente" });
    }

    // GET: api/Role/validate/{userId}/{roleId}
    /// <summary>
    /// Valida si se puede asignar un rol a un usuario
    /// </summary>
    [HttpGet("validate/{userId}/{roleId}")]
    public async Task<IActionResult> ValidateRoleAssignment(int userId, int roleId)
    {
        var validation = await _roleBusiness.ValidateRoleAssignmentAsync(userId, roleId);

        return Ok(new
        {
            isValid = validation.IsValid,
            errorMessage = validation.ErrorMessage
        });
    }
}
