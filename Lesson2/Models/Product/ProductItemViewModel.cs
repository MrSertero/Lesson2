namespace Lesson2.Models.Product
{
    /// <summary>
    /// Використовується для передачі даних про продукт із контролера до View продуктів
    /// </summary>
    public class ProductItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = String.Empty;
        public List<string>? Images { get; set; }
    }
}