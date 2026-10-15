/* Navbar Dropdown Submenus */

$(".open-nav-menu, .close-nav-menu, .menu-overlay").on("click", toggleNav);
$(".nav-menu").on("click", function (event) {
  if ($(event.target).is("[data-toggle]") && window.innerWidth <= 1200) {
    event.preventDefault();
    const menuItemHasChildren = $(event.target).parent(".menu-item-has-children");
    // if menuItemHasChildren is already expanded, collapse it
    if (menuItemHasChildren.hasClass("active")) {
      collapseSubMenu();
    }
    else {
      // collapse existing expanded menuItemHasChildren
      if ($(".nav-menu").find(".menu-item-has-children.active")) {
        collapseSubMenu();
      }
      // expand new menuItemHasChildren
      menuItemHasChildren.addClass("active");
      expandSubMenu(menuItemHasChildren);
    }
  }
});

function toggleNav() {
  $(".nav-menu").toggleClass("open");
  $(".menu-overlay").toggleClass("active");
  $(document.body).toggleClass("hidden-scrolling");
}

function collapseSubMenu() {
  $(".nav-menu").find(".menu-item-has-children.active .sub-menu")
    .removeAttr("style");
  $(".nav-menu").find(".menu-item-has-children.active")
    .removeClass("active");
}

function expandSubMenu(menuItemHasChildren) {
  const subMenu = menuItemHasChildren.find(".sub-menu");
  if (subMenu.length) {
    subMenu.css("max-height", subMenu[0].scrollHeight + "px");
  }
}

function resizeFix() {
  // if navMenu is open ,close it
  if ($(".nav-menu").hasClass("open")) {
    toggleNav();
  }
  // if menuItemHasChildren is expanded , collapse it
  if ($(".nav-menu").find(".menu-item-has-children.active")) {
    collapseSubMenu();
  }
}

$(window).on("resize", () => {
  if (window.innerWidth > 1200) {
    resizeFix();
  }
});

/*Back to top button*/
const backToTopBtn = $("#btn-back-to-top");

$(window).on("scroll", () => {
  scrollFunction();
});

function scrollFunction() {
  const scrollThreshold = 150;
  if (
    $(document).scrollTop() > scrollThreshold || //when user scrolls 150px down from the top
    $("html, body").scrollTop() > scrollThreshold) {
    backToTopBtn.css("display", "block");
  } else {
    backToTopBtn.css("display", "none");
  }
}

backToTopBtn.on("click", () => {
  $("body, html").animate({ scrollTop: 0 }, "fast");
});

/*Refresh captcha*/
$(function () {
  // register date picker
  jalaliDatepicker.startWatch({
    minDate: "attr",
    maxDate: "attr",
    time: false,
  });

  $(document).on("click", "[data-refresh-captcha]", function (event) {
    event.preventDefault();
    let newImageUrl = "/captcha?v=" + Date.now();
    $("#captchaImage").attr("src", newImageUrl);
  });
});

/* Toggle password visibility*/
$(document).on("click", "[data-password-visibility]", function () {
  const passwordInput = $(this).siblings(".password-input");
  if (passwordInput.attr("type") === "password") {
    passwordInput.attr("type", "text");
    $(this).html('<svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" fill="currentColor" class="bi bi-eye-slash" viewBox="0 0 16 16"><path d ="M13.359 11.238C15.06 9.72 16 8 16 8s-3-5.5-8-5.5a7.028 7.028 0 0 0-2.79.588l.77.771A5.944 5.944 0 0 1 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13.134 13.134 0 0 1 14.828 8c-.058.087-.122.183-.195.288-.335.48-.83 1.12-1.465 1.755-.165.165-.337.328-.517.486z" /><path d="M11.297 9.176a3.5 3.5 0 0 0-4.474-4.474l.823.823a2.5 2.5 0 0 1 2.829 2.829zm-2.943 1.299.822.822a3.5 3.5 0 0 1-4.474-4.474l.823.823a2.5 2.5 0 0 0 2.829 2.829"/><path d="M3.35 5.47c-.18.16-.353.322-.518.487A13.134 13.134 0 0 0 1.172 8l.195.288c.335.48.83 1.12 1.465 1.755C4.121 11.332 5.881 12.5 8 12.5c.716 0 1.39-.133 2.02-.36l.77.772A7.029 7.029 0 0 1 8 13.5C3 13.5 0 8 0 8s.939-1.721 2.641-3.238l.708.709zm10.296 8.884-12-12 .708-.708 12 12-.708.708"/></svg > ');
  } else {
    passwordInput.attr("type", "password");
    $(this).html('<svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" fill="currentColor" class="bi bi-eye" viewBox="0 0 16 16"><path d ="M16 8s-3-5.5-8-5.5S0 8 0 8s3 5.5 8 5.5S16 8 16 8M1.173 8a13.133 13.133 0 0 1 1.66-2.043C4.12 4.668 5.88 3.5 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13.133 13.133 0 0 1 14.828 8c-.058.087-.122.183-.195.288-.335.48-.83 1.12-1.465 1.755C11.879 11.332 10.119 12.5 8 12.5c-2.12 0-3.879-1.168-5.168-2.457A13.134 13.134 0 0 1 1.172 8z" /><path d="M8 5.5a2.5 2.5 0 1 0 0 5 2.5 2.5 0 0 0 0-5M4.5 8a3.5 3.5 0 1 1 7 0 3.5 3.5 0 0 1-7 0" /></svg >');
  }
});

/* Resend OTP code */
$(function () {
  const MILI_SECONDS = 1000;
  const MINUTS_TICK = 60 * MILI_SECONDS;
  const otpTimer = $('.otp-timer');
  const url = $('#ResendOtpHandlerUrl').data("ajax-url");
  startOtpTimer(Number($('#OtpLifetime').val()));

  $(document).on('click', '[data-otp-invoke]', function () {
    $('[data-otp-invoke]').attr('disabled', 'disabled');
    $.post(url, $("#OtpForm").serialize(),
      function (response) {
        $('[data-otp-invoke]').addClass('d-none');
        otpTimer.removeClass('d-none');
        $('#OtpLifetime').val(response.data);
        startOtpTimer(response.data);
      })
      .fail(function () {
        otpTimer.addClass('d-none');
        $('[data-otp-invoke]').removeClass('d-none');
        //TODO:پیغام خطا
      })
      .done(function () {
        $('[data-otp-invoke]').removeAttr('disabled');
      });
  });

  function startOtpTimer(lifetime) {
    $('#OtpLifetime').val(lifetime);

    if (lifetime > 0) {
      const minutes = Math.trunc(lifetime / MINUTS_TICK);
      const seconds = Math.trunc((lifetime - (minutes * MINUTS_TICK)) / 1000);
      otpTimer.text(`${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`);

      setTimeout(() => {
        lifetime -= 1000;
        startOtpTimer(lifetime);
      }, 1000);
    }
    else {
      otpTimer.addClass('d-none');
      $('[data-otp-invoke]').removeClass('d-none');
    }
  }

});

