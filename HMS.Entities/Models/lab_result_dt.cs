using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class lab_result_dt : Entity
    {
       
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal ResultId { get; set; }
        public decimal ParameterId { get; set; }
        public decimal Result { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }       
        public decimal CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual lab_result_mf lab_result_mf { get; set; }
        public virtual lab_parameter lab_parameter { get; set; }

    }
}
