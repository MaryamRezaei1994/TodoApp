using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _db;

        public TodoRepository(TodoDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(TodoItem item, CancellationToken cancellationToken = default)
        {
            await _db.Todos.AddAsync(item, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _db.Todos.FindAsync(new object[] { id }, cancellationToken);
            if (entity is null) return;
            _db.Todos.Remove(entity);
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Todos.AsNoTracking().OrderByDescending(t => t.CreatedAt).ToListAsync(cancellationToken);
        }

        public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.Todos.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(TodoItem item, CancellationToken cancellationToken = default)
        {
            _db.Todos.Update(item);
            await Task.CompletedTask;
        }
    }
}

