document.addEventListener("DOMContentLoaded", () => {
    function updateImages() {
        let images = document.querySelectorAll('[data-image-src]');

        images.forEach(img => {
            let imageSrc = img.getAttribute('data-image-src');

            if (window.innerWidth >= 1200) {
                img.src = "/uploading/600_" + imageSrc;
            } else if (window.innerWidth >= 720) {
                img.src = "/uploading/300_" + imageSrc;
            } else {
                img.src = "/uploading/150_" + imageSrc;
            }
        });
    }

    // Виклик функції при завантаженні сторінки
    updateImages();

    // Виклик функції при зміні розміру вікна
    window.addEventListener('resize', updateImages);
});