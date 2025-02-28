﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lesson2.Data.Entities
{
    /// <summary>
    /// CategoryEntity представляє таблицю categories в базі даних
    /// </summary>
    [Table("tbl_categories")]
    public class CategoryEntity
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Image { get; set; }
        [StringLength(4000)]
        public string? Description { get; set; }
        public virtual ICollection<ProductEntity> Products { get; set; }
    }
}