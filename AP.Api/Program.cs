using AP.Core.BusinessLogic;
using AP.Core.BusinessLogic;
using AP.Data.Models;
using AP.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Business Logic registrations

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=TaskDB;Trusted_Connection=True;TrustServerCertificate=True;"));


builder.Services.AddScoped<ITaskBusiness, TaskBusiness>();


// Repository registrations


builder.Services.AddScoped<IRepositoryTask, RepositoryTask>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();