using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.IPDashboard
{
    public class TechTransferAccounts
    {
        public string Inventor1 { get; set; }
        public string DeptCode { get; set; }
        public string InstID { get; set; }
        public string todaydate = DateTime.Today.ToString("dd/MM/yyyy");
    }
}