using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_employee_payroll_dt : Entity
    {
        public decimal ID { get; set; }
        public decimal PayrollID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal PayScheduleID { get; set; }
        public System.DateTime PayDate { get; set; }
        public decimal EmployeeID { get; set; }
        public string Type { get; set; }
        public int AllowDedID { get; set; }
        public decimal Amount { get; set; }
        public bool Taxable { get; set; }
        public Nullable<System.DateTime> AdjustmentDate { get; set; }
        public string AdjustmentType { get; set; }
        public Nullable<decimal> AdjustmentAmount { get; set; }
        public string AdjustmentComments { get; set; }
        public Nullable<decimal> AdjustmentBy { get; set; }
        public decimal RefID { get; set; }
        public string Remarks { get; set; }
        public Nullable<DateTime> ArrearReleatedMonth { get; set; }
        public virtual pr_employee_mf pr_employee_mf { get; set; }
        public virtual pr_employee_payroll_mf pr_employee_payroll_mf { get; set; }
    }
}
