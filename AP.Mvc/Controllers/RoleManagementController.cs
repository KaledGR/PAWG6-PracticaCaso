using Microsoft.AspNetCore.Mvc;
using AP.Models.DTOs;
using AP.Architecture;
using AP.Architecture.Providers;
using AP.Mvc.Helpers;
using System.Text.Json;

namespace AP.Mvc.Controllers
{
    public class RoleManagementController : Controller
    {
        private readonly IRestProvider _restProvider;
        private readonly ILogger<RoleManagementController> _logger;
        private const string RoleApiUrl = "https://localhost:7068/api/Role";

        public RoleManagementController(IRestProvider restProvider, ILogger<RoleManagementController> logger)
        {
            _restProvider = restProvider;
            _logger = logger;
        }

        // GET: RoleManagement
        public async Task<IActionResult> Index()
        {
            // Verificar autenticación
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Verificar que sea Manager o Admin
            if (!SessionHelper.IsManagerOrAdmin(HttpContext.Session))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            try
            {
                // Obtener usuarios con roles
                var usersResponse = await _restProvider.GetAsync($"{RoleApiUrl}/users-with-roles", null);
                var users = JsonSerializer.Deserialize<IEnumerable<UserDTO>>(usersResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Obtener todos los roles disponibles
                var rolesResponse = await _restProvider.GetAsync(RoleApiUrl, null);
                var roles = JsonSerializer.Deserialize<IEnumerable<RoleDTO>>(rolesResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                ViewBag.Roles = roles ?? Enumerable.Empty<RoleDTO>();
                return View(users ?? Enumerable.Empty<UserDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar usuarios y roles");
                TempData["Error"] = "Error al cargar los datos";
                ViewBag.Roles = Enumerable.Empty<RoleDTO>();
                return View(Enumerable.Empty<UserDTO>());
            }
        }

        // POST: RoleManagement/AssignRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(int userId, int roleId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!SessionHelper.IsManagerOrAdmin(HttpContext.Session))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            try
            {
                var dto = new AssignRoleDTO
                {
                    UserId = userId,
                    RoleId = roleId,
                    Description = "Asignado por Manager"
                };

                var json = JsonSerializer.Serialize(dto);
                var response = await _restProvider.PostAsync($"{RoleApiUrl}/assign", json);

                var result = JsonSerializer.Deserialize<JsonElement>(response);
                var message = result.GetProperty("message").GetString();

                TempData["Success"] = message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar rol");
                TempData["Error"] = "Error al asignar el rol. El usuario ya tiene un rol asignado.";
            }

            return RedirectToAction("Index");
        }

        // POST: RoleManagement/UpdateRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(int userId, int roleId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!SessionHelper.IsManagerOrAdmin(HttpContext.Session))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            try
            {
                var dto = new AssignRoleDTO
                {
                    UserId = userId,
                    RoleId = roleId,
                    Description = "Actualizado por Manager"
                };

                var json = JsonSerializer.Serialize(dto);
                var response = await _restProvider.PutAsync($"{RoleApiUrl}/update", null, json);

                var result = JsonSerializer.Deserialize<JsonElement>(response);
                var message = result.GetProperty("message").GetString();

                TempData["Success"] = message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol");
                TempData["Error"] = "Error al actualizar el rol";
            }

            return RedirectToAction("Index");
        }
    }
}