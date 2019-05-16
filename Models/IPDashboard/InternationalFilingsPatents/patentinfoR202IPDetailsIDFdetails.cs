using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.IPDashboard
{
    public class patentinfoR202IPDetailsIDFdetails
    {
        public string title { set; get; }
        public string type { get; set; }
        public string InitialFiling { set; get; }
        public string firstApplicant { get; set; }
        public string secondApplicant { get; set; }
        public string request_dt {get;set;}

        public string Specification { get; set; }
    }
}