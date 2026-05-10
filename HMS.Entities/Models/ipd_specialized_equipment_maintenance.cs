using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{
    public class ipd_specialized_equipment_maintenance : Entity
    {
        public int ID { get; set; }
        public decimal CompanyID { get; set; }

        public string EquipmentName { get; set; }

        public string Department { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string SupplierNameAndAddress { get; set; }

        public DateTime? InstallationDate { get; set; }

        public DateTime WarrantyPeriod { get; set; }

        public DateTime? CondemnationDate { get; set; }

        public bool? ScheduledMaintenance { get; set; }

        public DateTime? MaintenanceDate { get; set; }

        public DateTime? CalibrationDate { get; set; }

        public DateTime? ServiceRepairDate { get; set; }

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
