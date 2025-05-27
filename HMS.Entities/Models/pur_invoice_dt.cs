using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pur_invoice_dt : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal InvoiceID { get; set; }
        public decimal ItemID { get; set; }
        public string ItemDescription { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }   
        public virtual adm_company adm_company { get; set; }
        public virtual adm_item adm_item { get; set; }
        public virtual pur_invoice_mf pur_invoice_mf { get; set; }
        public int? BatchSarialNumber { get; set; }
        public Nullable<System.DateTime> ExpiredWarrantyDate { get; set; }
    }
}
