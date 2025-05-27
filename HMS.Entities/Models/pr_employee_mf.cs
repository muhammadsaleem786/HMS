using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_employee_mf : Entity
    {
        public pr_employee_mf()
        {
            this.adm_user_company = new List<adm_user_company>();
            this.pr_employee_allowance = new List<pr_employee_allowance>();
            this.pr_employee_ded_contribution = new List<pr_employee_ded_contribution>();
            this.pr_employee_leave = new List<pr_employee_leave>();
            this.pr_employee_payroll_dt = new List<pr_employee_payroll_dt>();
            this.pr_employee_payroll_mf = new List<pr_employee_payroll_mf>();
            this.pr_employee_Dependent = new List<pr_employee_Dependent>();
            this.pr_leave_application = new List<pr_leave_application>();
            this.pr_time_entry = new List<pr_time_entry>();
            this.pr_employee_document = new List<pr_employee_document>();
            //this.adm_user_mf = new List<adm_user_mf>();
            this.pr_loan = new List<pr_loan>();
        }
        [Key]
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string StreetAddress { get; set; }
        public Nullable<int> CityDropDownID { get; set; }
        public Nullable<int> CityID { get; set; }
        public string ZipCode { get; set; }
        public Nullable<int> CountryDropDownID { get; set; }
        public Nullable<int> CountryID { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string Mobile { get; set; }
        public string EmergencyContactPerson { get; set; }
        public string EmergencyContactNo { get; set; }
        public Nullable<System.DateTime> HireDate { get; set; }
        public Nullable<System.DateTime> JoiningDate { get; set; }
        public int PayTypeDropDownID { get; set; }
        public int PayTypeID { get; set; }
        public Nullable<double> BasicSalary { get; set; }
        public int StatusDropDownID { get; set; }
        public int StatusID { get; set; }
        public Nullable<System.DateTime> TerminatedDate { get; set; }
        public Nullable<System.DateTime> FinalSettlementDate { get; set; }

        public int PaymentMethodDropDownID { get; set; }
        public int PaymentMethodID { get; set; }
        public Nullable<int> SpecialtyTypeDropdownID { get; set; }
        public Nullable<int> SpecialtyTypeID { get; set; }
        public Nullable<int> ClassificationTypeDropdownID { get; set; }
        public Nullable<int> ClassificationTypeID { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string AccountNo { get; set; }
        public string SwiftCode { get; set; }
        public int EmployeeTypeDropDownID { get; set; }
        public int EmployeeTypeID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> TypeStartDate { get; set; }
        public Nullable<System.DateTime> TypeEndDate { get; set; }
        //[ForeignKey("DesignationID")]
        public Nullable<decimal> DesignationID { get; set; }
        //[ForeignKey("DepartmentID")]
        public Nullable<decimal> DepartmentID { get; set; }
        public Nullable<bool> TimeRule { get; set; }
        public Nullable<bool> EarlyOut { get; set; }
        public Nullable<bool> LeaveRestrictions { get; set; }
        public Nullable<bool> MissingAttendance { get; set; }
        public  Nullable<decimal> ShiftID { get; set; }
        public Nullable<bool> LastArrival {  get; set; }
        public Nullable<System.DateTime> NICNoExpiryDate { get; set; }
        public string NICNo { get; set; }
        public string NationalSecurityNo { get; set; }
        public Nullable<int> NationalityDropDownID { get; set; }
        public Nullable<int> NationalityID { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<decimal> PayScheduleID { get; set; }
        public string EmployeePic { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string SubContractCompanyName { get; set; }
        public string PassportNumber { get; set; }
        public Nullable<System.DateTime> PassportExpiryDate { get; set; }
        public string SCHSNO { get; set; }
        public Nullable<System.DateTime> SCHSNOExpiryDate { get; set; }
        public string MedicalInsuranceProvided { get; set; }
        public string InsuranceCardNo { get; set; }
        public Nullable<System.DateTime> InsuranceCardNoExpiryDate { get; set; }
        public Nullable<int> InsuranceClassTypeDropdownID { get; set; }
        public Nullable<int> InsuranceClassTypeID { get; set; }
        public string AirTicketProvided { get; set; }
        public Nullable<int> NoOfAirTicket { get; set; }
        public Nullable<int> AirTicketClassTypeDropdownID { get; set; }
        public Nullable<int> AirTicketClassTypeID { get; set; }
        public Nullable<int> AirTicketFrequencyTypeDropdownID { get; set; }
        public Nullable<int> AirTicketFrequencyTypeID { get; set; }
        public Nullable<int> OriginCountryDropdownTypeID { get; set; }
        public Nullable<int> OriginCountryTypeID { get; set; }
        public Nullable<int> DestinationCountryDropdownTypeID { get; set; }
        public Nullable<int> DestinationCountryTypeID { get; set; }
        public Nullable<int> OriginCityDropdownTypeID { get; set; }
        public Nullable<int> OriginCityTypeID { get; set; }
        public Nullable<int> DestinationCityDropdownTypeID { get; set; }
        public Nullable<int> DestinationCityTypeID { get; set; }
        public string AirTicketRemarks { get; set; }
        public Nullable<int> MaritalStatusTypeDropdownID { get; set; }
        public Nullable<int> MaritalStatusTypeID { get; set; }
        public Nullable<int> ContractTypeDropDownID { get; set; }
        public Nullable<int> ContractTypeID { get; set; }
        public Nullable<int> TotalPolicyAmountMonthly { get; set; }
        public bool? ExculdeFromPayroll {  get; set; }
        public Nullable<System.DateTime> EffectiveDate {  get; set; }
       // public virtual ICollection<adm_user_mf> adm_user_mf { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual ICollection<adm_user_company> adm_user_company { get; set; }
        public virtual pr_department pr_department { get; set; }
        public virtual pr_designation pr_designation { get; set; }
        public virtual ICollection<pr_employee_allowance> pr_employee_allowance { get; set; }
        public virtual ICollection<pr_employee_ded_contribution> pr_employee_ded_contribution { get; set; }
        public virtual ICollection<pr_employee_document> pr_employee_document { get; set; }
        public virtual ICollection<pr_employee_Dependent> pr_employee_Dependent { get; set; }
        public virtual ICollection<pr_employee_leave> pr_employee_leave { get; set; }
        public virtual pr_pay_schedule pr_pay_schedule { get; set; }
        //public virtual sys_drop_down_value CityList { get; set; }
        //public virtual sys_drop_down_value CountryList { get; set; }
        //public virtual sys_drop_down_value PayTypeList { get; set; }
        public virtual sys_drop_down_value StatusList { get; set; }
        //public virtual sys_drop_down_value PaymentMethodList { get; set; }
        //public virtual sys_drop_down_value EmployeeTypeList { get; set; }
        //public virtual sys_drop_down_value NationalityList { get; set; }
        //public virtual sys_drop_down_value InsuranceClassList { get; set; }
        //public virtual sys_drop_down_value AirTicketFrequencyList { get; set; }
        //public virtual sys_drop_down_value OriginCountryList { get; set; }
        //public virtual sys_drop_down_value DestinationCountryList { get; set; }
        //public virtual sys_drop_down_value OriginCityList { get; set; }
        //public virtual sys_drop_down_value DestinationCityList { get; set; }
        //public virtual sys_drop_down_value AirTicketClassTypeList { get; set; }
        //public virtual sys_drop_down_value MaritalStatusList { get; set; }
        //public virtual sys_drop_down_value ContractTypeList { get; set; }
        //public virtual sys_drop_down_value SpecialtyList { get; set; }
        //public virtual sys_drop_down_value ClassificationList { get; set; }
        public virtual ICollection<pr_employee_payroll_dt> pr_employee_payroll_dt { get; set; }
        public virtual ICollection<pr_employee_payroll_mf> pr_employee_payroll_mf { get; set; }
        public virtual ICollection<pr_leave_application> pr_leave_application { get; set; }
        public virtual ICollection<pr_time_entry> pr_time_entry { get; set; }
        public virtual ICollection<pr_loan> pr_loan { get; set; }
    }
}
