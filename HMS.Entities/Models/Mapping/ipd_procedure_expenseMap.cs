using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_procedure_expenseMap : EntityTypeConfiguration<ipd_procedure_expense>
    {
        public ipd_procedure_expenseMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Description)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("ipd_procedure_expense");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.ProcedureId).HasColumnName("ProcedureId");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");            
            this.Property(t => t.CategoryDropdownId).HasColumnName("CategoryDropdownId");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            // Relationships
            this.HasRequired(t => t.adm_company)
              .WithMany(t => t.ipd_procedure_expense)
              .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_procedure_expense)
                .HasForeignKey(d => d.CreatedBy);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_procedure_expense1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.sys_drop_down_value)
               .WithMany(t => t.ipd_procedure_expense)
               .HasForeignKey(d => new { d.CategoryId, d.CategoryDropdownId });
            this.HasOptional(t => t.ipd_procedure_mf)
             .WithMany(t => t.ipd_procedure_expense)
             .HasForeignKey(d => new { d.ProcedureId, d.CompanyID});
        }        
    }
}
