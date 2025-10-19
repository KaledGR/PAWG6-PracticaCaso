using AP.Data.Repositories;

using TaskModel = AP.Data.Models.Task;



namespace AP.Core.BusinessLogic
{
    public interface ITaskBusiness
    {
        /// <summary>
        /// Deletes the product associated with the product id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteTaskAsync(int id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<TaskModel>> GetTask(int? id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<bool> SaveTaskAsync(TaskModel task);
    }

    public class TaskBusiness(IRepositoryTask repositoryTask) : ITaskBusiness
    {
        /// </inheritdoc>
        public async Task<bool> SaveTaskAsync(TaskModel people)
        {

            return await repositoryTask.UpdateAsync(people);
        }

        /// </inheritdoc>
        public async Task<bool> DeleteTaskAsync(int id)
        {
            var people = await repositoryTask.FindAsync(id);
            return await repositoryTask.DeleteAsync(people);
        }

        /// </inheritdoc>
        public async Task<IEnumerable<TaskModel>> GetTask(int? id)
        {
            return id == null
                ? await repositoryTask.ReadAsync()
                : [await repositoryTask.FindAsync((int)id)];
        }

     
    }
}
