using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class lab_order : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }       
        public decimal PatientId { get; set; }
        public int SamplyTypeId { get; set; }
        public int SampleTypeDropdownId { get; set; }
        public DateTime CollectionDate { get; set; }
        public int DepartmentId { get; set; }
        public int DepartmentDropdownId { get; set; }
        public int PriorityLevelId { get; set; }
        public int PriorityLevelDropdownId { get; set; }
        public string OrderedBy { get; set; }
        public string ClinicalNotes { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }  

    }
}
