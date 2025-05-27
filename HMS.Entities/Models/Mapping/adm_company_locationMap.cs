using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class adm_company_locationMap : EntityTypeConfiguration<adm_company_location>
    {
        public adm_company_locationMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.LocationName)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Address)
                .HasMaxLength(500);

            this.Property(t => t.ZipCode)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("adm_company_location");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.LocationName).HasColumnName("LocationName");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.CountryDropDownID).HasColumnName("CountryDropDownID");
            this.Property(t => t.CountryID).HasColumnName("CountryID");
            this.Property(t => t.CityDropDownID).HasColumnName("CityDropDownID");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.ZipCode).HasColumnName("ZipCode");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.adm_company_location)
                .HasForeignKey(d => d.CompanyID);
            this.HasOptional(t => t.sys_drop_down_value)
                .WithMany(t => t.adm_company_location)
                .HasForeignKey(d => new { d.CountryID, d.CountryDropDownID });
            this.HasOptional(t => t.sys_drop_down_value1)
                .WithMany(t => t.adm_company_location1)
                .HasForeignKey(d => new { d.CityID, d.CityDropDownID });

        }
    }
}
