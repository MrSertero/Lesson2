using Lesson2.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lesson2.Data
{
    public class AppDBContext : DbContext
	{
		public AppDBContext(DbContextOptions<AppDBContext> options)
			: base(options) { }

		public DbSet<CategoryEntity> Categories{ get; set; }
		public DbSet<ProductEntity> Products { get; set; }
		public DbSet<ProductImageEntity> ProductImages { get; set; }
	}
}
