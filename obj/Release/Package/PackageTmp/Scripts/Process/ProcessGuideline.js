var unselectUser = [], selectUser = [];
$(function () {
    //Declare global variable
    var getProcessFlowDetailsURL = 'GetProcessFlowList',
        addProcessFlowDetailsURL = 'AddProcessFlow',
        insertProcessGuidelineURL = 'InsertProcessGuideline',
        updateProcessFlowDetailsURL = 'UpdateProcessFlow',
        getUnassignedUserDetailsURL = 'GetProcessFlowUserDetails',
        getApproverListURL = 'GetApproverList',
        getStatusURL = 'GetStatus',
        addApproverDetailsURL = 'AddApproverDetails',
        getAllApproverListURL = 'GetAllApproverList',
        loadControlsURL = 'LoadControls',
        getProcessGuideLineListURL = 'GetProcessGuideLineList';
        deletePGLWorkflowURL = 'DeletePGLWorkflow',
        mapProcessflowUserURL = 'MapProcessflowUser';
        unmapProcessflowUserURL = 'UnmapProcessflowUser';
        var db1, dataUnassignedUser, dataAssignedUser, pglDetailId = 0, pglId = 0, approverList, statusList, documentList;

    /*   Load controls   */
        $.ajax({
            type: "GET",
            url: loadControlsURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                approverList = result.ApproverList;
                statusList = result.StatusList;
                documentList = result.DocumentList;
                var optionSrch = $("#selectFunctionSrch"), option = $("#selectFunction");
                optionSrch.append($("<option />").val(-1).text('Select All'));
                $.each(result.FunctionList, function () {
                    optionSrch.append($("<option />").val(this.FunctionId).text(this.FunctionName));
                    option.append($("<option />").val(this.FunctionId).text(this.FunctionName));
                });
                Search();
            },
            error: function (err) {
                console.log("Error in loadControls : " + err);
            }
        });

    /*   Search grid declaration   */
        $("#gridProcessGuidelineSearch").jsGrid({
            autoload: true,
            paging: true,
            pageIndex: 1,
            pageSize: 5,
            data: db1,
            editing: false,
            selecting: true,
            fields: [
                { type: "number", name: "ProcessGuidelineId", title: "Id", visible: true, align: "left" },
                { type: "number", name: "FunctionId", title: "FunctionId", visible: false },
                { type: "text", name: "FunctionName", title: "Function Name" },
                { type: "text", name: "ProcessName", title: "Process Name" },
                {
                    name: "ProcessGuidelineId",
                    title: "Action",
                    itemTemplate: function (value, item) {
                        return $("<a>").attr("href", "#").attr("process", item.ProcessName).attr("functionId", item.FunctionId).text('Select').on("click", function () {
                            pglId = value;
                            $('#txtProcess').val($(this).attr("process"));
                            $('#divOuterPGL').css('display', 'none');
                            $('#divInnerPGL').css('display', 'block');
                            $('#selectFunction').val($(this).attr("functionId"));
                            GetProcessFlowList();
                            GetApproverFlowList();
                            return false;
                        });
                    }
                }
            ]
        });

    /*   Unassigned user grid declaration   */
        $("#gridUnassigneduser").jsGrid({
            autoload: true,
            paging: true,
            pageIndex: 1,
            pageSize: 5,
            editing: false,
            fields: [
                { type: "number", name: "ProcessGuidelineDetailId", title: "Id", visible: false },
                { type: "number", name: "UserId", title: "UserId", visible: false },
                { type: "text", name: "UserName", title: "User Name" },
                {
                    name: "UserFlag",
                    headerTemplate: function () {
                        return $("<input>").attr("type", "checkbox")
                                .on("change", function () {
                                    var chkValue = $(this).is(":checked");
                                    $('.singleChkUnassigned').each(function () {
                                        this.checked = chkValue;
                                    });
                                    selectUser = [];
                                    if ($(this).is(":checked")) {
                                        $('input[unassignkey]').each(function () {
                                            selectUser.push($(this).attr('unassignkey'));
                                        });
                                    }
                                });
                    },
                    itemTemplate: function (_, item) {
                        return $("<input>").attr("type", "checkbox")
                                .prop("checked", false)
                                .attr("class", "singleChkUnassigned")
                                .attr("unassignkey", item.UserId)
                                .on("change", function () {
                                    if ($(this).is(":checked")) {
                                        selectUser.push(item.UserId);
                                    }
                                    else {
                                        selectUser.pop(item.UserId);
                                    }
                                });
                    },
                    align: "center",
                    width: 50
                }
            ]
        });

    /*   Assigned user grid declaration   */
        $("#gridAssignedUser").jsGrid({
            autoload: true,
            paging: true,
            pageIndex: 1,
            pageSize: 5,
            editing: false,
            fields: [
                { type: "number", name: "ProcessGuidelineDetailId", title: "Id", visible: false },
                { type: "number", name: "UserId", title: "UserId", visible: false },
                { type: "text", name: "UserName", title: "User Name" },
                {
                    name: "UserFlag", title: "Select All",
                    headerTemplate: function () {
                        return $("<input>").attr("type", "checkbox")
                                .on("change", function () {
                                    var chkValue = $(this).is(":checked");
                                    $('.singleChkAssigned').each(function () {
                                        this.checked = chkValue;
                                    });
                                    unselectUser = [];
                                    if ($(this).is(":checked")) {
                                        $('input[assignkey]').each(function () {
                                            unselectUser.push($(this).attr('assignkey'));
                                        });
                                    }
                                });
                    },
                    itemTemplate: function (_, item) {
                        return $("<input>").attr("type", "checkbox")
                                .prop("checked", !(item.UserFlag))
                                .attr("class", "singleChkAssigned")
                                .attr("assignkey", item.UserId)
                                .on("change", function () {
                                    if ($(this).is(":checked")) {
                                        unselectUser.push(item.UserId);
                                    }
                                    else {
                                        unselectUser.pop(item.UserId);
                                    }
                                });
                    },
                    align: "center",
                    width: 50
                }
            ]
        });

    /*   Process flow grid declaration   */
        $("#gridProcessFlow").jsGrid({
            autoload: true,
            data: db1,
            paging: true,
            pageIndex: 1,
            pageSize: 4,
            editing: false,
            selecting: true,
            controller: {
                insertItem: function (item) {
                    
                    var input = {
                        ProcessGuidelineid: pglId,
                        ProcessGuidelineDetailId: pglDetailId,
                        FlowTitle: item.FlowTitle
                    };
                    $.ajax({
                        type: "POST",
                        url: addProcessFlowDetailsURL,
                        data: input,
                    }).done(function (response) {
                        GetProcessFlowList();
                        return;
                    }).fail(function (error) {
                        console.log("Process flow insert item :" + error);
                    });
                },
                updateItem: function (item) {
                    $.ajax({
                        type: "POST",
                        url: updateProcessFlowDetailsURL,
                        data: item
                    }).done(function (response) {
                        GetProcessFlowList();
                        return;
                    }).fail(function (error) {
                        console.log("Process flow update item :" + error);
                    });
                }
            },
            fields: [
                { type: "number", name: "ProcessGuidelineDetailId", title: "Id", visible: false },
                {
                    type: "text", name: "FlowTitle", title: "Flow Title", validate: {
                        message: "Flow Title is required", validator: function (value) {
                            return !(value == undefined);
                        }
                    }
                },
                {
                    type: "control",
                    editButton: false,
                    deleteButton: false,
                    headerTemplate: function () {
                        var grid = this._grid;
                        var isInserting = grid.inserting;
                        var $button = $("<input>").attr("type", "button")
                            .addClass([this.buttonClass, this.modeButtonClass, this.insertModeButtonClass].join(" "))
                            .on("click", function () {
                                isInserting = !isInserting;
                                grid.option("inserting", isInserting);
                            });
                        return $button;
                    },
                    itemTemplate: function (value, item) {
                        return $("<a>").attr("href", "#").text("Select")
                                .on("click", function () {
                                    //Select button event - load related user
                                    $('#spanAssignedUser,#spanApprover').html(item.FlowTitle);
                                    pglDetailId = item.ProcessGuidelineDetailId;
                                    selectUser = [];
                                    unselectUser = [];
                                    GetUserProcessflowList();
                                    GetApproverFlowList();
                                    return false;
                                });
                    }
                }
            ]
        });

    /*   Approvelist grid declaration   */
        $("#gridApproverList").jsGrid({
            autoload: true,
            data: db1,
            paging: true,
            pageIndex: 1,
            pageSize: 5,
            editing: true,
            selecting: true,
            deleteConfirm: "Do you really want to delete ?",
            rowClass: function (item, itemIndex) {
                return "client-" + itemIndex;
            },
            controller: {
                insertItem: function (item) {
                    if (pglDetailId > 0) {
                        model = {
                            processguidlineId: pglId,
                            ProcessGuidelineDetailId: pglDetailId,
                            ApproverLevel: item.ApproverLevel,
                            UserId: item.UserId,
                            StatusId: item.StatusId,
                            ApproveFlag: item.ApproveFlag,
                            RejectFlag: item.RejectFlag,
                            ClarifyFlag: item.ClarifyFlag,
                            MarkFlag: item.MarkFlag,
                            DocumentId: item.DocumentId
                        }
                        $.ajax({
                            type: "POST",
                            url: addApproverDetailsURL,
                            data: model

                        }).done(function (response) {
                            GetApproverFlowList();
                            return;
                        });
                    }
                    else {
                        alert("Please select any process flow");
                    }
                },
                updateItem: function (item) {
                    model = {
                        ProcessguidlineworkflowId: item.ProcessguidlineworkflowId,
                        processguidlineId: pglId,
                        ProcessGuidelineDetailId: pglDetailId,
                        ApproverLevel: item.ApproverLevel,
                        UserId: item.UserId,
                        StatusId: item.StatusId,
                        ApproveFlag: item.ApproveFlag,
                        RejectFlag: item.RejectFlag,
                        ClarifyFlag: item.ClarifyFlag,
                        MarkFlag: item.MarkFlag,
                        DocumentId: item.DocumentId
                    }
                    $.ajax({
                        type: "POST",
                        url: addApproverDetailsURL,
                        data: model
                    }).done(function (response) {
                        //Refresh/load process grid
                        GetApproverFlowList();
                        return;
                    }).fail(function (error) {
                        console.log("Src:ProocessGuideline.js;Failed in approver list grid :" + error);
                    });
                },
                deleteItem: function (item) {
                    $.ajax({
                        type: "POST",
                        url: deletePGLWorkflowURL,
                        data: { processguidlineworkflowId: item.ProcessguidlineworkflowId },
                        success: function (result) {
                            if (result == 1)
                                alert("Deleted successfully");
                            else
                                alert("Deleted failed");
                        },
                        error: function (err) {
                            console.log("Src:ProocessGuideline.js;error" + err + ".");
                        }
                    });
                }

            },
            fields: [
                { type: "number", name: "ProcessguidlineworkflowId", title: "Id", visible: false },
                {
                    type: "number", name: "ApproverLevel", title: "Level", min: 1, width: 50, validate: {
                        message: "Level should be greater than 1",
                        validator: function (value, item) {
                            var retVal = true;
                            if (value == undefined)
                                retVal = false;
                            else if (value < 1)
                                retVal = false;
                            return retVal;
                        }
                    }
                },
                {
                    type: "select", name: "UserId", title: "Approver", items: approverList, valueField: "ApproverId", textField: "ApproverName", validate: { message: "Approver is required", validator: function (value) { return value > 0; } }
                },
                { type: "select", name: "StatusId", title: "Status", items: statusList, valueField: "StatusId", textField: "StatusName", validate: { message: "Status is required", validator: function (value) { return value > 0; } } },
                { type: "select", name: "DocumentId", title: "Document", items: documentList, valueField: "DocumentId", textField: "DocumentName", validate: { message: "Document is required", validator: function (value) { return value > 0; } } },
                {
                    name: "ApproveFlag", type: "checkbox", title: "Approve", sorting: false, width: 40
                },
                {
                    name: "RejectFlag", type: "checkbox", title: "Reject", sorting: false, width: 40
                },
                {
                    name: "ClarifyFlag", type: "checkbox", title: "Clarify", sorting: false, width: 40
                },
                {
                    name: "MarkFlag", type: "checkbox", title: "Mark", sorting: false, width: 40
                },
                {
                    type: "control",
                    editButton: true,
                    deleteButton: true,
                    width: 50,
                    headerTemplate: function (args) {
                        var grid = this._grid;
                        var isInserting = grid.inserting;
                        var $button = $("<input>").attr("type", "button")
                            .addClass([this.buttonClass, this.modeButtonClass, this.insertModeButtonClass].join(" "))
                            .on("click", function () {
                                isInserting = !isInserting;
                                grid.option("inserting", isInserting);

                            });
                        return $button;
                    }
                }
            ],
            onRefreshed: function () {
                var $gridData = $("#gridApproverList .jsgrid-grid-body tbody");

                $gridData.sortable({
                    update: function (e, ui) {
                        debugger;
                        // array of indexes
                        var clientIndexRegExp = /\s*client-(\d+)\s*/;
                        var indexes = $.map($gridData.sortable("toArray", { attribute: "class" }), function (classes) {
                            return clientIndexRegExp.exec(classes)[1];
                        });
                        alert("Reordered indexes: " + indexes.join(", "));
                        // arrays of items
                        var items = $.map($gridData.find("tr"), function (row) {
                            return $(row).data("JSGridItem");
                        });
                        console && console.log("Reordered items", items);
                    }
                });
            }
        });

    //$('#btnSubmitApprover').off("click").on("click", function () {
    //    var input = {
    //        processGuidelineDetailId: pglDetailId,
    //        UserId: $('#selectApprover').val(),
    //        StatusId: $('#selectStartStatus').val(),
    //        ApproverFlow: ApproveFlag,
    //    };
    //    $.ajax({
    //        type: "POST",
    //        url: addApproverDetailsURL,
    //        data: JSON.stringify(input),
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        success: function (result) {
    //            $.ajax({
    //                type: "GET",
    //                url: getAllApproverListURL,
    //                data: { processGuidelineDetailId: pglDetailId },
    //                contentType: "application/json; charset=utf-8",
    //                dataType: "json",
    //                success: function (result) {
    //                    $("#gridApproverList").jsGrid({ data: result });
    //                },
    //                error: function (err) {
    //                    console.log("error : " + err);
    //                }
    //            });
    //        },
    //        error: function (errMsg) {
    //            console.log("error : " + err);
    //        }
    //    });
    //    return false;
    //});

        $('#btnAddNew').off("click").on("click", function () {
            pglId = 0;
            $('#selectFunction').prop('selectedIndex', 0);
            $('#txtProcess').val('');
            $('#divOuterPGL').css('display', 'none');
            $('#divInnerPGL').css('display', 'block');
            $("#gridProcessFlow").jsGrid({ data: {} });
            $("#gridUnassigneduser").jsGrid({ data: {} });
            $("#gridAssignedUser").jsGrid({ data: {} });
            $("#gridApproverList").jsGrid({ data: {} });
            unselectUser = [];
            selectUser = [];
            $('#spanAssignedUser,#spanApprover').html("Assigned Users :");
        });

        $('#btnClose').off("click").on("click", function () {
            $('#divOuterPGL').css('display', 'block');
            $('#divInnerPGL').css('display', 'none');
        });

        $('#btnUnmapUser').off("click").on("click", function () {
            var input = [];
            $.each(unselectUser, function (index, value) {
                input.push({ ProcessGuidelineDetailId: pglDetailId, UserId: value });
            });
            $.ajax({
                type: "POST",
                url: unmapProcessflowUserURL,
                data: JSON.stringify(input),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (response.result > 0) {
                    GetUserProcessflowList();
                    unselectUser = [];
                }
                else if (response.result == -1) {
                    console.log("Error in unmap user");
                }
            });
        });

        $('#btnSave').off("click").on("click", function () {
            var input = { ProcessGuidelineId: pglId, FunctionId: $('#selectFunction').val(), ProcessName: $('#txtProcess').val() };
            $.ajax({
                type: "POST",
                url: insertProcessGuidelineURL,
                data: JSON.stringify(input),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    var mode = pglId == 0 ? "Add" : "Update";
                    pglId = result.result;
                    Search();
                    if (mode == "Add")
                        alert("Saved successfully");
                    else
                        alert("Updated successfully");
                },
                error: function (errMsg) {
                    alert(errMsg);
                }
            });
        });

        $('#btnResetSave').off("click").on("click", function () {
            pglId = 0;
            $('#selectFunction').prop('selectedIndex', 0);
            $('#txtProcess').val('');
        });

        $('#btnSearchPGL').off('click').on('click', function () {
            Search();
        });

        $('#btnResetSrchPGL').off('click').on('click', function () {
            $('#selectFunctionSrch').prop('selectedIndex', 0);
            $('#txtProcessSrch').val('');
            Search();
        });

        $('#btnMapUser').off('click').on('click', function () {
            var input = [];
            $.each(selectUser, function (index, value) {
                input.push({ ProcessGuidelineDetailId: pglDetailId, UserId: value });
            });
            $.ajax({
                type: "POST",
                url: mapProcessflowUserURL,
                data: JSON.stringify(input),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (response.result > 0) {
                    GetUserProcessflowList();
                    selectUser = [];
                }
                else if (response.result == -1) {
                    console.log("Error in map user");
                }
            });

        });

        function Search() {
            var input = { functionId: $('#selectFunctionSrch').val(), processName: $('#txtProcessSrch').val() };
            $.ajax({
                type: "Get",
                url: getProcessGuideLineListURL,
                data: input,
                dataType: "json",
                success: function (result) {
                    $("#gridProcessGuidelineSearch").jsGrid({ data: result });
                },
                error: function (err) {
                    console.log("error in pgl search: " + err);
                }
            });
        };

        var GetProcessFlowList = function () {
            $.ajax({
                type: "GET",
                url: getProcessFlowDetailsURL,
                data: { processGuidelineId: pglId },
                success: function (result) {
                    dataProcessFlow = result;
                    $("#gridProcessFlow").jsGrid({ data: result });
                },
                error: function (err) {
                    console.log("error : " + err);
                }
            });
        };

        var GetApproverFlowList = function () {
            $.ajax({
                type: "GET",
                url: getAllApproverListURL,
                data: { processheaderid: pglId, processDetailId: pglDetailId },
                success: function (result) {
                    dataProcessFlow = result;
                    $("#gridApproverList").jsGrid({ data: result });
                },
                error: function (err) {
                    console.log("error : " + err);
                }
            });
        };

        var GetUserProcessflowList = function () {
            $.ajax({
                type: "GET",
                url: getUnassignedUserDetailsURL,
                data: { processGuidelineDetailId: pglDetailId },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    var assingedRows = $.grep(result, function (n, i) {
                        return n.UserFlag == true;
                    });
                    var unAssingedRows = $.grep(result, function (n, i) {
                        return n.UserFlag == false;
                    });
                    $("#gridAssignedUser").jsGrid({ data: assingedRows });
                    $("#gridUnassigneduser").jsGrid({ data: unAssingedRows });
                },
                error: function (err) {
                    console.log("Src:ProocessGuideline.js;method:GetUserProcessflowList;error" + err + ".");
                }
            });
        };

});