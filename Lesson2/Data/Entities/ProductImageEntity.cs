﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Lesson2.Data.Entities;

namespace Lesson2.Data.Entities
{
    /// <summary>
    /// ProductImageEntity представляє таблицю product_images в базі даних
    /// </summary>
    [Table("tbl_product_images")]
    public class ProductImageEntity
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Image { get; set; } = string.Empty;
        public int Priority { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual ProductEntity? Product { get; set; }
    }
}
