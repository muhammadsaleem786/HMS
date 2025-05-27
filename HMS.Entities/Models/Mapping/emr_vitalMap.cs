using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_vitalMap : EntityTypeConfiguration<emr_vital>
    {
        public emr_vitalMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.Measure)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("emr_vital");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.Measure).HasColumnName("Measure");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.VitalId).HasColumnName("VitalId");
            this.Property(t => t.VitalDropdownId).HasColumnName("VitalDropdownId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_vital)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_vital)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.emr_vital1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.emr_vital)
                .HasForeignKey(d => new { d.VitalId, d.VitalDropdownId });
        }
    }
}
