using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public partial class emr_complaint : Entity
    {
        public decimal ID { get; set; }
        public decimal CompanyID { get; set; }
        public string Complaint { get; set; }
        [NotMapped]
        public Nullable<decimal> favoriteId { get; set; }
        [NotMapped]
        public bool Isfavorite { get; set; }
        public decimal CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
    }
}
