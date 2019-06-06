$(function () {
    //Declare Proposal List
    var getProjectDetailsURL = 'GetEnhancedProjectList',
        getExtendedProjectDetailsURL = 'GetExtendedProjectList';
        EditEnhancement = 'EditProjectenhancement';
        EditExtension = 'EditProjectextension';
        var dbenhance;
        Getenhancelist();
        var DateField = function (config) {
            jsGrid.Field.call(this, config);
        };

        DateField.prototype = new jsGrid.Field({
            sorter: function (date1, date2) {
                return new Date(date1) - new Date(date2);
            },

            itemTemplate: function (value) {
                return new Date(value).toDateString();
            },

            filterTemplate: function () {
                var now = new Date();
                this._fromPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() - 1), changeYear: true });
                this._toPicker = $("<input>").datepicker({ defaultDate: now.setFullYear(now.getFullYear() + 1), changeYear: true });
                return $("<div>").append(this._fromPicker).append(this._toPicker);
            },

            insertTemplate: function (value) {
                return this._insertPicker = $("<input>").datepicker({ defaultDate: new Date() });
            },

            editTemplate: function (value) {
                return this._editPicker = $("<input>").datepicker().datepicker("setDate", new Date(value));
            },

            insertValue: function () {
                return this._insertPicker.datepicker("getDate").toISOString();
            },

            editValue: function () {
                return this._editPicker.datepicker("getDate").toISOString();
            },

            filterValue: function () {
                return {
                    from: this._fromPicker.datepicker("getDate"),
                    to: this._toPicker.datepicker("getDate")
                };
            }
        });

        jsGrid.fields.date = DateField;
    //Project List for Enhancement
        debugger;
        $("#gridProjectList").jsGrid({       
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,

        controller: {

            loadData: function (filter) {
                return $.grep(dbenhance, function (enhance) {

                    return (!filter.Projecttitle || enhance.Projecttitle.toLowerCase().indexOf(filter.Projecttitle.toLowerCase()) > -1)
                    && (!filter.ProjectNumber || enhance.ProjectNumber.toLowerCase().indexOf(filter.ProjectNumber.toLowerCase()) > -1)
                    && (!filter.PIname || enhance.PIname.toLowerCase().indexOf(filter.PIname.toLowerCase()) > -1)
                    && (!filter.PrsntDueDate.from || new Date(enhance.PrsntDueDate) >= filter.PrsntDueDate.from)
                  && (!filter.PrsntDueDate.to || new Date(enhance.PrsntDueDate) <= filter.PrsntDueDate.to);
                });
            }
        },
        fields: [
            { name: "Sno", title: "S.No", editing: false, align: "left", width:"30px" },
            { type: "number", name: "ProjectID", title: "Project Id", visible: false },
            { type: "number", name: "ProjectEnhancementID", title: "Project EnhancementID", visible: false },
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "60px" },
            { type: "text", name: "PIname", title: "Principal Investigator", editing: false, width: "70px" },
            { type: "date", name: "PrsntDueDate", title: "Current Due Date", width: 100, align: "center" },
            { type: "decimal", name: "OldSanctionValue", title: "Old Sanction Value", editing: false, width: "55px" },
            { type: "decimal", name: "EnhancedSanctionValue", title: "Enhanced Sanction Value", editing: false, width: "55px" },
            
           
            {
                type: "control", deleteButton: false, width: "25px",
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                }
            },
            
        ],


        onItemEditing: function (args) {

            // cancel editing of the row of item with field 'ID' = 0
            if (args.item.ProjectEnhancementID > 0) {
                var enhanceid = args.item.ProjectEnhancementID;
            }
            $('#addnewpage').hide();
            $('#gridlist').hide();
            $('#gridproject').hide();
            $('#saveproject').hide();
            $('#ProjectEnhancement').show();
            $('#updateproject').show();
            $.ajax({
                type: "POST",
                url: EditEnhancement,
                data: { EnhanceId: enhanceid },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",

                success: function (result) {
                    console.log(result);
                    debugger;
                    
                    $('input[name="ProjectID"]').val(result.ProjectID);
                    $('input[name="ProjectEnhancementID"]').val(result.ProjectEnhancementID);
                    $('#projectnum').val(result.ProjectNumber);
                    $('#projecttitle').val(result.Projecttitle);                
                    $('#oldsanctndvalue').val(result.OldSanctionValue);
                 

                    $('#enhancedsanctnvalue').val(result.EnhancedSanctionValue);
                    $('#docrefnum').val(result.DocumentReferenceNumber);
                    $('#docname').val(result.AttachmentName);
                 //   $('#ApprovalDocument').val(result.AttachmentPath);
                    $('#totalenhanced').val(result.TotalEnhancedAllocationvalue);
                    $('#totalallocated').val(result.TotalAllocatedvalue);
                    $('#PresentDueDate').val(result.PrsntDueDate);
                    $('#ExtendedDueDate').val(result.ExtndDueDate);
                    if (result.ExtndDueDate != null)
                    {
                        $('input[name=Extension_Qust_1][value=Yes]').attr('checked', 'checked');
                        $('#extensiondetail').css("display", "block");
                    }

                    document.getElementsByClassName('link1')[0].text = result.AttachmentName;
                    document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + result.AttachmentPath + "&filepath=~%2FContent%2FSupportDocuments%2F";

                    var AllocateId = result.AllocationId;
                    var AllocateHead = result.Allocationhead;
                    var OldAllocationvalue = result.OldAllocationvalue;
                    var EnhancedAllocationvalue = result.EnhancedAllocationvalue;
                    var HeadwiseTotalvalue = result.HeadwiseTotalAllocationvalue;
                   
                    $.each(AllocateId, function (i, val) {
                        if (i == 0) {
                            document.getElementsByName('AllocationId')[0].value = AllocateId[0];
                            document.getElementsByName('Allocationhead')[0].value = AllocateHead[0];
                            document.getElementsByName('OldAllocationvalue')[0].value = OldAllocationvalue[0];
                            document.getElementsByName('EnhancedAllocationvalue')[0].value = EnhancedAllocationvalue[0];
                            document.getElementsByName('HeadwiseTotalAllocationvalue')[0].value = HeadwiseTotalvalue[0];

                        } else {
                            var cln = $("#primaryAllocateDiv").clone().find("input").val("").end();
                            //$(cln).find('.dis-none').removeClass('dis-none');
                            $('#divAllocateContent').append(cln)
                           document.getElementsByName('AllocationId')[i].value = AllocateId[i];
                           document.getElementsByName('Allocationhead')[i].value = AllocateHead[i];
                           document.getElementsByName('OldAllocationvalue')[i].value = OldAllocationvalue[i];
                           document.getElementsByName('EnhancedAllocationvalue')[i].value = EnhancedAllocationvalue[i];
                           document.getElementsByName('HeadwiseTotalAllocationvalue')[i].value = HeadwiseTotalvalue[i];

                        }
                    });
                  
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });            
        },
        });           

    var dbextend;
    Getextensionlist();
    $("#gridextendProjectList").jsGrid({       
        paging: true,
        pageIndex: 1,        
        pageSize: 5,
        editing: true,
        filtering: true,

        controller: {

            loadData: function (filter) {
                return $.grep(dbextend, function (extend) {

                    return (!filter.Projecttitle || extend.Projecttitle.toLowerCase().indexOf(filter.Projecttitle.toLowerCase()) > -1)
                    && (!filter.ProjectNumber || extend.ProjectNumber.toLowerCase().indexOf(filter.ProjectNumber.toLowerCase()) > -1)
                    && (!filter.PIname || extend.PIname.toLowerCase().indexOf(filter.PIname.toLowerCase()) > -1)
                    && (!filter.PrsntDueDate.from || new Date(extend.PrsntDueDate) >= filter.PrsntDueDate.from)
                     && (!filter.PrsntDueDate.to || new Date(extend.PrsntDueDate) <= filter.PrsntDueDate.to);
                });
            }

        },

        fields: [
            { name: "Sno", title: "S.No", editing: false, align: "left", width: "30px" },
            { type: "number", name: "ProjectID", title: "Project Id", visible: false },
            { type: "number", name: "ProjectEnhancementID", title: "Project EnhancementID", visible: false },
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false },
            { type: "text", name: "ProjectNumber", title: "Project Number", align: "left", editing: false, width: "60px" },
            { type: "text", name: "PIname", title: "Principal Investigator", editing: false, width: "70px" },
            { type: "date", name: "PrsntDueDate", title: "Current Due Date", width: 100, align: "center" },
             { name: "EnhancedSanctionValue", title: "Sanction Value", editing: false, width: "55px" },
            {
                type: "control",deleteButton: false ,width: "25px",
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
                }
            },
        ],


        onItemEditing: function (args) {

            // cancel editing of the row of item with field 'ID' = 0

            if (args.item.ProjectEnhancementID > 0) {
                var enhanceid = args.item.ProjectEnhancementID;
            }
            $('#addnewpage').hide();
            $('#gridlist').hide();
            $('#gridproject').hide();
            $('#saveproject').hide();
            $('#projectextension').show();
            $('#updateproject').show();
            $.ajax({
                type: "POST",
                url: EditExtension,
                data: { ExtensionId: enhanceid },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",

                success: function (result) {
                    console.log(result);
                    debugger;
                  
                    $('input[name="ProjectID"]').val(result.ProjectID);
                    $('input[name="ProjectEnhancementID"]').val(result.ProjectEnhancementID);
                    $('#projectnum').val(result.ProjectNumber);
                    $('#projecttitle').val(result.Projecttitle);
                    $('#PresentDueDate').val(result.PrsntDueDate);
                    $('#ExtendedDueDate').val(result.ExtndDueDate);
                    $('#oldsanctndvalue').val(result.OldSanctionValue);

                    $('#docrefnum').val(result.DocumentReferenceNumber);
                    $('#docname').val(result.AttachmentName);
                    $('#ApprovalDocument').val(result.AttachmentPath);
                    var docname = result.AttachmentName
                    var path = result.AttachmentPath
                    document.getElementsByClassName('link1')[0].text = docname;
                    document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + path + "&filepath=~%2FContent%2FSupportDocuments%2F";
                   
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
    });
    $("#gridProjectList").jsGrid("option", "filtering", false);
    //Get project enhancement flow details
    //$.ajax({
    //    type: "GET",
    //    url: getProjectDetailsURL,
    //    data: param = "",
    //    contentType: "application/json; charset=utf-8",
    //    success: function (result) {
    //       // dataProposal = result;
    //        $("#gridProjectList").jsGrid({ data: result });
    //        $('#ProjectEnhancement').hide();
    //        $('#gridlist').show();
    //    },
    //    error: function (err) {
    //        console.log("error : " + err);
    //    }
    //});
     
    $("#gridextendProjectList").jsGrid("option", "filtering", false);
    //Get project extension flow details
    //$.ajax({
    //    type: "GET",
    //    url: getExtendedProjectDetailsURL,
    //    data: param = "",
    //    contentType: "application/json; charset=utf-8",
    //    success: function (result) {
    //        // dataProposal = result;
    //        $("#gridextendProjectList").jsGrid({ data: result });
    //        $('#projectextension').hide();
    //        $('#gridlist').show();
    //    },
    //    error: function (err) {
    //        console.log("error : " + err);
    //    }
    //});
   
   
    function Getenhancelist() {

        $.ajax({
            type: "GET",
            url: getProjectDetailsURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                // dataProposal = result;
                $("#gridProjectList").jsGrid({ data: result });
                $('#ProjectEnhancement').hide();
                $('#gridlist').show();
                dbenhance = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });

    }


    function Getextensionlist() {

        $.ajax({
            type: "GET",
            url: getExtendedProjectDetailsURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                // dataProposal = result;
                $("#gridextendProjectList").jsGrid({ data: result });
                $('#projectextension').hide();
                $('#gridlist').show();
                dbextend = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }

        });

    }
});