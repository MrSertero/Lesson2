using AutoMapper;
using Lesson2.Data.Entities;
using Lesson2.Models.Category;
using Lesson2.Models.Product;

namespace Lesson2.Mapper
{
    public class AppMapperProfile : Profile
    {
        public AppMapperProfile()
        {
            CreateMap<CategoryEntity, CategoryItenViewModel>();
            CreateMap<ProductCreateViewModel, CategoryEntity> ();

            CreateMap<ProductCreateViewModel, ProductEntity>()
                .ForMember(x => x.ProductImages, opt => opt.Ignore()) // Ігноруємо, якщо обробка зображень окрема
                .ForMember(x => x.Category, opt => opt.Ignore()); // Ігноруємо, якщо потрібно вручну призначити категорію

            CreateMap<ProductEntity, ProductItemViewModel>()
                .ForMember(x => x.Images, opt => opt.MapFrom(p => p.ProductImages.Select(x => x.Image).ToList()))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(c => c.Category.Name));
        }
    }
}
