using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_pay_schedule : Entity
    {
        public pr_pay_schedule()
        {
            this.pr_employee_allowance = new List<pr_employee_allowance>();
            this.pr_employee_ded_contribution = new List<pr_employee_ded_contribution>();
            this.pr_employee_mf = new List<pr_employee_mf>();
            this.pr_employee_payroll_mf = new List<pr_employee_payroll_mf>();
        }
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public int PayTypeDropDownID { get; set; }
        public int PayTypeID { get; set; }
        public string ScheduleName { get; set; }
        public System.DateTime PeriodStartDate { get; set; }
        public System.DateTime PeriodEndDate { get; set; }
        public int FallInHolidayDropDownID { get; set; }
        public int FallInHolidayID { get; set; }
        public System.DateTime PayDate { get; set; }
        public bool Lock { get; set; }
        public bool Active { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual ICollection<pr_employee_allowance> pr_employee_allowance { get; set; }
        public virtual ICollection<pr_employee_ded_contribution> pr_employee_ded_contribution { get; set; }
        public virtual ICollection<pr_employee_mf> pr_employee_mf { get; set; }
        public virtual ICollection<pr_employee_payroll_mf> pr_employee_payroll_mf { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
    }
}
