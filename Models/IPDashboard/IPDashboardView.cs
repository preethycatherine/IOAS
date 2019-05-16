using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.IPDashboard
{
    public class IPDashboardView
    {
        //indian patent fillings
        public List<IndianfillingsPatentsModel> patentList { get; set; }
        public wfadsModel PIInfo { get; set; }
        public List<PatentpaymentModel> patentpayment { get; set; }
        public patentinfoR102IPDetailsModel patentIPdetails { get; set; }
        public List<InventorDetailModel> patentinventordetails { get; set; }
        public IndianPatentStatusModel indpatentstatus { get; set; }
        public CommercializationModel comzmodel { get; set; }
        public IDFcostPaymentDetailsModel paymentdetails { get; set; }
        public IDFcostPaymenttotalModel paymenttotal { get; set; }
        public ReceiptdetailModel receiptdetail { get; set; }
        public List<patentinfoR102IPReceiptModel> patentreceipt { get; set; }
        public patentinfoR102IPReceipttotalModel patentreceipttotal { get; set; }

        //internation patent fillings
        public List<InternationalFilingsPatentsModel> internfillings { get; set; }
        public patentinfoR202IPDetails interncodetails { get; set; }
        public patentinfoR202IPDetailsIDFdetails idfdetails { get; set; }
        public List<patentinfoR202IPDetailsInventordetails>  invendetails { get; set; }
        public patentinfoR202IPDetailsIndianPatentstatus indpatdetails { get; set; }
        public PatentinfoR202IPDetailsInternationlPatStatus internationalpatstatus { get; set; }

        //tech transfer accoutns
        public TechTransferAccounts tectfr { get; set; }
        public List<patentinfoR102A> patinfos { get; set; }
        public patentinfoR102AIPDetailsModel patinfoipdetail { get; set; }
        public List<patentinfoR102AInventordetails> patinvendetail { get; set; }

        public patentinfoR102AIPDetailsIndPatStat patindpatstat { get; set; }
        public patentinfoR102AIPDetailsComercialization patcom { get; set; }
        public patentinfoR301PaymentDetailsR102Apaymentdet patpydet { get; set; }
        public List<patentinfoR301PaymentDetailsR102Acostdetails> patcstdet { get; set; }

        public patentinfoR301PaymentDetailsR102total pattotal { get; set; }
        public patentinfoR102AIPReceiptDetails patrecpdet { get; set; }

        public List<patentinfoR102APReceiptdetailstble> patrecptble { get; set; }
        public patentinfoR102APReceiptdetailstotal patrecptbletotal { get; set; }
        public IPDashboardView()
        {
            //indian patent fillings
            PIInfo = new wfadsModel();
            patentpayment = new List<PatentpaymentModel>();
            patentList = new List<IndianfillingsPatentsModel>();
            patentIPdetails = new patentinfoR102IPDetailsModel();
            patentinventordetails = new List<InventorDetailModel>();
            indpatentstatus = new IndianPatentStatusModel();
            comzmodel = new CommercializationModel();
            paymentdetails = new IDFcostPaymentDetailsModel();
            paymenttotal = new IDFcostPaymenttotalModel();
            receiptdetail = new ReceiptdetailModel();
            patentreceipt = new List<patentinfoR102IPReceiptModel>();
            patentreceipttotal = new patentinfoR102IPReceipttotalModel();

            //internation patent fillings
            internfillings = new List<InternationalFilingsPatentsModel>();
            interncodetails = new patentinfoR202IPDetails();
            idfdetails = new patentinfoR202IPDetailsIDFdetails();
            invendetails = new List<patentinfoR202IPDetailsInventordetails>();
            indpatdetails = new patentinfoR202IPDetailsIndianPatentstatus();
            internationalpatstatus = new PatentinfoR202IPDetailsInternationlPatStatus();



            //tech transfer
            tectfr = new TechTransferAccounts();
            patinfos = new  List<patentinfoR102A>();
            patinfoipdetail = new patentinfoR102AIPDetailsModel();
            patinvendetail = new List<patentinfoR102AInventordetails>();
            patindpatstat = new patentinfoR102AIPDetailsIndPatStat();
            patcom = new patentinfoR102AIPDetailsComercialization();
            patpydet = new patentinfoR301PaymentDetailsR102Apaymentdet();
            patcstdet = new List<patentinfoR301PaymentDetailsR102Acostdetails>();
            pattotal = new patentinfoR301PaymentDetailsR102total();
            patrecpdet = new patentinfoR102AIPReceiptDetails();
            patrecptble = new List<patentinfoR102APReceiptdetailstble>();

            patrecptbletotal= new patentinfoR102APReceiptdetailstotal();
        }
    }
}
