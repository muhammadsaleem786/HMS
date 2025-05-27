using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pur_vendor : Entity
    {
        public pur_vendor()
        {
            this.pur_invoice_mf = new List<pur_invoice_mf>();
        }
       
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string VendorPhone { get; set; }
        public string VendorEmail { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }       
        public string Phone { get; set; }
        public string Fax { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual ICollection<pur_invoice_mf> pur_invoice_mf { get; set; }

    }
}
