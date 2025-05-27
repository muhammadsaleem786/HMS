using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_employee_mfMap : EntityTypeConfiguration<pr_employee_mf>
    {
        public pr_employee_mfMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Gender)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.StreetAddress)
                .HasMaxLength(500);

            this.Property(t => t.ZipCode)
                .HasMaxLength(150);

            this.Property(t => t.Email)
                .HasMaxLength(250);

            this.Property(t => t.HomePhone)
                .HasMaxLength(150);

            this.Property(t => t.Mobile)
                .HasMaxLength(150);

            this.Property(t => t.EmergencyContactPerson)
                .HasMaxLength(250);

            this.Property(t => t.EmergencyContactNo)
                .HasMaxLength(150);

            this.Property(t => t.BankName)
                .HasMaxLength(250);

            this.Property(t => t.BranchName)
                .HasMaxLength(250);

            this.Property(t => t.BranchCode)
                .HasMaxLength(150);

            this.Property(t => t.AccountNo)
                .HasMaxLength(250);

            this.Property(t => t.SwiftCode)
                .HasMaxLength(150);

            this.Property(t => t.NICNo)
                .HasMaxLength(150);

            this.Property(t => t.NationalSecurityNo)
                .HasMaxLength(150);

            this.Property(t => t.EmployeeCode)
                .HasMaxLength(50);

            this.Property(t => t.EmployeePic)
                .HasMaxLength(250);

            this.Property(t => t.SubContractCompanyName)
                .HasMaxLength(250);

            this.Property(t => t.PassportNumber)
                .HasMaxLength(250);

            this.Property(t => t.SCHSNO)
                .HasMaxLength(150);

            this.Property(t => t.MedicalInsuranceProvided)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.InsuranceCardNo)
                .HasMaxLength(150);

            this.Property(t => t.AirTicketProvided)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.AirTicketRemarks)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("pr_employee_mf");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.DOB).HasColumnName("DOB");
            this.Property(t => t.StreetAddress).HasColumnName("StreetAddress");
            this.Property(t => t.CityDropDownID).HasColumnName("CityDropDownID");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.ZipCode).HasColumnName("ZipCode");
            this.Property(t => t.CountryDropDownID).HasColumnName("CountryDropDownID");
            this.Property(t => t.CountryID).HasColumnName("CountryID");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.HomePhone).HasColumnName("HomePhone");
            this.Property(t => t.Mobile).HasColumnName("Mobile");
            this.Property(t => t.EmergencyContactPerson).HasColumnName("EmergencyContactPerson");
            this.Property(t => t.EmergencyContactNo).HasColumnName("EmergencyContactNo");
            this.Property(t => t.HireDate).HasColumnName("HireDate");
            this.Property(t => t.JoiningDate).HasColumnName("JoiningDate");
            this.Property(t => t.PayTypeDropDownID).HasColumnName("PayTypeDropDownID");
            this.Property(t => t.PayTypeID).HasColumnName("PayTypeID");
            this.Property(t => t.BasicSalary).HasColumnName("BasicSalary");
            this.Property(t => t.StatusDropDownID).HasColumnName("StatusDropDownID");
            this.Property(t => t.StatusID).HasColumnName("StatusID");
            this.Property(t => t.TerminatedDate).HasColumnName("TerminatedDate");
            this.Property(t => t.FinalSettlementDate).HasColumnName("FinalSettlementDate");
            this.Property(t => t.PaymentMethodDropDownID).HasColumnName("PaymentMethodDropDownID");
            this.Property(t => t.PaymentMethodID).HasColumnName("PaymentMethodID");
            this.Property(t => t.BankName).HasColumnName("BankName");
            this.Property(t => t.BranchName).HasColumnName("BranchName");
            this.Property(t => t.BranchCode).HasColumnName("BranchCode");
            this.Property(t => t.AccountNo).HasColumnName("AccountNo");
            this.Property(t => t.SwiftCode).HasColumnName("SwiftCode");
            this.Property(t => t.EmployeeTypeDropDownID).HasColumnName("EmployeeTypeDropDownID");
            this.Property(t => t.EmployeeTypeID).HasColumnName("EmployeeTypeID");
            this.Property(t => t.TypeStartDate).HasColumnName("TypeStartDate");
            this.Property(t => t.TypeEndDate).HasColumnName("TypeEndDate");
            this.Property(t => t.DesignationID).HasColumnName("DesignationID");
            this.Property(t => t.DepartmentID).HasColumnName("DepartmentID");
            this.Property(t => t.NICNoExpiryDate).HasColumnName("NICNoExpiryDate");
            this.Property(t => t.NICNo).HasColumnName("NICNo");
            this.Property(t => t.NationalSecurityNo).HasColumnName("NationalSecurityNo");
            this.Property(t => t.NationalityDropDownID).HasColumnName("NationalityDropDownID");
            this.Property(t => t.NationalityID).HasColumnName("NationalityID");
            this.Property(t => t.EmployeeCode).HasColumnName("EmployeeCode");
            this.Property(t => t.PayScheduleID).HasColumnName("PayScheduleID");
            this.Property(t => t.EmployeePic).HasColumnName("EmployeePic");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.SubContractCompanyName).HasColumnName("SubContractCompanyName");
            this.Property(t => t.PassportNumber).HasColumnName("PassportNumber");
            this.Property(t => t.PassportExpiryDate).HasColumnName("PassportExpiryDate");
            this.Property(t => t.SCHSNO).HasColumnName("SCHSNO");
            this.Property(t => t.SCHSNOExpiryDate).HasColumnName("SCHSNOExpiryDate");
            this.Property(t => t.MedicalInsuranceProvided).HasColumnName("MedicalInsuranceProvided");
            this.Property(t => t.InsuranceCardNo).HasColumnName("InsuranceCardNo");
            this.Property(t => t.InsuranceCardNoExpiryDate).HasColumnName("InsuranceCardNoExpiryDate");
            this.Property(t => t.InsuranceClassTypeDropdownID).HasColumnName("InsuranceClassTypeDropdownID");
            this.Property(t => t.InsuranceClassTypeID).HasColumnName("InsuranceClassTypeID");
            this.Property(t => t.AirTicketProvided).HasColumnName("AirTicketProvided");
            this.Property(t => t.NoOfAirTicket).HasColumnName("NoOfAirTicket");
            this.Property(t => t.AirTicketClassTypeDropdownID).HasColumnName("AirTicketClassTypeDropdownID");
            this.Property(t => t.AirTicketClassTypeID).HasColumnName("AirTicketClassTypeID");
            this.Property(t => t.AirTicketFrequencyTypeDropdownID).HasColumnName("AirTicketFrequencyTypeDropdownID");
            this.Property(t => t.AirTicketFrequencyTypeID).HasColumnName("AirTicketFrequencyTypeID");
            this.Property(t => t.OriginCountryDropdownTypeID).HasColumnName("OriginCountryDropdownTypeID");
            this.Property(t => t.OriginCountryTypeID).HasColumnName("OriginCountryTypeID");
            this.Property(t => t.DestinationCountryDropdownTypeID).HasColumnName("DestinationCountryDropdownTypeID");
            this.Property(t => t.DestinationCountryTypeID).HasColumnName("DestinationCountryTypeID");
            this.Property(t => t.OriginCityDropdownTypeID).HasColumnName("OriginCityDropdownTypeID");
            this.Property(t => t.OriginCityTypeID).HasColumnName("OriginCityTypeID");
            this.Property(t => t.DestinationCityDropdownTypeID).HasColumnName("DestinationCityDropdownTypeID");
            this.Property(t => t.DestinationCityTypeID).HasColumnName("DestinationCityTypeID");
            this.Property(t => t.AirTicketRemarks).HasColumnName("AirTicketRemarks");
            this.Property(t => t.MaritalStatusTypeID).HasColumnName("MaritalStatusTypeID");
            this.Property(t => t.MaritalStatusTypeDropdownID).HasColumnName("MaritalStatusTypeDropdownID");
            this.Property(t => t.SpecialtyTypeDropdownID).HasColumnName("SpecialtyTypeDropdownID");
            this.Property(t => t.SpecialtyTypeID).HasColumnName("SpecialtyTypeID");
            this.Property(t => t.ClassificationTypeDropdownID).HasColumnName("ClassificationTypeDropdownID");
            this.Property(t => t.ClassificationTypeID).HasColumnName("ClassificationTypeID");
            this.Property(t => t.TotalPolicyAmountMonthly).HasColumnName("TotalPolicyAmountMonthly");
            this.Property(t => t.ExculdeFromPayroll).HasColumnName("ExculdeFromPayroll");
            this.Property(t => t.EffectiveDate).HasColumnName("EffectiveDate");
            this.Property(t => t.IsActive).HasColumnName("IsActive"); 
            this.Property(t => t.TimeRule).HasColumnName("TimeRule");
            this.Property(t => t.LastArrival).HasColumnName("LastArrival");
            this.Property(t => t.ShiftID).HasColumnName("ShiftID");
            this.Property(t => t.EarlyOut).HasColumnName("EarlyOut");
            this.Property(t => t.LeaveRestrictions).HasColumnName("LeaveRestrictions");
            this.Property(t => t.MissingAttendance).HasColumnName("MissingAttendance");


            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pr_employee_mf)
                .HasForeignKey(d => d.CompanyID);
            this.HasOptional(t => t.pr_department)
                .WithMany(t => t.pr_employee_mf)
                .HasForeignKey(d => new { d.DepartmentID, d.CompanyID });
            this.HasOptional(t => t.pr_designation)
                .WithMany(t => t.pr_employee_mf)
                .HasForeignKey(d => new { d.DesignationID, d.CompanyID });
            this.HasOptional(t => t.pr_pay_schedule)
                .WithMany(t => t.pr_employee_mf)
                .HasForeignKey(d => new { d.PayScheduleID, d.CompanyID });
            //this.HasOptional(t => t.CityList)
            //    .WithMany(t => t.pr_employee_mf)
            //    .HasForeignKey(d => new { d.CityID, d.CityDropDownID });
            //this.HasOptional(t => t.CountryList)
            //    .WithMany(t => t.pr_employee_mf1)
            //    .HasForeignKey(d => new { d.CountryID, d.CountryDropDownID });
            //this.HasOptional(t => t.DestinationCountryList)
            //    .WithMany(t => t.pr_employee_mf2)
            //    .HasForeignKey(d => new { d.DestinationCountryTypeID, d.DestinationCountryDropdownTypeID });
            //this.HasOptional(t => t.OriginCityList)
            //    .WithMany(t => t.pr_employee_mf3)
            //    .HasForeignKey(d => new { d.OriginCityTypeID, d.OriginCityDropdownTypeID });
            //this.HasOptional(t => t.DestinationCityList)
            //    .WithMany(t => t.pr_employee_mf4)
            //    .HasForeignKey(d => new { d.DestinationCityTypeID, d.DestinationCityDropdownTypeID });
            //this.HasOptional(t => t.AirTicketClassTypeList)
            //    .WithMany(t => t.pr_employee_mf5)
            //    .HasForeignKey(d => new { d.AirTicketClassTypeID, d.AirTicketClassTypeDropdownID });
            //this.HasRequired(t => t.PayTypeList)
            //    .WithMany(t => t.pr_employee_mf6)
            //    .HasForeignKey(d => new { d.PayTypeID, d.PayTypeDropDownID });
            this.HasRequired(t => t.StatusList)
                .WithMany(t => t.pr_employee_mf7)
                .HasForeignKey(d => new { d.StatusID, d.StatusDropDownID });
            //this.HasRequired(t => t.PaymentMethodList)
            //    .WithMany(t => t.pr_employee_mf8)
            //    .HasForeignKey(d => new { d.PaymentMethodID, d.PaymentMethodDropDownID });
            //this.HasRequired(t => t.EmployeeTypeList)
            //    .WithMany(t => t.pr_employee_mf9)
            //    .HasForeignKey(d => new { d.EmployeeTypeID, d.EmployeeTypeDropDownID });
            //this.HasOptional(t => t.NationalityList)
            //    .WithMany(t => t.pr_employee_mf10)
            //    .HasForeignKey(d => new { d.NationalityID, d.NationalityDropDownID });
            //this.HasOptional(t => t.InsuranceClassList)
            //    .WithMany(t => t.pr_employee_mf11)
            //    .HasForeignKey(d => new { d.InsuranceClassTypeID, d.InsuranceClassTypeDropdownID });
            //this.HasOptional(t => t.AirTicketFrequencyList)
            //    .WithMany(t => t.pr_employee_mf12)
            //    .HasForeignKey(d => new { d.AirTicketFrequencyTypeID, d.AirTicketFrequencyTypeDropdownID });
            //this.HasOptional(t => t.OriginCountryList)
            //    .WithMany(t => t.pr_employee_mf13)
            //    .HasForeignKey(d => new { d.OriginCountryTypeID, d.OriginCountryDropdownTypeID });
            //this.HasOptional(t => t.MaritalStatusList)
            //   .WithMany(t => t.pr_employee_mf14)
            //   .HasForeignKey(d => new { d.MaritalStatusTypeID, d.MaritalStatusTypeDropdownID });
            //this.HasOptional(t => t.ContractTypeList)
            //  .WithMany(t => t.pr_employee_mf15)
            //  .HasForeignKey(d => new { d.ContractTypeID, d.ContractTypeDropDownID });
            //this.HasOptional(t => t.SpecialtyList)
            // .WithMany(t => t.pr_employee_mf16)
            // .HasForeignKey(d => new { d.SpecialtyTypeID, d.SpecialtyTypeDropdownID });
            //this.HasOptional(t => t.ClassificationList)
            // .WithMany(t => t.pr_employee_mf17)
            // .HasForeignKey(d => new { d.ClassificationTypeID, d.ClassificationTypeDropdownID });
        }
    }
}
