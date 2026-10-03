using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class sale_invoice_mf : Entity
    {
        public sale_invoice_mf()
        {
            this.sale_invoice_dt = new List<sale_invoice_dt>();
            this.sale_payment = new List<sale_payment>();
        }
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public decimal CustomerID { get; set; }
        public string InvoiceNo { get; set; }
        public string OrderNo { get; set; }
        public System.DateTime InvoiceDate { get; set; }
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
        public virtual ICollection<sale_invoice_dt> sale_invoice_dt { get; set; }
        public virtual ICollection<sale_payment> sale_payment { get; set; }
        public virtual sale_customer sale_customer { get; set; }
    }
}
