using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class DashboardPrEmployeeModel
    {
        public decimal EmployeeID { get; set; }
        public double TotalVacationHours { get; set; }
        public double TotalSickHours { get; set; }
        public double VacationLeaveHours { get; set; }
        public double SickLeaveHours { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Deduction { get; set; }
        public decimal Contribution { get; set; }
        public decimal Tax { get; set; }
        public decimal NetSalary { get; set; }
        public decimal EmployeeProvidentFund { get; set; }
        public decimal EmployerProvidentFund { get; set; }
        public decimal EmployeeEOBI { get; set; }
        public decimal EmployerEOBI { get; set; }
        public decimal EmployeeGOSI { get; set; }
        public decimal EmployerGOSI { get; set; }
        public double GivenLoan { get; set; }
        public double RecoveredLoan { get; set; }
    }
}
