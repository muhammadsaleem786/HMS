using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_employee_payroll_mfMap : EntityTypeConfiguration<pr_employee_payroll_mf>
    {
        public pr_employee_payroll_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId, t.PayScheduleID, t.PayDate });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PayScheduleID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Status)
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("pr_employee_payroll_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyID");
            this.Property(t => t.PayScheduleID).HasColumnName("PayScheduleID");
            this.Property(t => t.PayDate).HasColumnName("PayDate");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.PayScheduleFromDate).HasColumnName("PayScheduleFromDate");
            this.Property(t => t.PayScheduleToDate).HasColumnName("PayScheduleToDate");
            this.Property(t => t.FromDate).HasColumnName("FromDate");
            this.Property(t => t.ToDate).HasColumnName("ToDate");
            this.Property(t => t.BasicSalary).HasColumnName("BasicSalary");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.AdjustmentDate).HasColumnName("AdjustmentDate");
            this.Property(t => t.AdjustmentType).HasColumnName("AdjustmentType");
            this.Property(t => t.AdjustmentAmount).HasColumnName("AdjustmentAmount");
            this.Property(t => t.AdjustmentComments).HasColumnName("AdjustmentComments");
            this.Property(t => t.AdjustmentBy).HasColumnName("AdjustmentBy");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pr_employee_payroll_mf)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.pr_employee_mf)
                .WithMany(t => t.pr_employee_payroll_mf)
                .HasForeignKey(d => new { d.EmployeeID, d.CompanyId });
            this.HasRequired(t => t.pr_pay_schedule)
                .WithMany(t => t.pr_employee_payroll_mf)
                .HasForeignKey(d => new { d.PayScheduleID, d.CompanyId});

        }
    }
}
