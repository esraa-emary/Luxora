const CONFIG = {
    typewriter: {
        typingSpeed: 120,
        deletingSpeed: 60,
        pauseAfterWord: 1400,
        pauseBeforeStart: 200,
        initialDelay: 200,
    },
    reveal: {
        threshold: 100,
        rootMargin: "0px 0px -100px 0px",
    },
    video: {
        preloadTimeout: 5000,
        fallbackImage: "assets/images/hero-fallback.jpg",
        mobileBreakpoint: 600,
    },
};
const words = [
    {
        text: "Luxury",
        video: "/Videos/a.mp4",
        fallback: "assets/images/luxury-fallback.jpg",
    },
    {
        text: "Comfort",
        video: "/Videos/com.mp4",
        fallback: "assets/images/comfort-fallback.jpg",
    },
    {
        text: "Elegance",
        video: "/Videos/ele.mp4",
        fallback: "assets/images/elegance-fallback.jpg",
    },
    {
        text: "Adventure",
        video: "/Videos/adv.mp4",
        fallback: "assets/images/adventure-fallback.jpg",
    },
];

const state = {
    currentWordIndex: 0,
    currentCharIndex: 0,
    isDeleting: false,
    typingTimer: null,
    isTypingActive: false,
    videoElement: null,
    typewriterElement: null,
    isMobile: false,
    intersectionObserver: null,
    preloadedVideos: new Set(),
};

const utils = {
    debounce(func, wait) {
        let timeout;
        return function (...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, args), wait);
        };
    },

    throttle(func, limit) {
        let inThrottle;
        return function (...args) {
            if (!inThrottle) {
                func.apply(this, args);
                inThrottle = true;
                setTimeout(() => (inThrottle = false), limit);
            }
        };
    },

    isMobileDevice() {
        return (
            window.innerWidth < CONFIG.video.mobileBreakpoint ||
            /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(
                navigator.userAgent
            )
        );
    },

    prefersReducedMotion() {
        return window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    },

    getElementById(id) {
        const el = document.getElementById(id);
        if (!el) console.warn(`Element with ID '${id}' not found`);
        return el;
    },

    preloadVideo(src) {
        return new Promise((resolve, reject) => {
            if (state.preloadedVideos.has(src)) {
                resolve();
                return;
            }
            const video = document.createElement("video");
            video.preload = "metadata";
            video.muted = true;

            const timeout = setTimeout(() => {
                reject(new Error("Video preload timeout"));
            }, CONFIG.video.preloadTimeout);

            video.addEventListener(
                "loadedmetadata",
                () => {
                    clearTimeout(timeout);
                    state.preloadedVideos.add(src);
                    resolve();
                },
                { once: true }
            );

            video.addEventListener(
                "error",
                () => {
                    clearTimeout(timeout);
                    reject(new Error("Video preload failed"));
                },
                { once: true }
            );

            video.src = src;
        });
    },

    createFallbackBackground(imageSrc) {
        const hero = document.querySelector(".hero-section");
        if (hero) {
            hero.style.backgroundImage = `url('${imageSrc}')`;
            hero.style.backgroundSize = "cover";
            hero.style.backgroundPosition = "center";
            hero.style.backgroundRepeat = "no-repeat";
        }
    },
};

class RevealManager {
    constructor() {
        this.elements = new Set();
        this.init();
    }

    init() {
        if ("IntersectionObserver" in window) {
            this.setupIntersectionObserver();
        } else {
            this.setupScrollListener();
        }
        this.updateElements();
    }

