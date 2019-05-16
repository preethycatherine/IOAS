using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Models
{
    public class InvoiceListModel
    {
        public Nullable<int> ProjectType { get; set; }
        public Nullable<int> PIId { get; set; }
        public string PIName { get; set; }
        public string SelectProject { get; set; }
        public string SelectInvoice { get; set; }
        public string SelectCancelInvoice { get; set; }
        public int Userrole { get; set; }
        public InvoiceSearchFieldModel SearchField { get; set; }
        public PagedData<InvoiceSearchResultModel> SearchResult { get; set; }
    }
    public class InvoiceSearchResultModel
    {
        public Nullable<int> InvoiceId { get; set; }
        public string PIName { get; set; }
        public Nullable<int> PIId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public string InvoiceDate { get; set; }
        public string ProjectTitle { get; set; }
        public string SACNumber { get; set; }
        public string Service { get; set; }
        public string InvoiceNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TotalInvoiceValue { get; set; }
        public string Status { get; set; }
    }
    public class InvoiceSearchFieldModel
    {
        public string InvoiceNumber { get; set; }        
        public Nullable<int> PIName { get; set; }
        public string SearchBy { get; set; }
        public string ProjectNumber { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        
    }
    public class CreateInvoiceModel
    {
        public int Sno { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Nullable<int> InvoiceDraftId { get; set; }
        public string InvoiceNumber { get; set; }
        public Nullable<int> InvoiceType { get; set; }
        public Nullable<DateTime> InvoiceDate { get; set; }
        public string Invoicedatestrng { get; set; }
        public Nullable<int>[] PreviousInvoiceId { get; set; }
        public string [] PreviousInvoiceNumber { get; set; }
        public string[] PreviousInvoiceDate { get; set; }
        public string[] PreviousInvoicedatestrng { get; set; }
        public Nullable<int>[] InstalmentId { get; set; }
        public Nullable<int> [] InstlmntNumber { get; set; }
        public Nullable<Decimal> [] InstalValue { get; set; }
        public Nullable<int>[] Instalmentyear { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal>[] PreviousInvoicevalue { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> AvailableBalance { get; set; }
        [Required]
        public string Projecttitle { get; set; }
        [Required]
        public Nullable<int> PIId { get; set; }        
        public Nullable<int> Prjcttype { get; set; }
        public Nullable<int> ProjectType { get; set; }
        public string SelectProject { get; set; }
        public string SelectInvoice { get; set; }
        public string NameofPI { get; set; }
        [Required]
        public Nullable<int> SponsoringAgency { get; set; }
        public string SponsoringAgencyName { get; set; }
        public int InvoicecrtdID { get; set; }
        public string ProjectNumber { get; set; }
        public string ProposalNumber { get; set; }
        public int PrpsalNumber { get; set; } 
        [Required]
        public Nullable<int> Department { get; set; }
        public string PIDepartmentName { get; set; }        
        public string Remarks { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> Sanctionvalue { get; set; }
        public string CurrentFinancialYear { get; set; }
        public string GSTNumber { get; set; }
        public string PAN { get; set; }
        public string TAN { get; set; }
        [Required]
        public string Agencyregname { get; set; }
        [Required]
        public string Agencyregaddress { get; set; }
        [Required]
        public string Agencydistrict { get; set; }
        [Required]
        public string Agencystate { get; set; }
        [Required]
        public Nullable<int> Agencystatecode { get; set; }
        [Required]
        public Nullable<int> AgencyPincode { get; set; }
        [Required]
        public string Agencycontactperson { get; set; }
        public string Agencycontactpersondesignation { get; set; }
        public string AgencycontactpersonEmail { get; set; }
        public string Agencycontactpersonmobile { get; set; }
        public string SanctionOrderNumber { get; set; }        
        [DataType(DataType.DateTime)]
        public Nullable<DateTime> SanctionOrderDate { get; set; }
        public string SODate { get; set; }
        public string SACNumber { get; set; }
        public string IITMGSTIN { get; set; }
        public Nullable<int> ServiceType { get; set; }
        [Required]
        public string DescriptionofServices { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        [Range(0, 999999999999999.999, ErrorMessage = "Invalid target value; Max 18 digits")]
        public Nullable<Decimal> TaxableValue { get; set; }
       
        public Nullable<Decimal> CGST { get; set; }
        
        public string CGSTPercentage { get; set; }
        
        public Nullable<Decimal> SGST { get; set; }
        
        public string SGSTPercentage { get; set; }
       
        public Nullable<Decimal> IGST { get; set; }
       
        public string IGSTPercentage { get; set; }
        public Nullable<Decimal> TotalTaxValue { get; set; }
        public string TotalTaxpercentage { get; set; }
        public Nullable<int> TotalTaxpercentageId { get; set; }
        public Nullable<Decimal> TotalInvoiceValue { get; set; }
        public string CurrencyCode { get; set; }
        public string TotalInvoiceValueinwords { get; set; }
        public string CommunicationAddress { get; set; }
        public int Bank { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public Nullable<int> BankAccountId { get; set; }
        public Nullable<int> Instalmentnumber { get; set; }
        public Nullable<int> Instlmntyr { get; set; }
        public Nullable<Decimal> Instalmentvalue { get; set; }
        public string[] Invoiced { get; set; }
        public Nullable<int> TaxStatus { get; set; }
        public string PONumber { get; set; }
    }

   
}