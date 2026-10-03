using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class sale_customerMap : EntityTypeConfiguration<sale_customer>
    {
        public sale_customerMap()
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
            this.Property(t => t.Phone)
                .HasMaxLength(300);
            this.Property(t => t.Email)
                .HasMaxLength(250);
            this.Property(t => t.Address)
               .HasMaxLength(250);           
            this.Property(t => t.City)
                .HasMaxLength(150);
            this.Property(t => t.State)
                .HasMaxLength(250);
            this.Property(t => t.ZipCode)
                .HasMaxLength(20);
            this.Property(t => t.Phone)
                .HasMaxLength(20);
            // Table & Column Mappings
            this.ToTable("sale_customer");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.CompanyName).HasColumnName("CompanyName");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Address).HasColumnName("Address");
           this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.ZipCode).HasColumnName("ZipCode");
            this.Property(t => t.Phone).HasColumnName("Phone");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.sale_customer)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.sale_customer)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.sale_customer1)
                .HasForeignKey(d => d.ModifiedBy);
        }
    }
}
