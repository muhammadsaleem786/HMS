using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Entities.Models
{
    public partial class adm_user_mf : Entity
    {
        public adm_user_mf()
        {
            this.emr_complaint = new List<emr_complaint>();
            this.emr_complaint1 = new List<emr_complaint>();
            this.ipd_diagnosis = new List<ipd_diagnosis>();
            this.ipd_diagnosis1 = new List<ipd_diagnosis>();
            this.emr_diagnos = new List<emr_diagnos>();
            this.emr_diagnos1 = new List<emr_diagnos>();
            this.emr_instruction = new List<emr_instruction>();
            this.emr_instruction1 = new List<emr_instruction>();
            this.emr_investigation = new List<emr_investigation>();
            this.emr_investigation1 = new List<emr_investigation>();
            this.emr_observation = new List<emr_observation>();
            this.emr_observation1 = new List<emr_observation>();
            this.adm_company = new List<adm_company>();
            this.adm_company1 = new List<adm_company>();
            this.adm_user_company = new List<adm_user_company>();
            this.adm_user_company1 = new List<adm_user_company>();
            this.adm_user_token = new List<adm_user_token>();
            this.emr_appointment_mf = new List<emr_appointment_mf>();
            this.emr_appointment_mf1 = new List<emr_appointment_mf>();
            this.emr_appointment_mf2 = new List<emr_appointment_mf>();
            this.emr_document = new List<emr_document>();
            this.emr_document1 = new List<emr_document>();
            this.emr_medicine = new List<emr_medicine>();
            this.emr_medicine1 = new List<emr_medicine>();
            this.emr_notes_favorite = new List<emr_notes_favorite>();
            this.emr_notes_favorite1 = new List<emr_notes_favorite>();
            this.emr_patient_mf = new List<emr_patient_mf>();
            this.emr_patient_mf1 = new List<emr_patient_mf>();
            this.emr_expense = new List<emr_expense>();
            this.emr_expense1 = new List<emr_expense>();
            this.emr_income = new List<emr_income>();
            this.emr_income1 = new List<emr_income>();
            this.emr_prescription_complaint = new List<emr_prescription_complaint>();
            this.emr_prescription_complaint1 = new List<emr_prescription_complaint>();
            this.emr_prescription_diagnos = new List<emr_prescription_diagnos>();
            this.emr_prescription_diagnos1 = new List<emr_prescription_diagnos>();
            this.emr_prescription_investigation = new List<emr_prescription_investigation>();
            this.emr_prescription_investigation1 = new List<emr_prescription_investigation>();
            this.emr_prescription_mf = new List<emr_prescription_mf>();
            this.emr_prescription_mf1 = new List<emr_prescription_mf>();
            this.emr_prescription_observation = new List<emr_prescription_observation>();
            this.emr_prescription_observation1 = new List<emr_prescription_observation>();
            this.emr_prescription_treatment = new List<emr_prescription_treatment>();
            this.emr_prescription_treatment1 = new List<emr_prescription_treatment>();
            this.emr_prescription_treatment_template = new List<emr_prescription_treatment_template>();
            this.emr_prescription_treatment_template1 = new List<emr_prescription_treatment_template>();
            this.emr_vital = new List<emr_vital>();
            this.emr_vital1 = new List<emr_vital>();
            this.emr_service_mf = new List<emr_service_mf>();
            this.emr_service_mf1 = new List<emr_service_mf>();
            this.emr_service_item = new List<emr_service_item>();
            this.emr_service_item1 = new List<emr_service_item>();

            this.emr_patient_bill = new List<emr_patient_bill>();
            this.emr_patient_bill1 = new List<emr_patient_bill>();
            this.ipd_admission = new List<ipd_admission>();
            this.ipd_admission1 = new List<ipd_admission>();
            this.ipd_admission_charges = new List<ipd_admission_charges>();
            this.ipd_admission_charges1 = new List<ipd_admission_charges>();
            this.ipd_admission_imaging = new List<ipd_admission_imaging>();
            this.ipd_admission_imaging1 = new List<ipd_admission_imaging>();
            this.ipd_admission_lab = new List<ipd_admission_lab>();
            this.ipd_admission_lab1 = new List<ipd_admission_lab>();
            this.ipd_admission_medication = new List<ipd_admission_medication>();
            this.ipd_admission_medication1 = new List<ipd_admission_medication>();
            this.ipd_admission_notes = new List<ipd_admission_notes>();
            this.ipd_admission_notes1 = new List<ipd_admission_notes>();
            this.ipd_admission_vital = new List<ipd_admission_vital>();
            this.ipd_admission_vital1 = new List<ipd_admission_vital>();
            this.ipd_procedure_charged = new List<ipd_procedure_charged>();
            this.ipd_procedure_charged1 = new List<ipd_procedure_charged>();
            this.ipd_procedure_medication = new List<ipd_procedure_medication>();
            this.ipd_procedure_medication1 = new List<ipd_procedure_medication>();
            this.ipd_procedure_mf = new List<ipd_procedure_mf>();
            this.ipd_procedure_mf1 = new List<ipd_procedure_mf>();
            this.emr_patient_bill_payment = new List<emr_patient_bill_payment>();
            this.emr_patient_bill_payment1 = new List<emr_patient_bill_payment>();
            this.user_payment = new List<user_payment>();
            this.user_payment1 = new List<user_payment>();
            this.user_payment2 = new List<user_payment>();
            this.ipd_admission_discharge = new List<ipd_admission_discharge>();
            this.ipd_admission_discharge1 = new List<ipd_admission_discharge>();
            this.adm_item = new List<adm_item>();
            this.adm_item1 = new List<adm_item>();
            this.pur_vendor = new List<pur_vendor>();
            this.pur_vendor1 = new List<pur_vendor>();
            this.pur_invoice_mf = new List<pur_invoice_mf>();
            this.pur_invoice_mf1 = new List<pur_invoice_mf>();
            this.inv_stock = new List<inv_stock>();
            this.inv_stock1 = new List<inv_stock>();
            this.pur_sale_mf = new List<pur_sale_mf>();
            this.pur_sale_mf1 = new List<pur_sale_mf>();
            this.pur_payment = new List<pur_payment>();
            this.pur_payment1 = new List<pur_payment>();
            this.ipd_input_output = new List<ipd_input_output>();
            this.ipd_input_output1 = new List<ipd_input_output>();
            this.ipd_medication_log = new List<ipd_medication_log>();
            this.ipd_medication_log1 = new List<ipd_medication_log>();
            this.adm_reminder_mf = new List<adm_reminder_mf>();
            this.adm_reminder_mf1 = new List<adm_reminder_mf>();
            this.adm_reminder_dt = new List<adm_reminder_dt>();
            this.adm_reminder_dt1 = new List<adm_reminder_dt>();

            this.adm_item_log = new List<adm_item_log>();
            this.adm_item_log1 = new List<adm_item_log>();
            this.pur_sale_hold_mf = new List<pur_sale_hold_mf>();
            this.pur_sale_hold_mf1 = new List<pur_sale_hold_mf>();

            this.ipd_procedure_expense = new List<ipd_procedure_expense>();
            this.ipd_procedure_expense1 = new List<ipd_procedure_expense>();

            this.adm_integration = new List<adm_integration>();
            this.adm_integration1 = new List<adm_integration>();
        }
        public decimal ID { get; set; }
        public string Email { get; set; }
        public string Pwd { get; set; }
        [NotMapped]
        public string CurrentPassword { get; set; }
        public string Name { get; set; }
        public string PhoneNo { get; set; }
        public string AccountType { get; set; }
        public int CultureID { get; set; }
        public Nullable<int> AccountStatus { get; set; }
        public string IsGenderDropdown { get; set; }
        public Nullable<int> AppointmentStatusId { get; set; }
        public decimal EmployeeID { get; set; }
        [NotMapped]
        public int[] GenderDropdown { get; set; }
        public int LoginFailureNo { get; set; }
        public bool UserLock { get; set; }
        public bool IsActivated { get; set; }
        public string UserImage { get; set; }
        public string RepotFooter { get; set; }
        public string ActivationToken { get; set; }
        public string Qualification { get; set; }
        public string Designation { get; set; }

        public string DesignationUrdu { get; set; }
        public string QualificationUrdu { get; set; }
        public string HeaderDescription { get; set; }
        public string NameUrdu { get; set; }
        public Nullable<System.DateTime> ActivationTokenDate { get; set; }
        public Nullable<System.DateTime> ActivatedDate { get; set; }
        public Nullable<System.DateTime> LastSignIn { get; set; }
        public string ForgotToken { get; set; }
        public Nullable<System.DateTime> ForgotTokenDate { get; set; }
        public bool PhoneNotification { get; set; }
        public bool EmailNotification { get; set; }
        [NotMapped]
        public string AppStartTime { get; set; }
        [NotMapped]
        public string AppEndTime { get; set; }
        public Nullable<TimeSpan> SlotTime { get; set; }
        public Nullable<TimeSpan> StartTime { get; set; }
        public Nullable<TimeSpan> EndTime { get; set; }
        public Boolean IsOverLap { get; set; }
        public Nullable<DateTime> ExpiryDate { get; set; }
        public Nullable<int> SpecialtyId { get; set; }
        public Nullable<int> SpecialtyDropdownId { get; set; }
        public string Type { get; set; }
        [NotMapped]
        public int[] DocWorkingDay { get; set; }
        public string OffDay { get; set; }
        public bool IsDeleted { get; set; }
        public string IsShowDoctor { get; set; }
        public Nullable<decimal> MultilingualId { get; set; }
        public Nullable<decimal> TemplateId { get; set; }

        /// <summary> Company table
        [NotMapped]
        public string CompanyName { get; set; }
        [NotMapped]
        public int CompanyTypeDropDownID { get; set; }
        [NotMapped]
        public Nullable<int> CompanyTypeID { get; set; }
        [NotMapped]
        public Nullable<int> GenderID { get; set; }
        [NotMapped]
        public string ContactPersonFirstName { get; set; }
        [NotMapped]
        public string ContactPersonLastName { get; set; }
        [NotMapped]
        public bool IsShowBillReceptionist { get; set; }
        [NotMapped]
        public string CompanyAddress1 { get; set; }
        [NotMapped]
        public string CompanyAddress2 { get; set; }
        [NotMapped]
        public Nullable<int> LanguageID { get; set; }
        [NotMapped]
        public Nullable<int> CityDropDownId { get; set; }
        [NotMapped]
        public string CompanyLogo { get; set; }
        [NotMapped]
        public string CountryDropdownId { get; set; }
        [NotMapped]
        public string Phone { get; set; }
        [NotMapped]
        public string Fax { get; set; }
        [NotMapped]
        public string PostalCode { get; set; }
        [NotMapped]
        public string Province { get; set; }
        [NotMapped]
        public string Website { get; set; }
        [NotMapped]
        public bool IsTrialVersion { get; set; }
        [NotMapped]
        public decimal CreatedBy { get; set; }
        [NotMapped]
        public System.DateTime CreatedDate { get; set; }
        [NotMapped]
        public Nullable<decimal> ModifiedBy { get; set; }
        [NotMapped]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        //public virtual pr_employee_mf pr_employee_mf { get; set; }
        /// </summary>
        public virtual ICollection<adm_company> adm_company { get; set; }
        public virtual ICollection<adm_company> adm_company1 { get; set; }
        public virtual ICollection<adm_user_company> adm_user_company { get; set; }
        public virtual ICollection<adm_user_company> adm_user_company1 { get; set; }
        public virtual ICollection<adm_user_token> adm_user_token { get; set; }
        public virtual ICollection<emr_appointment_mf> emr_appointment_mf { get; set; }
        public virtual ICollection<emr_appointment_mf> emr_appointment_mf1 { get; set; }
        public virtual ICollection<emr_appointment_mf> emr_appointment_mf2 { get; set; }
        public virtual ICollection<emr_patient_mf> emr_patient_mf { get; set; }
        public virtual ICollection<emr_patient_mf> emr_patient_mf1 { get; set; }
        public virtual ICollection<emr_document> emr_document { get; set; }
        public virtual ICollection<emr_document> emr_document1 { get; set; }
        public virtual ICollection<emr_prescription_complaint> emr_prescription_complaint { get; set; }
        public virtual ICollection<emr_prescription_complaint> emr_prescription_complaint1 { get; set; }
        public virtual ICollection<emr_prescription_diagnos> emr_prescription_diagnos { get; set; }
        public virtual ICollection<emr_prescription_diagnos> emr_prescription_diagnos1 { get; set; }
        public virtual ICollection<emr_prescription_investigation> emr_prescription_investigation { get; set; }
        public virtual ICollection<emr_prescription_investigation> emr_prescription_investigation1 { get; set; }
        public virtual ICollection<emr_prescription_mf> emr_prescription_mf { get; set; }
        public virtual ICollection<emr_prescription_mf> emr_prescription_mf1 { get; set; }
        public virtual ICollection<emr_notes_favorite> emr_notes_favorite { get; set; }
        public virtual ICollection<emr_notes_favorite> emr_notes_favorite1 { get; set; }
        public virtual ICollection<emr_prescription_observation> emr_prescription_observation { get; set; }
        public virtual ICollection<emr_prescription_observation> emr_prescription_observation1 { get; set; }
        public virtual ICollection<emr_prescription_treatment> emr_prescription_treatment { get; set; }
        public virtual ICollection<emr_prescription_treatment> emr_prescription_treatment1 { get; set; }
        public virtual ICollection<emr_prescription_treatment_template> emr_prescription_treatment_template { get; set; }
        public virtual ICollection<emr_prescription_treatment_template> emr_prescription_treatment_template1 { get; set; }
        public virtual ICollection<emr_medicine> emr_medicine { get; set; }
        public virtual ICollection<emr_medicine> emr_medicine1 { get; set; }
        public virtual ICollection<emr_expense> emr_expense { get; set; }
        public virtual ICollection<emr_expense> emr_expense1 { get; set; }
        public virtual ICollection<emr_income> emr_income { get; set; }
        public virtual ICollection<emr_income> emr_income1 { get; set; }
        public virtual ICollection<emr_complaint> emr_complaint { get; set; }
        public virtual ICollection<emr_complaint> emr_complaint1 { get; set; }
        public virtual ICollection<ipd_diagnosis> ipd_diagnosis { get; set; }
        public virtual ICollection<ipd_diagnosis> ipd_diagnosis1 { get; set; }
        public virtual ICollection<emr_diagnos> emr_diagnos { get; set; }
        public virtual ICollection<emr_diagnos> emr_diagnos1 { get; set; }
        public virtual ICollection<emr_instruction> emr_instruction { get; set; }
        public virtual ICollection<emr_instruction> emr_instruction1 { get; set; }
        public virtual ICollection<emr_investigation> emr_investigation { get; set; }
        public virtual ICollection<emr_investigation> emr_investigation1 { get; set; }
        public virtual ICollection<emr_observation> emr_observation { get; set; }
        public virtual ICollection<emr_observation> emr_observation1 { get; set; }
        public virtual ICollection<emr_vital> emr_vital { get; set; }
        public virtual ICollection<emr_vital> emr_vital1 { get; set; }
        public virtual ICollection<emr_patient_bill> emr_patient_bill { get; set; }
        public virtual ICollection<emr_patient_bill> emr_patient_bill1 { get; set; }
        public virtual ICollection<emr_service_mf> emr_service_mf { get; set; }
        public virtual ICollection<emr_service_mf> emr_service_mf1 { get; set; }
        public virtual ICollection<emr_service_item> emr_service_item { get; set; }
        public virtual ICollection<emr_service_item> emr_service_item1 { get; set; }
        public virtual sys_drop_down_value sys_drop_down_value { get; set; }
        public virtual ICollection<ipd_admission> ipd_admission { get; set; }
        public virtual ICollection<ipd_admission> ipd_admission1 { get; set; }
        public virtual ICollection<ipd_admission_charges> ipd_admission_charges { get; set; }
        public virtual ICollection<ipd_admission_charges> ipd_admission_charges1 { get; set; }
        public virtual ICollection<ipd_admission_imaging> ipd_admission_imaging { get; set; }
        public virtual ICollection<ipd_admission_imaging> ipd_admission_imaging1 { get; set; }
        public virtual ICollection<ipd_admission_lab> ipd_admission_lab { get; set; }
        public virtual ICollection<ipd_admission_lab> ipd_admission_lab1 { get; set; }
        public virtual ICollection<ipd_admission_medication> ipd_admission_medication { get; set; }
        public virtual ICollection<ipd_admission_medication> ipd_admission_medication1 { get; set; }
        public virtual ICollection<ipd_admission_notes> ipd_admission_notes { get; set; }
        public virtual ICollection<ipd_admission_notes> ipd_admission_notes1 { get; set; }
        public virtual ICollection<ipd_admission_vital> ipd_admission_vital { get; set; }
        public virtual ICollection<ipd_admission_vital> ipd_admission_vital1 { get; set; }
        public virtual ICollection<ipd_procedure_charged> ipd_procedure_charged { get; set; }
        public virtual ICollection<ipd_procedure_charged> ipd_procedure_charged1 { get; set; }
        public virtual ICollection<ipd_procedure_medication> ipd_procedure_medication { get; set; }
        public virtual ICollection<ipd_procedure_medication> ipd_procedure_medication1 { get; set; }
        public virtual ICollection<ipd_procedure_mf> ipd_procedure_mf { get; set; }
        public virtual ICollection<ipd_procedure_mf> ipd_procedure_mf1 { get; set; }
        public virtual ICollection<emr_patient_bill_payment> emr_patient_bill_payment { get; set; }
        public virtual ICollection<emr_patient_bill_payment> emr_patient_bill_payment1 { get; set; }
        public virtual ICollection<user_payment> user_payment { get; set; }
        public virtual ICollection<user_payment> user_payment1 { get; set; }
        public virtual ICollection<user_payment> user_payment2 { get; set; }
        public virtual ICollection<ipd_admission_discharge> ipd_admission_discharge { get; set; }
        public virtual ICollection<ipd_admission_discharge> ipd_admission_discharge1 { get; set; }
        public virtual ICollection<adm_item> adm_item { get; set; }
        public virtual ICollection<adm_item> adm_item1 { get; set; }
        public virtual ICollection<pur_vendor> pur_vendor { get; set; }
        public virtual ICollection<pur_vendor> pur_vendor1 { get; set; }
        public virtual ICollection<pur_invoice_mf> pur_invoice_mf { get; set; }
        public virtual ICollection<pur_invoice_mf> pur_invoice_mf1 { get; set; }
        public virtual ICollection<inv_stock> inv_stock { get; set; }
        public virtual ICollection<inv_stock> inv_stock1 { get; set; }
        public virtual ICollection<pur_sale_mf> pur_sale_mf { get; set; }
        public virtual ICollection<pur_sale_mf> pur_sale_mf1 { get; set; }
        public virtual ICollection<pur_sale_hold_mf> pur_sale_hold_mf { get; set; }
        public virtual ICollection<pur_sale_hold_mf> pur_sale_hold_mf1 { get; set; }

        public virtual ICollection<pur_payment> pur_payment { get; set; }
        public virtual ICollection<pur_payment> pur_payment1 { get; set; }

        public virtual ICollection<ipd_input_output> ipd_input_output { get; set; }
        public virtual ICollection<ipd_input_output> ipd_input_output1 { get; set; }
        public virtual ICollection<ipd_medication_log> ipd_medication_log { get; set; }
        public virtual ICollection<ipd_medication_log> ipd_medication_log1 { get; set; }
        public virtual ICollection<adm_reminder_mf> adm_reminder_mf { get; set; }
        public virtual ICollection<adm_reminder_mf> adm_reminder_mf1 { get; set; }
        public virtual ICollection<adm_reminder_dt> adm_reminder_dt { get; set; }
        public virtual ICollection<adm_reminder_dt> adm_reminder_dt1 { get; set; }
        public virtual ICollection<adm_item_log> adm_item_log { get; set; }
        public virtual ICollection<adm_item_log> adm_item_log1 { get; set; }

        public virtual ICollection<ipd_procedure_expense> ipd_procedure_expense { get; set; }
        public virtual ICollection<ipd_procedure_expense> ipd_procedure_expense1 { get; set; }

        public virtual ICollection<adm_integration> adm_integration { get; set; }
        public virtual ICollection<adm_integration> adm_integration1 { get; set; }

    }
}
