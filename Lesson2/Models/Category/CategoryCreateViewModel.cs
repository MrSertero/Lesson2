﻿using System.ComponentModel.DataAnnotations;

namespace Lesson2.Models.Category;
/// <summary>
/// Використовується для передачі даних із форми створення категорії до контролера
/// </summary>
public class CategoryCreateViewModel
{
    [Display(Name = "Назва категорії")]
    public string Name { get; set; } = String.Empty;
    //Тип для передачі файлі на сервер - із сторінки хочу отримати файл із <input type="file"/>
    [Display(Name = "Оберіть фото на ПК")]
    public IFormFile? Photo { get; set; }
    [Display(Name = "Короткий опис")]
    public string Description { get; set; } = string.Empty;
}