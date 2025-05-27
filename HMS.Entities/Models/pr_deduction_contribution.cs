using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pr_deduction_contribution : Entity
    {
        public pr_deduction_contribution()
        {
            this.pr_employee_ded_contribution = new List<pr_employee_ded_contribution>();
        }

        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string Category { get; set; }
        public string DeductionContributionName { get; set; }
        public string DeductionContributionType { get; set; }
        public double DeductionContributionValue { get; set; }
        public bool Default { get; set; }
        public bool Taxable { get; set; }
        public bool StartingBalance { get; set; }
        public bool SystemGenerated { get; set; }
        public bool Formula { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual ICollection<pr_employee_ded_contribution> pr_employee_ded_contribution { get; set; }
    }
}
