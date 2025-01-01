var dataTable;


$(document).ready(function () {

    // get the status from the query pramters
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get('status');
    loadDataTable(status);

});


function loadDataTable(status) {

    const Url = !status ? '/booking/getall' : '/booking/getall?status=' + status;
    dataTable = $('#tblBookings').DataTable({
        "ajax": {
            url: Url
        },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phone', "width": "10%" },
            { data: 'email', "width": "15%" },
            { data: 'status', "width": "10%" },
            { data: 'checkInDate', "width": "10%" },
            { data: 'nights', "width": "10%" },
            { data: 'totalCost', render: $.fn.dataTable.render.number(',', '.', 2), "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group">
                        <a href="/booking/bookingDetails?bookingId=${data}" class="btn btn-outline-warning mx-2">
                            <i class="bi bi-pencil-square"></i> Details
                        </a>
                    </div>`
                }
            }
        ]
    });
}