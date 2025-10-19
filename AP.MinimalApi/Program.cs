// AP.MinimalApi/Program.cs
using AP.Core.BusinessLogic;
using AP.Data.Models;
using AP.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using TaskModel = AP.Data.Models.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Registrar DbContext
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=TaskDB;Trusted_Connection=True;TrustServerCertificate=True;"));

// ✅ Registrar Business Logic y Repositories
builder.Services.AddScoped<ITaskBusiness, TaskBusiness>();
builder.Services.AddScoped<IRepositoryTask, RepositoryTask>();

// ✅ Response compression para GZip
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// ✅ CORS
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

// ✅ Minimal API Endpoints (READ ONLY)

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

app.Run();