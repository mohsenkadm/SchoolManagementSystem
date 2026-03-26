// SchoolMS Student Portal - portal.js
document.addEventListener('DOMContentLoaded', function () {

    // ---- Theme Toggle ----
    var savedTheme = localStorage.getItem('sp-theme') || 'dark';
    document.documentElement.setAttribute('data-theme', savedTheme);
    updateThemeIcon(savedTheme);

    var themeBtn = document.getElementById('themeToggle');
    if (themeBtn) {
        themeBtn.addEventListener('click', function () {
            var current = document.documentElement.getAttribute('data-theme') || 'dark';
            var next = current === 'dark' ? 'light' : 'dark';
            document.documentElement.setAttribute('data-theme', next);
            localStorage.setItem('sp-theme', next);
            updateThemeIcon(next);
        });
    }

    function updateThemeIcon(theme) {
        var icon = document.querySelector('#themeToggle i');
        if (icon) {
            icon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
        }
    }

    // ---- Mobile Sidebar ----
    var menuToggle = document.querySelector('.sp-menu-toggle');
    var sidebar = document.querySelector('.sp-sidebar');
    var overlay = document.querySelector('.sp-sidebar-overlay');

    if (menuToggle && sidebar) {
        menuToggle.addEventListener('click', function () {
            sidebar.classList.toggle('open');
        });
    }
    if (overlay && sidebar) {
        overlay.addEventListener('click', function () {
            sidebar.classList.remove('open');
        });
    }

    // ---- Tab System ----
    document.querySelectorAll('.sp-tab-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var target = this.getAttribute('data-tab');
            var parent = this.closest('.sp-tabs');
            parent.querySelectorAll('.sp-tab-btn').forEach(function (b) { b.classList.remove('active'); });
            parent.querySelectorAll('.sp-tab-content').forEach(function (c) { c.classList.remove('active'); });
            this.classList.add('active');
            var panel = parent.querySelector('#' + target);
            if (panel) panel.classList.add('active');
        });
    });

    // ---- Star Rating ----
    document.querySelectorAll('.sp-stars').forEach(function (container) {
        var stars = container.querySelectorAll('i');
        stars.forEach(function (star, index) {
            star.addEventListener('click', function () {
                var rating = index + 1;
                var videoId = container.getAttribute('data-video-id');
                stars.forEach(function (s, i) {
                    s.classList.toggle('filled', i < rating);
                });
                fetch('/Courses/Rate', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body: 'videoId=' + videoId + '&rating=' + rating
                });
            });
            star.addEventListener('mouseenter', function () {
                stars.forEach(function (s, i) {
                    s.style.color = i <= index ? '#fbbf24' : '';
                });
            });
            star.addEventListener('mouseleave', function () {
                stars.forEach(function (s) {
                    s.style.color = '';
                });
            });
        });
    });

    // ---- Mark as Seen ----
    document.querySelectorAll('.btn-mark-seen').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            var videoId = this.getAttribute('data-video-id');
            var item = this.closest('.sp-video-item');
            fetch('/Courses/MarkSeen', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: 'videoId=' + videoId
            }).then(function () {
                if (item) item.classList.add('seen');
                btn.innerHTML = '<i class="fas fa-check"></i>';
                btn.disabled = true;
            });
        });
    });

    // ---- Comment Form ----
    document.querySelectorAll('.sp-comment-form').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            var input = form.querySelector('input, textarea');
            var videoId = form.getAttribute('data-video-id');
            var comment = input.value.trim();
            if (!comment) return;
            var btn = form.querySelector('button');
            btn.disabled = true;
            btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
            fetch('/Courses/AddComment', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: 'videoId=' + videoId + '&comment=' + encodeURIComponent(comment)
            }).then(function () {
                var list = form.closest('.sp-tab-content').querySelector('.sp-comment-list');
                if (list) {
                    var item = document.createElement('div');
                    item.className = 'sp-comment-item';
                    item.innerHTML = '<div class="sp-avatar" style="width:32px;height:32px;font-size:0.7rem;"><i class="fas fa-user"></i></div><div class="sp-comment-body"><div class="sp-comment-name">أنت</div><div class="sp-comment-text">' + escapeHtml(comment) + '</div><div class="sp-comment-date">الآن</div></div>';
                    list.prepend(item);
                }
                input.value = '';
                btn.disabled = false;
                btn.innerHTML = '<i class="fas fa-paper-plane"></i> إرسال';
            });
        });
    });

    // ---- Note Form ----
    document.querySelectorAll('.sp-note-form').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            var input = form.querySelector('textarea');
            var videoId = form.getAttribute('data-video-id');
            var noteText = input.value.trim();
            if (!noteText) return;
            var btn = form.querySelector('button');
            btn.disabled = true;
            fetch('/Courses/AddNote', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: 'videoId=' + videoId + '&noteText=' + encodeURIComponent(noteText)
            }).then(function () {
                var list = form.closest('.sp-tab-content').querySelector('.sp-note-list');
                if (list) {
                    var item = document.createElement('div');
                    item.className = 'sp-comment-item';
                    item.innerHTML = '<div class="sp-comment-body"><div class="sp-comment-text">' + escapeHtml(noteText) + '</div><div class="sp-comment-date">الآن</div></div>';
                    list.prepend(item);
                }
                input.value = '';
                btn.disabled = false;
            });
        });
    });

    // ---- Quiz ----
    document.querySelectorAll('.sp-quiz-option').forEach(function (option) {
        option.addEventListener('click', function () {
            var quizItem = this.closest('.sp-quiz-item');
            if (quizItem.classList.contains('answered')) return;
            var questionId = quizItem.getAttribute('data-question-id');
            var answer = this.getAttribute('data-answer');
            quizItem.querySelectorAll('.sp-quiz-option').forEach(function (o) { o.classList.remove('selected'); });
            this.classList.add('selected');

            fetch('/Courses/SubmitQuiz', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: 'questionId=' + questionId + '&answer=' + encodeURIComponent(answer)
            }).then(function (r) { return r.json(); })
            .then(function (data) {
                quizItem.classList.add('answered');
                if (data.isCorrect) {
                    option.classList.add('correct');
                    showQuizResult(quizItem, true);
                } else {
                    option.classList.add('wrong');
                    var correct = quizItem.querySelector('.sp-quiz-option[data-correct="true"]');
                    if (correct) correct.classList.add('correct');
                    showQuizResult(quizItem, false);
                }
            });
        });
    });

    function showQuizResult(item, isCorrect) {
        var existing = item.querySelector('.sp-quiz-result');
        if (existing) existing.remove();
        var div = document.createElement('div');
        div.className = 'sp-quiz-result ' + (isCorrect ? 'correct' : 'wrong');
        div.innerHTML = isCorrect
            ? '<i class="fas fa-check-circle"></i> إجابة صحيحة!'
            : '<i class="fas fa-times-circle"></i> إجابة خاطئة';
        item.appendChild(div);
    }

    function escapeHtml(text) {
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(text));
        return div.innerHTML;
    }

    // ---- Notification Dropdown ----
    var notifBtn = document.getElementById('notifDropdownBtn');
    var notifPanel = document.getElementById('notifDropdownPanel');
    if (notifBtn && notifPanel) {
        notifBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            notifPanel.classList.toggle('show');
        });
        document.addEventListener('click', function () {
            notifPanel.classList.remove('show');
        });
        notifPanel.addEventListener('click', function (e) { e.stopPropagation(); });
    }

    // ---- Scroll Reveal Animations ----
    var revealEls = document.querySelectorAll('.sp-reveal');
    if (revealEls.length > 0) {
        var revealObserver = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('revealed');
                    revealObserver.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12 });
        revealEls.forEach(function (el) { revealObserver.observe(el); });
    }

    // ---- Screen Protection (prevent screenshots & recording) ----
    var protectedArea = document.querySelector('.sp-protected');
    if (protectedArea) {
        // Disable right-click on protected areas
        protectedArea.addEventListener('contextmenu', function (e) { e.preventDefault(); });

        // Disable drag on images/videos
        protectedArea.addEventListener('dragstart', function (e) { e.preventDefault(); });

        // Block PrintScreen & common screenshot shortcuts
        document.addEventListener('keyup', function (e) {
            if (e.key === 'PrintScreen') {
                // Overwrite clipboard with blank
                try { navigator.clipboard.writeText(''); } catch (ex) { }
                document.body.style.filter = 'blur(40px)';
                setTimeout(function () { document.body.style.filter = ''; }, 2000);
            }
        });

        document.addEventListener('keydown', function (e) {
            if (e.key === 'PrintScreen') {
                e.preventDefault();
                document.body.style.filter = 'blur(40px)';
                setTimeout(function () { document.body.style.filter = ''; }, 2000);
            }
            // Ctrl+Shift+S (Win Snipping), Ctrl+Shift+I (DevTools), F12, Ctrl+U
            if ((e.ctrlKey && e.shiftKey && (e.key === 'S' || e.key === 's' || e.key === 'I' || e.key === 'i')) ||
                e.key === 'F12' ||
                (e.ctrlKey && (e.key === 'u' || e.key === 'U')) ||
                (e.ctrlKey && (e.key === 'p' || e.key === 'P')) ||
                (e.ctrlKey && (e.key === 's' || e.key === 'S')) ||
                (e.metaKey && e.shiftKey && (e.key === '3' || e.key === '4' || e.key === '5'))) {
                e.preventDefault();
                document.body.style.filter = 'blur(40px)';
                setTimeout(function () { document.body.style.filter = ''; }, 2000);
            }
        });

        // Blur video when window loses focus (tab switch = possible recording)
        document.addEventListener('visibilitychange', function () {
            var player = document.getElementById('playerArea');
            if (player) {
                if (document.hidden) {
                    player.style.filter = 'blur(30px)';
                    player.style.transition = 'filter 0.3s';
                } else {
                    player.style.filter = '';
                }
            }
        });

        // Blur on window blur (alt-tab, screen recording apps)
        window.addEventListener('blur', function () {
            var player = document.getElementById('playerArea');
            if (player) {
                player.style.filter = 'blur(30px)';
                player.style.transition = 'filter 0.3s';
            }
        });
        window.addEventListener('focus', function () {
            var player = document.getElementById('playerArea');
            if (player) {
                player.style.filter = '';
            }
        });

        // Disable PiP (Picture-in-Picture) on all videos
        document.querySelectorAll('video').forEach(function (v) {
            v.disablePictureInPicture = true;
            v.setAttribute('controlslist', 'nodownload noplaybackrate');
        });

        // Detect screen capture / recording via permissions API
        if (navigator.mediaDevices && navigator.mediaDevices.addEventListener) {
            try {
                navigator.mediaDevices.addEventListener('devicechange', function () {
                    var player = document.getElementById('playerArea');
                    if (player) {
                        player.style.filter = 'blur(30px)';
                        setTimeout(function () { player.style.filter = ''; }, 3000);
                    }
                });
            } catch (ex) { }
        }

        // Override getDisplayMedia to prevent screen capture
        if (navigator.mediaDevices) {
            var origGetDisplayMedia = navigator.mediaDevices.getDisplayMedia;
            navigator.mediaDevices.getDisplayMedia = function () {
                document.body.style.filter = 'blur(40px)';
                return origGetDisplayMedia ? origGetDisplayMedia.apply(this, arguments).then(function (stream) {
                    document.body.style.filter = 'blur(40px)';
                    return stream;
                }) : Promise.reject(new Error('Screen capture blocked'));
            };
        }
    }

    // ---- OTP Input Auto-focus ----
    var otpContainer = document.querySelector('.sp-otp-inputs');
    if (otpContainer) {
        var otpInputs = otpContainer.querySelectorAll('input');
        otpInputs.forEach(function (input, idx) {
            input.addEventListener('input', function () {
                this.value = this.value.replace(/[^0-9]/g, '');
                if (this.value.length === 1 && idx < otpInputs.length - 1) {
                    otpInputs[idx + 1].focus();
                }
            });
            input.addEventListener('keydown', function (e) {
                if (e.key === 'Backspace' && !this.value && idx > 0) {
                    otpInputs[idx - 1].focus();
                }
            });
            input.addEventListener('paste', function (e) {
                e.preventDefault();
                var data = (e.clipboardData || window.clipboardData).getData('text').replace(/[^0-9]/g, '');
                for (var i = 0; i < otpInputs.length && i < data.length; i++) {
                    otpInputs[i].value = data[i];
                }
                var lastIdx = Math.min(data.length, otpInputs.length) - 1;
                if (lastIdx >= 0) otpInputs[lastIdx].focus();
            });
        });
    }
});
