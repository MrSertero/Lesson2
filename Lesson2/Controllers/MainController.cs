﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Lesson2.Data;
using Lesson2.Data.Entities;
using Lesson2.Interfaces;
using Lesson2.Models.Category;
using Lesson2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lesson2.Controllers
{
    public class MainController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly IImageWorker _imageWorker;
        //Зберігає різну інформацію про MVC проект
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        //DI - Depencecy Injection
        public MainController(AppDBContext context,
            IWebHostEnvironment environment, IImageWorker imageWorker,
            IMapper mapper)
        {
            _dbContext = context;
            _environment = environment;
            _imageWorker = imageWorker;
            _mapper = mapper;
        }

        //метод у контролері називаться - action - дія
        /// <summary>
        /// Повертає список категорій із бази даних у вигляді моделей CategoryItenViewModel
        /// </summary>
        public IActionResult Index()
        {
            var model = _dbContext.Categories
                .ProjectTo<CategoryItenViewModel>(_mapper.ConfigurationProvider)
                .ToList();
            return View(model);
        }
        /// <summary>
        /// Повертає порожню форму для створення нової категорії
        /// </summary>
        [HttpGet] //це означає, що буде відображатися сторінки для перегляду
        public IActionResult Create()
        {
            //Ми повертає View - пусту, яка відобраєате сторінку де потрібно ввести дані для категорії
            return View();
        }
        /// <summary>
        /// Зберігає нову категорію в базу даних на основі даних з форми.
        /// </summary>
        [HttpPost] //це означає, що ми отримуємо дані із форми від клієнта
        public IActionResult Create(CategoryCreateViewModel model)
        {
            var entity = _mapper.Map<CategoryEntity>(model);
            //Збережння в Базу даних інформації
            var dirName = "uploading";
            var dirSave = Path.Combine(_environment.WebRootPath, dirName);
            if (!Directory.Exists(dirSave))
            {
                Directory.CreateDirectory(dirSave);
            }
            if (model.Photo != null)
            {
                string fileName = _imageWorker.Save(model.Photo);
                entity.Image = fileName;
            }
            _dbContext.Categories.Add(entity);
            _dbContext.SaveChanges();
            //Переходимо до списку усіх категорій, тобото визиваємо метод Index нашого контролера
            return Redirect("/");
        }
        /// <summary>
        /// Видаляє категорію з бази даних за її ID.
        /// </summary>
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _dbContext.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(category.Image))
            {
                _imageWorker.Delete(category.Image);
            }
            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();

            return Json(new { text = "Ми його видалили" });
        }
        /// <summary>
        /// Повертає форму для редагування існуючої категорії
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _dbContext.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            var model = new CategoryCreateViewModel
            {
                Name = category.Name,
                Description = category.Description,
                Photo = null // Оскільки файл не завантажуєм
            };

            return View("Create", model);
        }
        /// <summary>
        ///  Оновлює категорію в базі даних на основі введених даних
        /// </summary>
        [HttpPost]
        public IActionResult Edit(int id, CategoryCreateViewModel model)
        {
            var category = _dbContext.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }


            category.Name = model.Name;
            category.Description = model.Description;


            if (model.Photo != null)
            {
                if (!string.IsNullOrEmpty(category.Image))
                {
                    _imageWorker.Delete(category.Image);
                }

                string fileName = _imageWorker.Save(model.Photo);
                category.Image = fileName;
            }

            _dbContext.Categories.Update(category);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
