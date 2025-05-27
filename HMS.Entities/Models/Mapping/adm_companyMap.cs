using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class adm_companyMap : EntityTypeConfiguration<adm_company>
    {
        public adm_companyMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyName)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.ContactPersonFirstName)
                .HasMaxLength(250);

            this.Property(t => t.ContactPersonLastName)
                .HasMaxLength(250);

            this.Property(t => t.Phone)
                .HasMaxLength(150);

            this.Property(t => t.Fax)
                .HasMaxLength(150);

            this.Property(t => t.Website)
                .HasMaxLength(150);

            this.Property(t => t.Email)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("adm_company");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyName).HasColumnName("CompanyName");
            this.Property(t => t.CompanyTypeDropDownID).HasColumnName("CompanyTypeDropDownID");
            this.Property(t => t.CompanyTypeID).HasColumnName("CompanyTypeID");
            this.Property(t => t.GenderID).HasColumnName("GenderID");
            this.Property(t => t.ContactPersonFirstName).HasColumnName("ContactPersonFirstName");
            this.Property(t => t.ContactPersonLastName).HasColumnName("ContactPersonLastName");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.Website).HasColumnName("Website");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.IsTrialVersion).HasColumnName("IsTrialVersion");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.CompanyAddress1).HasColumnName("CompanyAddress1");
            this.Property(t => t.CompanyAddress2).HasColumnName("CompanyAddress2");
            this.Property(t => t.CityDropDownId).HasColumnName("CityDropDownId");
            this.Property(t => t.CompanyLogo).HasColumnName("CompanyLogo");
            this.Property(t => t.CountryDropdownId).HasColumnName("CountryDropdownId");
            this.Property(t => t.LanguageID).HasColumnName("LanguageID");
            this.Property(t => t.PostalCode).HasColumnName("PostalCode");
            this.Property(t => t.Province).HasColumnName("Province");
            this.Property(t => t.IsShowBillReceptionist).HasColumnName("IsShowBillReceptionist");
            
            this.Property(t => t.IsBackDatedAppointment).HasColumnName("IsBackDatedAppointment");
            this.Property(t => t.IsUpdateBillDate).HasColumnName("IsUpdateBillDate");
            this.Property(t => t.StandardShiftHours).HasColumnName("StandardShiftHours");

            this.Property(t => t.IsCNICMandatory).HasColumnName("IsCNICMandatory");
            this.Property(t => t.DateFormatDropDownID).HasColumnName("DateFormatDropDownID");
            this.Property(t => t.DateFormatId).HasColumnName("DateFormatId");
            this.Property(t => t.ReceiptFooter).HasColumnName("ReceiptFooter");
            this.Property(t => t.SalaryMethodID).HasColumnName("SalaryMethodID");
            this.Property(t => t.WDMonday).HasColumnName("WDMonday");
            this.Property(t => t.WDTuesday).HasColumnName("WDTuesday");
            this.Property(t => t.WDWednesday).HasColumnName("WDWednesday");
            this.Property(t => t.WDThursday).HasColumnName("WDThursday");
            this.Property(t => t.WDFriday).HasColumnName("WDFriday");
            this.Property(t => t.WDSatuday).HasColumnName("WDSatuday");
            this.Property(t => t.WDSunday).HasColumnName("WDSunday");
            this.Property(t => t.TokenNo).HasColumnName("TokenNo");
            // Relationships
            this.HasOptional(t => t.adm_user_mf)
                .WithMany(t => t.adm_company)
                .HasForeignKey(d => d.ModifiedBy);
            this.HasRequired(t => t.adm_user_mf1)
                .WithMany(t => t.adm_company1)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.sys_drop_down_value)
                .WithMany(t => t.adm_company)
                .HasForeignKey(d => new { d.CompanyTypeID, d.CompanyTypeDropDownID });

        }
    }
}
