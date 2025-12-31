using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.DTOs;
using TodoApp.Application.Services;

namespace TodoApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _service;

        public TodosController(ITodoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _service.GetAllAsync();
            if (res.StatusCode == 500) return StatusCode(500, res.Message ?? "internalservererror");
            if (res.StatusCode == 400) return BadRequest(new { message = res.Message });
            return Ok(res.Content);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var res = await _service.GetByIdAsync(id);
            if (res.StatusCode == 500) return StatusCode(500, res.Message ?? "internalservererror");
            if (res.StatusCode == 400) return BadRequest(new { message = res.Message });
            return Ok(res.Content);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoDto dto)
        {
            var res = await _service.CreateAsync(dto);
            if (res.StatusCode == 500) return StatusCode(500, res.Message ?? "internalservererror");
            if (res.StatusCode == 400) return BadRequest(new { message = res.Message });
            return Ok(res.Content);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TodoDto dto)
        {
            var res = await _service.UpdateAsync(id, dto);
            if (res.StatusCode == 500) return StatusCode(500, res.Message ?? "internalservererror");
            if (res.StatusCode == 400) return BadRequest(new { message = res.Message });
            return Ok(res.Content);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _service.DeleteAsync(id);
            if (res.StatusCode == 500) return StatusCode(500, res.Message ?? "internalservererror");
            if (res.StatusCode == 400) return BadRequest(new { message = res.Message });
            return Ok(res.Content);
        }

        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> ToggleComplete(Guid id)
        {
            var res = await _service.ToggleCompleteAsync(id);
            if (res.StatusCode == 500) return StatusCode(500, res.Message ?? "internalservererror");
            if (res.StatusCode == 400) return BadRequest(new { message = res.Message });
            return Ok(res.Content);
        }

        [HttpGet("byname/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var res = await _service.GetByNameAsync(name);
            if (res.StatusCode == 500) return StatusCode(500, res.Message ?? "internalservererror");
            if (res.StatusCode == 400) return BadRequest(new { message = res.Message });
            return Ok(res.Content);
        }
    }
}

