namespace IOAS.Models.Others
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblTravelinsurancepdf")]
    public partial class tblTravelinsurancepdf
    {
        [Key]
        [StringLength(50)]
        public string InsuranceCode { get; set; }

        [StringLength(50)]
        public string PdfPath { get; set; }

        public virtual tblTravelInsurance tblTravelInsurance { get; set; }
    }
}
