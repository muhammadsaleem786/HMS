using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class ipd_specialized_equipment_maintenanceMap : EntityTypeConfiguration<ipd_specialized_equipment_maintenance>
    {
        public ipd_specialized_equipment_maintenanceMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("ipd_specialized_equipment_maintenance");

            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.EquipmentName).HasColumnName("EquipmentName");
            this.Property(t => t.Department).HasColumnName("Department");
            this.Property(t => t.PurchaseDate).HasColumnName("PurchaseDate");
            this.Property(t => t.SupplierNameAndAddress).HasColumnName("SupplierNameAndAddress");
            this.Property(t => t.InstallationDate).HasColumnName("InstallationDate");
            this.Property(t => t.WarrantyPeriod).HasColumnName("WarrantyPeriod");
            this.Property(t => t.CondemnationDate).HasColumnName("CondemnationDate");
            this.Property(t => t.ScheduledMaintenance).HasColumnName("ScheduledMaintenance");
            this.Property(t => t.MaintenanceDate).HasColumnName("MaintenanceDate");
            this.Property(t => t.CalibrationDate).HasColumnName("CalibrationDate");
            this.Property(t => t.ServiceRepairDate).HasColumnName("ServiceRepairDate");
            this.Property(t => t.Remarks).HasColumnName("Remarks");
            this.Property(t => t.Attachment).HasColumnName("Attachment");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.ipd_specialized_equipment_maintenance)
                .HasForeignKey(d => d.CompanyID);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.ipd_specialized_equipment_maintenance)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.ipd_specialized_equipment_maintenance1)
                .HasForeignKey(d => d.ModifiedBy);
        }
    }
}
