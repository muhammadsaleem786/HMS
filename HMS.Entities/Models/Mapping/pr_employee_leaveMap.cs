using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_employee_leaveMap : EntityTypeConfiguration<pr_employee_leave>
    {
        public pr_employee_leaveMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID, t.EmployeeID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmployeeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("pr_employee_leave");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.LeaveTypeID).HasColumnName("LeaveTypeID");
            this.Property(t => t.Hours).HasColumnName("Hours");

            // Relationships
            this.HasRequired(t => t.pr_employee_mf)
                .WithMany(t => t.pr_employee_leave)
                .HasForeignKey(d => new { d.EmployeeID, d.CompanyID });
            this.HasRequired(t => t.pr_leave_type)
                .WithMany(t => t.pr_employee_leave)
                .HasForeignKey(d => new { d.LeaveTypeID, d.CompanyID });

        }
    }
}
