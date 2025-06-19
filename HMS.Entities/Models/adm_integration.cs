using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{

    public partial class adm_integration : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public string UserName { get; set; }
        public string EmailFrom { get; set; }
        public string Password { get; set; }
        public string Masking { get; set; }
        public string SMTP { get; set; }
        public Nullable<int> PortNo { get; set; }
        public bool IsActive { get; set; }
        public string Type {  get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
    }
}
