using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_leave_type : Entity
    {
        public pr_leave_type()
        {
            this.pr_employee_leave = new List<pr_employee_leave>();
            this.pr_leave_application = new List<pr_leave_application>();
        }

        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string Category { get; set; }
        public string TypeName { get; set; }
        public int AccuralDropDownID { get; set; }
        public int AccrualFrequencyID { get; set; }
        public double EarnedValue { get; set; }
        public bool SystemGenerated { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual ICollection<pr_employee_leave> pr_employee_leave { get; set; }
        public virtual ICollection<pr_leave_application> pr_leave_application { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
    }
}
