﻿$(function () {
    //Declare Proposal List
    var getProposalDetailsURL = 'GetProposalList',
    Editproposal = 'EditProposal',
    Deleteproposal = 'Deleteproposal',
    getsrchProposalDetailsURL = 'GetSearchProposalList';
    var dbProposal;
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
    //Declare Proposal List grid
    $("#gridProposalList").jsGrid({
        paging: true,
        pageIndex: 1,
        pageSize: 5,
        editing: true,
        filtering: true,
        controller: {

            loadData: function (filter) {
                return $.grep(dbProposal, function (proposal) {

                    return (!filter.ProposalNumber || proposal.ProposalNumber.toLowerCase().indexOf(filter.ProposalNumber.toLowerCase()) > -1)
                   && (!filter.Projecttitle || proposal.Projecttitle.toLowerCase().indexOf(filter.Projecttitle.toLowerCase()) > -1)
                   && (!filter.NameofPI || proposal.NameofPI.toLowerCase().indexOf(filter.NameofPI.toLowerCase()) > -1)
                   && (!filter.EmpCode || proposal.EmpCode.toLowerCase().indexOf(filter.EmpCode.toLowerCase()) > -1)
                    && (!filter.Prpsalinwrddate.from || new Date(proposal.Prpsalinwrddate) >= filter.Prpsalinwrddate.from)
                && (!filter.Prpsalinwrddate.to || new Date(proposal.Prpsalinwrddate) <= filter.Prpsalinwrddate.to);
                });
            }

        },
        fields: [
            {  name: "Sno", title: "S.No", editing: false, align: "left", width: "50px" },
            { type: "number", name: "ProposalID", title: "ProposalId", visible: false },
            { type: "text", name: "Projecttitle", title: "Project Title", editing: false },
            { type: "text", name: "ProposalNumber", title: "Proposal Number", editing: false },            
            { type: "text", name: "NameofPI", title: "Principal Investigator", editing: false },
            { type: "text", name: "EmpCode", title: "PI Code", editing: false, width: "60px" },
            { name: "Prpsalinwrddate", title: "Inward Date", type: "date", width: 100, align: "center" },
            { type: "decimal", name: "TotalValue", title: "Proposal Value", editing: false },
            {
                //itemTemplate: function(value, item) {
                //    var $result = $([]);

                //    if(item.name == "Status" && item.name.value == 1 ) {
                //        $result = $result.add(this._createEditButton(item));
                //    }

                //    if (item.name == "Status" && (item.name.value > 1 || item.name.value == 0)) {
                //        $result = $result.add(this._createDeleteButton(item));
                //    }

                //    return $result;
                //}
            //    type: "control", deleteButton: false, width: "25px",
            //    _createFilterSwitchButton: function () {
            //        return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
            //    },

                type: "control", width: "100px",
            _createFilterSwitchButton: function () {
                return this._createOnOffSwitchButton("filtering", this.searchModeButtonClass, false);
            }
}
        ],
        onItemEditing: function (args) {

            // cancel editing of the row of item with field 'ID' = 0

            if (args.item.ProposalID > 0) {
                var proposalid = args.item.ProposalID;
            }
            $('#addnewpage').hide();
            $('#gridlist').hide();
            $('#saveproposal').hide();
            $('#createproposal').show();
            $('#updateproposal').show();
            $.ajax({
                type: "POST",
                url: Editproposal,
                data: { ProposalId: proposalid },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",

                success: function (result) {
                    debugger;
                    console.log(result);                    
                    $('input[name="ProposalID"]').val(result.ProposalID);
                    $('#propslnum').val(result.ProposalNumber);
                    $('#Prjcttype').val(result.ProjectType);
                    $('#Proposalinwarddate').val(result.Prpsalinwrddate);
                    $('#txtprpslsrc').val(result.ProposalSource);
                    $('#txtprjcttitle').val(result.Projecttitle);

                    if (result.ProjectType == 2) {
                        $("#sponprojectsubtype").addClass('dis-none');
                        $("#projectscheme").addClass('dis-none');
                        $("#projectschemecode").addClass('dis-none');
                        $("#projectcategory").removeClass('dis-none');
                        $("#sponsanctionnumber").addClass('dis-none');
                        var typeId = result.ProjectType;
                        var subtypeId = 2;
                        $.getJSON("../Account/LoadPrjctcategorybytype", { typeid: typeId },
                                           function (locationdata) {
                                               var select = $("#Prjctconsultype");
                                               select.empty();

                                               $.each(locationdata, function (index, itemData) {

                                                   select.append($('<option/>', {
                                                       value: itemData.id,
                                                       text: itemData.name,
                                                   }));
                                                   $('#Prjctconsultype').val(result.Schemename);
                                                   $('#Prjctconsultypecode').val(result.SchemeCode);
                                               });
                                           });
                        $.getJSON("../Account/Loadcategorybytype", { typeid: typeId },
                          function (locationdata) {
                              var ctgryselect = $("#Prjctcategory");
                              ctgryselect.empty();

                              $.each(locationdata, function (index, itemData) {

                                  ctgryselect.append($('<option/>', {
                                      value: itemData.codevalAbbr,
                                      text: itemData.CodeValDetail,
                                  }));
                                  $("#Prjctcategory").val(result.ProjectCategory);
                                  $('input[name="ProjectCategory"]').val(result.ProjectCategory);
                              });
                          });
                        $.getJSON("../Account/LoadAgencybysubtype", { subtypeid: subtypeId },
                          function (locationdata) {
                              var select = $("#Agncy");
                              select.empty();

                              $.each(locationdata, function (index, itemData) {

                                  select.append($('<option/>', {
                                      value: itemData.id,
                                      text: itemData.name,
                                  }));
                                  $('#Agncy').val(result.SponsoringAgency);
                              });
                          });
                                                
                        $("#projectconstype").removeClass('dis-none');
                        $("#projectconstypecode").removeClass('dis-none');
                    }

                    $('#department').val(result.Department);

                    var PIselect = $("#PI");
                    PIselect.empty();
                    $.each(result.MasterPIListDepWise, function (index, itemData) {

                        PIselect.append($('<option/>', {
                            value: itemData.id,
                            text: itemData.name
                        }));
                    });
                    $('#PI').val(result.PIname);
                    PIselect.selectpicker('refresh');

                    $('#txtPIemail').val(result.PIEmail);                   
                    $('#txttotalvalue').val(result.TotalValue);


                    $('#txtprjctduratnyears').val(result.Projectdurationyears);
                    
                    $('#txtpersonapplied').val(result.Personapplied);
                    $('#txtremarks').val(result.Remarks);

                    $('#Inputdate').val(result.Inptdate);
                    $('#Inptdate').val(result.Inptdate);
                    $('#ProposalApproveddate').val(result.PrpsalAprveddate);
                    $('#Prjctsubtype').val(result.ProjectSubtype);
                    if (result.ProjectType == 1)
                    {
                        var typeId = result.ProjectType;
                        var subtypeid = result.ProjectSubtype;
                        $.getJSON("../Account/LoadAgencybysubtype", { subtypeid: subtypeid },
                          function (locationdata) {
                              var select = $("#Agncy");
                              select.empty();

                              $.each(locationdata, function (index, itemData) {

                                  select.append($('<option/>', {
                                      value: itemData.id,
                                      text: itemData.name,
                                  }));
                                  $('#Agncy').val(result.SponsoringAgency);
                              });
                          });
                        $("#sponprojectsubtype").removeClass('dis-none');                      
                        if (result.ProjectSubtype == 1 || result.ProjectSubtype == 0) {
                            $("#projectcategory").addClass('dis-none');
                            $("#projectscheme").addClass('dis-none');
                            $("#projectschemecode").addClass('dis-none');
                            $("#projectconstype").addClass('dis-none');
                            $("#projectconstypecode").addClass('dis-none');
                           
                        }
                        if (result.ProjectSubtype == 2) {
                            $("#projectcategory").removeClass('dis-none');
                            $("#projectscheme").removeClass('dis-none');
                            $("#projectschemecode").removeClass('dis-none');
                            $("#projectconstype").addClass('dis-none');
                            $("#projectconstypecode").addClass('dis-none');
                            var SchemeList = result.SchemeList;                           
                        //    var schemeselect = $("#Prjctscheme");                            
                            
                           
                       $.getJSON("../Account/LoadPrjctcategorybytype", { typeid: typeId },
                              function (locationdata) {
                                 var schemeselect = $("#Prjctscheme");
                                 schemeselect.empty();
                                 $.each(locationdata, function (index, itemData) {
                                      schemeselect.append($('<option/>', {
                                         value: itemData.id,
                                         text: itemData.name,

                                      }));
                                      $("#Prjctscheme").val(result.Schemename); 
                                      $("#schemecode").val(result.SchemeCode);
                                      $('input[name="Schemename"]').val(result.Schemename);
                                      $('input[name="SchemeCode"]').val(result.SchemeCode);
                                });
                             });
                            
                      $.getJSON("../Account/Loadcategorybytype", { typeid: typeId },
                           function (locationdata) {
                               var ctgryselect = $("#Prjctcategory");
                               ctgryselect.empty();

                               $.each(locationdata, function (index, itemData) {

                                   ctgryselect.append($('<option/>', {
                                       value: itemData.codevalAbbr,
                                       text: itemData.CodeValDetail,

                                   }));
                                   $("#Prjctcategory").val(result.ProjectCategory);
                                   $('input[name="ProjectCategory"]').val(result.ProjectCategory);
                               });
                           });
                          
                        }
                        $("#sponsanctionnumber").removeClass('dis-none');
                        $("#sanctionnumber").val(result.SanctionNumber);
                    }
               //     $('#Agncy').val(result.SponsoringAgency);                   
                    
                    
                    //$('#Prjctconsultype').val(result.Schemename);
                    //$('#Prjctconsultypecode').val(result.SchemeCode);
                    $('#txtprjctduratnmonths').val(result.Projectdurationmonths);
                    $('#txtpersonappliedinstitute').val(result.PersonAppliedInstitute);
                    $('#txtpersonappliedplace').val(result.PersonAppliedPlace);
                    $('#txtprpsltaxes').val(result.ApplicableTaxes);                   
                    $('#txtprpslbasicval').val(result.BasicValue);                   
                    $('#sanctionnumber').val(result.SanctionNumber);
                    
                    var CoPIName = result.CoPIname;
                    var CoPIemail = result.CoPIEmail;
                    var CoPIDep = result.CoPIDepartment;
                    var CoPIID = result.CoPIid;
                    var PIListDepWise = result.PIListDepWise;
                    var select = $("#CoPI");
                    select.empty();
                    if (PIListDepWise != null)
                    {
                        $.each(PIListDepWise[0], function (index, itemData) {

                            select.append($('<option/>', {
                                value: itemData.id,
                                text: itemData.name
                            }));
                        });
                        select.selectpicker('refresh');
                    
                    
                    $.each(CoPIName, function (i, val) {
                        if (i == 0) {
                            document.getElementsByName('CoPIname')[0].value = CoPIName[0];
                            document.getElementsByName('CoPIDepartment')[0].value = CoPIDep[0];
                            document.getElementsByName('CoPIEmail')[0].value = CoPIemail[0];
                            document.getElementsByName('CoPIid')[0].value = CoPIID[0];
                            $('#CoPI').selectpicker('refresh');
                        } else {
                            var cln = $("#primaryDiv").clone().find("input").val("").end();
                            var cloneElement = $("#primaryDiv").find('#CoPI').parent().clone();
                            cln.find('#CoPI').parent().replaceWith(selectPickerApiElement($(cloneElement), "all", PIListDepWise[i], CoPIName[i]));
                            $(cln).find('.dis-none').removeClass('dis-none');
                            $('#divContent').append(cln);
                            document.getElementsByName('CoPIname')[i].value = CoPIName[i];
                            document.getElementsByName('CoPIDepartment')[i].value = CoPIDep[i];
                            document.getElementsByName('CoPIEmail')[i].value = CoPIemail[i];
                            document.getElementsByName('CoPIid')[i].value = CoPIID[i];
                        }
                    });
                    }
                    if (result.Otherinstcopi_Qust_1 == "Yes")
                    {
                        $('input[name=Otherinstcopi_Qust_1][value=' + result.Otherinstcopi_Qust_1 + ']').attr('checked', 'checked');
                        $('#otherinstitutecopidetail').css("display", "block");
                    }
                    var OtherInstCoPIName = result.OtherInstituteCoPIName;
                    var OtherInstCoPIRemarks = result.RemarksforOthrInstCoPI;
                    var OtherInstCoPIInst = result.CoPIInstitute;
                    var OtherInstCoPIDep = result.OtherInstituteCoPIDepartment;
                    var OtherInstCoPIID = result.OtherInstituteCoPIid;
                    var OtherInstPIListDepWise = result.OtherInstCoPIListDepWise;
                    
                    var select = $("#OtherinstituteCoPI");
                    select.empty();
                    if (OtherInstPIListDepWise != null) {
                        $.each(OtherInstPIListDepWise[0], function (index, itemData) {

                            select.append($('<option/>', {
                                value: itemData.id,
                                text: itemData.name
                            }));
                        });
                        select.selectpicker('refresh');
                        $.each(OtherInstCoPIName, function (i, val) {
                            if (i == 0) {
                                document.getElementsByName('OtherInstituteCoPIName')[0].value = OtherInstCoPIName[0];
                                document.getElementsByName('CoPIInstitute')[0].value = OtherInstCoPIInst[0];
                                document.getElementsByName('OtherInstituteCoPIDepartment')[0].value = OtherInstCoPIDep[0];
                                document.getElementsByName('RemarksforOthrInstCoPI')[0].value = OtherInstCoPIRemarks[0];
                                document.getElementsByName('OtherInstituteCoPIid')[0].value = OtherInstCoPIID[0];
                                $('#OtherinstituteCoPI').selectpicker('refresh');
                            } else {
                                var cln = $("#primaryotherinstcopiDiv").clone().find("input").val("").end();
                                var cloneElement = $("#primaryotherinstcopiDiv").find('#OtherinstituteCoPI').parent().clone();
                                cln.find('#OtherinstituteCoPI').parent().replaceWith(selectPickerApiElement($(cloneElement), "all", OtherInstPIListDepWise[i], OtherInstituteCoPIName[i]));
                                $(cln).find('.dis-none').removeClass('dis-none');
                                $('#divotherinstcopiContent').append(cln);
                                document.getElementsByName('OtherInstituteCoPIName')[i].value = OtherInstCoPIName[i];
                                document.getElementsByName('CoPIInstitute')[i].value = OtherInstituteCoPIInst[i];
                                document.getElementsByName('OtherInstituteCoPIDepartment')[i].value = CoPIDep[i];
                                document.getElementsByName('RemarksforOthrInstCoPI')[i].value = CoPIemail[i];
                                document.getElementsByName('OtherInstituteCoPIid')[i].value = CoPIID[i];
                            }
                        });
                    }
                    var Docname = result.DocName;
                    var Attachname = result.AttachName;
                    var Doctype = result.DocType;
                    var Docpath = result.DocPath;
                    var DocID = result.Docid;

                    $.each(Docname, function (i, val) {
                        if (i == 0) {
                            document.getElementsByName('DocType')[0].value = Doctype[0];
                            document.getElementsByName('AttachName')[0].value = Attachname[0];
                            document.getElementsByName('Docid')[0].value = DocID[0];
                            document.getElementsByClassName('link1')[0].text = Docname[0];
                            document.getElementsByClassName('link1')[0].href = "ShowDocument?file=" + Docpath[0] + "&filepath=~%2FContent%2FProposalDocuments%2F";
                        } else {
                            var cln = $("#DocprimaryDiv").clone().find("input").val("").end();
                            // $(cln).find('.dis-none').removeClass('dis-none');
                            $('#DocdivContent').append(cln)
                            document.getElementsByName('DocType')[i].value = Doctype[i];
                            document.getElementsByName('AttachName')[i].value = Attachname[i];
                            document.getElementsByName('Docid')[i].value = DocID[i];
                            document.getElementsByClassName('link1')[i].text = Docname[i];
                            document.getElementsByClassName('link1')[i].href = "ShowDocument?file=" + Docpath[i] + "&filepath=~%2FContent%2FProposalDocuments%2F";
                        }

                        //    ShowDocument?file=c61479f3-93e7-48b4-84a2-7f970e02c432_UAY2017_proposal_template 1.pdf&filepath=~%2FContent%2FProposalDocuments%2F
                    });
                                     
                                      
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });

        },
        onItemDeleting: function (args) {
            if (args.item.ProposalID > 0) {
                var proposalid = args.item.ProposalID;
            }
            $.ajax({
                type: "POST",
                url: Deleteproposal,
                data: { ProposalId: proposalid },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",

                success: function (result) {
                    if (result == 1) {
                        $("#alertSuccess").text('Proposal has been deleted successfully.');
                        $('#Success').modal('show');
                        
                    } else {
                        $("#FailedAlert").text('Something went wrong please contact administrator.');
                        $('#Failed').modal('show');
                    }
                    setTimeout(function () { load(); }, 4000);
                },
                error: function (err) {
                    console.log("error1 : " + err);
                }
            });
        }
    });
    $("#gridProposalList").jsGrid("option", "filtering", false);
    load();
    function load() {
        $.ajax({
            type: "GET",
            url: getProposalDetailsURL,
            data: param = "",
            contentType: "application/json; charset=utf-8",
            success: function (result) {
                // dataProposal = result;
                $("#gridProposalList").jsGrid({ data: result });
                $('#createproposal').hide();
                $('#gridlist').show();
                dbProposal = result;
            },
            error: function (err) {
                console.log("error : " + err);
            }
        });
    }

});
var selectPickerApiElement = function (el, choice, options, select) {
    $(el).find('select').selectpicker({
        liveSearch: true
    });
    $(el).children().eq(2).siblings().remove();
    if (choice == "add") {
        $(el).find('.selectpicker').append("<option>" + options + "</option>");
    } else if (choice == "all" && select != '') {
        $(el).find('.selectpicker').children().remove();
        for (var i = 0 ; i < options.length ; i++) {
            $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
        }
        $(el).find('.selectpicker option[value=' + select + ']').attr('selected', 'selected');
    } else if (choice == "all" && select == '') {
        $(el).find('.selectpicker').children().remove();
        for (var i = 0 ; i < options.length ; i++) {
            $(el).find('.selectpicker').append("<option value=" + options[i].id + ">" + options[i].name + "</option>");
        }
    } else if (choice == "empty") {
        $(el).find('.selectpicker').children().remove();
        $(el).find('.selectpicker').append("<option value=''>Select any</option>");
    } else {
        var selectOptionsLength = $(el).find('.selectpicker').children().length;
        for (var i = 1 ; i <= selectOptionsLength ; i++) {
            if (options == $(el).find('.selectpicker').children().eq(i).val()) {
                $(el).find('.selectpicker').children().eq(i).remove();
                break;
            } else {
                continue;
            }

        }

    }
    $(el).find('select').selectpicker('refresh');
    return $(el).children().first().unwrap();

}