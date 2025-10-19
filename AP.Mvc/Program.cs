// AP.Mvc/Program.cs
using AP.Architecture;
using PAW3.Mvc.ServiceLocator;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Registrar RestProvider y TaskApiClient
builder.Services.AddSingleton<IRestProvider, RestProvider>();
builder.Services.AddScoped<IServiceTaskService, TaskApiClient>();

// Configuración adicional
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Index}/{id?}");

app.Run();