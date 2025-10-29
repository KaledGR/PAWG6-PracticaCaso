using Microsoft.AspNetCore.Mvc;
using AP.Models.DTOs;
using AP.Architecture;
using AP.Architecture.Providers;
using AP.Mvc.Helpers;
using System.Text.Json;

namespace AP.Mvc.Controllers
{
    public class ApprovalsController : Controller
    {
        private readonly IRestProvider _restProvider;
        private readonly ILogger<ApprovalsController> _logger;
        private const string ApprovalApiUrl = "https://localhost:7068/api/Approval";

        public ApprovalsController(IRestProvider restProvider, ILogger<ApprovalsController> logger)
        {
            _restProvider = restProvider;
            _logger = logger;
        }

        // GET: Approvals
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
                // Obtener todas las tareas para aprobación
                var response = await _restProvider.GetAsync($"{ApprovalApiUrl}/all", null);
                var tasks = JsonSerializer.Deserialize<IEnumerable<TaskDTO>>(response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(tasks ?? Enumerable.Empty<TaskDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar tareas para aprobación");
                TempData["Error"] = "Error al cargar las tareas";
                return View(Enumerable.Empty<TaskDTO>());
            }
        }

        // POST: Approvals/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int taskId)
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
                var currentUser = SessionHelper.GetUser(HttpContext.Session);
                var dto = new ApproveTaskDTO
                {
                    TaskId = taskId,
                    Approved = true,
                    ApprovedBy = currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(dto);
                var response = await _restProvider.PostAsync($"{ApprovalApiUrl}/approve", json);

                var result = JsonSerializer.Deserialize<JsonElement>(response);
                var message = result.GetProperty("message").GetString();

                TempData["Success"] = message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aprobar tarea {TaskId}", taskId);
                TempData["Error"] = "Error al aprobar la tarea";
            }

            return RedirectToAction("Index");
        }

        // POST: Approvals/Deny
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(int taskId)
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
                var currentUser = SessionHelper.GetUser(HttpContext.Session);
                var dto = new ApproveTaskDTO
                {
                    TaskId = taskId,
                    Approved = false,
                    ApprovedBy = currentUser!.UserId
                };

                var json = JsonSerializer.Serialize(dto);
                var response = await _restProvider.PostAsync($"{ApprovalApiUrl}/deny", json);

                var result = JsonSerializer.Deserialize<JsonElement>(response);
                var message = result.GetProperty("message").GetString();

                TempData["Success"] = message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al denegar tarea {TaskId}", taskId);
                TempData["Error"] = "Error al denegar la tarea";
            }

            return RedirectToAction("Index");
        }
    }
}