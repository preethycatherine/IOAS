using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.IPDashboard
{
    public class patentinfoR102IPReceiptModel
    {     
        public string Party { get; set; }
        public string TransType { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentGroup { get; set; }
        public string TechTransferNo { get; set; }
        public string PaymentRef { get; set; }      
        public string PaymentDescription { get; set; }
        public string cost_Rs { get; set; }      
        public string SubmissionDt { get; set; }
    }
}