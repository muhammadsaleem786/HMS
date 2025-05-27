using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class ipd_procedure_expense : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public Nullable<decimal> ProcedureId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int CategoryDropdownId { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual ipd_procedure_mf ipd_procedure_mf { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }

    }
}
