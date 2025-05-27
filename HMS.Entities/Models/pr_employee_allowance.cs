using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_employee_allowance : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal EmployeeID { get; set; }
        public System.DateTime EffectiveFrom { get; set; }
        public Nullable<System.DateTime> EffectiveTo { get; set; }
        public decimal PayScheduleID { get; set; }
        public decimal AllowanceID { get; set; }
        public double Percentage { get; set; }
        public double Amount { get; set; }
        public bool Taxable { get; set; }
        public Nullable<bool> IsHouseOrTransAllow { get; set; }
        public virtual pr_allowance pr_allowance { get; set; }
        public virtual pr_employee_mf pr_employee_mf { get; set; }
        public virtual pr_pay_schedule pr_pay_schedule { get; set; }
    }
}
