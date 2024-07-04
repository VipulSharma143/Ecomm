var dataTable;
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "orderDate", "width": "25%" },
            { "data": "orderTotal", "width": "15%" },
            { "data": "paymentStatus", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (data) {
                    return `<a href="/Admin/Order/Details/${data}" class="btn btn-success">View Details</a>`;
                }
            }
        ]
    });
}
