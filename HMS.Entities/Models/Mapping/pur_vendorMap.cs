using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class pur_vendorMap : EntityTypeConfiguration<pur_vendor>
    {
        public pur_vendorMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });
            // Properties
            this.Property(t => t.ID)
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.FirstName)
                .HasMaxLength(150);
            this.Property(t => t.LastName)
                .HasMaxLength(150);
            this.Property(t => t.CompanyName)
                .HasMaxLength(250);
            this.Property(t => t.VendorPhone)
                .IsRequired()
                .HasMaxLength(300);
            this.Property(t => t.VendorEmail)
                .HasMaxLength(250);
            this.Property(t => t.Address)
               .HasMaxLength(250);
            this.Property(t => t.Address2)
                .HasMaxLength(250);
            this.Property(t => t.City)
                .HasMaxLength(150);
            this.Property(t => t.State)
                .HasMaxLength(250);
            this.Property(t => t.ZipCode)
                .HasMaxLength(20);
            this.Property(t => t.Phone)
                .HasMaxLength(20);
            this.Property(t => t.Fax)
                .HasMaxLength(20);
            // Table & Column Mappings
            this.ToTable("pur_vendor");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.CompanyName).HasColumnName("CompanyName");
            this.Property(t => t.VendorPhone).HasColumnName("VendorPhone");
            this.Property(t => t.VendorEmail).HasColumnName("VendorEmail");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Address2).HasColumnName("Address2");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.ZipCode).HasColumnName("ZipCode");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Fax).HasColumnName("Fax");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pur_vendor)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.pur_vendor)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.pur_vendor1)
                .HasForeignKey(d => d.ModifiedBy);
        }
    }
}
