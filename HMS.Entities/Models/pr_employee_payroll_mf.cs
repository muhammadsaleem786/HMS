using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_employee_payroll_mf : Entity
    {
        public pr_employee_payroll_mf()
        {
            this.pr_employee_payroll_dt = new List<pr_employee_payroll_dt>();
        }

        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal PayScheduleID { get; set; }
        public System.DateTime PayDate { get; set; }
        public decimal EmployeeID { get; set; }
        public System.DateTime PayScheduleFromDate { get; set; }
        public System.DateTime PayScheduleToDate { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public Nullable<decimal> BasicSalary { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> AdjustmentDate { get; set; }
        public string AdjustmentType { get; set; }
        public Nullable<decimal> AdjustmentAmount { get; set; }
        public string AdjustmentComments { get; set; }
        public Nullable<decimal> AdjustmentBy { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual pr_employee_mf pr_employee_mf { get; set; }
        public virtual ICollection<pr_employee_payroll_dt> pr_employee_payroll_dt { get; set; }
        public virtual pr_pay_schedule pr_pay_schedule { get; set; }
    }
}
