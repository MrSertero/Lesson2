using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bogus;
using Lesson2.Data;
using Lesson2.Data.Entities;
using Lesson2.Interfaces;
using Lesson2.Models.Product;
using Lesson2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        /// <summary>
        /// Повертає список продуктів із бази даних у вигляді моделей ProductItemViewModel
        /// </summary>
        public IActionResult Index()
        {
            List<ProductItemViewModel> model = _dbContext.Products
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();
            return View(model);
        }
        /// <summary>
        /// Повертає форму для створення нового продукту
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _dbContext.Categories
                .Select(x => new { Value = x.Id, Text = x.Name })
                .ToList();

            ProductCreateViewModel viewModel = new()
            {
                CategoryList = new SelectList(categories, "Value", "Text")
            };

            return View(viewModel);
        }
        /// <summary>
        /// Створює новий продукт та зберігає його в базі даних
        /// </summary>
        [HttpPost]
        public IActionResult Create(ProductCreateViewModel model)
        {
            var entity = _mapper.Map<ProductEntity>(model);

            var dirName = "uploading";
            var dirSave = Path.Combine(_environment.WebRootPath, dirName);

            if (!Directory.Exists(dirSave))
            {
                Directory.CreateDirectory(dirSave);
            }

            entity.ProductImages = new List<ProductImageEntity>();

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
            _dbContext.Products.Add(entity);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
        /// <summary>
        /// Видаляє продукт за його ID
        /// </summary>
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
        /// <summary>
        /// Показує детальну інформацію про продукт
        /// </summary>
        public IActionResult Details(int id)
        {
            var product = _dbContext.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Мапінг даних з ProductEntity на ProductItemViewModel
            var viewModel = _mapper.Map<ProductItemViewModel>(product);

            return View(viewModel);
        }
        /// <summary>
        /// Повертає форму для редагування продукту
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _dbContext.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<ProductEditViewModel>(product);

            var categories = _dbContext.Categories
                .Select(x => new { Value = x.Id, Text = x.Name })
                .ToList();

            model.CategoryList = new SelectList(categories, "Value", "Text");

            return View(model);
        }
        /// <summary>
        /// Оновлює дані продукту та зображення
        /// </summary>
        [HttpPost]
        public IActionResult Edit(ProductEditViewModel model)
        {
            var product = _dbContext.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == model.Id);

            if (product == null)
            {
                return NotFound();
            }

            _mapper.Map(model, product);

            // Видалення вибраних зображень
            if (model.ImagesToDelete != null && model.ImagesToDelete.Count > 0)
            {
                foreach (var image in model.ImagesToDelete)
                {
                    var productImage = _dbContext.ProductImages.FirstOrDefault(img => img.Image == image);
                    if (productImage != null)
                    {
                        _imageWorker.Delete(productImage.Image); // Видалення зображення з диска
                        _dbContext.ProductImages.Remove(productImage); // Видалення зображення з бази даних
                    }
                }
            }

            // Збереження нових фото
            if (model.NewPhotos != null && model.NewPhotos.Count > 0)
            {
                foreach (var photo in model.NewPhotos)
                {
                    if (photo.Length > 0)
                    {
                        var imageEntity = new ProductImageEntity()
                        {
                            Product = product,
                            Image = _imageWorker.Save(photo),
                            Priority = product.ProductImages.Count // порядок
                        };

                        product.ProductImages.Add(imageEntity);
                    }
                }
            }

            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
