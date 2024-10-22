using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lesson2.Models.Product
{
    /// <summary>
    /// Використовується для передачі даних із форми редагування продукту до контролера
    /// </summary>
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public List<string> ExistingImages { get; set; }
        public List<IFormFile> NewPhotos { get; set; }
        public List<string> ImagesToDelete { get; set; } = new List<string>();

        public int SelectedCategoryId { get; set; }
        public SelectList CategoryList { get; set; }
    }
}
