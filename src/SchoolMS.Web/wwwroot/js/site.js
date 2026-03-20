// Site.js - School Management System v3.0
$(document).ready(function () {
    // Initialize AOS
    if (typeof AOS !== 'undefined') {
        AOS.init({ duration: 600, once: true, offset: 60 });
    }

    // Sidebar Toggle (responsive + desktop collapse)
    var isRtl = document.documentElement.getAttribute('dir') === 'rtl';
    var sidebarWidth = getComputedStyle(document.documentElement).getPropertyValue('--sidebar-width').trim() || '280px';

    $('#sidebarToggle').click(function () {
        var sidebar = $('#sidebar');
        if ($(window).width() < 992) {
            sidebar.toggleClass('show');
        } else {
            sidebar.toggleClass('collapsed');
            var collapsed = sidebar.hasClass('collapsed');
            if (isRtl) {
                $('#content').css('margin-right', collapsed ? '0' : sidebarWidth);
            } else {
                $('#content').css('margin-left', collapsed ? '0' : sidebarWidth);
            }
            sidebar.css('width', collapsed ? '0' : sidebarWidth);
        }
    });

    $('#sidebarClose').click(function () {
        $('#sidebar').removeClass('show');
    });

    // Close sidebar on overlay click (mobile)
    $(document).on('click', function (e) {
        if ($(window).width() < 992 && $('#sidebar').hasClass('show')) {
            if (!$(e.target).closest('#sidebar, #sidebarToggle').length) {
                $('#sidebar').removeClass('show');
            }
        }
    });

    // Keyboard shortcut: Ctrl+K for search focus
    $(document).on('keydown', function (e) {
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            var searchInput = $('#globalSearch input, #menuSearch');
            if (searchInput.length) searchInput.first().focus();
        }
    });

    // Toastr Configuration
    if (typeof toastr !== 'undefined') {
        toastr.options = {
            closeButton: true,
            progressBar: true,
            positionClass: isRtl ? "toast-top-left" : "toast-top-right",
            timeOut: 3000,
            showMethod: "fadeIn",
            hideMethod: "fadeOut"
        };
    }

    // Initialize Select2
    if ($.fn.select2) {
        $('.select2').select2({ theme: 'bootstrap-5', width: '100%', dir: isRtl ? 'rtl' : 'ltr' });
    }

    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(function (el) {
        new bootstrap.Tooltip(el);
    });
});

// Counter Animation
function animateCounter(element, target) {
    var current = 0;
    var increment = target / 50;
    var timer = setInterval(function () {
        current += increment;
        if (current >= target) { current = target; clearInterval(timer); }
        $(element).text(Math.floor(current).toLocaleString());
    }, 30);
}

// Confirm Delete with SweetAlert2
function confirmDelete(url, id, callback) {
    Swal.fire({
        title: 'Are you sure?',
        text: "This action cannot be undone!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#e94560',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete it!'
    }).then(function (result) {
        if (result.isConfirmed) {
            $.ajax({
                url: url + '/' + id,
                type: 'DELETE',
                headers: { 'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() },
                success: function () {
                    toastr.success('Deleted successfully!');
                    if (callback) callback();
                },
                error: function () { toastr.error('An error occurred while deleting.'); }
            });
        }
    });
}
