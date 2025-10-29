using Microsoft.AspNetCore.Mvc;
using TaskModel = AP.Data.Models.Task;
using PAW3.Mvc.ServiceLocator;
using AP.Models.DTOs;
using AP.Mvc.Helpers;
using System.Text.Json;
using AP.Architecture;

namespace AP.Mvc.Controllers
{
    public class TasksController : Controller
    {
        private readonly IServiceTaskService _apiClient;
        private readonly ILogger<TasksController> _logger;
        private readonly IRestProvider _restProvider;
        private const string ApprovalApiUrl = "https://localhost:7068/api/Approval";

        public TasksController(
            IServiceTaskService apiClient,
            ILogger<TasksController> logger,
            IRestProvider restProvider)
        {
            _apiClient = apiClient;
            _logger = logger;
            _restProvider = restProvider;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            // Verificar autenticación
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // ✅ Obtener solo tareas aprobadas o denegadas (no las null)
                var response = await _restProvider.GetAsync($"{ApprovalApiUrl}/approved-denied", null);
                var tasks = JsonSerializer.Deserialize<IEnumerable<TaskDTO>>(response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Pasar información del usuario actual a la vista
                ViewBag.CurrentUser = SessionHelper.GetUser(HttpContext.Session);

                return View(tasks ?? Enumerable.Empty<TaskDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de tareas");
                TempData["Error"] = "No se pudieron cargar las tareas. Por favor, intente más tarde.";
                return View(Enumerable.Empty<TaskDTO>());
            }
        }

        // GET: Tasks/GetTaskData/5
        [HttpGet]
        public async Task<IActionResult> GetTaskData(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return Unauthorized();
            }

            try
            {
                var task = await _apiClient.GetByIdAsync<TaskDTO>("task", id);
                if (task == null)
                {
                    return NotFound();
                }
                return Json(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos de la tarea {TaskId}", id);
                return StatusCode(500, "Error al cargar los datos de la tarea");
            }
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskModel task)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            try
            {
                // ✅ IMPORTANTE: Crear siempre con Approved = null
                task.Approved = null;
                task.CreatedAt = DateTime.UtcNow;

                await _apiClient.CreateAsync("task", task);

                TempData["Success"] = "Tarea creada exitosamente. Pendiente de aprobación.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la tarea");
                TempData["Error"] = "No se pudo crear la tarea. Por favor, intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Tasks/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskModel task)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id != task.Id)
            {
                TempData["Error"] = "ID de tarea no coincide.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            try
            {
                var success = await _apiClient.UpdateAsync("task", id, task);

                if (!success)
                {
                    TempData["Error"] = "No se pudo actualizar la tarea.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Tarea actualizada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la tarea {TaskId}", id);
                TempData["Error"] = "No se pudo actualizar la tarea. Por favor, intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Tasks/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var success = await _apiClient.DeleteAsync("task", id);

                if (!success)
                {
                    TempData["Error"] = "No se pudo eliminar la tarea.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Tarea eliminada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la tarea {TaskId}", id);
                TempData["Error"] = "No se pudo eliminar la tarea. Por favor, intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}