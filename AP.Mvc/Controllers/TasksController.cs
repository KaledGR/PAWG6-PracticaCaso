// AP.Mvc/Controllers/TasksController.cs
using Microsoft.AspNetCore.Mvc;
using TaskModel = AP.Data.Models.Task;
using PAW3.Mvc.ServiceLocator;
using AP.Models.DTOs;

namespace AP.Mvc.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskApiClient _apiClient;
        private readonly ILogger<TasksController> _logger;

        public TasksController(IServiceTaskService apiClient, ILogger<TasksController> logger)
        {
            _apiClient = (TaskApiClient?)apiClient;
            _logger = logger;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            try
            {
                var tasks = await _apiClient.GetDataAsync<TaskDTO>("task");
                return View(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la lista de tareas");
                TempData["Error"] = "No se pudieron cargar las tareas. Por favor, intente más tarde.";
                return View(Enumerable.Empty<TaskModel>());
            }
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                var task = await _apiClient.GetByIdAsync<TaskDTO>("task", id.Value);

                if (task == null)
                {
                    TempData["Error"] = "Tarea no encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los detalles de la tarea {TaskId}", id);
                TempData["Error"] = "No se pudieron cargar los detalles de la tarea.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View(new TaskModel
            {
                Status = "Pendiente",
                CreatedAt = DateTime.Now
            });
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskModel task)
        {
            if (!ModelState.IsValid)
                return View(task);

            try
            {
                task.CreatedAt = DateTime.Now;
                await _apiClient.CreateAsync("task", task);

                TempData["Success"] = "Tarea creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la tarea");
                ModelState.AddModelError("", "No se pudo crear la tarea. Por favor, intente nuevamente.");
                return View(task);
            }
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                var task = await _apiClient.GetByIdAsync<TaskDTO>("task", id.Value);
                if (task == null)
                {
                    TempData["Error"] = "Tarea no encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la tarea {TaskId} para editar", id);
                TempData["Error"] = "No se pudo cargar la tarea para editar.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskModel task)
        {
            if (id != task.Id)
            {
                TempData["Error"] = "ID de tarea no coincide.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
                return View(task);

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
                ModelState.AddModelError("", "No se pudo actualizar la tarea. Por favor, intente nuevamente.");
                return View(task);
            }
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                var task = await _apiClient.GetByIdAsync<TaskDTO>("task", id.Value);
                if (task == null)
                {
                    TempData["Error"] = "Tarea no encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                return View(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la tarea {TaskId} para eliminar", id);
                TempData["Error"] = "No se pudo cargar la tarea para eliminar.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
