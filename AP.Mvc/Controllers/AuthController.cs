using Microsoft.AspNetCore.Mvc;
using AP.Models.DTOs;
using AP.Architecture;
using AP.Architecture.Providers;
using AP.Mvc.Helpers;
using System.Text.Json;

namespace AP.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IRestProvider _restProvider;
        private readonly ILogger<AuthController> _logger;
        private const string LoginApiUrl = "https://localhost:7064/api/auth/login";

        public AuthController(IRestProvider restProvider, ILogger<AuthController> logger)
        {
            _restProvider = restProvider;
            _logger = logger;
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir a Home
            if (SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Index", "Tasks");
            }

            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Llamar al MinimalAPI para login
                var json = JsonSerializer.Serialize(model);
                var response = await _restProvider.PostAsync(LoginApiUrl, json);

                var loginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loginResponse == null || !loginResponse.Success)
                {
                    TempData["Error"] = loginResponse?.Message ?? "Error al iniciar sesión";
                    return View(model);
                }

                // Guardar usuario en sesión
                SessionHelper.SetUser(HttpContext.Session, loginResponse.User!);

                TempData["Success"] = $"Bienvenido, {loginResponse.User!.Username}!";
                return RedirectToAction("Index", "Tasks");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login");
                TempData["Error"] = "Error al conectar con el servidor";
                return View(model);
            }
        }

        // GET: Auth/Logout
        public IActionResult Logout()
        {
            SessionHelper.ClearSession(HttpContext.Session);
            TempData["Success"] = "Sesión cerrada exitosamente";
            return RedirectToAction("Login");
        }

        // GET: Auth/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}