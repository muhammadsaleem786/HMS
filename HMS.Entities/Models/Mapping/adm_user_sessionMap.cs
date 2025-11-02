using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{

    public class adm_user_sessionMap : EntityTypeConfiguration<adm_user_session>
    {
        public adm_user_sessionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("adm_user_session");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.ShiftStartTime).HasColumnName("ShiftStartTime");
            this.Property(t => t.ShiftEndTime).HasColumnName("ShiftEndTime");
            this.Property(t => t.StartingCash).HasColumnName("StartingCash");
            this.Property(t => t.TransactionsCount).HasColumnName("TransactionsCount");
            this.Property(t => t.TotalCollection).HasColumnName("TotalCollection");
            this.Property(t => t.Refunds).HasColumnName("Refunds");
            this.Property(t => t.ClosingBalance).HasColumnName("ClosingBalance");
            this.Property(t => t.OpeningComments).HasColumnName("OpeningComments");
            this.Property(t => t.ClosingComments).HasColumnName("ClosingComments");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            this.Property(t => t.UserId).HasColumnName("UserId");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.adm_user_session)
                .HasForeignKey(d => d.CompanyId);
            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.adm_user_session)
                .HasForeignKey(d => d.CreatedBy);
            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.adm_user_session1)
                .HasForeignKey(d => d.ModifiedBy);           
        }
    }
}
