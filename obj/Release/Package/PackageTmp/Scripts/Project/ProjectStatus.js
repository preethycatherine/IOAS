var getProjectURL = 'GetProjectDetails'
var db;
$(function () {

    var loadControlsURL = 'LoadActionDDL',
      //UpdateStatusURL = 'UpdateStatusDetails';
     UpdateStatusURL = 'PopupUpdateStatus';
    var statusList;

    $.ajax({
        type: "GET",
        url: loadControlsURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            debugger;          
            statusList = result;
            console.log(result);
        },
        error: function (err) {
            console.log("Error in loadControls : " + err);
        }
    });

    $("#ProjectList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
       // selecting: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (ow) {

                    return (!filter.ProjectNo || ow.ProjectNo.toLowerCase().indexOf(filter.ProjectNo.toLowerCase()) > -1)
                    //&& (!filter.ProjectTittle || ow.ProjectTittle.toLowerCase().indexOf(filter.ProjectTittle.toLowerCase()) > -1)
                    && (!filter.SanctionOrderNo || ow.SanctionOrderNo.toLowerCase().indexOf(filter.SanctionOrderNo.toLowerCase()) > -1)
                    //&& (!filter.PIName || ow.PIName.toLowerCase().indexOf(filter.PIName.toLowerCase()) > -1)
                    && (!filter.Status || ow.Status.toLowerCase().indexOf(filter.Status.toLowerCase()) > -1);

                });
            }

        },
      
        fields: [
            { type: "number", name: "slNo", title: "S.No", editing: false,width:50 },
            { type: "number", name: "ProjectID", title: "Id", visible: false },
            { type: "text", name: "ProjectNo", title: "Project No", editing: false },
            { type: "text", name: "ProjectTittle", title: "Project Tittle", editing: false,width:200 },
            { type: "text", name: "SanctionOrderNo", title: "Sanction Order No", editing: false },
            { type: "text", name: "PIName", title: "PI Name", editing: false },
            { type: "text", name: "Status", title: "Status", editing: false },
            //{
            //    type: "control", type: "select", name: "StatusId", title: "Action",
            //    items: statusList
            //    , valueField: "id", textField: "name", selectedIndex: 0, visible: true
            //},
            {
                type: "", width: 120,title:"Action",
                //type: "control", width: 100,
                //_createFilterSwitchButton: function () {
                //    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false)
                //},
                itemTemplate: function (value, item) {
                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);
                    if (item.Status == "Active") {
                        statusList = [{id:0,name: "Select Action"},{ id:2,name: "InActive" }, {id:3, name: "Completed"}]
                    } else{
                        statusList = [{ id:0,name: "Select Action"}, {id:1, name: "Active"}]
                    }
                   
                    var $customSelect = $("<select>")
                        .attr("class", "form-control").prop("selectedIndex",0)                   
                    $.each(statusList, function (index, itemData) {
                        $customSelect.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name
                        }));
                    });

                    $customSelect.change(function (e) {
                        debugger;
                        model = {
                            ProjectId:item.ProjectID,
                            StatusId: $(this).val()
                        }
                        if ($(this).val() > 0) {
                            //var choice = confirm('Do you really want to update this project?');
                            //if (choice === true) {
                            $.ajax({
                                type: "POST",
                                url: UpdateStatusURL,
                                data: model,
                                success: function (result) {
                                    $("#popup").html(result);
                                    $('#Update_modal').modal('toggle');
                                },
                                error: function (err) {
                                    console.log("error1 : " + err);
                                }
                            });
                                //return true;
                            //}
                        }
                        $(this).val(0);
                        return false;
                        e.stopPropagation();
                    });
                  
                    return $result.add($customSelect);
                }
}

        
            //{
            //    type: "control",
            //    editButton: true,
            //    deleteButton: false,
            //    width: 50,
            //    headerTemplate: function (args) {
            //        var grid = this._grid;
            //        var isInserting = grid.inserting;
            //        var $button = $("<input>").attr("type", "button")
            //            .addClass([this.buttonClass, this.modeButtonClass, this.insertModeButtonClass].join(" "))
            //            .on("click", function () {
            //                isInserting = !isInserting;
            //                grid.option("inserting", isInserting);

            //            });
            //        return $button;
            //    }
            //}
        ],
        //onItemUpdated: function (args) {
        //    console.log(args.item);
        //    model = {
        //        ProjectId: args.item.ProjectId,
        //        StatusId: args.item.StatusId
        //    }
        //    $.ajax({
        //        type: "POST",
        //        url: UpdateStatusURL,
        //        data: model
        //    }).done(function (response) {
        //        //Refresh/load process grid
        //        loadDetails();
        //        return;
        //    }).fail(function (error) {
        //        console.log("Src:ProocessGuideline.js;Failed in approver list grid :" + error);
        //    });
        //},
    });
    $("#ProjectList").jsGrid("option", "filtering", false);
    loadDetails();
    
});
var loadDetails = function loadDetails() {
    $.ajax({
        type: "GET",
        url: getProjectURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            db = result;
            $("#ProjectList").jsGrid({ data: db });
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
};