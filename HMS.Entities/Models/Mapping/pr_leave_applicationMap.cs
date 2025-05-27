using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_leave_applicationMap : EntityTypeConfiguration<pr_leave_application>
    {
        public pr_leave_applicationMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("pr_leave_application");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.LeaveTypeID).HasColumnName("LeaveTypeID");
            this.Property(t => t.FromDate).HasColumnName("FromDate");
            this.Property(t => t.ToDate).HasColumnName("ToDate");
            this.Property(t => t.Hours).HasColumnName("Hours");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pr_leave_application)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.pr_employee_mf)
                .WithMany(t => t.pr_leave_application)
                .HasForeignKey(d => new { d.EmployeeID, d.CompanyID });
            this.HasRequired(t => t.pr_leave_type)
                .WithMany(t => t.pr_leave_application)
                .HasForeignKey(d => new { d.LeaveTypeID, d.CompanyID });
        }
    }
}
