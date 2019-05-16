
$(function () {
    var Getdepartmentlist = 'GetDepartmentlist',
        Editdepartment = 'GetEditDepartmentlist',
        Deletedeprtment = 'DeleteDepartment';
        //fillterdepartment = 'GetDepartmentlist';
    $('#departmentgrid').jsGrid({
        autoload: true,
        paging: true,
        editing: true,
        pageIndex: 1,
        pageSize: 5,
        filtering: false,
       
       
        fields: [
                    { type: "number", name: "Sno", title: "S.No", editing: false, align: "left", width: "50px", },
                    { type: "number", name: "Departmentid", title: "Department Id", visible: false },
                    { type: "text", name: "Departmentname", title: "Department Name", editing: false},
                    { type: "text", name: "HOD", title: "HOD", editing: false },
                    {type: "control"},
                   
        ],
         
        onItemEditing: function (args) {
            if (args.item.Departmentid > 0) {
                var deptId = args.item.Departmentid;
            }
            $('#Adddepartment').show();
            $('#tblrowid').hide();
            $('#addnewstatusbar').hide();
            $('#btnSave').hide();
            $('#btnUpdate').show();
            $.ajax({
                type: "POST",
                url: Editdepartment,
                data: { DepartmentId: deptId },
                success: function (result) {
                    $('#txtdeptid').val(result.Departmentid);
                    $('#txtdepartment').val(result.Departmentname);
                    $('#txthod').val(result.HOD);
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
        onItemDeleting: function (args) {
            if (args.item.Departmentid > 0) {
                var deptId = args.item.Departmentid;
            }
            $.ajax({
                type: "POST",
                url: Deletedeprtment,
                data: { DepartmentId: deptId },
                success: function (result) {
                   if (result == 1)
                    {
                        $('#Adddepartment').hide();
                        $('#tblrowid').show();
                        $('#addnewstatusbar').show();
                        $('#btnSave').hide();
                        $('#btnUpdate').hide();
                        Getdepartment();
                       $('#deletemodal').modal('show');
                       
                    }
                    else if(result==2)
                    {
                        $('#Adddepartment').hide();
                        $('#tblrowid').show();
                        $('#addnewstatusbar').show();
                        $('#btnSave').hide();
                        $('#btnUpdate').hide();
                        Getdepartment();
                        $('#warringmodal').modal('show');
                       
                    }
                    else {
                       
                        $('#Adddepartment').hide();
                        $('#tblrowid').show();
                        $('#addnewstatusbar').show();
                        $('#btnSave').hide();
                        $('#btnUpdate').hide();
                        $('#Errormodal').modal('show');
                    }
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },

       
    });
    
    $.ajax({
      
        type: "GET",
        url: Getdepartmentlist,
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            
            $("#departmentgrid").jsGrid({
                data: result
                
               
            });
            
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
    function Getdepartment() {

        $.ajax({

            type: "GET",
            url: Getdepartmentlist,
            data: "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {

                $("#departmentgrid").jsGrid({
                    data: result


                });

            },
            error: function (err) {
                console.log("error : " + err);
            }

        });

    }
});
