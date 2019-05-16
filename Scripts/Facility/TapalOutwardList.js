var getOutwardURL = 'GetOutwardDetails'
var db;
$(function () {
    var tapalDetails = 'PopupTapalDetails';
    var getOutwardForEdit = 'GetOutwardForEdit';   
    $("#OutwardList").jsGrid({        
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: false,
        filtering: true,
        //selecting: true,
        controller: {

            loadData: function (filter) {
                return $.grep(db, function (ow) {

                    return (!filter.TapalType || ow.TapalType.toLowerCase().indexOf(filter.TapalType.toLowerCase()) > -1)
                    && (!filter.SenderDetails || ow.SenderDetails.toLowerCase().indexOf(filter.SenderDetails.toLowerCase()) > -1)
                    && (!filter.Department || ow.Department.toLowerCase().indexOf(filter.Department.toLowerCase()) > -1)
                    && (!filter.User || ow.User.toLowerCase().indexOf(filter.User.toLowerCase()) > -1);

                });
            }

        },
        fields: [
            { type: "number", name: "slNo", title: "S.No", editing: false },
            { type: "number", name: "TapalId", title: "TapalId", visible: false },
            //{ type: "text",   name: "ReceiptDt",     title: "Receipt Date",   editing: false },
            { type: "text", name: "TapalType", title: "Tapal Type", editing: false },
            { type: "text", name: "SenderDetails", title: "Sender Details", editing: false },
            //{ type: "text", name: "InwardDate", title: "Inward Date", editing: false },
            { type: "text", name: "Department", title: "Department", editing: false },
            { type: "text", name: "User", title: "User", editing: false },
            //{ type: "text",   name: "Remarks",       title: "Remarks",        editing: false },
            { type: "text", name: "OutwardDate", title: "Outward Date", editing: false },
            {
                name: "DocDetail",
                title: "Documents",
                itemTemplate: function (value, item) {
                    var elementDiv = $("<div>");
                    elementDiv.attr("class", "ls-dts");
                    $.each(item.DocDetail, function (index, itemData) {
                        var $link = $("<a>").attr("class", "ion-document icn").attr("href", itemData.href).attr("target", "_blank").html('');
                        elementDiv.append($link);
                    });
                    return elementDiv;
                }
            },
            {
                type: "control", width: 100,
                _createFilterSwitchButton: function () {
                    return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false)
                },
                itemTemplate: function (value, item) {
                    var $result = jsGrid.fields.control.prototype.itemTemplate.apply(this, arguments);

                    var $customButton = $("<button>")
                        .attr("class", "ion-eye")
                        .click(function (e) {
                            $.ajax({
                                type: "POST",
                                url: tapalDetails,
                                data: { TapalId: item.TapalId },
                                success: function (result) {
                                    $("#popup").html(result);
                                    $('#notify_modal').modal('toggle');
                                },
                                error: function (err) {
                                    console.log("error1 : " + err);
                                }
                            });
                            e.stopPropagation();
                        });

                    var $customButtonEdit = $("<button>")
                   .attr("class", "ion-edit")
                   .click(function (e) {
                       $.ajax({
                           type: "GET",
                           url: getOutwardForEdit,
                           data: { TapalId: item.TapalId },
                           success: function (result) {
                               if (result == -1) {
                                   $("#FailedAlert").text('Error. Try again!');
                                   $('#Failed').modal('show');
                               } else {
                                   $("#popup").html(result);
                                   $('#EditOutwardModal').modal('toggle');
                               }
                           },
                           error: function (err) {
                               console.log("error1 : " + err);
                           }
                       });
                       e.stopPropagation();
                   });
                    // return $result.add($customButton);
                    return $("<div>").append($customButton).append($customButtonEdit);
                }
            }
        ],
    });
    $("#OutwardList").jsGrid("option", "filtering", false);
    loadOutwart();
    
});
var loadOutwart = function loadOutwart() {
    $.ajax({
        type: "GET",
        url: getOutwardURL,
        data: param = "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            db = result;
            $("#OutwardList").jsGrid({ data: db });
        },
        error: function (err) {
            console.log("error : " + err);
        }

    });
};