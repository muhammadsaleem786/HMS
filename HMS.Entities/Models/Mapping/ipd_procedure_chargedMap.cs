using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_procedure_chargedMap : EntityTypeConfiguration<ipd_procedure_charged>
    {
        public ipd_procedure_chargedMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Item)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ipd_procedure_charged");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.ProcedureId).HasColumnName("ProcedureId");
            this.Property(t => t.AppointmentId).HasColumnName("AppointmentId");
            this.Property(t => t.PatientId).HasColumnName("PatientId");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Item).HasColumnName("Item");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ItemId).HasColumnName("ItemId");
            this.Property(t => t.IsBatch).HasColumnName("IsBatch");
            this.Property(t => t.Batch).HasColumnName("Batch");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_procedure_charged)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_procedure_charged)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_procedure_charged1)
                .HasForeignKey(d => d.ModifiedBy);
            
            this.HasRequired(t => t.ipd_procedure_mf)
                .WithMany(t => t.ipd_procedure_charged)
                .HasForeignKey(d => new { d.ProcedureId, d.CompanyId });
            this.HasRequired(t => t.adm_item)
                .WithMany(t => t.ipd_procedure_charged)
                .HasForeignKey(d => new { d.ItemId, d.CompanyId });

        }
    }
}
