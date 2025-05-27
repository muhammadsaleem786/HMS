using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_diagnosMap : EntityTypeConfiguration<emr_diagnos>
    {
        public emr_diagnosMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Diagnos)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("emr_diagnos");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.Diagnos).HasColumnName("Diagnos");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_diagnos)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.emr_diagnos1)
                .HasForeignKey(d => d.ModifiedBy);

        }
    }
}
