using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(TodoItem item, CancellationToken cancellationToken = default);
        Task UpdateAsync(TodoItem item, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}