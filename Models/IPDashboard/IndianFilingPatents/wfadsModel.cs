using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.IPDashboard
{
    public class wfadsModel
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentCode { get; set; }
        public string todaydate = DateTime.Today.ToString("dd/MM/yyyy");
        
    }
}