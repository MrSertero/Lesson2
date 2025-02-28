﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Lesson2.Data.Entities;

namespace Lesson2.Data.Entities
{
    /// <summary>
    /// ProductEntity представляє таблицю products в базі даних
    /// </summary>
    [Table("tbl_products")]
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; } = String.Empty;
        public decimal Price { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public CategoryEntity? Category { get; set; }
        public virtual ICollection<ProductImageEntity>? ProductImages { get; set; }
    }
}
