document.addEventListener("DOMContentLoaded", function () {
    $('.ui-data-table').each(function () {
        const table = $(this);

        if ($.fn.DataTable.isDataTable(table)) {
            return;
        }

        const searching = table.data('searching') === true || table.data('searching') === "true";
        const paging = table.data('paging') === true || table.data('paging') === "true";
        const ordering = table.data('ordering') === true || table.data('ordering') === "true";
        const pageSize = parseInt(table.data('page-size')) || 10;

        table.DataTable({
            pageLength: pageSize,
            lengthMenu: [5, 10, 25, 50, 100],
            ordering: ordering,
            searching: searching,
            paging: paging,
            responsive: true,
            language: {
                search: "جستجو:",
                lengthMenu: "نمایش _MENU_ ردیف",
                info: "نمایش _START_ تا _END_ از _TOTAL_ ردیف",
                infoEmpty: "داده‌ای برای نمایش وجود ندارد",
                infoFiltered: "(فیلتر شده از _MAX_ ردیف)",
                zeroRecords: "موردی پیدا نشد",
                paginate: {
                    first: "اول",
                    last: "آخر",
                    next: "بعدی",
                    previous: "قبلی"
                }
            }
        });
    });
});
