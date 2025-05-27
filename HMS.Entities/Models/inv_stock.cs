using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class inv_stock : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal ItemID { get; set; }
        public decimal Quantity { get; set; }
        public Nullable<int> BatchSarialNumber { get; set; }
        public Nullable<DateTime> ExpiredWarrantyDate { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
      
        public virtual adm_item adm_item { get; set; }
    }
}
