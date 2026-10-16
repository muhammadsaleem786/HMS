using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class adm_user_mfMap : EntityTypeConfiguration<adm_user_mf>
    {
        public adm_user_mfMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Pwd)
                .HasMaxLength(1000);

            this.Property(t => t.Name)
                .HasMaxLength(100);

            this.Property(t => t.PhoneNo)
                .HasMaxLength(50);

            this.Property(t => t.AccountType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.ActivationToken)
                .HasMaxLength(1000);

            this.Property(t => t.ForgotToken)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("adm_user_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Pwd).HasColumnName("Pwd");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.PhoneNo).HasColumnName("PhoneNo");
            this.Property(t => t.AccountType).HasColumnName("AccountType");
            this.Property(t => t.CultureID).HasColumnName("CultureID");
            this.Property(t => t.AccountStatus).HasColumnName("AccountStatus");
            this.Property(t => t.LoginFailureNo).HasColumnName("LoginFailureNo");
            this.Property(t => t.UserLock).HasColumnName("UserLock");
            this.Property(t => t.IsActivated).HasColumnName("IsActivated");
            this.Property(t => t.ActivationToken).HasColumnName("ActivationToken");
            this.Property(t => t.ActivationTokenDate).HasColumnName("ActivationTokenDate");
            this.Property(t => t.ActivatedDate).HasColumnName("ActivatedDate");
            this.Property(t => t.LastSignIn).HasColumnName("LastSignIn");
            this.Property(t => t.ForgotToken).HasColumnName("ForgotToken");
            this.Property(t => t.ForgotTokenDate).HasColumnName("ForgotTokenDate");
            this.Property(t => t.PhoneNotification).HasColumnName("PhoneNotification");
            this.Property(t => t.EmailNotification).HasColumnName("EmailNotification");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.MultilingualId).HasColumnName("MultilingualId");
            this.Property(t => t.IsOverLap).HasColumnName("IsOverLap");
            this.Property(t => t.IsShowDoctor).HasColumnName("IsShowDoctor");
            this.Property(t => t.Qualification).HasColumnName("Qualification");
            this.Property(t => t.Designation).HasColumnName("Designation");
            this.Property(t => t.SpecialtyId).HasColumnName("SpecialtyId");
            this.Property(t => t.SpecialtyDropdownId).HasColumnName("SpecialtyDropdownId");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.ShowOnCalendar).HasColumnName("ShowOnCalendar");
            this.Property(t => t.UserImage).HasColumnName("UserImage");
            this.Property(t => t.ExpiryDate).HasColumnName("ExpiryDate");
            this.Property(t => t.IsGenderDropdown).HasColumnName("IsGenderDropdown");
            this.Property(t => t.RepotFooter).HasColumnName("RepotFooter");
            this.Property(t => t.AppointmentStatusId).HasColumnName("AppointmentStatusId");
            this.Property(t => t.TemplateId).HasColumnName("TemplateId");
            this.Property(t => t.DesignationUrdu).HasColumnName("DesignationUrdu");
            this.Property(t => t.QualificationUrdu).HasColumnName("QualificationUrdu");
            this.Property(t => t.HeaderDescription).HasColumnName("HeaderDescription");
            this.Property(t => t.HeaderDescriptionUrdu).HasColumnName("HeaderDescriptionUrdu");
            this.Property(t => t.NameUrdu).HasColumnName("NameUrdu");
            this.Property(t => t.Instruction).HasColumnName("Instruction");
            this.Property(t => t.TotalUser).HasColumnName("TotalUser");
            this.Property(t => t.Charges).HasColumnName("Charges");
            this.Property(t => t.BrandImage).HasColumnName("BrandImage");
            this.Property(t => t.BrandDescription).HasColumnName("BrandDescription");
            this.Property(t => t.DrugMode).HasColumnName("DrugMode");
            this.Property(t => t.DeductOTCharges).HasColumnName("DeductOTCharges");
            this.Property(t => t.DeductMedicineCost).HasColumnName("DeductMedicineCost");
            this.Property(t => t.DeductConsumables).HasColumnName("DeductConsumables");
            this.Property(t => t.DeductImplantCost).HasColumnName("DeductImplantCost");
            this.Property(t => t.DeductLabConsumables).HasColumnName("DeductLabConsumables");
            this.Property(t => t.DeductRoomCharges).HasColumnName("DeductRoomCharges");
            this.Property(t => t.DoctorShare).HasColumnName("DoctorShare");
            this.Property(t => t.OwnerShare).HasColumnName("OwnerShare");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.IsShowHeader).HasColumnName("IsShowHeader");
            // Relationships
            this.HasOptional(t => t.sys_drop_down_value)
                .WithMany(t => t.adm_user_mf)
                .HasForeignKey(d => new { d.SpecialtyId, d.SpecialtyDropdownId });
        }
    }
}
