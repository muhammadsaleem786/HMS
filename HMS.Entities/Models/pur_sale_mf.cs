using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pur_sale_mf : Entity
    {
        public pur_sale_mf()
        {
            this.pur_sale_dt = new List<pur_sale_dt>();
        }
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal CustomerId { get; set; }
        public decimal? ReturnInvoiceId { get; set; }
        public int SaleTypeDropDownId { get; set; }
        public int SaleTypeID { get; set; }
        public DateTime Date { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        [NotMapped]
        public decimal SaleHoldId { get; set; }
        public int DiscountType { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public Nullable<decimal> Total { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual emr_patient_mf emr_patient_mf { get; set; }
        public virtual ICollection<pur_sale_dt> pur_sale_dt { get; set; }
    }
}
