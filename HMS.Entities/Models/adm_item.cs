using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models
{

    public partial class adm_item : Entity
    {
        public adm_item()
        {
            this.pur_invoice_dt = new List<pur_invoice_dt>();
            this.pur_sale_dt = new List<pur_sale_dt>();
            this.pur_sale_hold_dt = new List<pur_sale_hold_dt>();
            this.inv_stock = new List<inv_stock>();
            this.ipd_medication_log = new List<ipd_medication_log>();
            this.emr_service_item = new List<emr_service_item>();
            this.ipd_procedure_charged = new List<ipd_procedure_charged>();
            this.ipd_admission_medication = new List<ipd_admission_medication>();
        }
        public decimal ID { get; set; }
        public decimal CompanyId { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Image { get; set; }
        public int UnitDropDownID { get; set; }
        public Nullable<int> UnitID { get; set; }
        public int ItemTypeDropDownID { get; set; }
        public Nullable<int> ItemTypeId { get; set; }
        public int CategoryDropDownID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> GroupId { get; set; }
        public Nullable<decimal> InstructionId {  get; set; }
        public int GroupDropDownId { get; set; }
        public bool TrackInventory { get; set; }
        public bool IsActive { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public Nullable<decimal> InventoryOpeningStock { get; set; }
        public Nullable<decimal> InventoryStockPerUnit { get; set; }
        public Nullable<decimal> InventoryStockQuantity { get; set; }
        public int SaveStatus { get; set; }
        public Boolean POSItem { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public virtual adm_company adm_company { get; set; }
        public virtual adm_user_mf adm_user_mf { get; set; }
        public virtual adm_user_mf adm_user_mf1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value2 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value3 { get; set; }
        public virtual ICollection<pur_invoice_dt> pur_invoice_dt { get; set; }
        public virtual ICollection<pur_sale_dt> pur_sale_dt { get; set; }
        public virtual ICollection<pur_sale_hold_dt> pur_sale_hold_dt { get; set; }
        public virtual ICollection<inv_stock> inv_stock { get; set; }
        public virtual ICollection<ipd_medication_log> ipd_medication_log { get; set; }
        public virtual ICollection<emr_service_item> emr_service_item { get; set; }
        public virtual ICollection<ipd_procedure_charged> ipd_procedure_charged { get; set; }
        public virtual ICollection<ipd_admission_medication> ipd_admission_medication { get; set; }
        public virtual emr_instruction emr_instruction { get; set; }
    }
}
