using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.IPDashboard
{
    public class PatentpaymentModel
    {
        public string PaymentRefOrChequeNo { get; set; }
        public string CostGroup { get; set; }
        public string Activity { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDt { get; set; }
        public string PaymentOrChequeDt { get; set; }
        public string PType { get; set; }
        public string Party { get; set; }
        public string PaymentAmtINR { get; set; }
    }
}