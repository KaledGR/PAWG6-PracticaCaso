using AP.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using AP.Data.Models;
using TaskModel = AP.Data.Models.Task;

namespace AP.Data.Repositories;

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
}

public class RepositoryTask : RepositoryBase<TaskModel>, IRepositoryTask
{
    public async new Task<bool> ExistsAsync(TaskModel entity)
    {
        return await DbContext.Tasks.AnyAsync(x => x.Id == entity.Id);
    }
}