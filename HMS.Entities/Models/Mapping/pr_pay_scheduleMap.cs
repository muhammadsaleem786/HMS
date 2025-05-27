using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_pay_scheduleMap : EntityTypeConfiguration<pr_pay_schedule>
    {
        public pr_pay_scheduleMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });
            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.ScheduleName)
                .IsRequired()
                .HasMaxLength(250);
            // Table & Column Mappings
            this.ToTable("pr_pay_schedule");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.PayTypeDropDownID).HasColumnName("PayTypeDropDownID");
            this.Property(t => t.PayTypeID).HasColumnName("PayTypeID");
            this.Property(t => t.ScheduleName).HasColumnName("ScheduleName");
            this.Property(t => t.PeriodStartDate).HasColumnName("PeriodStartDate");
            this.Property(t => t.PeriodEndDate).HasColumnName("PeriodEndDate");
            this.Property(t => t.FallInHolidayDropDownID).HasColumnName("FallInHolidayDropDownID");
            this.Property(t => t.FallInHolidayID).HasColumnName("FallInHolidayID");
            this.Property(t => t.PayDate).HasColumnName("PayDate");
            this.Property(t => t.Lock).HasColumnName("Lock");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pr_pay_schedule)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.pr_pay_schedule)
                .HasForeignKey(d => new { d.PayTypeID, d.PayTypeDropDownID });
            this.HasRequired(t => t.sys_drop_down_value1)
                .WithMany(t => t.pr_pay_schedule1)
                .HasForeignKey(d => new { d.FallInHolidayID, d.FallInHolidayDropDownID });
        }
    }
}