    setupIntersectionObserver() {
        const options = {
            root: null,
            rootMargin: CONFIG.reveal.rootMargin,
            threshold: 0.1,
        };
        state.intersectionObserver = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                entry.target.classList.toggle("active", entry.isIntersecting);
            });
        }, options);
    }

    setupScrollListener() {
        const throttledReveal = utils.throttle(() => this.revealElements(), 16);
        window.addEventListener("scroll", throttledReveal, { passive: true });
    }

    updateElements() {
        const revealEls = document.querySelectorAll(".reveal");
        if (state.intersectionObserver) {
            this.elements.forEach((el) => state.intersectionObserver.unobserve(el));
            this.elements.clear();
            revealEls.forEach((el) => {
                state.intersectionObserver.observe(el);
                this.elements.add(el);
            });
        } else {
            this.elements = new Set(revealEls);
            this.revealElements();
        }
    }

    revealElements() {
        this.elements.forEach((el) => {
            const rect = el.getBoundingClientRect();
            const isVisible = rect.top < window.innerHeight - CONFIG.reveal.threshold;
            el.classList.toggle("active", isVisible);
        });
    }

    destroy() {
        if (state.intersectionObserver) {
            state.intersectionObserver.disconnect();
        }
    }
}

class VideoManager {
    constructor() {
        this.currentVideoIndex = -1;
        this.transitionInProgress = false;
        this.init();
    }

    init() {
        state.videoElement = utils.getElementById("heroVideo");
        if (!state.videoElement) return;

        state.videoElement.muted = true;
        state.videoElement.playsInline = true;
        state.videoElement.autoplay = true;
        state.videoElement.loop = true;

        state.videoElement.addEventListener("error", (e) => {
            console.warn("Video error:", e);
            this.handleVideoError();
        });

        this.preloadInitialVideos();
    }

    async preloadInitialVideos() {
        if (state.isMobile) return;
        const preloadPromises = words
            .slice(0, 2)
            .map((w) =>
                utils
                    .preloadVideo(w.video)
                    .catch((err) =>
                        console.warn("Failed to preload video:", w.video, err)
                    )
            );
        await Promise.allSettled(preloadPromises);
    }

    async setVideoForIndex(index) {
        if (
            state.isMobile ||
            !state.videoElement ||
            index === this.currentVideoIndex ||
            this.transitionInProgress
        )
            return;

        const word = words[index];
        if (!word) return;

        const source = state.videoElement.querySelector("source");
        if (!source) return;

        const newSrc = word.video;
        if (source.getAttribute("src") === newSrc) return;

        this.transitionInProgress = true;

        try {
            if (!state.preloadedVideos.has(newSrc)) {
                await utils.preloadVideo(newSrc);
            }

            state.videoElement.pause();
            source.setAttribute("src", newSrc);
            state.videoElement.load();

            const playPromise = state.videoElement.play();
            if (playPromise && typeof playPromise.then === "function") {
                await playPromise;
            }

            this.currentVideoIndex = index;
            this.transitionInProgress = false;
        } catch (error) {
            console.warn("Failed to switch video:", error);
            this.handleVideoError(word.fallback);
            this.transitionInProgress = false;
        }
    }

    handleVideoError(fallbackImage) {
        if (state.videoElement) {
            state.videoElement.style.display = "none";
        }
        const fallback = fallbackImage || CONFIG.video.fallbackImage;
        utils.createFallbackBackground(fallback);
    }

    switchVideo(src) {
        const idx = words.findIndex((w) => w.video.includes(src));
        if (idx !== -1) {
            this.setVideoForIndex(idx);
        }
    }

    getCurrentVideoElement() {
        return state.videoElement;
    }
}

class TypewriterEffect {
    constructor() {
        this.init();
    }

    init() {
        state.typewriterElement = utils.getElementById("typewriter");
        if (!state.typewriterElement) {
            console.warn("Typewriter element not found");
            return;
        }

        if (utils.prefersReducedMotion()) {
            this.showStaticText();
            return;
        }

        this.start();
    }

    showStaticText() {
        if (state.typewriterElement && words.length > 0) {
            state.typewriterElement.textContent = words[0].text;
        }
    }

    start() {
        if (state.isTypingActive) return;
        state.isTypingActive = true;
        state.typingTimer = setTimeout(
            () => this.typeEffect(),
            CONFIG.typewriter.initialDelay
        );
    }

    stop() {
        if (state.typingTimer) {
            clearTimeout(state.typingTimer);
            state.typingTimer = null;
        }
        state.isTypingActive = false;
    }

