using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCasoDeEstudioContext<CasoDeEsudio>(opt => opt.UseInMemoryDatabase("TaskList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/task", async (CasoDeEstudio db) =>
    await db.Tasks.ToListAsync());

app.MapGet("/task/complete", async (CasoDeEstudio db) =>
    await db.Task.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/task/{id}", async (int id, CasoDeEstudio db) =>
    await db.Tasks.FindAsync(id)
        is Task task
            ? Results.Ok(task)
            : Results.NotFound());




app.Run();