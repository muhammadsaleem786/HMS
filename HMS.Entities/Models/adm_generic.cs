using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{

    public partial class adm_generic : Entity
    {
        public adm_generic()
        {
            this.adm_item = new List<adm_item>();
        }
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public string GenericName { get; set; }
        public string CommonUse { get; set; }
        public int SpecialtyId { get; set; }
        public int SpecialtyDropDownID { get; set; }
        public int CategoryID { get; set; }
        public int CategoryDropDownID { get; set; }
        public bool IsActive { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
        public virtual ICollection<adm_item> adm_item { get; set; }
    }
}
