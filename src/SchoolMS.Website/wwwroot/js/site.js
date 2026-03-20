// SchoolMS Marketing Website - site.js
document.addEventListener('DOMContentLoaded', function () {

    // Initialize AOS
    if (typeof AOS !== 'undefined') {
        AOS.init({ duration: 700, once: true, offset: 80 });
    }

    // ---- Navbar scroll effect ----
    var navbar = document.querySelector('.website-navbar');
    if (navbar) {
        window.addEventListener('scroll', function () {
            navbar.classList.toggle('scrolled', window.scrollY > 50);
        });
    }

    // ---- Smooth scroll for anchor links ----
    document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
        anchor.addEventListener('click', function (e) {
            var target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                var offset = 80;
                var pos = target.getBoundingClientRect().top + window.pageYOffset - offset;
                window.scrollTo({ top: pos, behavior: 'smooth' });
            }
        });
    });

    // ---- Counter animation ----
    var counters = document.querySelectorAll('.counter');
    var observed = new Set();
    if (counters.length > 0 && 'IntersectionObserver' in window) {
        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting && !observed.has(entry.target)) {
                    observed.add(entry.target);
                    var el = entry.target;
                    var target = parseInt(el.getAttribute('data-target') || '0', 10);
                    var duration = 2000;
                    var start = 0;
                    var startTime = null;
                    function step(timestamp) {
                        if (!startTime) startTime = timestamp;
                        var progress = Math.min((timestamp - startTime) / duration, 1);
                        var eased = 1 - Math.pow(1 - progress, 3);
                        el.textContent = Math.floor(eased * target).toLocaleString();
                        if (progress < 1) requestAnimationFrame(step);
                        else el.textContent = target.toLocaleString();
                    }
                    requestAnimationFrame(step);
                }
            });
        }, { threshold: 0.3 });
        counters.forEach(function (c) { observer.observe(c); });
    }

    // ---- FAQ accordion ----
    document.querySelectorAll('.faq-question').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var item = this.closest('.faq-item');
            var isOpen = item.classList.contains('open');
            // Close all
            document.querySelectorAll('.faq-item.open').forEach(function (el) {
                el.classList.remove('open');
            });
            // Toggle current
            if (!isOpen) item.classList.add('open');
        });
    });

    // ---- Pricing toggle ----
    var billingToggle = document.getElementById('billingToggle');
    if (billingToggle) {
        billingToggle.addEventListener('change', function () {
            var yearly = this.checked;
            document.querySelectorAll('.monthly-price').forEach(function (el) {
                el.style.display = yearly ? 'none' : 'inline';
            });
            document.querySelectorAll('.yearly-price').forEach(function (el) {
                el.style.display = yearly ? 'inline' : 'none';
            });
        });
    }

    // ---- Registration form submit ----
    var registerForm = document.getElementById('schoolRegisterForm');
    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            e.preventDefault();
            var btn = registerForm.querySelector('button[type="submit"]');
            var originalText = btn.innerHTML;
            btn.disabled = true;
            btn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>جارٍ الإرسال...';

            var formData = new FormData(registerForm);
            var data = {};
            formData.forEach(function (value, key) {
                if (key !== '__RequestVerificationToken') data[key] = value;
            });

            var token = document.querySelector('input[name="__RequestVerificationToken"]');

            fetch('/Home/Register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token ? token.value : ''
                },
                body: JSON.stringify(data)
            })
            .then(function (res) {
                if (res.ok) return res.json();
                throw new Error('Failed');
            })
            .then(function () {
                btn.innerHTML = '<i class="fas fa-check me-2"></i>تم الإرسال بنجاح!';
                btn.classList.remove('btn-gradient');
                btn.style.background = '#22c55e';
                registerForm.reset();
                setTimeout(function () {
                    btn.disabled = false;
                    btn.innerHTML = originalText;
                    btn.style.background = '';
                    btn.classList.add('btn-gradient');
                }, 4000);
            })
            .catch(function () {
                btn.innerHTML = '<i class="fas fa-times me-2"></i>حدث خطأ، حاول مرة أخرى';
                btn.style.background = '#ef4444';
                setTimeout(function () {
                    btn.disabled = false;
                    btn.innerHTML = originalText;
                    btn.style.background = '';
                }, 3000);
            });
        });
    }

    // ---- Mobile menu toggle ----
    var mobileBtn = document.querySelector('.mobile-menu-btn');
    if (mobileBtn) {
        mobileBtn.addEventListener('click', function () {
            var links = document.querySelector('.nav-links');
            var actions = document.querySelector('.nav-actions');
            if (links) links.style.display = links.style.display === 'flex' ? 'none' : 'flex';
            if (actions) actions.style.display = actions.style.display === 'flex' ? 'none' : 'flex';
        });
    }
});
