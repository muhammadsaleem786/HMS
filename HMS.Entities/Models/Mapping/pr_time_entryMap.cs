using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_time_entryMap : EntityTypeConfiguration<pr_time_entry>
    {
        public pr_time_entryMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Remarks)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("pr_time_entry");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.TimeIn).HasColumnName("TimeIn");
            this.Property(t => t.TimeOut).HasColumnName("TimeOut");
            this.Property(t => t.StatusDropDownID).HasColumnName("StatusDropDownID");
            this.Property(t => t.StatusID).HasColumnName("StatusID");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.LocationID).HasColumnName("LocationID");
            this.Property(t => t.AttendanceDate).HasColumnName("AttendanceDate");
            
            // Relationships
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.pr_time_entry)
                .HasForeignKey(d => new { d.StatusID, d.StatusDropDownID });
            this.HasRequired(t => t.pr_employee_mf)
               .WithMany(t => t.pr_time_entry)
               .HasForeignKey(d => new { d.EmployeeID, d.CompanyID });
        }
    }
}
