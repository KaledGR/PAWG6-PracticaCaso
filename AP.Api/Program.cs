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

builder.Services.AddDbContext<TaskDbContext>();




// Business Logic registrations

builder.Services.AddScoped<ITaskBusiness, TaskBusiness>();
builder.Services.AddScoped<IAuthBusiness, AuthBusiness>();           // ✅ NUEVO
builder.Services.AddScoped<IUserBusiness, UserBusiness>();           // ✅ NUEVO
builder.Services.AddScoped<IRoleBusiness, RoleBusiness>();           // ✅ NUEVO
builder.Services.AddScoped<IApprovalBusiness, ApprovalBusiness>();   // ✅ NUEVO

// Repository registrations

builder.Services.AddScoped<IRepositoryTask, RepositoryTask>();
builder.Services.AddScoped<IRepositoryUser, RepositoryUser>();        // ✅ NUEVO
builder.Services.AddScoped<IRepositoryRole, RepositoryRole>();        // ✅ NUEVO
builder.Services.AddScoped<IRepositoryUserRole, RepositoryUserRole>(); // ✅ NUEVO




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