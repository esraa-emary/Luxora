
    class EnhancedLoginForm {
        constructor() {
        this.form = document.getElementById('loginForm');
    this.emailInput = document.getElementById('email');
    this.passwordInput = document.getElementById('password');
    this.passwordToggle = document.getElementById('passwordToggle');
    this.submitBtn = document.getElementById('submitBtn');
    this.rememberMe = document.getElementById('rememberMe');

    this.isLoading = false;
    this.validationRules = this.initValidationRules();

    this.init();
            }

    init() {
        this.setupEventListeners();
    this.createParticles();
    this.setupFormValidation();
    this.loadSavedCredentials();
    this.setupAccessibility();
            }

    initValidationRules() {
                return {
        email: {
        required: true,
    pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
    message: 'Please enter a valid email address'
                    },
    password: {
        required: true,
    minLength: 6,
    message: 'Password must be at least 6 characters long'
                    }
                };
            }

    setupEventListeners() {
        this.form.addEventListener('submit', this.handleSubmit.bind(this));
    this.passwordToggle.addEventListener('click', this.togglePasswordVisibility.bind(this));
                this.emailInput.addEventListener('blur', () => this.validateField('email'));
                this.passwordInput.addEventListener('blur', () => this.validateField('password'));
                this.emailInput.addEventListener('input', () => this.clearValidationError('email'));
                this.passwordInput.addEventListener('input', () => this.clearValidationError('password'));
                document.getElementById('googleSignIn').addEventListener('click', () => this.handleSocialLogin('google'));
                document.getElementById('facebookSignIn').addEventListener('click', () => this.handleSocialLogin('facebook'));
    document.getElementById('forgotPasswordLink').addEventListener('click', this.handleForgotPassword.bind(this));
    document.addEventListener('keydown', this.handleKeyboardShortcuts.bind(this));
            }

    setupFormValidation() {
        this.form.setAttribute('novalidate', 'true');
                [this.emailInput, this.passwordInput].forEach(input => {
        input.addEventListener('focus', (e) => {
            e.target.parentElement.style.transform = 'scale(1.02)';
        });
                    
                    input.addEventListener('blur', (e) => {
        e.target.parentElement.style.transform = 'scale(1)';
                    });
                });
            }

    validateField(fieldName) {
                const input = document.getElementById(fieldName);
    const rule = this.validationRules[fieldName];
    const value = input.value.trim();

    let isValid = true;
    let message = '';

    if (rule.required && !value) {
        isValid = false;
    message = `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
                } else if (value && rule.pattern && !rule.pattern.test(value)) {
        isValid = false;
    message = rule.message;
                } else if (value && rule.minLength && value.length < rule.minLength) {
        isValid = false;
    message = rule.message;
                }

    this.showValidationResult(fieldName, isValid, message);
    return isValid;
            }

    showValidationResult(fieldName, isValid, message) {
                const input = document.getElementById(fieldName);
    const errorElement = document.getElementById(`${fieldName}Error`);

    input.classList.remove('error', 'success');
    errorElement.classList.remove('show', 'error', 'success');

    if (!isValid && message) {
        input.classList.add('error');
    errorElement.classList.add('show', 'error');
    errorElement.querySelector('span').textContent = message;
                } else if (isValid && input.value.trim()) {
        input.classList.add('success');
    errorElement.classList.add('show', 'success');
    errorElement.innerHTML = '<i class="fas fa-check-circle"></i><span>Looks good!</span>';
                }
            }

    clearValidationError(fieldName) {
                const input = document.getElementById(fieldName);
    const errorElement = document.getElementById(`${fieldName}Error`);

    input.classList.remove('error');
    errorElement.classList.remove('show');
            }

    togglePasswordVisibility() {
                const isPassword = this.passwordInput.type === 'password';
    this.passwordInput.type = isPassword ? 'text' : 'password';
    this.passwordToggle.className = isPassword ? 'fas fa-eye-slash password-toggle' : 'fas fa-eye password-toggle';
    this.passwordInput.setAttribute('aria-label',
    isPassword ? 'Password visible' : 'Password hidden'
    );
            }

    async handleSubmit(event) {
    // Validate all fields before submission
    const isEmailValid = this.validateField('email');
    const isPasswordValid = this.validateField('password');

    if (!isEmailValid || !isPasswordValid) {
        event.preventDefault();
        this.showNotification('Please correct the errors below', 'error');
        return;
    }

    // Save credentials if remember me is checked
    if (this.rememberMe.checked) {
        this.saveCredentials();
    }

    // Let the form submit normally to the server
    // The AuthController will handle the login
            }

    async simulateLogin() {
                return new Promise((resolve, reject) => {
        setTimeout(() => {
            const email = this.emailInput.value;
            const password = this.passwordInput.value;

            if (email === 'admin@bookify.com' && password === 'admin123') {
                resolve({ success: true, user: { email, name: 'Admin User' } });
            } else if (password.length < 6) {
                reject(new Error('Password must be at least 6 characters'));
            } else {
                Math.random() > 0.3 ? resolve({ success: true }) : reject(new Error('Invalid credentials'));
            }
        }, 2000);
                });
            }

    setLoadingState(loading) {
        this.isLoading = loading;
    this.submitBtn.classList.toggle('loading', loading);
    this.submitBtn.disabled = loading;

    const btnText = this.submitBtn.querySelector('.btn-text');
    btnText.textContent = loading ? 'Signing In...' : 'Sign In';
            }

    handleSocialLogin(provider) {
        this.showNotification(`Redirecting to ${provider.charAt(0).toUpperCase() + provider.slice(1)}...`, 'warning');
                setTimeout(() => {
        window.open(`https://${provider}.com/oauth/authorize?client_id=bookify`, '_blank');
                }, 1000);
            }

    handleForgotPassword(event) {
        event.preventDefault();

    const email = this.emailInput.value;
    if (!email) {
        this.showNotification('Please enter your email address first', 'warning');
    this.emailInput.focus();
    return;
                }

    if (!this.validationRules.email.pattern.test(email)) {
        this.showNotification('Please enter a valid email address', 'error');
    return;
                }

    this.showNotification(`Password reset link sent to ${email}`, 'success');
            }

    saveCredentials() {
                const credentials = {
        email: this.emailInput.value,
    rememberMe: true,
    timestamp: Date.now()
                };

    localStorage.setItem('bookify_credentials', JSON.stringify(credentials));
            }

    loadSavedCredentials() {
                const saved = localStorage.getItem('bookify_credentials');
    if (!saved) return;

    try {
                    const credentials = JSON.parse(saved);
    const thirtyDaysAgo = Date.now() - (30 * 24 * 60 * 60 * 1000);
                    
                    if (credentials.timestamp > thirtyDaysAgo) {
        this.emailInput.value = credentials.email;
    this.rememberMe.checked = credentials.rememberMe;
                    } else {
        localStorage.removeItem('bookify_credentials');
                    }
                } catch (error) {
        localStorage.removeItem('bookify_credentials');
                }
            }

    createParticles() {
                const container = document.getElementById('particlesContainer');
    const particleCount = 50;

    for (let i = 0; i < particleCount; i++) {
                    const particle = document.createElement('div');
    particle.className = 'particle';
    const size = Math.random() * 4 + 2;
    const delay = Math.random() * 8;
    const duration = Math.random() * 5 + 8;
    const startX = Math.random() * 100;

    particle.style.width = `${size}px`;
    particle.style.height = `${size}px`;
    particle.style.left = `${startX}%`;
    particle.style.animationDelay = `${delay}s`;
    particle.style.animationDuration = `${duration}s`;

    container.appendChild(particle);
                }
            }

    setupAccessibility() {
        this.emailInput.setAttribute('aria-describedby', 'emailError');
    this.passwordInput.setAttribute('aria-describedby', 'passwordError');
                this.form.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter' && e.target.tagName === 'INPUT') {
                        const inputs = Array.from(this.form.querySelectorAll('input[type="email"], input[type="password"]'));
    const currentIndex = inputs.indexOf(e.target);

    if (currentIndex < inputs.length - 1) {
        e.preventDefault();
    inputs[currentIndex + 1].focus();
                        }
                    }
                });
            }

    handleKeyboardShortcuts(event) {
                if (event.altKey && event.key === 'l') {
        event.preventDefault();
    this.emailInput.focus();
                }
    if (event.altKey && event.key === 'p') {
        event.preventDefault();
    this.passwordInput.focus();
                }
    if (event.ctrlKey && event.key === 'Enter') {
        event.preventDefault();
    if (!this.isLoading) {
        this.form.dispatchEvent(new Event('submit'));
                    }
                }
            }

    showNotification(message, type = 'info') {
                const existing = document.querySelectorAll('.notification');
                existing.forEach(n => n.remove());

    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.textContent = message;

    document.body.appendChild(notification);

                setTimeout(() => notification.classList.add('show'), 100);
                setTimeout(() => {
        notification.classList.remove('show');
                    setTimeout(() => notification.remove(), 400);
                }, 5000);
            }
        }
        document.addEventListener('DOMContentLoaded', () => {
        new EnhancedLoginForm();
        });

        document.addEventListener('visibilitychange', () => {
            const particles = document.querySelectorAll('.particle');
            particles.forEach(particle => {
        particle.style.animationPlayState = document.hidden ? 'paused' : 'running';
            });
        });
        const preloadResources = () => {
            const links = [
    'https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&family=Playfair+Display:wght@400;600;700&display=swap',
    'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css'
    ];
            
            links.forEach(href => {
                const link = document.createElement('link');
    link.rel = 'preload';
    link.as = 'style';
    link.href = href;
    document.head.appendChild(link);
            });
        };

    preloadResources();
