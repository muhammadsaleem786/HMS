using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{

    public partial class pr_attendance : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public string IPAddress { get; set; }
        public int PortNo { get; set; }
        public string LastDataSync { get; set; }
        public string Password { get; set; }
        public string LocationCode { get; set; }
        public string DeviceSerialNo { get; set; }
        public bool IsActive { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
    }
}
