using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bogus;
using Lesson2.Data;
using Lesson2.Data.Entities;
using Lesson2.Interfaces;
using Lesson2.Models.Product;
using Lesson2.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;

namespace Lesson2.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageWorker _imageWorker;
        private readonly IWebHostEnvironment _environment;
        //DI - Depencecy Injection
        public ProductsController(AppDBContext context, IMapper mapper,
            IWebHostEnvironment environment, IImageWorker imageWorker)
        {
            _dbContext = context;
            _mapper = mapper;
            _imageWorker = imageWorker;
            _environment = environment;
        }
        public IActionResult Index()
        {
            List<ProductItemViewModel> model = _dbContext.Products
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();
            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductCreateViewModel model)
        {
            //var entity = _mapper.Map<ProductEntity>(model); // ПОМИЛКА з decimal

            var entity = new ProductEntity
            {
                Name = model.Name,
                Price = decimal.Parse(model.Price, CultureInfo.InvariantCulture)
            };

            // Збереження в Базу даних інформації
            var dirName = "uploading";
            var dirSave = Path.Combine(_environment.WebRootPath, dirName);

            if (!Directory.Exists(dirSave))
            {
                Directory.CreateDirectory(dirSave);
            }

            entity.ProductImages = new List<ProductImageEntity>(); // Ініціалізуємо колекцію

            var categories = _dbContext.Categories.ToList();

            if (model.Photos != null && model.Photos.Count > 0)
            {
                int priority = 0;
                // Збереження фотографій
                foreach (var photo in model.Photos)
                {
                    if (photo.Length > 0)
                    {
                        var productImageEntity = new ProductImageEntity()
                        {
                            Product = entity,
                            Image = _imageWorker.Save(photo),
                            Priority = priority
                        };
                        entity.ProductImages.Add(productImageEntity); // Додаємо до колекції

                        priority++;
                    }
                }
            }
            entity.Category = categories[categories.Count - 1];
            _dbContext.Products.Add(entity);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            if(product.ProductImages != null && product.ProductImages.Any())
            {
                foreach (var image in product.ProductImages)
                {
                    if (!string.IsNullOrEmpty(image.Image))
                    {
                        _imageWorker.Delete(image.Image);
                    }
                }
            }

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return Json(new { text = "Ми його видалили" });
        }
    }
}
