using TodoApp.Application.DTOs;
using TodoApp.Application.Common;

namespace TodoApp.Application.Services
{
    public interface ITodoService
    {
        Task<ServiceResponse<IEnumerable<TodoDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<TodoDto?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<TodoDto?>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<ServiceResponse<TodoDto>> CreateAsync(TodoDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResponse<TodoDto?>> UpdateAsync(Guid id, TodoDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<TodoDto?>> ToggleCompleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

