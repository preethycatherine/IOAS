using IOAS.DataModel.Repository;
using IOAS.DataModel.Repository.wfads;
using IOAS.Models;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IOAS.Controllers
{
    public class IPDashboardController : Controller
    {
       
        public ActionResult IndianfillingsPatents(string instid = null)
        {
            Session["institute_bkpbutton"] = instid;
            Models.IPDashboard.wfadsModel records = new Models.IPDashboard.wfadsModel();
            List<Models.IPDashboard.IndianfillingsPatentsModel> records1 = new List<Models.IPDashboard.IndianfillingsPatentsModel>();
            try
            {
               
                using (WFADSEF obj = new WFADSEF())
                {
                    records = obj.Database.SqlQuery<Models.IPDashboard.wfadsModel>(string.Format("SELECT EmployeeId,EmployeeName,DepartmentCode FROM Faculty_Details where EmployeeId like  '%{0}%' ", instid)).FirstOrDefault();
                }
                using (PatentModel patentDb = new PatentModel())
                {
                     records1 = patentDb.Database.SqlQuery<Models.IPDashboard.IndianfillingsPatentsModel>(string.Format("select  fileno,Title, Type,Applcn_no,convert(varchar, Filing_dt, 103) as Filing_dt,Pat_no,convert(varchar, Pat_dt, 103) as Pat_dt, Attorney, Status from PatDetails where InstID like '%{0}%' ", instid)).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e);
            }
            return View("IndianfillingsPatents", new IOAS.Models.IPDashboard.IPDashboardView() { PIInfo = records, patentList = records1 });
        }
        public ActionResult patentinfoR102IPDetails(string fileno = null)
        {
            Models.IPDashboard.patentinfoR102IPDetailsModel records = new Models.IPDashboard.patentinfoR102IPDetailsModel();
            List<Models.IPDashboard.InventorDetailModel> records1 = new List<Models.IPDashboard.InventorDetailModel>();
            Models.IPDashboard.IndianPatentStatusModel records3 = new Models.IPDashboard.IndianPatentStatusModel();
            Models.IPDashboard.CommercializationModel recods4 = new Models.IPDashboard.CommercializationModel();
            try
            {
                using (PatentModel patentDb = new PatentModel())
                {
                    records = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102IPDetailsModel>(string.Format("SELECT title,type,InitialFiling,firstApplicant,secondApplicant,convert(varchar, request_dt, 103) as request_dt,Specification FROM PatDetails WHERE fileno LIKE '%{0}%' ", fileno)).FirstOrDefault();                
                    records1 = patentDb.Database.SqlQuery<Models.IPDashboard.InventorDetailModel>(string.Format("select SlNo+1 as SlNo,InventorName,InventorType,InventorID,DeptOrOrganisation as Dept from coinventordetails where fileno like '%{0}%' union select 1 as SlNo, Inventor1 as InventorName, InventorType, InstID as InventorID, Department as Dept from patdetails where fileno like '%{0}%'", fileno)).ToList();
                    records3 = patentDb.Database.SqlQuery<Models.IPDashboard.IndianPatentStatusModel>(string.Format("select Attorney,Applcn_no,convert(varchar, Filing_dt, 103) as Filing_dt,Examination,convert(varchar, Exam_dt, 103) as Exam_dt,Publication,convert(varchar, Pub_dt, 103) as Pub_dt,Status,Sub_status, Pat_no,convert(varchar, Pat_dt, 103) as Pat_dt from patdetails where fileno LIKE '%{0}%'", fileno)).FirstOrDefault();
                    recods4 = patentDb.Database.SqlQuery<Models.IPDashboard.CommercializationModel>(string.Format("select convert(varchar, Filing_dt, 103) as Filing_dt,Commercial,InventionNo,convert(varchar, Validity_from_dt, 103) as Validity_from_dt,convert(varchar, Validity_to_dt, 103) as Validity_to_dt,Industry1,Industry2,Industry3,IPC_Code,Abstract,DevelopmentStatus,Commercialized,PatentLicense,TechTransNo,Remarks from patdetails where fileno like '%{0}%'", fileno)).FirstOrDefault();
                }


            }
            catch (Exception e)
            { Console.WriteLine("Error:" + e); }
            return View("patentinfoR102IPDetails" ,new IOAS.Models.IPDashboard.IPDashboardView() { patentIPdetails = records, patentinventordetails=records1,indpatentstatus=records3,comzmodel=recods4 });
        }

        public ActionResult patentinfoR301PaymentDetailsR102(string fileno = null)
        {
            Models.IPDashboard.IDFcostPaymentDetailsModel records = new Models.IPDashboard.IDFcostPaymentDetailsModel();
            List<Models.IPDashboard.PatentpaymentModel> records1 = new List<Models.IPDashboard.PatentpaymentModel>();
            Models.IPDashboard.IDFcostPaymenttotalModel records3 = new Models.IPDashboard.IDFcostPaymenttotalModel();
            try
            {
                using (PatentModel patentDb = new PatentModel())
                {
                    records = patentDb.Database.SqlQuery<Models.IPDashboard.IDFcostPaymentDetailsModel>(string.Format("SELECT FileNo,Title,Applcn_no,convert(varchar(10),Filing_dt,103) as Filing_dt,Inventor1,Department,Pat_no,convert(varchar(10),Pat_dt,103) as Pat_dt FROM PatDetails WHERE FileNo LIKE '%{0}%' ", fileno)).FirstOrDefault();
                    records1 = patentDb.Database.SqlQuery<Models.IPDashboard.PatentpaymentModel>(string.Format("SELECT convert(varchar,PaymentOrChequeDt,103) as PaymentOrChequeDt,CostGroup,Activity,InvoiceNo,convert(varchar,InvoiceDt,103) as InvoiceDt,PaymentRefOrChequeNo,PType,Party,convert(varchar,dbo.udf_NumberToCurrency(PaymentAmtINR,'IND')) as PaymentAmtINR FROM PatentPayment WHERE FileNo like '%{0}%' ", fileno)).ToList();
                    records3 = patentDb.Database.SqlQuery<Models.IPDashboard.IDFcostPaymenttotalModel>(string.Format("SELECT convert(varchar,dbo.udf_NumberToCurrency(SUM(PaymentAmtINR), 'IND')) as PaymentAmtINR from PatentPayment where FileNo like '%{0}%' ", fileno)).FirstOrDefault();

                }

            }
            catch (Exception e)
            { Console.WriteLine("Error:" + e); }
            return View("patentinfoR301PaymentDetailsR102", new IOAS.Models.IPDashboard.IPDashboardView {paymentdetails=records,patentpayment=records1,paymenttotal=records3 });
        }

        public ActionResult Patentreceipt(string fileno = null)
        {

            Models.IPDashboard.ReceiptdetailModel records = new Models.IPDashboard.ReceiptdetailModel();
            List<Models.IPDashboard.patentinfoR102IPReceiptModel> records1 = new List<Models.IPDashboard.patentinfoR102IPReceiptModel>();
            Models.IPDashboard.patentinfoR102IPReceipttotalModel records2 = new Models.IPDashboard.patentinfoR102IPReceipttotalModel();
            try
            {
                using (PatentModel patentDb = new PatentModel())
                {
                    records = patentDb.Database.SqlQuery<Models.IPDashboard.ReceiptdetailModel>(string.Format("select title,Inventor1,department,applcn_no,convert(varchar,filing_dt,103) as filing_dt ,pat_no,convert(varchar,pat_dt,103) as  pat_dt from patdetails where fileno LIKE '%{0}%' ", fileno)).FirstOrDefault();
                    records1 = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102IPReceiptModel>(string.Format("select convert(varchar(10),SubmissionDt,103) as SubmissionDt, TransType,convert(varchar(10),PaymentDate,103) as PaymentDate,Party,PaymentGroup,TechTransferNo,PaymentDescription, dbo.udf_NumberToCurrency((cost_Rs), 'IND') as cost_Rs,PaymentRef from patentreceipt where fileno like '%{0}%' ", fileno.Trim())).ToList();
                    records2= patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102IPReceipttotalModel>(string.Format("select convert(varchar,dbo.udf_NumberToCurrency(SUM(cost_Rs), 'IND')) as cost_Rs from patentreceipt where fileno like '%{0}%' ", fileno.Trim())).FirstOrDefault();
                }
            }
            catch (Exception e)
            { Console.WriteLine("Error:" + e); }
            return View("Patentreceipt", new IOAS.Models.IPDashboard.IPDashboardView {receiptdetail=records ,patentreceipt=records1,patentreceipttotal=records2});
        }

        public ActionResult InternationalFilingsPatents(string instid = null)
        {
            Session["institute_bkbutton"] = instid;
            List<Models.IPDashboard.InternationalFilingsPatentsModel> records = new List<Models.IPDashboard.InternationalFilingsPatentsModel>();
            try
            {
                using (PatentModel patentDb = new PatentModel())
                {
                    records = patentDb.Database.SqlQuery<Models.IPDashboard.InternationalFilingsPatentsModel>(string.Format(" select B.subFileNo,B.Country,B.ApplicationNo as Applcn_no,convert(varchar,B.FilingDt,103) as Filing_dt,B.PatentNo,convert(nvarchar,B.PatentDt,103) PatentDt,B.TYPE,B.Attorney,B.Status,B.SubStatus from INTERNATIONAL as B WHERE exists(select a.fileno, A.Title, A.Applcn_no, convert(varchar, A.Filing_dt, 103) as Filing_dt, A.Pat_no, A.Pat_dt, A.Type, A.Attorney, A.Status, A.Sub_Status from PatDetails A WHERE B.fileno = A.fileno and A.InstID like '%{0}%') ", instid)).ToList();
                }
            }
            catch (Exception e)
            { Console.WriteLine("Error:" + e); }
            Session["fileno"] = instid;
            return View("InternationalFilingsPatents", new IOAS.Models.IPDashboard.IPDashboardView { internfillings = records });
        }

        public ActionResult patentinfoR202IPDetails(string fileno = null, string instid = null, string subfileno = null)
        {
            Models.IPDashboard.patentinfoR202IPDetails records = new Models.IPDashboard.patentinfoR202IPDetails();
            Models.IPDashboard.patentinfoR202IPDetailsIDFdetails records1 = new Models.IPDashboard.patentinfoR202IPDetailsIDFdetails();
            List<Models.IPDashboard.patentinfoR202IPDetailsInventordetails> records2 = new List<Models.IPDashboard.patentinfoR202IPDetailsInventordetails>();
            Models.IPDashboard.patentinfoR202IPDetailsIndianPatentstatus records3 = new Models.IPDashboard.patentinfoR202IPDetailsIndianPatentstatus();
            Models.IPDashboard.PatentinfoR202IPDetailsInternationlPatStatus records4 = new Models.IPDashboard.PatentinfoR202IPDetailsInternationlPatStatus();            
            try
            {
                using (PatentModel patentDb = new PatentModel())
                { 
                    records = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR202IPDetails>(string.Format("SELECT Inventor1,DeptCode,InstID FROM PatDetails WHERE InstID LIKE '%{0}%'", Session["fileno"])).FirstOrDefault();
                    records1 = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR202IPDetailsIDFdetails>(string.Format("SELECT title,type,InitialFiling,firstApplicant,secondApplicant,convert(nvarchar(10),request_dt,103) as request_dt,Specification FROM PatDetails WHERE fileno LIKE '%{0}%'", fileno)).FirstOrDefault();
                    records2 = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR202IPDetailsInventordetails>(string.Format("select SlNo+1 as SlNo,InventorName,InventorType,InventorID,DeptOrOrganisation as Dept from coinventordetails where fileno like '%{0}%' union select 1 as SlNo,Inventor1 as InventorName,InventorType,InstID as InventorID,Department as Dept from patdetails where fileno like '%{0}%'", fileno)).ToList();
                    records3= patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR202IPDetailsIndianPatentstatus>(string.Format("select Attorney,Applcn_no, convert(nvarchar(10),Filing_dt,103) as Filing_dt ,Examination, convert(nvarchar(10),Exam_dt,103) as Exam_dt,Publication,convert(nvarchar(10),Pub_dt,103) as Pub_dt,Status,Sub_status, Pat_no,convert(nvarchar(10),Pat_dt,103) as Pat_dt from patdetails where fileno LIKE '%{0}%'", fileno)).FirstOrDefault();
                    records4 = patentDb.Database.SqlQuery<Models.IPDashboard.PatentinfoR202IPDetailsInternationlPatStatus>(string.Format("select subFileNo,convert(nvarchar(10),RequestDt,103) as RequestDt,Country,partner,convert(nvarchar(15),PartnerNo,103) as PartnerNo,type,Attorney,ApplicationNo , convert(nvarchar, FilingDt, 103) as dt, PublicationNo, convert(nvarchar, PublicationDt, 103) as pbdt,Status, SubStatus, PatentNo, convert(nvarchar(10),PatentDt,103) as PatentDt, Remark from international  where subFileNo like '%{0}%'", subfileno)).FirstOrDefault();
                }
            }
            catch (Exception e)
            { Console.WriteLine("Error:" + e); }
            return View("patentinfoR202IPDetails", new IOAS.Models.IPDashboard.IPDashboardView { interncodetails = records, idfdetails = records1, invendetails = records2, indpatdetails=records3, internationalpatstatus=records4 });
        }

        public ActionResult TechTransferAccounts(string instid = null)
        {
            Session["institute_backbutton"] = instid;    
            Models.IPDashboard.TechTransferAccounts records = new Models.IPDashboard.TechTransferAccounts();
            List<Models.IPDashboard.patentinfoR102A> records1 = new List<Models.IPDashboard.patentinfoR102A>();
            try
            {
                using (PatentModel patentDb = new PatentModel())
                {
                    records=patentDb.Database.SqlQuery<Models.IPDashboard.TechTransferAccounts>(string.Format("SELECT Inventor1,DeptCode,InstID FROM PatDetails WHERE InstID LIKE '%{0}%'", instid)).FirstOrDefault();
                    records1= patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102A>(string.Format("SELECT convert(nvarchar(15),fileno,103) as fileno,TITLE,INVENTOR1 AS INVENTOR,INDUSTRY1,INDUSTRY2,ABSTRACT,DEVELOPMENTSTATUS,COMMERCIALIZED,(SELECT convert(varchar, dbo.udf_NumberToCurrency(SUM(PAYMENTAMTINR), 'IND')) FROM PATENTPAYMENT WHERE fileno = PATDETAILS.fileno) AS PAYMENT,(SELECT  convert(varchar, dbo.udf_NumberToCurrency(SUM(cost_Rs), 'IND'))  FROM PATENTRECEIPT WHERE fileno = PATDETAILS.fileno) AS RECEIPT FROM PATDETAILS WHERE InstID like '%{0}%'", instid)).ToList();
                }
            }
            catch (Exception e)
            { Console.WriteLine("Error:" + e); }
            return View("TechTransferAccounts", new IOAS.Models.IPDashboard.IPDashboardView { tectfr = records,patinfos=records1});
        }

        public ActionResult patentinfoR102AIPDetails(string fileno = null)
        {
            Session["institute_bkbutton"] = fileno;
            Models.IPDashboard.patentinfoR102AIPDetailsModel records = new Models.IPDashboard.patentinfoR102AIPDetailsModel();
            List<Models.IPDashboard.patentinfoR102AInventordetails> records1 = new List<Models.IPDashboard.patentinfoR102AInventordetails>();
            Models.IPDashboard.patentinfoR102AIPDetailsIndPatStat records2 = new Models.IPDashboard.patentinfoR102AIPDetailsIndPatStat();
            Models.IPDashboard.patentinfoR102AIPDetailsComercialization records3 = new Models.IPDashboard.patentinfoR102AIPDetailsComercialization();
            try
            {
                using (PatentModel patentDb = new PatentModel())
                {
                    records = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102AIPDetailsModel>(string.Format("SELECT title,type,InitialFiling,firstApplicant,secondApplicant,convert(nvarchar(15),request_dt,103) as request_dt,Specification FROM PatDetails WHERE fileno LIKE '%{0}%'", fileno)).FirstOrDefault();
                    records1= patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102AInventordetails>(string.Format("select convert(varchar(15),SlNo+1,103) as SlNo,InventorName,InventorType,convert(nvarchar(15),InventorID,103) as InventorID,DeptOrOrganisation as Dept from coinventordetails where fileno like '%{0}%' union select convert(varchar(15),1,103) as SlNo, Inventor1 as InventorName, InventorType, convert(varchar(15),InstID,103) as  InventorID, Department as Dept from patdetails where fileno like '%{0}%'", fileno.Trim())).ToList();
                    records2 = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102AIPDetailsIndPatStat>(string.Format("select Attorney,Applcn_no,convert(nvarchar(15),Filing_dt,103) as  Filing_dt,Examination,convert(nvarchar(15),Exam_dt,103) as  Exam_dt,Publication,convert(nvarchar(15),Pub_dt,103) as Pub_dt,Status,Sub_status, Pat_no,convert(nvarchar(15),Pat_dt,103) as Pat_dt from patdetails where fileno like  '%{0}%'", fileno)).FirstOrDefault();
                    records3 = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102AIPDetailsComercialization>(string.Format("select Commercial,InventionNo,convert(nvarchar(15),Validity_from_dt,103) as Validity_from_dt,convert(nvarchar(15),Validity_to_dt,103) as Validity_to_dt,Industry1,Industry2,Industry3,IPC_Code,Abstract,DevelopmentStatus,Commercialized,PatentLicense,TechTransNo,Remarks from patdetails where fileno like   '%{0}%'", fileno)).FirstOrDefault();

                }
            }
            catch (Exception e)
            { Console.WriteLine("Error:" + e); }
            return View("patentinfoR102AIPDetails", new IOAS.Models.IPDashboard.IPDashboardView { patinfoipdetail = records, patinvendetail=records1, patindpatstat = records2, patcom=records3 });
        }        
        public ActionResult patentinfoR301PaymentDetailsR102A(string fileno = null)
        {

            Models.IPDashboard.patentinfoR301PaymentDetailsR102Apaymentdet records = new Models.IPDashboard.patentinfoR301PaymentDetailsR102Apaymentdet();
            List<Models.IPDashboard.patentinfoR301PaymentDetailsR102Acostdetails> records1 = new List<Models.IPDashboard.patentinfoR301PaymentDetailsR102Acostdetails>();
            Models.IPDashboard.patentinfoR301PaymentDetailsR102total records2 = new Models.IPDashboard.patentinfoR301PaymentDetailsR102total();

            try
            {
                using (PatentModel patentDb = new PatentModel())
                {
                    records = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR301PaymentDetailsR102Apaymentdet>(string.Format("SELECT FileNo,Title,Applcn_no,convert(nvarchar(10),Filing_dt,103) as Filing_dt,Inventor1,Department,Pat_no,convert(nvarchar(10),Pat_dt,103) as Pat_dt FROM PatDetails WHERE FileNo LIKE '%{0}%'", fileno)).FirstOrDefault();
                    records1 = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR301PaymentDetailsR102Acostdetails>(string.Format("SELECT convert(nvarchar(10),PaymentOrChequeDt,103) as PaymentOrChequeDt,CostGroup,Activity,InvoiceNo,convert(nvarchar(10),InvoiceDt,103) as InvoiceDt,PaymentRefOrChequeNo,PType,Party, dbo.udf_NumberToCurrency(PaymentAmtINR, 'IND') as amount FROM PatentPayment WHERE FileNo like  '%{0}%'", fileno)).ToList();
                    records2 = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR301PaymentDetailsR102total>(string.Format("select dbo.udf_NumberToCurrency(SUM(PaymentAmtINR), 'IND') as total from PatentPayment where FileNo like '%{0}%'", fileno)).FirstOrDefault();

                }
            }
            catch (Exception e)
            { Console.WriteLine("Error:" + e); }
            return View("patentinfoR301PaymentDetailsR102A", new IOAS.Models.IPDashboard.IPDashboardView { patpydet=records, patcstdet=records1, pattotal=records2 });
        }
        public ActionResult patentinfoR102AIPReceipt(string fileno = null)
        {
            Models.IPDashboard.patentinfoR102AIPReceiptDetails records = new Models.IPDashboard.patentinfoR102AIPReceiptDetails();
           List< Models.IPDashboard.patentinfoR102APReceiptdetailstble> records1 = new List<Models.IPDashboard.patentinfoR102APReceiptdetailstble>();
            Models.IPDashboard.patentinfoR102APReceiptdetailstotal records2 = new Models.IPDashboard.patentinfoR102APReceiptdetailstotal();
            try
            {
                using (PatentModel patentDb = new PatentModel())
                {
                    records = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102AIPReceiptDetails>(string.Format("select fileno,title,Inventor1 as Inventor,department,applcn_no as ApplicationNo,convert(nvarchar(10),filing_dt,103) as filing_dt,pat_no as PatentNo,convert(nvarchar(10),pat_dt,103) as  PatentDt from patdetails where fileno LIKE '%{0}%'", fileno)).FirstOrDefault();
                    records1 = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102APReceiptdetailstble>(string.Format("select convert(nvarchar(10),EntryDt,103) as EntryDt ,FileNo,TechTransferNo,Party,PartyRefNo,convert(nvarchar(10),SubmissionDt,103) as SubmissionDt,TransType,TransDescription, PaymentGroup,PaymentDescription,convert(nvarchar(10),Currency,103) as  Currency,convert(nvarchar(10),ForeignCost,103) as ForeignCost,convert(nvarchar(10),ExRate,103) as ExRate,dbo.udf_NumberToCurrency(cost_Rs, 'IND') as total,convert(nvarchar(10),PaymentDate,103) as PaymentDate,PaymentRef,Year from patentreceipt where fileno LIKE '%{0}%'", fileno)).ToList();
                    records2 = patentDb.Database.SqlQuery<Models.IPDashboard.patentinfoR102APReceiptdetailstotal>(string.Format("select dbo.udf_NumberToCurrency(SUM(cost_Rs),'IND') as total from patentreceipt where fileno like '%{0}%'", fileno)).FirstOrDefault();

                }
            }
            catch (Exception e)
            { Console.WriteLine("Error:" + e); }
            return View("patentinfoR102AIPReceipt", new IOAS.Models.IPDashboard.IPDashboardView { patrecpdet=records,patrecptble=records1,patrecptbletotal=records2 });
        }

        public ActionResult Glossary()
        {
            return View("Glossary", new IOAS.Models.IPDashboard.IPDashboardView {});
        }

    }
}