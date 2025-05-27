using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_employee_ded_contributionMap : EntityTypeConfiguration<pr_employee_ded_contribution>
    {
        public pr_employee_ded_contributionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID, t.EmployeeID, t.EffectiveFrom });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmployeeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Category)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("pr_employee_ded_contribution");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.EffectiveFrom).HasColumnName("EffectiveFrom");
            this.Property(t => t.EffectiveTo).HasColumnName("EffectiveTo");
            this.Property(t => t.PayScheduleID).HasColumnName("PayScheduleID");
            this.Property(t => t.Category).HasColumnName("Category");
            this.Property(t => t.DeductionContributionID).HasColumnName("DeductionContributionID");
            this.Property(t => t.Percentage).HasColumnName("Percentage");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.StartingBalance).HasColumnName("StartingBalance");
            this.Property(t => t.Taxable).HasColumnName("Taxable");

            // Relationships
            this.HasRequired(t => t.pr_deduction_contribution)
                .WithMany(t => t.pr_employee_ded_contribution)
                .HasForeignKey(d => new { d.DeductionContributionID, d.CompanyID });
            this.HasRequired(t => t.pr_employee_mf)
                .WithMany(t => t.pr_employee_ded_contribution)
                .HasForeignKey(d => new { d.EmployeeID, d.CompanyID });
            this.HasRequired(t => t.pr_pay_schedule)
                .WithMany(t => t.pr_employee_ded_contribution)
                .HasForeignKey(d => new { d.PayScheduleID, d.CompanyID });

        }
    }
}
