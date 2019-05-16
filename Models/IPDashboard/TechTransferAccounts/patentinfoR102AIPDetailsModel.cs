using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.IPDashboard
{
    public class patentinfoR102AIPDetailsModel
    {
        public string title { get; set; }
        public string type { get; set; }
        public string InitialFiling { get; set; }
        public string firstApplicant { get; set; }
        public string secondApplicant { get; set; }
        public string request_dt { get; set; }
        public string Specification { get; set; }
    }
}