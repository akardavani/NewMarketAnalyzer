// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//$(function () {
//    // فعال‌سازی Select2
//    $('.select2').select2({
//        placeholder: "نمادها را انتخاب کنید",
//        allowClear: true,
//        width: '100%'
//    });

//    // فعال‌سازی تقویم شمسی
//    $('.persian-date').persianDatepicker({
//        format: 'YYYY/MM/DD',
//        initialValue: false,
//        autoClose: true
//    });
//});


$(function () {
    $('#symbolsSelect').select2({
        placeholder: "نمادها را انتخاب کنید",
        allowClear: true,
        width: '100%'
    });

    // فعال‌سازی تقویم شمسی با زبان فارسی
    $("#startDate").persianDatepicker({
        format: 'YYYY/MM/DD',
        initialValue: false,
        autoClose: true,
        calendar: {
            persian: {
                locale: 'fa' // زبان فارسی
            }
        }
    });

    $("#endDate").persianDatepicker({
        format: 'YYYY/MM/DD',
        initialValue: false,
        autoClose: true,
        calendar: {
            persian: {
                locale: 'fa' // زبان فارسی
            }
        }
    });
});

