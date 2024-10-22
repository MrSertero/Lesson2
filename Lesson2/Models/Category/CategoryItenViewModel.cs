using Lesson2.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Lesson2.Models.Category
{
    /// <summary>
    /// Використовується для передачі даних між контролером і View
    /// </summary>
    public class CategoryItenViewModel
    {
        public int Id {  get; set; }

        public string Name { get; set; } = string.Empty;
        public string Image { get; set; }

        public string? Description { get; set; }
    }
}
