
var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "10%" },
            { "data": "streetAddress", "width": "10%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "10%" },
            { "data": "postalCode", "width": "10%" },
            { "data": "phoneNumber", "width": "10%" },
            {
                "data": "isAuthorizedCompany",
                "render": function (data) {
                    if (data) {
                        return `<input type="chechbox" checked disabled />`;
                    }
                    else {
                        return `<input type="checkbox" disabled />`;
                    }
                }
            },   
            {
                "data": "id",
                "render": function (data) {
                    return `
                       <div class="text-center">
                      <a href="/Admin/Company/Upsert/${data}" class="btn btn-info">
                      <i class="fas fa-edit"></i>
                      </a>
                     <a class="btn btn-danger" onclick=Delete('/Admin/Company/Delete/${data}')>
                       <i class="fas fa-trash-alt"></i>
                       </a>
                       </div>
                          `;
                }
            }
        ]
    })

}
function Delete(url) {
    // alert(url);
    swal({
        title: "Want to delete data?",
        text: "Delete Information!!!",
        icon: "warning",
        buttons: true,
        dangerModel: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })

}
