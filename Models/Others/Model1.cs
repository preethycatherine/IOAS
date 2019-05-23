namespace IOAS.Models.Others
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model11")
        {
        }

        public virtual DbSet<tblTravelInsurance> tblTravelInsurances { get; set; }
        public virtual DbSet<tblTravelinsurancepdf> tblTravelinsurancepdfs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblTravelInsurance>()
                .Property(e => e.adhar_card_name)
                .IsUnicode(false);

            modelBuilder.Entity<tblTravelInsurance>()
                .Property(e => e.disease)
                .IsUnicode(false);

            modelBuilder.Entity<tblTravelInsurance>()
                .Property(e => e.disease_details)
                .IsUnicode(false);

            modelBuilder.Entity<tblTravelInsurance>()
                .Property(e => e.Premium_Amount)
                .HasPrecision(18, 0);

            modelBuilder.Entity<tblTravelInsurance>()
                .HasOptional(e => e.tblTravelinsurancepdf)
                .WithRequired(e => e.tblTravelInsurance);
        }
    }
}
