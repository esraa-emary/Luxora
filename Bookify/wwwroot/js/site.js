/////////////////////////////////////////////////////////////// layout

document.addEventListener("DOMContentLoaded", () => {
    const foots = document.querySelectorAll(".foot");
    const options = { threshold: 0.2 };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry, i) => {
            if (entry.isIntersecting) {
                setTimeout(() => {
                    entry.target.classList.add("show");
                }, i * 300);
            } else {
                entry.target.classList.remove("show");
            }
        });
    }, options);

    foots.forEach(foot => observer.observe(foot));
});


const checkbox = document.querySelector('.hamburgers input');
const navbar = document.getElementById('navbarContent');

checkbox.addEventListener('change', () => {
    if (checkbox.checked) {
        navbar.style.display = 'block';
    } else {
        navbar.style.display = 'none';
    }
});
