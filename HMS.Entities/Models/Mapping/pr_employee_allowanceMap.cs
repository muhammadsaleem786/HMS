using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_employee_allowanceMap : EntityTypeConfiguration<pr_employee_allowance>
    {
        public pr_employee_allowanceMap()
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

            // Table & Column Mappings
            this.ToTable("pr_employee_allowance");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.EffectiveFrom).HasColumnName("EffectiveFrom");
            this.Property(t => t.EffectiveTo).HasColumnName("EffectiveTo");
            this.Property(t => t.PayScheduleID).HasColumnName("PayScheduleID");
            this.Property(t => t.AllowanceID).HasColumnName("AllowanceID");
            this.Property(t => t.Percentage).HasColumnName("Percentage");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Taxable).HasColumnName("Taxable");
            this.Property(t => t.IsHouseOrTransAllow).HasColumnName("IsHouseOrTransAllow");

            // Relationships
            this.HasRequired(t => t.pr_allowance)
                .WithMany(t => t.pr_employee_allowance)
                .HasForeignKey(d => new { d.AllowanceID, d.CompanyID });
            this.HasRequired(t => t.pr_employee_mf)
                .WithMany(t => t.pr_employee_allowance)
                .HasForeignKey(d => new { d.EmployeeID, d.CompanyID });
            this.HasRequired(t => t.pr_pay_schedule)
                .WithMany(t => t.pr_employee_allowance)
                .HasForeignKey(d => new { d.PayScheduleID, d.CompanyID });
        }
    }
}
