using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.CustomModel
{
    public class PayrollDetailPaginationSearchSortModel
    {
        public decimal UniqueID { get; set; }
        public string EmployeeName { get; set; }
        public string ScheduleName { get; set; }
        public string Status { get; set; }
        public string PayPeriod { get; set; }
        public DateTime PayDate { get; set; }
        public decimal? BaseSalary { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? AllowanceTaxable { get; set; }
        public decimal? Deduction { get; set; }
        public decimal? DeductionTaxable { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal Tax { get; set; }
        public decimal NetSalary { get; set; }
    }
}
