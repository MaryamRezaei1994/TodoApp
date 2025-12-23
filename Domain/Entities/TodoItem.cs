using System.ComponentModel.DataAnnotations;

namespace TodoApp.Domain.Entities
{
    public class TodoItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime? CreatedAt { get; set; } 

        public DateTime? DueDate { get; set; }

        // 1 = low, 2 = medium, 3 = high
        public int Priority { get; set; } = 2;
    }
}