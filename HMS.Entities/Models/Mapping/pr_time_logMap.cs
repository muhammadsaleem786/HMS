using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{

    public class pr_time_logMap : EntityTypeConfiguration<pr_time_log>
    {
        public pr_time_logMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID });
            // Properties
            this.Property(t => t.ID)
     .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmployeeCode)
                .IsRequired()
                .HasMaxLength(25);

            // Table & Column Mappings
            this.ToTable("pr_time_log");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.EmployeeCode).HasColumnName("EmployeeCode");
            this.Property(t => t.AttendanceTime).HasColumnName("AttendanceTime");
            this.Property(t => t.LocationCode).HasColumnName("LocationCode");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.AttendanceMode).HasColumnName("AttendanceMode");
            this.Property(t => t.IPAddress).HasColumnName("IPAddress");       
        }
    }
}
