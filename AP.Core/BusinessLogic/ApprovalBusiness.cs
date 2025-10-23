// AP.Core/BusinessLogic/ApprovalBusiness.cs
using AP.Core.BusinessLogic;
using AP.Data.Models;
using AP.Data.Repositories;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using TaskModel = AP.Data.Models.Task;

namespace AP.Core.BusinessLogic
{
    public interface IApprovalBusiness
    {
        /// <summary>
        /// Obtiene todas las tareas para el flujo de aprobación (ordenadas)
        /// </summary>
        Task<IEnumerable<TaskModel>> GetAllTasksForApprovalAsync();

        /// <summary>
        /// Aprueba una tarea
        /// </summary>
        Task<(bool Success, string Message)> ApproveTaskAsync(int taskId, int approvedBy);

        /// <summary>
        /// Deniega una tarea
        /// </summary>
        Task<(bool Success, string Message)> DenyTaskAsync(int taskId, int approvedBy);

        /// <summary>
        /// Verifica si una tarea puede ser aprobada después de ser denegada
        /// </summary>
        Task<bool> CanBeApprovedAfterDenialAsync(int taskId);

        /// <summary>
        /// Obtiene solo las tareas aprobadas o denegadas (para vista normal)
        /// </summary>
        Task<IEnumerable<TaskModel>> GetApprovedOrDeniedTasksAsync();
    }

    public class ApprovalBusiness : IApprovalBusiness
    {
        private readonly IRepositoryTask _taskRepository;
        private readonly IRepositoryUserRole _userRoleRepository;

        public ApprovalBusiness(IRepositoryTask taskRepository, IRepositoryUserRole userRoleRepository)
        {
            _taskRepository = taskRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<IEnumerable<TaskModel>> GetAllTasksForApprovalAsync()
        {
            var allTasks = await _taskRepository.ReadAsync();

            // Ordenar: null primero, luego aprobadas, luego denegadas
            return allTasks.OrderBy(t => t.Approved.HasValue ? (t.Approved.Value ? 1 : 2) : 0)
                          .ThenByDescending(t => t.CreatedAt);
        }

        public async Task<(bool Success, string Message)> ApproveTaskAsync(int taskId, int approvedBy)
        {
            // Verificar que quien aprueba sea Manager
            var isManager = await _userRoleRepository.HasRoleAsync(approvedBy, "Manager");
            if (!isManager)
            {
                return (false, "Solo los Managers pueden aprobar tareas");
            }

            var task = await _taskRepository.FindAsync(taskId);
            if (task == null)
            {
                return (false, "Tarea no encontrada");
            }

            // Si está denegada, verificar restricción de 24 horas
            if (task.Approved == false)
            {
                var canBeApproved = await CanBeApprovedAfterDenialAsync(taskId);
                if (!canBeApproved)
                {
                    return (false, "No se puede aprobar una tarea denegada con más de 24 horas de antigüedad");
                }
            }

            task.Approved = true;
            var result = await _taskRepository.UpdateAsync(task);

            return result
                ? (true, "Tarea aprobada exitosamente")
                : (false, "Error al aprobar la tarea");
        }

        public async Task<(bool Success, string Message)> DenyTaskAsync(int taskId, int approvedBy)
        {
            // Verificar que quien deniega sea Manager
            var isManager = await _userRoleRepository.HasRoleAsync(approvedBy, "Manager");
            if (!isManager)
            {
                return (false, "Solo los Managers pueden denegar tareas");
            }

            var task = await _taskRepository.FindAsync(taskId);
            if (task == null)
            {
                return (false, "Tarea no encontrada");
            }

            task.Approved = false;
            var result = await _taskRepository.UpdateAsync(task);

            return result
                ? (true, "Tarea denegada exitosamente")
                : (false, "Error al denegar la tarea");
        }

        public async Task<bool> CanBeApprovedAfterDenialAsync(int taskId)
        {
            var task = await _taskRepository.FindAsync(taskId);
            if (task == null) return false;

            // Si no está denegada, puede ser aprobada
            if (task.Approved != false) return true;

            // Si está denegada, verificar que tenga menos de 24 horas
            if (!task.CreatedAt.HasValue) return false;

            var hoursSinceCreation = (DateTime.UtcNow - task.CreatedAt.Value).TotalHours;
            return hoursSinceCreation < 24;
        }

        public async Task<IEnumerable<TaskModel>> GetApprovedOrDeniedTasksAsync()
        {
            var allTasks = await _taskRepository.ReadAsync();

            // Solo mostrar tareas aprobadas o denegadas (no las null)
            // Ordenar con aprobadas primero
            return allTasks
                .Where(t => t.Approved.HasValue)
                .OrderByDescending(t => t.Approved)  // true primero (aprobadas)
                .ThenByDescending(t => t.CreatedAt);
        }
    }
}