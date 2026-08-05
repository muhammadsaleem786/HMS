using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class adm_user_share : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal DoctorId { get; set; }
        public Nullable<decimal> ServiceId { get; set; }
        public Nullable<decimal> LabId { get; set; }
        public Nullable<decimal> ItemId { get; set; }
        public decimal DoctorShare { get; set; }
        public string OwnerShare { get; set; }
        public string Category { get; set; }
        public string SharedMethod { get; set; }

        [NotMapped]
        public decimal DisDoctorShare { get; set; }
        [NotMapped]
        public decimal DisOwnerShare { get; set; }
        [NotMapped]
        public bool DeductOTCharges { get; set; }
        [NotMapped]
        public bool DeductMedicineCost { get; set; }
        [NotMapped]
        public bool DeductConsumables { get; set; }
        [NotMapped]
        public bool DeductImplantCost { get; set; }
        [NotMapped]
        public bool DeductLabConsumables { get; set; }
        [NotMapped]
        public bool DeductRoomCharges { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual adm_user_mf adm_user_mf2 { get; set; }
        public virtual emr_service_mf emr_service_mf { get; set; }
        public virtual emr_investigation emr_investigation { get; set; }
        public virtual adm_item adm_item { get; set; }
    }
}
