using AP.Architecture;
using PAW3.Mvc.ServiceLocator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// ✅ AGREGAR: Habilitar sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registrar RestProvider y TaskApiClient
builder.Services.AddSingleton<IRestProvider, RestProvider>();
builder.Services.AddScoped<IServiceTaskService, TaskApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ✅ AGREGAR: Usar sesiones
app.UseSession();

app.UseRouting();
app.UseAuthorization();

// ✅ CAMBIAR: Ruta por defecto a Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();