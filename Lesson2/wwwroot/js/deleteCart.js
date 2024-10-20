document.addEventListener("DOMContentLoaded", () => {
    var deleteButtons = document.querySelectorAll(".btn-danger");

    deleteButtons.forEach((button) => {
        button.addEventListener("click", function (event) {
            event.preventDefault(); // Зупиняємо стандартну дію посилання

            var categoryId = this.getAttribute('data-delete'); // Отримуємо значення data-delete

            // Динамічно встановлюємо значення в модалку (якщо є назва або інші дані)
            document.getElementById('categoryName').textContent = `ID категорії: ${categoryId}`;

            // Відкрити модальне вікно, використовуючи клас 'show'
            var modalDelete = document.getElementById("modalDelete");
            modalDelete.classList.add("show");
            modalDelete.style.display = 'block'; // Або можна використовувати додавання display: block

            // Закриття модального вікна при натисканні на кнопку "Скасувати" або кнопку закриття
            var closeModalButtons = document.querySelectorAll("[data-dismiss='modal'], .close");

            closeModalButtons.forEach((button) => {
                button.addEventListener("click", function () {
                    modalDelete.classList.remove("show");
                    modalDelete.style.display = 'none';
                });
            });
        });
    });
});

//document.addEventListener("DOMContentLoaded", () => {
//    var deleteButtons = document.querySelectorAll(".btn-danger");

//    deleteButtons.forEach( (button) => {
//        button.addEventListener("click",  () => {
//            var categoryId = this.getAttribute('data-id');

//            var modalDelete = document.getElementById("modalDelete")

//            //if (confirm("Ви впевнені, що хочете видалити цю категорію?")) {
//            //    window.location.href = '/Main/DeleteCategory?id=' + categoryId;
//            //}
//        });
//    });
//});