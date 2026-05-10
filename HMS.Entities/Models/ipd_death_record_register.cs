using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public class ipd_death_record_register : Entity
    {
        public int ID { get; set; }
        public decimal CompanyID { get; set; }
        public DateTime DeathDate { get; set; }

        public string MRNo { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }

        public string FatherHusbandName { get; set; }

        public int? Age { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string IDCardNo { get; set; }

        public string ContractNo { get; set; }

        public DateTime DateTimeOfDeath { get; set; }

        public string ProbableCauseOfDeath { get; set; }

        public string DeathCertificateHandedOverTo { get; set; }

        public bool AttendantSignature { get; set; }

        public bool DoctorSignature { get; set; }

        public string Attachment { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
    }
}
