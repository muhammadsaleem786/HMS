using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_loanMap : EntityTypeConfiguration<pr_loan>
    {
        public pr_loanMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Description)
                .HasMaxLength(500);

            this.Property(t => t.DeductionType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.AdjustmentType)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.AdjustmentComments)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("pr_loan");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.PaymentMethodDropdownID).HasColumnName("PaymentMethodDropdownID");
            this.Property(t => t.PaymentMethodID).HasColumnName("PaymentMethodID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.PaymentStartDate).HasColumnName("PaymentStartDate");
            this.Property(t => t.LoanDate).HasColumnName("LoanDate");
            this.Property(t => t.LoanAmount).HasColumnName("LoanAmount");
            this.Property(t => t.DeductionType).HasColumnName("DeductionType");
            this.Property(t => t.DeductionValue).HasColumnName("DeductionValue");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.AdjustmentDate).HasColumnName("AdjustmentDate");
            this.Property(t => t.AdjustmentType).HasColumnName("AdjustmentType");
            this.Property(t => t.AdjustmentAmount).HasColumnName("AdjustmentAmount");
            this.Property(t => t.AdjustmentComments).HasColumnName("AdjustmentComments");
            this.Property(t => t.AdjustmentBy).HasColumnName("AdjustmentBy");
            this.Property(t => t.InstallmentByBaseSalary).HasColumnName("InstallmentByBaseSalary");
            this.Property(t => t.LoanTypeID).HasColumnName("LoanTypeID");
            this.Property(t => t.LoanTypeDropdownID).HasColumnName("LoanTypeDropdownID");
            this.Property(t => t.LoanCode).HasColumnName("LoanCode");
            this.Property(t => t.ApprovalStatusID).HasColumnName("ApprovalStatusID");
            
            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pr_loan)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.pr_employee_mf)
                .WithMany(t => t.pr_loan)
                .HasForeignKey(d => new { d.EmployeeID, d.CompanyID });
            this.HasOptional(t => t.sys_drop_down_value)
                .WithMany(t => t.pr_loan)
                .HasForeignKey(d => new { d.PaymentMethodID, d.PaymentMethodDropdownID });

        }
    }
}
