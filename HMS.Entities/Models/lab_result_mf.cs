using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class lab_result_mf : Entity
    {
        public lab_result_mf()
        {
            this.lab_result_dt = new List<lab_result_dt>();
        }
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal PatientId { get; set; }
        public decimal CatalogId { get; set; }
        public decimal PatientTestId { get; set; }
        public string Comment { get; set; }
        public decimal CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual emr_investigation emr_investigation { get; set; }
        public virtual lab_patient_test lab_patient_test { get; set; }
        public virtual ICollection<lab_result_dt> lab_result_dt { get; set; }
        public virtual emr_patient_mf emr_patient_mf { get; set; }


    }
}
