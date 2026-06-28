using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class lab_patient_test : Entity
    {
        public lab_patient_test()
        {
            this.lab_result_mf = new List<lab_result_mf>();
        }
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal PatientId { get; set; }
        public int Age { get; set; }
        public int PriorityId { get; set; }
        public int PriorityDropdownId { get; set; }
        public int StatusId { get; set; }
        public int StatusDropdownId { get; set; }
        public string ClinicalNotes { get; set; }
        public string SerialNo { get; set; }
        public decimal CatalogId { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<DateTime> SampleDate { get; set; }
        public Nullable<DateTime> CompletedDate { get; set; }
        public Nullable<DateTime> InProgressDate { get; set; }
        public string ReferringPhysician { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public string DiscountType { get; set; }
        public decimal Total { get; set; }
        [NotMapped]
        public string CatalogIds { get; set; }
        public decimal CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
        public virtual emr_patient_mf emr_patient_mf { get; set; }
        public virtual emr_investigation emr_investigation { get; set; }
        public virtual ICollection<lab_result_mf> lab_result_mf { get; set; }

    }
}
