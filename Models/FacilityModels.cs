﻿using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    #region Tapal
    public class CreateTapalModel
    {
        public Nullable<Int32> TapalId { get; set; }
        public PagedData<ListTapalModel> GetTapalInventryDetails { get; set; }
        [Required]
        [Display(Name = "Tapal Type")]
        public int selTapalType { get; set; }
        [Required]
        [Display(Name = "Sender Details")]
        public string SenderDetails { get; set; }
        //[Required]
        //[Display(Name = "Project Tabal")]
        public string tapalType { get; set; }
        public bool ProjectTabal { get; set; }
        [RequiredIf("tapalType", "Yes", ErrorMessage = "PI name filed is required")]
        public Nullable<int> PIName { get; set; }
        [RequiredIf("tapalType", "Yes", ErrorMessage = "Project number filed is required")]
        public Nullable<int> ProjectNo { get; set; }
        [Required]
        [Display(Name = "Receipt Date")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ReceiptDate { get; set; }

        [Display(Name = "Documents")]
        [RequiredIf("TapalId", 0, ErrorMessage = "Documents filed is required")]
        public HttpPostedFileBase[] files { get; set; }

        public List<DocumentDetailModel> DocDetail { get; set; }

        public string Notes { get; set; }
        public int selDepartment { get; set; }
        public int selUser { get; set; }
        public int selRole { get; set; }
        public int selAction { get; set; }
        public string Remarks { get; set; }
        public string ProjectTittle { get; set; }
        public string ProjectType { get; set; }
        public string ProjectSubType { get; set; }
        public string ProjectCategory { get; set; }
    }

    public class DocumentDetailModel
    {
        public int TapalDocDetailId { get; set; }
        public string DocName { get; set; }
        public string FileName { get; set; }
        public int TabalId { get; set; }
        public string href { get; set; }
    }

    public class ListTapalModel
    {
        public int slNo { get; set; }
        public int TapalId { get; set; }
        public string TapalNo { get; set; }
        public string TapalType { get; set; }
        public string SenderDetails { get; set; }
        public bool ProjectTabal { get; set; }
        public string PIName { get; set; }
        public string ProjectNo { get; set; }
        public string ReceiptDt { get; set; }
        public string Department { get; set; }
        public string User { get; set; }
        public string Remarks { get; set; }
        public string ReceiptDate { get; set; }
        public List<DocumentDetailModel> DocDetail { get; set; }
        public string InwardDate { get; set; }
        public string OutwardDate { get; set; }
        public int CreateUserId { get; set; }
        public int Action { get; set; }
        public string strAction { get; set; }
        public bool IsClosed { get; set; }
        public List<RoleAccessDetailModel> Roles { get; set; }
    }
    public class TapalDetailsModel
    {
        public Nullable<int> TapalId { get; set; }       
        public string TapalType { get; set; }
        public string SenderDetails { get; set; }
        public string ReceiptDate { get; set; }
        public bool ProjectTabal { get; set; }
        public string ProjectNumber { get; set; }
        public string PIName { get; set; }
        public string Remarks { get; set; }
        public string FromUser { get; set; }
        public List<DocumentDetailModel> DocDetail { get; set; }
    }

    public class ProjectDetailModel
    {
        public int ProjectId { get; set; }
        public string ProjectTittle { get; set; }
        public string ProjectType { get; set; }
        public string ProjectSubType { get; set; }
        public string ProjectCategory { get; set; }
    }
    #endregion
    #region SRB
    public class SRBListModel
    {
        public SRBSearchFieldModel SearchField { get; set; }
        public PagedData<SRBSearchResultModel> SearchResult { get; set; }
    }
    public class SRBSearchResultModel
    {
        public Nullable<int> SRBId { get; set; }
        public Nullable<int> ItemCategory { get; set; }
        public string ItemName { get; set; }
        public string DocName { get; set; }
        public string DocFullName { get; set; }
        public string InwardDate { get; set; }
        public string PONumber { get; set; }
    }
    public class SRBSearchFieldModel
    {
        public string ItemName { get; set; }
        public Nullable<int> ItemCategory { get; set; }
        public Nullable<int> PIName { get; set; }
        public string SearchBy { get; set; }
        public Nullable<DateTime> FromPODate { get; set; }
        public Nullable<DateTime> ToPODate { get; set; }
        public Nullable<DateTime> FromSRBDate { get; set; }
        public Nullable<DateTime> ToSRBDate { get; set; }
    }
    public class SRBDetailsModel
    {
        public Nullable<int> SRBId { get; set; }
        public string ItemName { get; set; }
        public string ItemNumber { get; set; }
        public string InwardDate { get; set; }
        public string Department { get; set; }
        public string PONumber { get; set; }
        public string PurchaseDate { get; set; }
        public string ProjectNumber { get; set; }
        public string PIName { get; set; }
        public string Remarks { get; set; }
        public string ItemCategory { get; set; }
        public string DocName { get; set; }
        public string DocFullName { get; set; }
    }
    public class SRBModel
    {
        public Nullable<int> SRBId { get; set; }

        [Required]
        [Display(Name = "Item name")]
        [StringLength(300)]
        public string ItemName { get; set; }

        //[Required]
        //[Display(Name = "Item number")]
        //[StringLength(50)]
        public string ItemNumber { get; set; }

        [Required]
        [Display(Name = "Inward date")]
        public Nullable<DateTime> InwardDate { get; set; }

        //[Required]
        //public Nullable<int> Department { get; set; }

        [Required]
        [Display(Name = "PO number")]
        [StringLength(50)]
        public string PONumber { get; set; }

        [Required]
        [Display(Name = "Purchase date")]
        public Nullable<DateTime> PurchaseDate { get; set; }       

        [Display(Name = "Project number")]
        [StringLength(50)]
        public string ProjectNumber { get; set; }

        public bool IsIncludeProjectDetails { get; set; }

        [RequiredIf("IsIncludeProjectDetails", "True", ErrorMessage = "Project number filed is required")]
        public Nullable<Int32> PIProjectNumber { get; set; }
        
        [RequiredIf("IsIncludeProjectDetails", "True", ErrorMessage = "PI name filed is required")]
        public Nullable<Int32> PIName { get; set; }

        [StringLength(3000)]
        public string Remarks { get; set; }

        [Required]
        [Display(Name = "Item category")]
        public Nullable<int> ItemCategory { get; set; }

        
        [Display(Name = "PO document")]
        [RequiredIf("SRBId", 0, ErrorMessage = "PO document filed is required")]
        public HttpPostedFileBase PODocument { get; set; }
        public string DocName { get; set; }
        public string DocFullName { get; set; }
    }
    #endregion
}