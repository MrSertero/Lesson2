using AutoMapper;
using Lesson2.Data.Entities;
using Lesson2.Models.Category;
using Lesson2.Models.Product;
using System.Globalization;

namespace Lesson2.Mapper
{
    public class AppMapperProfile : Profile
    {
        /// <summary>
        /// Налаштовую AutoMapper для перетворення між різними об'єктами моделей і сутностями
        /// </summary>
        public AppMapperProfile()
        {
            CreateMap<CategoryEntity, CategoryItenViewModel>();
            CreateMap<CategoryCreateViewModel, CategoryEntity>();

            CreateMap<ProductEntity, ProductItemViewModel>()
                .ForMember(x => x.Images, opt => opt.MapFrom(p => p.ProductImages.Select(x => x.Image).ToList()))
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(c => c.Category.Name));

            CreateMap<ProductCreateViewModel, ProductEntity>()
                .ForMember(x => x.Price, opt => opt.MapFrom(p => Decimal.Parse(p.Price.Replace('.', ','), new CultureInfo("uk-UA"))));

            CreateMap<ProductEntity, ProductEditViewModel>()
                .ForMember(x => x.ExistingImages, opt => opt.MapFrom(p => p.ProductImages.Select(x => x.Image).ToList()))
                .ForMember(x => x.SelectedCategoryId, opt => opt.MapFrom(p => p.CategoryId))
                .ForMember(x => x.Price, opt => opt.MapFrom(p => p.Price));
           
            CreateMap<ProductEditViewModel, ProductEntity>()
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.SelectedCategoryId));
        }
    }
}
