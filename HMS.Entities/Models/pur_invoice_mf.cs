using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pur_invoice_mf : Entity
    {
        public pur_invoice_mf()
        {
            this.pur_invoice_dt = new List<pur_invoice_dt>();
            this.pur_payment = new List<pur_payment>();
        }
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal VendorID { get; set; }
        public string BillNo { get; set; }
        public string OrderNo { get; set; }
        public System.DateTime BillDate { get; set; }
        public System.DateTime DueDate { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public bool IsItemLevelDiscount { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int SaveStatus { get; set; }
        [NotMapped]
        public string action { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual ICollection<pur_invoice_dt> pur_invoice_dt { get; set; }
        public virtual ICollection<pur_payment> pur_payment { get; set; }
        public virtual pur_vendor pur_vendor { get; set; }
    }
}
