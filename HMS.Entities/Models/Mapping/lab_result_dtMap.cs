using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{

    public class lab_result_dtMap : EntityTypeConfiguration<lab_result_dt>
    {
        public lab_result_dtMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyId });
            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            this.Property(t => t.CompanyId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            
            // Table & Column Mappings
            this.ToTable("lab_result_dt");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
            this.Property(t => t.ResultId).HasColumnName("ResultId");
            this.Property(t => t.ParameterId).HasColumnName("ParameterId");
            this.Property(t => t.Result).HasColumnName("Result");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.adm_company)
                .WithMany(t => t.lab_result_dt)
                .HasForeignKey(d => d.CompanyId);

            this.HasRequired(t => t.adm_user_mf)
                .WithMany(t => t.lab_result_dt)
                .HasForeignKey(d => d.CreatedBy);

            this.HasOptional(t => t.adm_user_mf1)
                .WithMany(t => t.lab_result_dt1)
                .HasForeignKey(d => d.ModifiedBy);

            this.HasRequired(t => t.lab_result_mf)
               .WithMany(t => t.lab_result_dt)
               .HasForeignKey(d => new { d.ResultId, d.CompanyId });

            this.HasRequired(t => t.lab_parameter)
            .WithMany(t => t.lab_result_dt)
            .HasForeignKey(d => new { d.ParameterId, d.CompanyId });


        }
    }
}
