using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.IPDashboard
{
    public class IndianfillingsPatentsModel
    {
        public string InstID { get; set; }
        public string Deptcode { get; set; }
        public string Inventor1 { get; set; }

        public string FileNo { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Applcn_no { get; set; }
        public string Attorney { get; set; }
        
        public string Filing_dt { get; set; }
        public string Pat_dt { get; set; }
        public string Pat_no { get; set; }        
        public string Status { get; set; }       
    }
}