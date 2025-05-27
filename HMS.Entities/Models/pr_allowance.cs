using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_allowance : Entity
    {
        public pr_allowance()
        {
            this.pr_employee_allowance = new List<pr_employee_allowance>();
        }

        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public int CategoryDropDownID { get; set; }
        public int CategoryID { get; set; }
        public string AllowanceName { get; set; }
        public string AllowanceType { get; set; }
        public double AllowanceValue { get; set; }
        public bool Taxable { get; set; }
        public bool Default { get; set; }
        public bool Formula { get; set; }
        public bool SystemGenerated { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual ICollection<pr_employee_allowance> pr_employee_allowance { get; set; }
    }
}
