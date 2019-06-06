using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace IOAS.Models.Others
{
    public class insuranceInput
    {
        [Required]
        [Display(Name = "Employee Code")]
        public string Employee_Code { get; set; }
        [Required]
        [Display(Name = "Journey start date")]
        public string Start_Date { get; set; }
        [Required]
        [Display(Name = "Return Date")]
        public string Return_Date { get; set; }
        [Required]
        [Display(Name = "Date of birth")]
        public string DOB { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string First_Name { get; set; }
        [Required]
        [Display(Name ="Last Name")]
        public string Surname { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Nominee Name ")]
        public string Nominee_Name { get; set; }
        [Required]
        [Display(Name = "Passport Number")]
        public string Passport_Number { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "Mail Id ")]
        public string mail { get; set; }
        [Required]
        [Display(Name = "Adhaar card name")]
        public string adhar_card_name { get; set; }
        [Required]
        [Display(Name = "Disease type")]
        public string disease { get; set; }

        public string disease_details { get; set; }
        [Required]
        [Display(Name = "Project Number")]
        public string Project_no { get; set; }
        public string Project_otr { get; set; }
        public string Creation_date { get; set; }
     
        public insuranceInput()
        {
            Creation_date = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        }


    }
}