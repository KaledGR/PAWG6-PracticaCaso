// AP.MinimalApi/Program.cs
using AP.Core.BusinessLogic;
using AP.Data.Models;
using AP.Data.Repositories;
using AP.Models.DTOs;

using Microsoft.EntityFrameworkCore;
using TaskModel = AP.Data.Models.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar DbContext
builder.Services.AddDbContext<TaskDbContext>();

// Registrar Business Logic y Repositories
builder.Services.AddScoped<ITaskBusiness, TaskBusiness>();
builder.Services.AddScoped<IRepositoryTask, RepositoryTask>();
builder.Services.AddScoped<IRepositoryUser, RepositoryUser>();
builder.Services.AddScoped<IRepositoryRole, RepositoryRole>();
builder.Services.AddScoped<IRepositoryUserRole, RepositoryUserRole>();

// Asegúrate de registrar los servicios que faltan
builder.Services.AddScoped<IAuthBusiness, AuthBusiness>(); 
builder.Services.AddScoped<IUserBusiness, UserBusiness>(); 

// Response compression para GZip
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseCors("AllowAll");

// Minimal API Endpoints (READ ONLY)

// GET: Obtener todas las tareas
app.MapGet("/api/TaskApi", async (ITaskBusiness taskBusiness) =>
{
    var tasks = await taskBusiness.GetTask(null);
    return Results.Ok(tasks);
})
.WithName("GetAllTasks")
.WithOpenApi()
.WithTags("Tasks");

// GET: Obtener una tarea por ID
app.MapGet("/api/TaskApi/{id:int}", async (int id, ITaskBusiness taskBusiness) =>
{
    var tasks = await taskBusiness.GetTask(id);
    if (tasks == null || !tasks.Any())
        return Results.NotFound(new { message = $"Tarea con ID {id} no encontrada" });

    return Results.Ok(tasks.First());
})
.WithName("GetTaskById")
.WithOpenApi()
.WithTags("Tasks");

// GET: Obtener tareas por estado (endpoint adicional)
app.MapGet("/api/TaskApi/status/{status}", async (string status, IRepositoryTask repository) =>
{
    var allTasks = await repository.ReadAsync();
    var tasksByStatus = allTasks.Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
    return Results.Ok(tasksByStatus);
})
.WithName("GetTasksByStatus")
.WithOpenApi()
.WithTags("Tasks");

// ============================================
// LOGIN ENDPOINT (Minimal API)
// ============================================
app.MapPost("/api/auth/login", async (
    LoginDTO loginDto,
    IAuthBusiness authBusiness,
    IUserBusiness userBusiness) =>
{
    try
    {
        // Validar que el email exista
        var user = await authBusiness.ValidateUserAsync(loginDto.Email, loginDto.Password);

        if (user == null)
        {
            return Results.Json(new LoginResponseDTO
            {
                Success = false,
                Message = "Email no encontrado o usuario inactivo"
            }, statusCode: 401);
        }

        // Obtener usuario con roles
        var userWithRoles = await authBusiness.GetUserWithRolesAsync(loginDto.Email);

        // Mapear a DTO
        var userDto = new UserDTO
        {
            UserId = userWithRoles.UserId,
            Username = userWithRoles.Username,
            Email = userWithRoles.Email,
            FullName = userWithRoles.FullName,
            IsActive = userWithRoles.IsActive,
            CreatedAt = userWithRoles.CreatedAt,
            LastLogin = userWithRoles.LastLogin,
            RoleName = userWithRoles.UserRoles.FirstOrDefault()?.Role?.RoleName,
            RoleId = userWithRoles.UserRoles.FirstOrDefault()?.RoleId
        };

        return Results.Json(new LoginResponseDTO
        {
            Success = true,
            Message = "Login exitoso",
            User = userDto
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new LoginResponseDTO
        {
            Success = false,
            Message = $"Error en el servidor: {ex.Message}"
        }, statusCode: 500);
    }
})
.WithName("Login")
.WithOpenApi()
.WithTags("Authentication");

app.Run();