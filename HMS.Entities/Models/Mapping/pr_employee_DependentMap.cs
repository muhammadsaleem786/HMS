using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Entities.Models.Mapping
{
    public class pr_employee_DependentMap : EntityTypeConfiguration<pr_employee_Dependent>
    {
        public pr_employee_DependentMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.CompanyID, t.EmployeeID });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CompanyID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.EmployeeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.FirstName)
                .HasMaxLength(250);

            this.Property(t => t.LastName)
                .HasMaxLength(250);

            this.Property(t => t.Gender)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.IdentificationNumber)
                .HasMaxLength(150);

            this.Property(t => t.PassportNumber)
                .HasMaxLength(150);

            this.Property(t => t.Remarks)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("pr_employee_Dependent");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CompanyID).HasColumnName("CompanyID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.RelationshipDropdownID).HasColumnName("RelationshipDropdownID");
            this.Property(t => t.RelationshipTypeID).HasColumnName("RelationshipTypeID");
            this.Property(t => t.IsEmergencyContact).HasColumnName("IsEmergencyContact");
            this.Property(t => t.IsTicketEligible).HasColumnName("IsTicketEligible");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.NationalityTypeDropdownID).HasColumnName("NationalityTypeDropdownID");
            this.Property(t => t.NationalityTypeID).HasColumnName("NationalityTypeID");
            this.Property(t => t.IdentificationNumber).HasColumnName("IdentificationNumber");
            this.Property(t => t.PassportNumber).HasColumnName("PassportNumber");
            this.Property(t => t.MaritalStatusTypeDropdownID).HasColumnName("MaritalStatusTypeDropdownID");
            this.Property(t => t.MaritalStatusTypeID).HasColumnName("MaritalStatusTypeID");
            this.Property(t => t.DOB).HasColumnName("DOB");
            this.Property(t => t.Remarks).HasColumnName("Remarks");

            // Relationships
            this.HasRequired(t => t.pr_employee_mf)
                .WithMany(t => t.pr_employee_Dependent)
                .HasForeignKey(d => new { d.EmployeeID, d.CompanyID });
            this.HasRequired(t => t.sys_drop_down_value)
                .WithMany(t => t.pr_employee_Dependent)
                .HasForeignKey(d => new { d.NationalityTypeID, d.NationalityTypeDropdownID });
            this.HasRequired(t => t.sys_drop_down_value1)
                .WithMany(t => t.pr_employee_Dependent1)
                .HasForeignKey(d => new { d.MaritalStatusTypeID, d.MaritalStatusTypeDropdownID });
            this.HasRequired(t => t.sys_drop_down_value2)
                .WithMany(t => t.pr_employee_Dependent2)
                .HasForeignKey(d => new { d.RelationshipTypeID, d.RelationshipDropdownID });

        }
    }
}
