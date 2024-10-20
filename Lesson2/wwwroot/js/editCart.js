document.addEventListener("DOMContentLoaded", () => {
    let buttonsEdit = document.querySelectorAll('[data-editing]');

    buttonsEdit.forEach(btn => {
        btn.addEventListener("click", function (event) {
            event.preventDefault();
            let id = this.getAttribute('data-editing');

            window.location.href = '/Main/Edit/' + id;
        });
    });
});