    typeEffect() {
        if (!state.typewriterElement || !state.isTypingActive) return;

        const currentWord = words[state.currentWordIndex];
        if (!currentWord) return;

        const { text } = currentWord;

        if (!state.isDeleting) {
            state.currentCharIndex++;
            state.typewriterElement.textContent = text.substring(
                0,
                state.currentCharIndex
            );

            if (state.currentCharIndex === text.length) {
                state.isDeleting = true;
                videoManager.setVideoForIndex(state.currentWordIndex);
                state.typingTimer = setTimeout(
                    () => this.typeEffect(),
                    CONFIG.typewriter.pauseAfterWord
                );
                return;
            }

            state.typingTimer = setTimeout(
                () => this.typeEffect(),
                CONFIG.typewriter.typingSpeed
            );
        } else {
            state.currentCharIndex--;
            state.typewriterElement.textContent = text.substring(
                0,
                state.currentCharIndex
            );

            if (state.currentCharIndex === 0) {
                state.isDeleting = false;
                state.currentWordIndex = (state.currentWordIndex + 1) % words.length;
                state.typingTimer = setTimeout(
                    () => this.typeEffect(),
                    CONFIG.typewriter.pauseBeforeStart
                );
            } else {
                state.typingTimer = setTimeout(
                    () => this.typeEffect(),
                    CONFIG.typewriter.deletingSpeed
                );
            }
        }
    }

    destroy() {
        this.stop();
    }
}

class ResponsiveManager {
    constructor() {
        this.init();
    }

    init() {
        this.updateMobileState();
        this.setupEventListeners();
        this.handleMobileOptimizations();
    }

    updateMobileState() {
        state.isMobile = utils.isMobileDevice();
    }

    setupEventListeners() {
        const debouncedResize = utils.debounce(() => {
            this.updateMobileState();
            this.handleMobileOptimizations();
            revealManager.updateElements();
        }, 250);

        window.addEventListener("resize", debouncedResize);

        document.addEventListener("visibilitychange", () => {
            if (document.hidden) {
                typewriterEffect.stop();
            } else if (state.isTypingActive) {
                setTimeout(() => typewriterEffect.start(), 500);
            }
        });
    }

    handleMobileOptimizations() {
        if (state.isMobile) {
            if (state.videoElement) {
                state.videoElement.style.display = "none";
            }
            utils.createFallbackBackground(CONFIG.video.fallbackImage);
            document.body.classList.add("mobile-optimized");
        } else {
            if (state.videoElement) {
                state.videoElement.style.display = "block";
            }
            document.body.classList.remove("mobile-optimized");
        }
    }
}

class PerformanceMonitor {
    constructor() {
        this.metrics = {
            startTime: performance.now(),
            domContentLoaded: null,
            fullyLoaded: null,
        };
        this.init();
    }

    init() {
        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", () => {
                this.metrics.domContentLoaded =
                    performance.now() - this.metrics.startTime;
                this.logMetric("DOMContentLoaded", this.metrics.domContentLoaded);
            });
        } else {
            this.metrics.domContentLoaded = 0;
        }

        window.addEventListener("load", () => {
            this.metrics.fullyLoaded = performance.now() - this.metrics.startTime;
            this.logMetric("Fully Loaded", this.metrics.fullyLoaded);
        });
    }

    logMetric(name, time) {
        if (time > 1000) {
            console.warn(`Performance: ${name} took ${time.toFixed(2)}ms`);
        }
    }
}

let revealManager;
let videoManager;
let typewriterEffect;
let responsiveManager;
let performanceMonitor;

function initializeHeroSection() {
    try {
        performanceMonitor = new PerformanceMonitor();
        responsiveManager = new ResponsiveManager();
        revealManager = new RevealManager();
        videoManager = new VideoManager();
        typewriterEffect = new TypewriterEffect();

        console.log("Hero section initialized successfully");
    } catch (error) {
        console.error("Failed to initialize hero section:", error);
    }
}

function cleanupHeroSection() {
    if (typewriterEffect) typewriterEffect.destroy();
    if (revealManager) revealManager.destroy();
    if (state.typingTimer) clearTimeout(state.typingTimer);
}

