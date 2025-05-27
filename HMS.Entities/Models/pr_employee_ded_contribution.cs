using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_employee_ded_contribution : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal EmployeeID { get; set; }
        public System.DateTime EffectiveFrom { get; set; }
        public Nullable<System.DateTime> EffectiveTo { get; set; }
        public decimal PayScheduleID { get; set; }
        public string Category { get; set; }
        public decimal DeductionContributionID { get; set; }
        public double Percentage { get; set; }
        public double Amount { get; set; }
        public double StartingBalance { get; set; }
        public Nullable<bool> Taxable { get; set; }
        public virtual pr_deduction_contribution pr_deduction_contribution { get; set; }
        public virtual pr_employee_mf pr_employee_mf { get; set; }
        public virtual pr_pay_schedule pr_pay_schedule { get; set; }
    }
}
