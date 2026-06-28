using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
namespace HMS.Entities.Models
{
    public partial class lab_parameter : Entity
    {
        public lab_parameter()
        {
            this.lab_result_dt = new List<lab_result_dt>();
        }
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public decimal CatalogId { get; set; }
        public string ParameterName { get; set; }
        public int UnitId { get; set; }
        public int UnitDropdownId { get; set; }
        public string ReferenceRange { get; set; }
        public string NormalMinimum { get; set; }
        public string NormalMaximum { get; set; }
        public decimal CriticalLow { get; set; }
        public decimal CriticalHigh { get; set; }
        public decimal CreatedBy { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual emr_investigation emr_investigation { get; set; }
        public virtual ICollection<lab_result_dt> lab_result_dt { get; set; }
    }
}
