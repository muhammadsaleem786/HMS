using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace HMS.Entities.Models.Mapping
{
    public class pr_loan_payment_dtMap : EntityTypeConfiguration<pr_loan_payment_dt>
    {
        public pr_loan_payment_dtMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Comment)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.AdjustmentType)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.AdjustmentComments)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("pr_loan_payment_dt");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.LoanID).HasColumnName("LoanID");
            this.Property(t => t.PaymentDate).HasColumnName("PaymentDate");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.AdjustmentDate).HasColumnName("AdjustmentDate");
            this.Property(t => t.AdjustmentType).HasColumnName("AdjustmentType");
            this.Property(t => t.AdjustmentAmount).HasColumnName("AdjustmentAmount");
            this.Property(t => t.AdjustmentComments).HasColumnName("AdjustmentComments");
            this.Property(t => t.AdjustmentBy).HasColumnName("AdjustmentBy");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.pr_loan_payment_dt)
                .HasForeignKey(d => d.CompanyID);
            this.HasRequired(t => t.pr_loan)
                .WithMany(t => t.pr_loan_payment_dt)
                .HasForeignKey(d => new { d.LoanID, d.CompanyID });

        }
    }
}
