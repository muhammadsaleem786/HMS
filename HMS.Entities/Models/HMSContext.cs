using System.Data.Entity;
using Repository.Pattern.Ef6;
using HMS.Entities.Models.Mapping;

namespace HMS.Entities.Models
{
    public partial class HMSContext : DataContext
    {
        static HMSContext()
        {
            Database.SetInitializer<HMSContext>(null);
        }
        public HMSContext() : base("Name=HMSContext")
        {
        }
        public DbSet<adm_company> adm_company { get; set; }
        public DbSet<adm_multilingual_dt> adm_multilingual_dt { get; set; }
        public DbSet<adm_multilingual_mf> adm_multilingual_mf { get; set; }
        public DbSet<adm_role_dt> adm_role_dt { get; set; }
        public DbSet<adm_role_mf> adm_role_mf { get; set; }
        public DbSet<adm_user_mf> adm_user_mf { get; set; }
        public DbSet<adm_user_company> adm_user_company { get; set; }
        public DbSet<emr_notes_favorite> emr_notes_favorite { get; set; }
        public DbSet<adm_user_token> adm_user_token { get; set; }
        public DbSet<sys_drop_down_mf> sys_drop_down_mf { get; set; }
        public DbSet<sys_drop_down_value> sys_drop_down_value { get; set; }
        public DbSet<emr_patient_mf> emr_patient_mf { get; set; }
        public DbSet<emr_appointment_mf> emr_appointment_mf { get; set; }
        public DbSet<emr_document> emr_document { get; set; }
        public DbSet<emr_prescription_complaint> emr_prescription_complaint { get; set; }
        public DbSet<emr_prescription_diagnos> emr_prescription_diagnos { get; set; }
        public DbSet<emr_prescription_investigation> emr_prescription_investigation { get; set; }
        public DbSet<emr_prescription_mf> emr_prescription_mf { get; set; }
        public DbSet<emr_prescription_observation> emr_prescription_observation { get; set; }
        public DbSet<emr_prescription_treatment> emr_prescription_treatment { get; set; }
        public DbSet<emr_prescription_treatment_template> emr_prescription_treatment_template { get; set; }
        public DbSet<emr_medicine> emr_medicine { get; set; }
        public DbSet<emr_expense> emr_expense { get; set; }
        public DbSet<emr_income> emr_income { get; set; }
        public DbSet<emr_complaint> emr_complaint { get; set; }
        public DbSet<emr_diagnos> emr_diagnos { get; set; }
        public DbSet<emr_instruction> emr_instruction { get; set; }
        public DbSet<emr_investigation> emr_investigation { get; set; }
        public DbSet<emr_observation> emr_observation { get; set; }
        public DbSet<emr_service_mf> emr_service_mf { get; set; }
        public DbSet<emr_service_item> emr_service_item { get; set; }
        public DbSet<emr_patient_bill> emr_patient_bill { get; set; }
        public DbSet<emr_patient_bill_payment> emr_patient_bill_payment { get; set; }
        public DbSet<emr_vital> emr_vital { get; set; }

        public DbSet<ipd_admission> ipd_admission { get; set; }
        public DbSet<ipd_admission_charges> ipd_admission_charges { get; set; }
        public DbSet<ipd_admission_imaging> ipd_admission_imaging { get; set; }
        public DbSet<ipd_admission_lab> ipd_admission_lab { get; set; }
        public DbSet<ipd_admission_medication> ipd_admission_medication { get; set; }
        public DbSet<ipd_admission_notes> ipd_admission_notes { get; set; }
        public DbSet<ipd_admission_vital> ipd_admission_vital { get; set; }
        public DbSet<ipd_procedure_charged> ipd_procedure_charged { get; set; }
        public DbSet<ipd_procedure_medication> ipd_procedure_medication { get; set; }
        public DbSet<ipd_procedure_mf> ipd_procedure_mf { get; set; }
        public DbSet<ipd_diagnosis> ipd_diagnosis { get; set; }
        public DbSet<contact> contact { get; set; }
        
        public DbSet<user_payment> user_payment { get; set; }
        public DbSet<ipd_admission_discharge> ipd_admission_discharge { get; set; }
        public DbSet<adm_item> adm_item { get; set; }
        public DbSet<pur_vendor> pur_vendor { get; set; }
        public DbSet<adm_item_log> adm_item_log { get; set; }
        public DbSet<ipd_procedure_expense> ipd_procedure_expense { get; set; }
        //employee

