using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_documentMap : EntityTypeConfiguration<emr_document>
    {
        public emr_documentMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DocumentUpload)
                .HasMaxLength(500);

            this.Property(t => t.Remarks)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("emr_document");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.DocumentUpload).HasColumnName("DocumentUpload");
            this.Property(t => t.DocumentTypeId).HasColumnName("DocumentTypeId");
            this.Property(t => t.DocumentTypeDropdownId).HasColumnName("DocumentTypeDropdownId");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.ReferenceNo).HasColumnName("ReferenceNo");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_document)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_document)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.emr_document1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.emr_document)
                .HasForeignKey(d => new { d.DocumentTypeId, d.DocumentTypeDropdownId });

        }
    }
}
