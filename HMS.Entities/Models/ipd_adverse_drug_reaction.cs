using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public class ipd_adverse_drug_reaction : Entity
    {
        public int ID { get; set; }
        public int PatientId { get; set; }
        public decimal CompanyID { get; set; }
        public DateTime Date { get; set; }

        public string PatientName { get; set; }

        public int? Age { get; set; }
        public string Gender { get; set; }

        public string Contact { get; set; }

        public string MedicineGiven { get; set; }

        public string ReactionObserved { get; set; }

        public string ActionTaken { get; set; }

        public string Outcome { get; set; }

        public bool DoctorSignature { get; set; }

        public string Remarks { get; set; }

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