        public DbSet<pr_employee_mf> pr_employee_mf { get; set; }
        public DbSet<pr_allowance> pr_allowance { get; set; }
        public DbSet<pr_deduction_contribution> pr_deduction_contribution { get; set; }
        public DbSet<pr_department> pr_department { get; set; }
        public DbSet<pr_designation> pr_designation { get; set; }
        public DbSet<pr_employee_allowance> pr_employee_allowance { get; set; }
        public DbSet<pr_employee_ded_contribution> pr_employee_ded_contribution { get; set; }
        public DbSet<pr_employee_leave> pr_employee_leave { get; set; }
        public DbSet<pr_employee_payroll_mf> pr_employee_payroll_mf { get; set; }
        public DbSet<pr_employee_payroll_dt> pr_employee_payroll_dt { get; set; }
        public DbSet<pr_leave_application> pr_leave_application { get; set; }
        public DbSet<pr_leave_type> pr_leave_type { get; set; }
        public DbSet<pr_loan> pr_loan { get; set; }
        public DbSet<pr_loan_payment_dt> pr_loan_payment_dt { get; set; }     
        public DbSet<pr_pay_schedule> pr_pay_schedule { get; set; }
        public DbSet<pr_time_entry> pr_time_entry { get; set; }
        public DbSet<pr_employee_document> pr_employee_document { get; set; }
        public DbSet<pr_employee_Dependent> pr_employee_Dependent { get; set; }
        public DbSet<sys_holidays> sys_holidays { get; set; }
        public DbSet<pur_invoice_mf> pur_invoice_mf { get; set; }
        public DbSet<pur_invoice_dt> pur_invoice_dt { get; set; }
        public DbSet<inv_stock> inv_stock { get; set; }

