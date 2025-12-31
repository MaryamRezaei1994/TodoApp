using Microsoft.Extensions.Caching.Memory;
using TodoApp.Application.Common;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Services
{
    public class TodoService(ITodoRepository repository, IMemoryCache cache) : ITodoService
    {
        private readonly IMemoryCache _cache = cache;
        private static readonly string ListCacheKey = "TodoList";
        public async Task<ServiceResponse<TodoDto?>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                // Try list cache first
                if (_cache.TryGetValue<IEnumerable<TodoDto>>(ListCacheKey, out var cachedList))
                {
                    var found = cachedList.FirstOrDefault(x => string.Equals(x.Title, name, StringComparison.OrdinalIgnoreCase));
                    if (found is not null) return new ServiceResponse<TodoDto?>(200, found);
                }

                var items = await repository.GetAllAsync(cancellationToken);
                var i = items.FirstOrDefault(x => string.Equals(x.Title, name, StringComparison.OrdinalIgnoreCase));
                if (i is null) return new ServiceResponse<TodoDto?>(400, null, "Item not found");
                var dto = new TodoDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    IsCompleted = i.IsCompleted,
                    CreatedAt = i.CreatedAt ?? DateTime.UtcNow,
                    DueDate = i.DueDate,
                    Priority = i.Priority
                };
                return new ServiceResponse<TodoDto?>(200, dto);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new ServiceResponse<TodoDto?>(500, null, "Internal Server Error" + ex.Message);
            }
        }

        public async Task<ServiceResponse<TodoDto>> CreateAsync(TodoDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await repository.GetAllAsync(cancellationToken);
                if (items.Any(x => string.Equals(x.Title, dto.Title, StringComparison.OrdinalIgnoreCase)))
                    return new ServiceResponse<TodoDto>(400, null, "An item with the same name already exists.");

                var entity = new TodoItem
                {
                    Id = Guid.NewGuid(),
                    Title = dto.Title,
                    Description = dto.Description,
                    IsCompleted = dto.IsCompleted,
                    CreatedAt = DateTime.UtcNow,
                    DueDate = dto.DueDate,
                    Priority = dto.Priority
                };

                await repository.AddAsync(entity, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);

                dto.Id = entity.Id;
                dto.CreatedAt = entity.CreatedAt ?? DateTime.UtcNow;

                // invalidate list cache and set individual item cache
                _cache.Remove(ListCacheKey);
                var itemKey = $"TodoItem:{dto.Id}";
                _cache.Set(itemKey, dto, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

                return new ServiceResponse<TodoDto>(200, dto);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new ServiceResponse<TodoDto>(500, null, "Internal Server Error" + ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var existing = await repository.GetByIdAsync(id, cancellationToken);
                if (existing is null) return new ServiceResponse<bool>(400, false, "Item not found");

                await repository.DeleteAsync(id, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);
                // remove caches related to this item and the list
                _cache.Remove($"TodoItem:{id}");
                _cache.Remove(ListCacheKey);
                return new ServiceResponse<bool>(200, true);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new ServiceResponse<bool>(500, false, "Internal Server Error" + ex.Message);
            }
        }

        public async Task<ServiceResponse<IEnumerable<TodoDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // try cached list
                if (_cache.TryGetValue<IEnumerable<TodoDto>>(ListCacheKey, out var cached))
                {
                    return new ServiceResponse<IEnumerable<TodoDto>>(200, cached);
                }

                var items = await repository.GetAllAsync(cancellationToken);
                var dtos = items.Select(i => new TodoDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    IsCompleted = i.IsCompleted,
                    CreatedAt = i.CreatedAt ?? DateTime.UtcNow,
                    DueDate = i.DueDate,
                    Priority = i.Priority
                }).ToList();

                _cache.Set(ListCacheKey, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

                return new ServiceResponse<IEnumerable<TodoDto>>(200, dtos);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new ServiceResponse<IEnumerable<TodoDto>>(500, Enumerable.Empty<TodoDto>(), "Internal Server Error" + ex.Message);
            }
        }

        public async Task<ServiceResponse<TodoDto?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var itemKey = $"TodoItem:{id}";
                if (_cache.TryGetValue<TodoDto>(itemKey, out var cached))
                {
                    return new ServiceResponse<TodoDto?>(200, cached);
                }

                var i = await repository.GetByIdAsync(id, cancellationToken);
                if (i is null) return new ServiceResponse<TodoDto?>(400, null, "Item not found");
                var dto = new TodoDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    IsCompleted = i.IsCompleted,
                    CreatedAt = i.CreatedAt ?? DateTime.UtcNow,
                    DueDate = i.DueDate,
                    Priority = i.Priority
                };

                _cache.Set(itemKey, dto, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

                return new ServiceResponse<TodoDto?>(200, dto);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new ServiceResponse<TodoDto?>(500, null, "Internal Server Error"+ex.Message);
            }
        }
        
        public async Task<ServiceResponse<TodoDto?>> ToggleCompleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var i = await repository.GetByIdAsync(id, cancellationToken);
                if (i is null) return new ServiceResponse<TodoDto?>(400, null, "Item not found");
                i.IsCompleted = !i.IsCompleted;
                await repository.UpdateAsync(i, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);
                var dto = new TodoDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    IsCompleted = i.IsCompleted,
                    CreatedAt = i.CreatedAt ?? DateTime.UtcNow,
                    DueDate = i.DueDate,
                    Priority = i.Priority
                };
                // update caches
                var itemKey = $"TodoItem:{id}";
                _cache.Set(itemKey, dto, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
                _cache.Remove(ListCacheKey);

                return new ServiceResponse<TodoDto?>(200, dto);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new ServiceResponse<TodoDto?>(500, null, "Internal Server Error"+ex.Message);
            }
        }

        public async Task<ServiceResponse<TodoDto?>> UpdateAsync(Guid id, TodoDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var existing = await repository.GetByIdAsync(id, cancellationToken);
                if (existing is null) return new ServiceResponse<TodoDto?>(400, null, "Item not found");
                var items = await repository.GetAllAsync(cancellationToken);
                if (items.Any(x => x.Id != id && string.Equals(x.Title, dto.Title, StringComparison.OrdinalIgnoreCase)))
                    return new ServiceResponse<TodoDto?>(400, null, "An item with the same name already exists.");

                existing.Title = dto.Title;
                existing.Description = dto.Description;
                existing.DueDate = dto.DueDate;
                existing.Priority = dto.Priority;
                existing.IsCompleted = dto.IsCompleted;

                await repository.UpdateAsync(existing, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);

                dto.Id = existing.Id;
                dto.CreatedAt = existing.CreatedAt ?? DateTime.UtcNow;

                // update caches: item and invalidate list
                var itemKey = $"TodoItem:{id}";
                _cache.Set(itemKey, dto, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
                _cache.Remove(ListCacheKey);

                return new ServiceResponse<TodoDto?>(200, dto);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new ServiceResponse<TodoDto?>(500, null, "Internal Server Error" + ex.Message);
            }
        }
    }
}