window.heroSection = {
    switchVideo: (src) => videoManager?.switchVideo(src),
    startTypewriter: () => typewriterEffect?.start(),
    stopTypewriter: () => typewriterEffect?.stop(),
    updateRevealElements: () => revealManager?.updateElements(),
    getCurrentVideo: () => videoManager?.getCurrentVideoElement(),
    isTransitioning: () => videoManager?.transitionInProgress || false,
    getState: () => ({ ...state }),
    cleanup: cleanupHeroSection,
};

document.addEventListener("DOMContentLoaded", initializeHeroSection);
window.addEventListener("beforeunload", cleanupHeroSection);
window.addEventListener("error", (event) => {
    console.error("Global error in hero section:", event.error);
});

function updateCountdown(endDate, countdownId) {
    const now = new Date().getTime();
    const distance = endDate - now;

    if (distance < 0) {
        document.getElementById(`days${countdownId}`).textContent = "00";
        document.getElementById(`hours${countdownId}`).textContent = "00";
        document.getElementById(`minutes${countdownId}`).textContent = "00";
        document.getElementById(`seconds${countdownId}`).textContent = "00";
        return;
    }

    const days = Math.floor(distance / (1000 * 60 * 60 * 24));
    const hours = Math.floor(
        (distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)
    );
    const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((distance % (1000 * 60)) / 1000);

    document.getElementById(`days${countdownId}`).textContent = days
        .toString()
        .padStart(2, "0");
    document.getElementById(`hours${countdownId}`).textContent = hours
        .toString()
        .padStart(2, "0");
    document.getElementById(`minutes${countdownId}`).textContent = minutes
        .toString()
        .padStart(2, "0");
    document.getElementById(`seconds${countdownId}`).textContent = seconds
        .toString()
        .padStart(2, "0");
}

const endDate1 = new Date();
endDate1.setDate(endDate1.getDate() + 5);

const endDate2 = new Date();
endDate2.setDate(endDate2.getDate() + 3);

const endDate3 = new Date();
endDate3.setDate(endDate3.getDate() + 7);

setInterval(function () {
    updateCountdown(endDate1, 1);
    updateCountdown(endDate2, 2);
    updateCountdown(endDate3, 3);
}, 1000);
updateCountdown(endDate1, 1);
updateCountdown(endDate2, 2);
updateCountdown(endDate3, 3);
const tabs = document.querySelectorAll(".offer-tab");
tabs.forEach((tab) => {
    tab.addEventListener("click", () => {
        tabs.forEach((t) => t.classList.remove("active"));
        tab.classList.add("active");
    });
});
const buttons = document.querySelectorAll(".offer-btn");
buttons.forEach((button) => {
    button.addEventListener("click", function () {
        this.innerHTML = '<i class="fas fa-check"></i> Added to Cart';
        this.style.background = "#2ecc71";
        setTimeout(() => {
            this.innerHTML = "Book Now";
            this.style.background = "";
        }, 2000);
    });
});
const filters = document.querySelectorAll(".gallery-filter");
const items = document.querySelectorAll(".gallery-item");

filters.forEach((filter) => {
    filter.addEventListener("click", () => {

        filters.forEach((f) => f.classList.remove("active"));
        filter.classList.add("active");
        const category = filter.getAttribute("data-filter");

        items.forEach((item) => {
            if (
                category === "all" ||
                item.getAttribute("data-category") === category
            ) {
                item.style.display = "block";
                setTimeout(() => {
                    item.style.opacity = 1;
                    item.style.transform = "translateY(0)";
                }, 10);
            } else {
                item.style.opacity = 0;
                item.style.transform = "translateY(20px)";
                setTimeout(() => {
                    item.style.display = "none";
                }, 300);
            }
        });
    });
});
const lightbox = document.getElementById("lightbox");
const lightboxImage = document.getElementById("lightboxImage");
const lightboxTitle = document.getElementById("lightboxTitle");
const lightboxDesc = document.getElementById("lightboxDesc");
const closeLightbox = document.getElementById("closeLightbox");

