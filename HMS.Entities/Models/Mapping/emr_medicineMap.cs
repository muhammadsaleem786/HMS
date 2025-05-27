using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_medicineMap : EntityTypeConfiguration<emr_medicine>
    {
        public emr_medicineMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Medicine)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("emr_medicine");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.Medicine).HasColumnName("Medicine");
            this.Property(t => t.UnitId).HasColumnName("UnitId");
            this.Property(t => t.UnitDropdownId).HasColumnName("UnitDropdownId");
            this.Property(t => t.TypeId).HasColumnName("TypeId");
            this.Property(t => t.TypeDropdownId).HasColumnName("TypeDropdownId");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Measure).HasColumnName("Measure");
            this.Property(t => t.InstructionId).HasColumnName("InstructionId");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_medicine)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_medicine)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.emr_medicine1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasOptional(t => t.sys_drop_down_value)
                .WithMany(t => t.emr_medicine)
                .HasForeignKey(d => new { d.UnitId, d.UnitDropdownId });
            this.HasOptional(t => t.sys_drop_down_value1)
                .WithMany(t => t.emr_medicine1)
                .HasForeignKey(d => new { d.TypeId, d.TypeDropdownId });

            this.HasOptional(t => t.emr_instruction)
               .WithMany(t => t.emr_medicine)
               .HasForeignKey(d => new { d.InstructionId,d.CompanyID });
        }
    }
}
