namespace IOAS.Models.Others
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblTravelInsurance")]
    public partial class tblTravelInsurance
    {
        [Key]
        [StringLength(50)]
        public string InsuranceCode { get; set; }

        [Required]
        [StringLength(50)]
        public string Employee_Code { get; set; }

        [StringLength(50)]
        public string Start_Date { get; set; }

        public int? Journey_Days { get; set; }

        [StringLength(50)]
        public string Return_Date { get; set; }

        [Required]
        [StringLength(50)]
        public string DOB { get; set; }

        [StringLength(50)]
        public string First_Name { get; set; }

        [StringLength(50)]
        public string Middle_Name { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        [StringLength(50)]
        public string Gender { get; set; }

        [StringLength(50)]
        public string Nominee_Name { get; set; }

        [StringLength(50)]
        public string Passport_Number { get; set; }

        [StringLength(50)]
        public string Mobile { get; set; }

        [StringLength(50)]
        public string mail { get; set; }

        [StringLength(50)]
        public string adhar_card_name { get; set; }

        [StringLength(150)]
        public string disease { get; set; }

        [StringLength(150)]
        public string disease_details { get; set; }

        [StringLength(50)]
        public string Project_no { get; set; }

        [StringLength(50)]
        public string Creation_date { get; set; }

        [StringLength(50)]
        public string Policy_No { get; set; }

        public decimal? Premium_Amount { get; set; }

        public virtual tblTravelinsurancepdf tblTravelinsurancepdf { get; set; }
    }
}
