using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{

    public partial class user_payment : Entity
    {
        public decimal ID { get; set; }
        public decimal UserId { get; set; }
        public decimal CompanyId { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public DateTime Date { get; set; }
        public decimal CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual adm_user_mf adm_user_mf2 { get; set; }
    }
}
