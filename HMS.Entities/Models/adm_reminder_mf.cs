using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class adm_reminder_mf : Entity
    {
        public adm_reminder_mf()
        {
            this.adm_reminder_dt = new List<adm_reminder_dt>();
            this.emr_patient_mf = new List<emr_patient_mf>();

        }
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public string Name { get; set; }
        public bool IsEnglish { get; set; }
        public bool IsUrdu { get; set; }
        public string MessageBody { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual ICollection<adm_reminder_dt> adm_reminder_dt { get; set; }
        public virtual ICollection<emr_patient_mf> emr_patient_mf { get; set; }

    }
}
