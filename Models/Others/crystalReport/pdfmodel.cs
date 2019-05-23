using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IOAS.Models.Others.crystalReport
{
    public class pdfmodel
    {
        public string Employee_Code { get; set; }
        public string Start_Date { get; set; }
        public int Journey_Days { get; set; }
        public string Return_Date { get; set; }
        public string DOB { get; set; }

        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        public string Nominee_Name { get; set; }
        public string Passport_Number { get; set; }
        public string Mobile { get; set; }
        public string mail { get; set; }
        public string adhar_card_name { get; set; }
        public string disease { get; set; }
        public string disease_details { get; set; }
        public string project_no { get; set; }
        public string Creation_date { get; set; }
        public string Policy_No { get; set; }
        public decimal Premium_Amount { get; set; }
        public string InuranceCode { get; set; }
    }
}