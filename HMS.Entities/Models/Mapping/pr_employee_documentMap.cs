using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class pr_employee_documentMap : EntityTypeConfiguration<pr_employee_document>
    {
        public pr_employee_documentMap()
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
            this.ToTable("pr_employee_document");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.DocumentTypeID).HasColumnName("DocumentTypeID");
            this.Property(t => t.DocumentTypeDropdownID).HasColumnName("DocumentTypeDropdownID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.AttachmentPath).HasColumnName("AttachmentPath");
            this.Property(t => t.UploadDate).HasColumnName("UploadDate");
            this.Property(t => t.ExpireDate).HasColumnName("ExpireDate");
    
            // Relationships
            this.HasRequired(t => t.pr_employee_mf)
                    .WithMany(t => t.pr_employee_document)
                    .HasForeignKey(d => new { d.EmployeeID, d.CompanyID });
            this.HasRequired(t => t.sys_drop_down_value)
           .WithMany(t => t.pr_employee_document)
           .HasForeignKey(d => new { d.DocumentTypeID, d.DocumentTypeDropdownID });

        }
    }
}
