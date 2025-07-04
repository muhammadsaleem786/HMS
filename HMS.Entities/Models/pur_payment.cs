﻿using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class pur_payment : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal InvoiveId { get; set; }
        public Nullable<int> PaymentMethodDropdownID { get; set; }
        public Nullable<int> PaymentMethodID { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public string Notes { get; set; }       
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual pur_invoice_mf pur_invoice_mf { get; set; }

    }
}
