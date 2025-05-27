using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class emr_expenseMap : EntityTypeConfiguration<emr_expense>
    {
        public emr_expenseMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Remark)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.InvoiceNumber)
                .HasMaxLength(50);

            this.Property(t => t.Vendor)
                .HasMaxLength(150);

            this.Property(t => t.PaymentRemrks)
                .HasMaxLength(250);

            this.Property(t => t.Attachment)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("emr_expense");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.CategoryDropdownId).HasColumnName("CategoryDropdownId");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Remark).HasColumnName("Remark");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.ClinicId).HasColumnName("ClinicId");
            this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
            this.Property(t => t.InvoiceNumber).HasColumnName("InvoiceNumber");
            this.Property(t => t.Vendor).HasColumnName("Vendor");
            this.Property(t => t.PaymentStatusId).HasColumnName("PaymentStatusId");
            this.Property(t => t.PaymentStatusDropdownId).HasColumnName("PaymentStatusDropdownId");
            this.Property(t => t.PaymentRemrks).HasColumnName("PaymentRemrks");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.emr_expense)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.emr_expense)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.emr_expense1)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.emr_expense)
                .HasForeignKey(d => new { d.CategoryId, d.CategoryDropdownId });

        }
    }
}
