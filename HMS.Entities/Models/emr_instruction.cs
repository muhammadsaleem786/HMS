using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class emr_instruction : Entity
    {
        public emr_instruction()
        {
            this.emr_medicine = new List<emr_medicine>();
            this.adm_item = new List<adm_item>();
        }
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string Instructions { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual ICollection<emr_medicine> emr_medicine { get; set; }
        public virtual ICollection<adm_item> adm_item { get; set; }

    }
}