items.forEach((item) => {
    item.addEventListener("click", () => {
        const img = item.querySelector("img");
        const title = item.querySelector(".gallery-item-title").textContent;
        const desc = item.querySelector(".gallery-item-desc").textContent;

        lightboxImage.src = img.src;
        lightboxTitle.textContent = title;
        lightboxDesc.textContent = desc;
        lightbox.style.display = "flex";
    });
});

closeLightbox.addEventListener("click", () => {
    lightbox.style.display = "none";
});

lightbox.addEventListener("click", (e) => {
    if (e.target === lightbox) {
        lightbox.style.display = "none";
    }
});

const videoPlaceholder = document.getElementById("videoPlaceholder");
const hotelVideo = document.getElementById("hotelVideo");

videoPlaceholder.addEventListener("click", () => {
    hotelVideo.src = "https://www.youtube.com/embed/0yBc0dero6I?autoplay=1";
    hotelVideo.style.display = "block";
    videoPlaceholder.style.display = "none";
});
document.addEventListener("DOMContentLoaded", function () {
    const filters = document.querySelectorAll(".gallery-filter");
    const items = document.querySelectorAll(".gallery-item");
    const lightbox = document.getElementById("lightbox");
    const lightboxImage = document.getElementById("lightboxImage");
    const lightboxTitle = document.getElementById("lightboxTitle");
    const lightboxDesc = document.getElementById("lightboxDesc");
    const lightboxTags = document.getElementById("lightboxTags");
    const closeLightbox = document.getElementById("closeLightbox");
    const prevBtn = document.getElementById("prevBtn");
    const nextBtn = document.getElementById("nextBtn");
    const currentImageIndex = document.getElementById("currentImageIndex");
    const totalImages = document.getElementById("totalImages");
    const searchInput = document.getElementById("searchInput");
    const loadMoreBtn = document.getElementById("loadMoreBtn");
    const favoriteBtns = document.querySelectorAll(".favorite-btn");
    const favCount = document.getElementById("favCount");
    const videoPlaceholder = document.getElementById("videoPlaceholder");
    const hotelVideo = document.getElementById("hotelVideo");

    let currentIndex = 0;
    let visibleItems = Array.from(items);
    let favorites = JSON.parse(localStorage.getItem("hotelFavorites")) || [];
    let itemsToShow = 12;
    let allItemsVisible = false;
    totalImages.textContent = items.length;
    updateFavoriteDisplay();
    function animateStats() {
        const stats = document.querySelectorAll(".stat-number");
        stats.forEach((stat) => {
            const target = parseInt(stat.getAttribute("data-target"));
            let current = 0;
            const increment = target / 50;
            const timer = setInterval(() => {
                current += increment;
                if (current >= target) {
                    stat.textContent = target;
                    clearInterval(timer);
                } else {
                    stat.textContent = Math.floor(current);
                }
            }, 40);
        });
    }
    setTimeout(animateStats, 500);
    filters.forEach((filter, index) => {
        filter.addEventListener("click", () => {
            filters.forEach((f) => f.classList.remove("active"));
            filter.classList.add("active");
            const category = filter.getAttribute("data-filter");
            filterItems(category);
        });
    });

    function filterItems(category, searchTerm = "") {
        visibleItems = [];

        items.forEach((item, index) => {
            const itemCategory = item.getAttribute("data-category");
            const itemTags = item.getAttribute("data-tags").toLowerCase();
            const itemTitle = item
                .querySelector(".gallery-item-title")
                .textContent.toLowerCase();
            const itemDesc = item
                .querySelector(".gallery-item-desc")
                .textContent.toLowerCase();

            const matchesCategory = category === "all" || itemCategory === category;
            const matchesSearch =
                searchTerm === "" ||
                itemTags.includes(searchTerm) ||
                itemTitle.includes(searchTerm) ||
                itemDesc.includes(searchTerm);

            if (matchesCategory && matchesSearch) {
                visibleItems.push(item);
                item.style.display = "block";
                setTimeout(() => {
                    item.style.opacity = "1";
                    item.style.transform = "translateY(0)";
                }, index * 50);
            } else {
                item.style.opacity = "0";
                item.style.transform = "translateY(30px)";
                setTimeout(() => {
                    item.style.display = "none";
                }, 300);
            }
        });
        updateLoadMoreButton();
    }
    searchInput.addEventListener("input", (e) => {
        const searchTerm = e.target.value.toLowerCase();
        const activeFilter = document
            .querySelector(".gallery-filter.active")
            .getAttribute("data-filter");
        filterItems(activeFilter, searchTerm);
    });
    loadMoreBtn.addEventListener("click", () => {
        loadMoreBtn.classList.add("loading");

        setTimeout(() => {
            itemsToShow += 6;
            updateItemVisibility();
            loadMoreBtn.classList.remove("loading");
        }, 1000);
    });

    function updateLoadMoreButton() {
        if (visibleItems.length <= itemsToShow) {
            loadMoreBtn.style.display = "none";
        } else {
            loadMoreBtn.style.display = "inline-flex";
        }
    }

    function updateItemVisibility() {
        visibleItems.forEach((item, index) => {
            if (index < itemsToShow) {
                item.style.display = "block";
            } else {
                item.style.display = "none";
            }
        });
        updateLoadMoreButton();
    }
    favoriteBtns.forEach((btn) => {
        btn.addEventListener("click", (e) => {
            e.stopPropagation();
            const item = btn.closest(".gallery-item");
            const title = item.querySelector(".gallery-item-title").textContent;
            const imageSrc = item.querySelector("img").src;

            if (favorites.some((fav) => fav.title === title)) {
                favorites = favorites.filter((fav) => fav.title !== title);
                btn.classList.remove("active");
                btn.innerHTML = '<i class="far fa-heart"></i>';
            } else {
                favorites.push({ title, imageSrc });
                btn.classList.add("active");
                btn.innerHTML = '<i class="fas fa-heart"></i>';
            }

            localStorage.setItem("hotelFavorites", JSON.stringify(favorites));
            updateFavoriteDisplay();
        });
    });

    function updateFavoriteDisplay() {
        favCount.textContent = favorites.length;
        favoriteBtns.forEach((btn) => {
            const item = btn.closest(".gallery-item");
            const title = item.querySelector(".gallery-item-title").textContent;

            if (favorites.some((fav) => fav.title === title)) {
                btn.classList.add("active");
                btn.innerHTML = '<i class="fas fa-heart"></i>';
            } else {
                btn.classList.remove("active");
                btn.innerHTML = '<i class="far fa-heart"></i>';
            }
        });
    }
    items.forEach((item, index) => {
        item.addEventListener("click", (e) => {
            if (!e.target.closest(".favorite-btn")) {
                openLightbox(index);
            }
        });
    });

    function openLightbox(index) {
        const item = items[index];
        currentIndex = index;

        const img = item.querySelector("img");
        const title = item.querySelector(".gallery-item-title").textContent;
        const desc = item.querySelector(".gallery-item-desc").textContent;
        const tags = item.querySelectorAll(".tag");

        // Set lightbox content
        lightboxImage.src = img.src;
        lightboxTitle.textContent = title;
        lightboxDesc.textContent = desc;
        lightboxTags.innerHTML = "";
        tags.forEach((tag) => {
            lightboxTags.appendChild(tag.cloneNode(true));
        });

        // Force the lightbox to be centered
        lightbox.style.display = "flex";
        lightbox.style.justifyContent = "center";
        lightbox.style.alignItems = "center";
        lightbox.style.flexDirection = "column";

        lightboxImage.style.maxHeight = "80vh";
        lightboxImage.style.maxWidth = "90%";
        lightboxImage.style.margin = "0 auto";
        lightboxImage.style.display = "block";
        lightboxImage.style.objectFit = "contain";

        document.body.style.overflow = "hidden";

        // Scroll the clicked gallery item to the center of viewport
        item.scrollIntoView({ behavior: "smooth", block: "center" });
    }

    // Close lightbox function
    function closeLightboxModal() {
        lightbox.style.display = "none";
        document.body.style.overflow = "auto";
    }



    function closeLightboxModal() {
        lightbox.style.display = "none";
        document.body.style.overflow = "auto";
    }
    prevBtn.addEventListener("click", () => {
        currentIndex = currentIndex > 0 ? currentIndex - 1 : items.length - 1;
        openLightbox(currentIndex);
    });

    nextBtn.addEventListener("click", () => {
        currentIndex = currentIndex < items.length - 1 ? currentIndex + 1 : 0;
        openLightbox(currentIndex);
    });
    closeLightbox.addEventListener("click", closeLightboxModal);

    lightbox.addEventListener("click", (e) => {
        if (e.target === lightbox) {
            closeLightboxModal();
        }
    });
    document.addEventListener("keydown", (e) => {
        if (lightbox.style.display === "flex") {
            switch (e.key) {
                case "Escape":
                    closeLightboxModal();
                    break;
                case "ArrowLeft":
                    currentIndex = currentIndex > 0 ? currentIndex - 1 : items.length - 1;
                    openLightbox(currentIndex);
                    break;
                case "ArrowRight":
                    currentIndex = currentIndex < items.length - 1 ? currentIndex + 1 : 0;
                    openLightbox(currentIndex);
                    break;
            }
        }
    });
    videoPlaceholder.addEventListener("click", () => {
        hotelVideo.src = "/Videos/ele.mp4";
        hotelVideo.style.display = "block";
        videoPlaceholder.style.display = "none";
    });
    document.getElementById("shareGallery").addEventListener("click", () => {
        if (navigator.share) {
            navigator.share({
                title: "Luxury Hotel Gallery",
                text: "Check out our beautiful hotel gallery!",
                url: window.location.href,
            });
        } else {
            const shareUrl = window.location.href;
            navigator.clipboard.writeText(shareUrl).then(() => {
                showNotification("Gallery link copied to clipboard!");
            });
        }
    });

    document.getElementById("downloadCatalog").addEventListener("click", () => {
        showNotification("Catalog download will start shortly...");
        setTimeout(() => {
            showNotification(
                "Thank you for your interest! Catalog sent to your email."
            );
        }, 2000);
    });

    document.getElementById("viewFavorites").addEventListener("click", () => {
        if (favorites.length === 0) {
            showNotification(
                "No favorites added yet. Click the heart icon on images to add favorites!"
            );
            return;
        }
        const activeFilter = document.querySelector(
            '.gallery-filter[data-filter="all"]'
        );
        activeFilter.click();

        setTimeout(() => {
            items.forEach((item) => {
                const title = item.querySelector(".gallery-item-title").textContent;
                const isFavorite = favorites.some((fav) => fav.title === title);

                if (isFavorite) {
                    item.style.display = "block";
                    item.style.opacity = "1";
                    item.style.transform = "translateY(0)";
                    item.classList.add("favorite-highlight");
                } else {
                    item.style.opacity = "0";
                    item.style.transform = "translateY(30px)";
                    setTimeout(() => {
                        item.style.display = "none";
                    }, 300);
                }
            });
        }, 100);

        showNotification(`Showing ${favorites.length} favorite images`);
    });
    function showNotification(message, type = "info") {
        const notification = document.createElement("div");
        notification.className = `notification notification-${type}`;
        notification.innerHTML = `
                <i class="fas fa-info-circle"></i>
                <span>${message}</span>
                <button class="notification-close">&times;</button>
            `;
        notification.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                background: var(--glass-bg);
                border: 1px solid var(--glass-border);
                border-radius: 10px;
                padding: 15px 20px;
                color: var(--text-light);
                backdrop-filter: blur(10px);
                z-index: 10000;
                display: flex;
                align-items: center;
                gap: 10px;
                min-width: 300px;
                box-shadow: 0 10px 30px var(--shadow-color);
                animation: slideInRight 0.3s ease;
                max-width: 400px;
            `;

        document.body.appendChild(notification);
        const closeBtn = notification.querySelector(".notification-close");
        closeBtn.style.cssText = `
                background: none;
                border: none;
                color: var(--text-light);
                font-size: 1.2rem;
                cursor: pointer;
                padding: 0;
                margin-left: auto;
            `;

        closeBtn.addEventListener("click", () => {
            notification.style.animation = "slideOutRight 0.3s ease forwards";
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.parentNode.removeChild(notification);
                }
            }, 300);
        });
        setTimeout(() => {
            if (notification.parentNode) {
                notification.style.animation = "slideOutRight 0.3s ease forwards";
                setTimeout(() => {
                    if (notification.parentNode) {
                        notification.parentNode.removeChild(notification);
                    }
                }, 300);
            }
        }, 5000);
    }
    const notificationStyles = document.createElement("style");
    notificationStyles.textContent = `
            @keyframes slideInRight {
                from {
                    transform: translateX(100%);
                    opacity: 0;
                }
                to {
                    transform: translateX(0);
                    opacity: 1;
                }
            }
            
            @keyframes slideOutRight {
                from {
                    transform: translateX(0);
                    opacity: 1;
                }
                to {
                    transform: translateX(100%);
                    opacity: 0;
                }
            }
            
            .favorite-highlight {
                animation: pulse 0.6s ease-in-out;
            }
            
            @keyframes pulse {
                0%, 100% { transform: scale(1); }
                50% { transform: scale(1.05); }
            }
            
            .notification-info i {
                color: var(--accent-blue);
            }
            
            .notification-success i {
                color: var(--success-green);
            }
            
            .notification-warning i {
                color: var(--secondary-gold);
            }
        `;
    document.head.appendChild(notificationStyles);
    const observerOptions = {
        threshold: 0.1,
        rootMargin: "0px 0px -50px 0px",
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add("fade-in");
            }
        });
    }, observerOptions);
    items.forEach((item) => {
        observer.observe(item);
    });
    const imageObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                const img = entry.target;
                if (img.dataset.src) {
                    img.src = img.dataset.src;
                    img.removeAttribute("data-src");
                    imageObserver.unobserve(img);
                }
            }
        });
    });
    const images = document.querySelectorAll(".gallery-item img");

    images.forEach((img) => {
        const parent = img.parentElement;
        parent.style.display = 'flex';
        parent.style.justifyContent = 'center';
        parent.style.alignItems = 'center';

        imageObserver.observe(img);
    });

    function smoothScroll(target) {
        target.scrollIntoView({
            behavior: "smooth",
            block: "center",
        });
    }
    function initializeGallery() {
        updateItemVisibility();
        const galleryGrid = document.getElementById("galleryGrid");
        galleryGrid.classList.add("fade-in");
        items.forEach((item, index) => {
            if (index < itemsToShow) {
                setTimeout(() => {
                    item.classList.add("slide-up");
                }, index * 100);
            }
        });
    }
    let touchStartX = 0;
    let touchEndX = 0;

    lightbox.addEventListener("touchstart", (e) => {
        touchStartX = e.changedTouches[0].screenX;
    });

    lightbox.addEventListener("touchend", (e) => {
        touchEndX = e.changedTouches[0].screenX;
        handleSwipe();
    });

    function handleSwipe() {
        const swipeThreshold = 50;
        const diff = touchStartX - touchEndX;

        if (Math.abs(diff) > swipeThreshold) {
            if (diff > 0) {
                nextBtn.click();
            } else {
                prevBtn.click();
            }
        }
    }
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }
    const debouncedSearch = debounce((searchTerm) => {
        const activeFilter = document
            .querySelector(".gallery-filter.active")
            .getAttribute("data-filter");
        filterItems(activeFilter, searchTerm);
    }, 300);

    searchInput.addEventListener("input", (e) => {
        debouncedSearch(e.target.value.toLowerCase());
    });
    initializeGallery();
    setTimeout(() => {
        showNotification(
            "Welcome to our luxury hotel gallery! Use filters to explore different areas.",
            "info"
        );
    }, 1000);
});
