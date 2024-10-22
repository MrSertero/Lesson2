using Lesson2.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Lesson2.Services
{
    public class ImageWorker : IImageWorker
    {
        private readonly IWebHostEnvironment _environment;
        private const string dirName = "uploading";
        private int[] sizes = [50, 150, 300, 600, 1200];
        public ImageWorker(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        /// <summary>
        /// Завантажує зображення за вказаною URL-адресою і зберігає його після стиснення
        /// </summary>
        public string Save(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Send a GET request to the image URL
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    // Check if the response status code indicates success (e.g., 200 OK)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the image bytes from the response content
                        byte[] imageBytes = response.Content.ReadAsByteArrayAsync().Result;
                        return CompresImage(imageBytes);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to retrieve image. Status code: {response.StatusCode}");
                        return String.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return String.Empty;
            }
        }
        /// <summary>
        /// Зберігає завантажений файл-зображення після стиснення
        /// </summary>
        public string Save(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File cannot be null or empty.", nameof(file));
                }

                // Зчитуємо файл у MemoryStream
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream); // Копіюємо вміст файлу в пам'ять
                    byte[] imageBytes = memoryStream.ToArray(); // Отримуємо масив байтів
                    return CompresImage(imageBytes); // Викликаємо метод для обробки зображення
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return String.Empty;
            }
        }

        /// <summary>
        /// Стискає зображення до кількох версій різних розмірів та зберігає їх у форматі .webp
        /// </summary>
        private string CompresImage(byte[] bytes)
        {
            string imageName = Guid.NewGuid().ToString() + ".webp";

            var dirSave = Path.Combine(_environment.WebRootPath, dirName);
            if (!Directory.Exists(dirSave))
            {
                Directory.CreateDirectory(dirSave);
            }


            foreach (int size in sizes)
            {
                var path = Path.Combine(dirSave, $"{size}_{imageName}");
                using (var image = Image.Load(bytes))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(size, size),
                        Mode = ResizeMode.Max
                    }));
                    image.SaveAsWebp(path);
                    //image.Save(path, new WebpEncoder()); // Save the resized image
                }
            }

            return imageName;
        }
        /// <summary>
        /// Видаляє всі версії зображення різних розмірів з файлової системи
        /// </summary>
        public void Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return; // Ігноруємо порожні назви файлів
            }

            // Складаємо шляхи до всіх зменшених версій
            var filePaths = sizes.Select(size => Path.Combine(_environment.WebRootPath, dirName, $"{size}_{fileName}"))
                               .ToList();

            // Видаляємо всі файли одночасно для покращення продуктивності
            Parallel.ForEach(filePaths, filePath =>
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        // Логуємо або обробляємо помилки видалення окремих файлів
                        Console.WriteLine($"Помилка видалення файлу '{filePath}': {ex.Message}");
                    }
                }
            });
        }
    }
}
