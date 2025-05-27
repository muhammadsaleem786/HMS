using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class ipd_procedure_mf:Entity
    {
        public ipd_procedure_mf()
        {
            this.ipd_procedure_charged = new List<ipd_procedure_charged>();
            this.ipd_procedure_medication = new List<ipd_procedure_medication>();
            this.ipd_procedure_expense = new List<ipd_procedure_expense>();
        }

        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal AdmissionId { get; set; }
        public Nullable<decimal> AppointmentId { get; set; }
        public decimal PatientId { get; set; }
        public decimal ServiceId { get; set; }
        public string PatientProcedure { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<System.TimeSpan> Time { get; set; }
        public Nullable<int> CPTCodeId { get; set; }
        public Nullable<int> CPTCodeDropdownId { get; set; }
        public string Location { get; set; }
        public string Physician { get; set; }
        public string Assistant { get; set; }
        public string Notes { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual ipd_admission ipd_admission { get; set; }
        public virtual ICollection<ipd_procedure_charged> ipd_procedure_charged { get; set; }
        public virtual ICollection<ipd_procedure_medication> ipd_procedure_medication { get; set; }
        public virtual ICollection<ipd_procedure_expense> ipd_procedure_expense { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual emr_service_mf emr_service_mf { get; set; }
    }
}
