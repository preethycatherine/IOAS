using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IOAS.Models
{
    public class CrystalReportViewModels
    {
    }
    public class ProposalRepotViewModels
    {
        [Required]
        [Display(Name = "From date")]
        public DateTime FromDate { get; set; }
        [Required]
        [Display(Name = "To date")]
        public DateTime ToDate { get; set; }
        [Required]
        [Display(Name = "Project type")]
        public int ProjecttypeId { get; set; }
        public string Proposalnumber { get; set; }
        public string PI { get; set; }
        public string ProposalTitle { get; set; }
        public string Department { get; set; }
        public DateTime InwardDate { get; set; }
        public int Durationofprojectyears { get; set; }
        public int Durationofprojectmonths { get; set; }
        public DateTime Crtd_TS { get; set; }
        public string keysearch { get; set; }

    }
}