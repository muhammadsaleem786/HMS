using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_employee_payroll_dtMap : EntityTypeConfiguration<pr_employee_payroll_dt>
    {
        public pr_employee_payroll_dtMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.PayrollID, t.CompanyId, t.PayScheduleID, t.PayDate });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PayrollID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PayScheduleID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Type)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.AdjustmentType)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.AdjustmentComments)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("pr_employee_payroll_dt");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PayrollID).HasColumnName("PayrollID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyID");
            this.Property(t => t.PayScheduleID).HasColumnName("PayScheduleID");
            this.Property(t => t.PayDate).HasColumnName("PayDate");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.AllowDedID).HasColumnName("AllowDedID");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Taxable).HasColumnName("Taxable");
            this.Property(t => t.AdjustmentDate).HasColumnName("AdjustmentDate");
            this.Property(t => t.AdjustmentType).HasColumnName("AdjustmentType");
            this.Property(t => t.AdjustmentAmount).HasColumnName("AdjustmentAmount");
            this.Property(t => t.AdjustmentComments).HasColumnName("AdjustmentComments");
            this.Property(t => t.AdjustmentBy).HasColumnName("AdjustmentBy");
            this.Property(t => t.RefID).HasColumnName("RefID");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.ArrearReleatedMonth).HasColumnName("ArrearReleatedMonth");

            // Relationships
            this.HasRequired(t => t.pr_employee_mf)
                .WithMany(t => t.pr_employee_payroll_dt)
                .HasForeignKey(d => new { d.EmployeeID, d.CompanyId });
            this.HasRequired(t => t.pr_employee_payroll_mf)
                .WithMany(t => t.pr_employee_payroll_dt)
                .HasForeignKey(d => new { d.PayrollID, d.CompanyId, d.PayScheduleID, d.PayDate });

        }
    }
}
