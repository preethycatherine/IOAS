var adl_roles;
$(function () {
    //Declare Variable
    var getcreateuserURL = 'GetCreateUserlist',
     Editcreateuser = 'EditUserlist',
     Deleteuserlist = 'DeletUserlist',
     searchuserlist = 'GetsearchUserlist';
    var db;
    GetUserlis();
  $("#UserList").jsGrid({
        
        autoload: true,
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,
       
        controller: {
            
            loadData: function (filter) {
                return $.grep(db, function (user) {
                    
                    return (!filter.Firstname || user.Firstname.toLowerCase().indexOf(filter.Firstname.toLowerCase()) > -1)
                    && (!filter.Username || user.Username.toLowerCase().indexOf(filter.Username.toLowerCase()) > -1)
                    && (!filter.RoleName || user.RoleName.toLowerCase().indexOf(filter.RoleName.toLowerCase()) > -1)
                    && (!filter.DepartmentName || user.DepartmentName.toLowerCase().indexOf(filter.DepartmentName.toLowerCase()) > -1);
                    
                });
                }
            
        },
        fields: [
            { type: "number", name: "Sno", title: "S.No", editing: false,align:"left",width:"50px"},
            { type: "number", name: "Userid", title: "UserId",visible:false },
            { type: "text", name: "Firstname", title: "Name", editing: false, width: "250px" },
            {type:"text",name:"Username",title:"Email", editing: false, width: "250px"},
            { type: "number", name: "RoleId", title: "Role Id", visible: false },
            { type: "text", name: "RoleName", title: "Role name", editing: false, width: "150px"},
            { type: "number", name: "DepartmentId", title: "Department Id", visible: false },
            { type: "text", name: "DepartmentName", title: "Department name", editing: false,width:"170px" },
            
            {
                
                name: "Image",width: "100px",
                
                itemTemplate: function (val, item) {
                    if (val == null) {
                        return $("<img>").attr("src", "../Content/IOASContent/img/Image_placeholder.png").css({ height: 50, width: 50 })
                    }
                    else {
                        return $("<img>").attr("src", "../Content/UserImage/" + val).css({ height: 50, width: 50 })
                    }
                },
            
            },
            
        
        {
            type: "control", width: "100px",
            _createFilterSwitchButton: function () {
                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
        }
        },
        
        
            
        ],
       
        onItemEditing: function (args) {
           
            // cancel editing of the row of item with field 'ID' = 0
            
            if (args.item.Userid > 0) {
                var userid=args.item.Userid;
            }

            $.ajax({
                type: "POST",
                url: Editcreateuser,
                data: { UserId: userid },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",
               
                success: function (result) {
                  adl_roles = result.SelectedRoles;
                   $("#txtfirstname").val(result.Firstname);
                    $("#txtlastname").val(result.Lastname);
                    $("#ddlgender").val(result.Gender);
                    $("#Dateofbirth").val(result.DateOfbrith);
                    $("#ddlrolelist").val(result.RoleId);
                    $("#txtupdateusername").val(result.Username);
                    $("#txtuserid").val(result.UserId);
                    $("#ddldepartment").val(result.Department);
                    //Filldept();
                    Fillrole(result.RoleId);
                    
                    $('#gridlist,#addnewpage,#createuserid').hide();
                   $('#createuser,#updateuserid').show();
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
            
           

        },
        onItemDeleting: function(args) {
            if (args.item.Userid > 0) {
                var userid = args.item.Userid;
            }
            $.ajax({
                type: "POST",
                url: Deleteuserlist,
                data: { UserId: userid },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",

                success: function (result) {

                    
                    $('#createuserid').hide();
                    $('#updateuserid').hide();
                    $('#createuser').hide();
                    $('#gridlist').show();
                    
                    $('#deletemodal').modal('show');
                       
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        },
        
    });
  $("#UserList").jsGrid("option", "filtering", false);
    //Get User flow details
    $.ajax({
        type: "GET",
        url: getcreateuserURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            dataProcessFlow = result;
            $("#UserList").jsGrid({ data: result });

            $('#updateuserid').hide();
            $('#createuser').hide();
            $('#gridlist').show();
           
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });

    
    function GetUserlis() {

        $.ajax({
            type: "GET",
            url: getcreateuserURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                
                $("#UserList").jsGrid({ data: result });
                $('#updateuserid').hide();
                $('#createuser').hide();
                $('#gridlist').show();
                db = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });

    }
  

        
    
});

