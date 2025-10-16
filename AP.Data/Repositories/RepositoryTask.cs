using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TaskModel = AP.Data.Models.Task;

namespace AP.Data.Repositories
{
    public interface IRepositoryTask
    {

        Task<bool> UpsertAsync(TaskModel entity, bool isUpdating);
        Task<bool> CreateAsync(TaskModel entity);
        Task<bool> DeleteAsync(TaskModel entity);
        Task<IEnumerable<TaskModel>> ReadAsync();
        Task<TaskModel> FindAsync(int id);
        Task<bool> UpdateAsync(TaskModel entity);
        Task<bool> UpdateManyAsync(IEnumerable<TaskModel> entities);
        Task<bool> ExistsAsync(TaskModel entity);
        Task<bool> CheckBeforeSavingAsync(TaskModel entity);

    }

    public class RepositoryTaskModel : RepositoryBase<TaskModel>, IRepositoryTask
    {
        public async Task<bool> CheckBeforeSavingAsync(TaskModel entity)
        {
            var exists = await ExistsAsync(entity);
            if (exists)
            {
                // algo mas 
            }

            return await UpsertAsync(entity, exists);
        }

        public async new Task<bool> ExistsAsync(TaskModel entity)
        {
            return await DbContext.TaskModel.AnyAsync(x => x.Id == entity.Id);
        }
    }
}
