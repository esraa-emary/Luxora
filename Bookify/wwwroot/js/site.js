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

/////////////////////////////////////////////////////////////// index
/////////////////////////// hero section

document.addEventListener("DOMContentLoaded", () => {
    const nextArrow = document.querySelector(".next");
    const prevArrow = document.querySelector(".prev");

    if (window.innerWidth <= 650) {
        initSlider(".hero-section-img-small", 3000, nextArrow, prevArrow);
        document.querySelector(".hero-section-img-small").dataset.initialized = true;
    } else {
        initSlider(".hero-section-img-large", 3000, nextArrow, prevArrow);
        document.querySelector(".hero-section-img-large").dataset.initialized = true;
    }

    window.addEventListener("resize", () => {
        if (window.innerWidth <= 650) {
            const small = document.querySelector(".hero-section-img-small");
            if (small && !small.dataset.initialized) {
                initSlider(".hero-section-img-small", 3000, nextArrow, prevArrow);
                small.dataset.initialized = true;
            }
        } else {
            const large = document.querySelector(".hero-section-img-large");
            if (large && !large.dataset.initialized) {
                initSlider(".hero-section-img-large", 3000, nextArrow, prevArrow);
                large.dataset.initialized = true;
            }
        }
    });
});

function initSlider(containerSelector, intervalTime, nextArrow, prevArrow) {
    const container = document.querySelector(containerSelector);
    if (!container) return;

    const slides = container.querySelectorAll("img");
    let slideIndex = 0;
    let slideInterval;

    let dotsContainer = container.parentElement.querySelector(".dots");
    if (!dotsContainer) {
        dotsContainer = document.createElement("div");
        dotsContainer.classList.add("dots");
        container.parentElement.appendChild(dotsContainer);
    }
    dotsContainer.innerHTML = ""; 

    slides.forEach((_, index) => {
        const dot = document.createElement("span");
        dot.classList.add("dot");
        if (index === 0) dot.classList.add("active");
        dot.addEventListener("click", () => {
            clearInterval(slideInterval);
            goToSlide(index);
            slideInterval = setInterval(nextSlide, intervalTime);
        });
        dotsContainer.appendChild(dot);
    });

    const dots = dotsContainer.querySelectorAll(".dot");

    if (slides.length > 0) {
        slides.forEach(slide => slide.classList.remove("active"));
        slides[0].classList.add("active");

        slideInterval = setInterval(nextSlide, intervalTime);

        if (nextArrow) {
            nextArrow.onclick = () => {
                clearInterval(slideInterval);
                nextSlide();
                slideInterval = setInterval(nextSlide, intervalTime);
            };
        }

        if (prevArrow) {
            prevArrow.onclick = () => {
                clearInterval(slideInterval);
                prevSlide();
                slideInterval = setInterval(nextSlide, intervalTime);
            };
        }
    }

    function nextSlide() {
        changeSlide(1);
    }

    function prevSlide() {
        changeSlide(-1);
    }

    function goToSlide(index) {
        slides[slideIndex].classList.remove("active");
        dots[slideIndex].classList.remove("active");

        slideIndex = index;

        slides[slideIndex].classList.add("active");
        dots[slideIndex].classList.add("active");
    }

    function changeSlide(direction) {
        slides[slideIndex].classList.remove("active");
        dots[slideIndex].classList.remove("active");

        slideIndex = (slideIndex + direction + slides.length) % slides.length;

        slides[slideIndex].classList.add("active");
        dots[slideIndex].classList.add("active");
    }
}

/////////////////////////// review section

const reviews = document.querySelectorAll(".review");

function handleScroll() {
    const triggerBottom = window.innerHeight * 0.85; 

    reviews.forEach(review => {
        const rect = review.getBoundingClientRect();

        if (rect.top < triggerBottom && rect.bottom > 0) {
            review.classList.add("show");
        } else {
            review.classList.remove("show"); 
        }
    });
}

window.addEventListener("scroll", handleScroll);
window.addEventListener("load", handleScroll);
