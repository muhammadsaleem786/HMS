using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class adm_user_share : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal DoctorId { get; set; }
        public decimal GeneralDoctorShare { get; set; }
        public decimal GeneralOwnerShare { get; set; }

        public decimal ProcedureDoctorShare { get; set; }
        public decimal ProcedureOwnerShare { get; set; }

        public decimal DiscountDoctorShare { get; set; }
        public decimal DiscountOwnerShare { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }  
    }
}
