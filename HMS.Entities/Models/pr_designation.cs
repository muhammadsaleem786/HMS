using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_designation : Entity
    {
        public pr_designation()
        {
            this.pr_employee_mf = new List<pr_employee_mf>();
        }

        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string DesignationName { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual ICollection<pr_employee_mf> pr_employee_mf { get; set; }
    }
}
