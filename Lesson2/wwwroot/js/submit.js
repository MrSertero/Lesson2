document.getElementById('submitCategoryBtn').addEventListener('click', function (event) {
    event.preventDefault(); // Запобігає стандартному відправленню форми

    // Отримання значень полів форми
    const name = document.getElementById('Name').value.trim();
    const description = document.getElementById('Description').value.trim();
    const image = document.getElementById('Image').files[0];

    // Валідація полів
    let errorMessage = '';
    if (!name) {
        errorMessage += 'Назва категорії є обов’язковою.\n';
    }

    if (description.length > 4000) {
        errorMessage += 'Опис не може перевищувати 4000 символів.\n';
    }

    if (image && image.size > 5000000) { // Перевірка розміру файлу (5 МБ)
        errorMessage += 'Зображення не може перевищувати 5 МБ.\n';
    }

    // Виведення помилки або надсилання форми
    if (errorMessage) {
        alert(errorMessage); // Виведення повідомлення з помилками
    } else {
        // Якщо помилок немає, відправляємо форму через AJAX або стандартне відправлення
        document.getElementById('createCategoryForm').submit();
    }
});