        public DbSet<pur_sale_mf> pur_sale_mf { get; set; }
        public DbSet<pur_sale_dt> pur_sale_dt { get; set; }
        public DbSet<pur_sale_hold_mf> pur_sale_hold_mf { get; set; }
        public DbSet<pur_sale_hold_dt> pur_sale_hold_dt { get; set; }
        public DbSet<pur_payment> pur_payment { get; set; }
        public DbSet<ipd_input_output> ipd_input_output { get; set; }
        public DbSet<ipd_medication_log> ipd_medication_log { get; set; }
        public DbSet<adm_reminder_mf> adm_reminder_mf { get; set; }
        public DbSet<adm_reminder_dt> adm_reminder_dt { get; set; }
        public DbSet<adm_integration> adm_integration { get; set; }
        public DbSet<pr_attendance> pr_attendance { get; set; }
        public DbSet<pr_time_log> pr_time_log { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new contactMap());
            modelBuilder.Configurations.Add(new ipd_admission_dischargeMap());
            modelBuilder.Configurations.Add(new user_paymentMap());
            modelBuilder.Configurations.Add(new ipd_diagnosisMap());
            modelBuilder.Configurations.Add(new emr_notes_favoriteMap());
            modelBuilder.Configurations.Add(new emr_service_mfMap());
            modelBuilder.Configurations.Add(new emr_service_itemMap());
            modelBuilder.Configurations.Add(new emr_patient_billMap());
            modelBuilder.Configurations.Add(new emr_complaintMap());
            modelBuilder.Configurations.Add(new emr_diagnosMap());
            modelBuilder.Configurations.Add(new emr_instructionMap());
            modelBuilder.Configurations.Add(new emr_investigationMap());
            modelBuilder.Configurations.Add(new emr_observationMap());
            modelBuilder.Configurations.Add(new emr_incomeMap());
            modelBuilder.Configurations.Add(new emr_expenseMap());
            modelBuilder.Configurations.Add(new emr_appointment_mfMap());
            modelBuilder.Configurations.Add(new emr_patient_mfMap());
            modelBuilder.Configurations.Add(new adm_companyMap());
            modelBuilder.Configurations.Add(new adm_multilingual_dtMap());
            modelBuilder.Configurations.Add(new adm_multilingual_mfMap());
            modelBuilder.Configurations.Add(new adm_role_dtMap());
            modelBuilder.Configurations.Add(new adm_role_mfMap());
            modelBuilder.Configurations.Add(new adm_user_mfMap());
            modelBuilder.Configurations.Add(new emr_prescription_complaintMap());
            modelBuilder.Configurations.Add(new emr_prescription_diagnosMap());
            modelBuilder.Configurations.Add(new emr_prescription_investigationMap());
            modelBuilder.Configurations.Add(new emr_prescription_mfMap());
            modelBuilder.Configurations.Add(new emr_prescription_observationMap());
            modelBuilder.Configurations.Add(new emr_prescription_treatmentMap());
            modelBuilder.Configurations.Add(new emr_prescription_treatment_templateMap());
            modelBuilder.Configurations.Add(new adm_user_companyMap());
            modelBuilder.Configurations.Add(new adm_user_tokenMap()); modelBuilder.Configurations.Add(new emr_documentMap());
            modelBuilder.Configurations.Add(new sys_drop_down_mfMap());
            modelBuilder.Configurations.Add(new sys_drop_down_valueMap());
            modelBuilder.Configurations.Add(new emr_medicineMap());
            modelBuilder.Configurations.Add(new emr_vitalMap());
            modelBuilder.Configurations.Add(new ipd_admissionMap());
            modelBuilder.Configurations.Add(new ipd_admission_chargesMap());
            modelBuilder.Configurations.Add(new ipd_admission_imagingMap());
            modelBuilder.Configurations.Add(new ipd_admission_labMap());
            modelBuilder.Configurations.Add(new ipd_admission_medicationMap());
            modelBuilder.Configurations.Add(new ipd_admission_notesMap());
            modelBuilder.Configurations.Add(new ipd_admission_vitalMap());
            modelBuilder.Configurations.Add(new ipd_procedure_chargedMap());
            modelBuilder.Configurations.Add(new ipd_procedure_medicationMap());
            modelBuilder.Configurations.Add(new ipd_procedure_mfMap());
            modelBuilder.Configurations.Add(new emr_patient_bill_paymentMap());
            //employee
            modelBuilder.Configurations.Add(new pr_employee_mfMap());
            modelBuilder.Configurations.Add(new pr_employee_allowanceMap());
            modelBuilder.Configurations.Add(new pr_allowanceMap());
            modelBuilder.Configurations.Add(new pr_deduction_contributionMap());
            modelBuilder.Configurations.Add(new pr_departmentMap());
            modelBuilder.Configurations.Add(new pr_designationMap());
            modelBuilder.Configurations.Add(new pr_employee_ded_contributionMap());
            modelBuilder.Configurations.Add(new pr_employee_leaveMap());
            modelBuilder.Configurations.Add(new pr_employee_payroll_mfMap());
            modelBuilder.Configurations.Add(new pr_employee_payroll_dtMap());
            modelBuilder.Configurations.Add(new pr_leave_applicationMap());
            modelBuilder.Configurations.Add(new pr_leave_typeMap());
            modelBuilder.Configurations.Add(new pr_loanMap());
            modelBuilder.Configurations.Add(new pr_loan_payment_dtMap());
            modelBuilder.Configurations.Add(new pr_pay_scheduleMap());
            modelBuilder.Configurations.Add(new pr_employee_DependentMap());
            modelBuilder.Configurations.Add(new pr_employee_documentMap());
            modelBuilder.Configurations.Add(new pr_time_entryMap());
            modelBuilder.Configurations.Add(new sys_holidaysMap());
            modelBuilder.Configurations.Add(new adm_itemMap());
            modelBuilder.Configurations.Add(new pur_vendorMap());
            modelBuilder.Configurations.Add(new pur_invoice_mfMap());
            modelBuilder.Configurations.Add(new pur_invoice_dtMap());
            modelBuilder.Configurations.Add(new pur_sale_mfMap());
            modelBuilder.Configurations.Add(new pur_sale_dtMap());
            modelBuilder.Configurations.Add(new pur_sale_hold_mfMap());
            modelBuilder.Configurations.Add(new pur_sale_hold_dtMap());
            modelBuilder.Configurations.Add(new inv_stockMap());
            modelBuilder.Configurations.Add(new pur_paymentMap());
            modelBuilder.Configurations.Add(new ipd_input_outputMap());
            modelBuilder.Configurations.Add(new ipd_medication_logMap());
            modelBuilder.Configurations.Add(new adm_reminder_mfMap());
            modelBuilder.Configurations.Add(new adm_reminder_dtMap());
            modelBuilder.Configurations.Add(new adm_item_logMap());
            modelBuilder.Configurations.Add(new ipd_procedure_expenseMap());
            modelBuilder.Configurations.Add(new adm_integrationMap());
            modelBuilder.Configurations.Add(new pr_attendanceMap());
            modelBuilder.Configurations.Add(new pr_time_logMap());
        }
    }
}
