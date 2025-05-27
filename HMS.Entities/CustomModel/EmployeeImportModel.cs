using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class EmployeeImportModel
    {
        public string ErrorDescription { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string CNIC { get; set; }
        public string EmployeeAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyPhone { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Location { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime JoiningDate { get; set; }
        public decimal Salary { get; set; }
        public string SalaryPaymentMethod { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string AccountTitle { get; set; }
        public string AccountNumber { get; set; }
    }
}
