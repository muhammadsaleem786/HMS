using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class ipd_admission_lab:Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal AdmissionId { get; set; }
        public Nullable<decimal> AppointmentId { get; set; }
        public decimal PatientId { get; set; }
        public int LabTypeId { get; set; }
        public Nullable<int> LabTypeDropdownId { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> CollectDate { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public Nullable<System.DateTime> ReportDate { get; set; }
        public string OrderingPhysician { get; set; }
        public string Parameter { get; set; }
        public string ResultValues { get; set; }
        public string ABN { get; set; }
        public string Flags { get; set; }
        public string Comment { get; set; }
        public string TestPerformedAt { get; set; }
        public string TestDescription { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int StatusId { get; set; }
        public int StatusDropdownId { get; set; }
        public int ResultId { get; set; }
        public int ResultDropdownId { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual ipd_admission ipd_admission { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value2 { get; set; }
    }
}
