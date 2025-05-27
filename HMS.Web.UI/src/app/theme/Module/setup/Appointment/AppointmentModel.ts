export class emr_patient {
    ID: number;
    PatientName: string;
    Gender: number;
    DOB: Date;
    Age: number;
    Email: string;
    Mobile: string;
    ContactNo: string;
    CNIC: string;
    Image: string;
    Notes: string;
    MRNO: string;
    BloodGroupId: number;
    BloodGroupDropDownId: number;
    EmergencyNo: string;
    Address: string;
    ReferredBy: string;
    AnniversaryDate: Date;
    Illness_Diabetes: boolean;
    Illness_Tuberculosis: boolean;
    Illness_HeartPatient: boolean;
    Illness_LungsRelated: boolean;
    Illness_BloodPressure: boolean;
    Illness_Migraine: boolean;
    Illness_Other: string;
    Allergies_Food: string;
    Allergies_Drug: string;
    Allergies_Other: string;
    Habits_Smoking: string;
    Habits_Drinking: string;
    Habits_Tobacco: string;
    Habits_Other: string;
    MedicalHistory: string;
    CurrentMedication: string;
    HabitsHistory: string;
    PrefixTittleId: number;
    PrefixDropdownId: number;
    BillTypeId: number;
    BillTypeDropdownId: number;
    Father_Husband: string;
}
export class emr_Appointment {
    ID: number;
    PatientId: number;
    PatientName: string;
    MrNo: string;
    CNIC: string;
    Mobile: string;
    DoctorId: number;
    PatientProblem: string;
    Notes: string;
    AppointmentDate: Date;
    AppointmentTime: string;
    StatusId: number = 25;
    TokenNo: number;
    ReminderId: number;
    //bill filed
    AppointmentId: number;
    ServiceId: number;
    BillDate: Date;
    Price: number;
    Discount: number;
    PaidAmount: number;
    DoctorName: string;
    ServiceName: string;
    OutstandingBalance: number;
    Remarks: string;
    VisitId: number;
    Location: string;
    IsAdmission: boolean;
    IsAdmit: boolean;
    AdmissionId: number;
    PrimaryDescription: string;
    SecondaryDescription: string;
    IsExistFollowUp: boolean;
}
export class emr_document {
    ID: number;
    Remarks: string;
    DocumentUpload: string;
    Date: Date;
    DocumentTypeId: number;
    DocumentTypeDropdownId: number;
    ReferenceNo: string;
}
export class emr_vital {
    ID: number;
    Measure: string;
    Measure2: string;
    Date: Date;
    VitalId: number;
    VitalDropdownId: number;
    PatientId: number;
    Name: number;
    Unit: string;
}

export class emr_notes_favorite {
    ID: number;
    CompanyId: number;
    DoctorId: number;
    ReferenceId: number;
    ReferenceType: string;
}

export class patientInfo {
    PatientName: string;
    Age: string;
    Gender: string;
    AppointmentDate: string;
    ClinicIogo: string;
}
export class DoctorInfo {
    Name: string;
    Designation: string;
    PhoneNo: string;
    Qualification: string;
    TemplateId: string;
    Footer: string;
}
export class InvoiceCompanyInfo {
    CompanyName: string;
    CompanyAddress: string;
    CompanyPhone: string;
    CompanyEmail: string;
    PatientName: string;
    PatientAddress: string;
    PatientEmail: string;
    PatientMobile: string;
    BillDate: string;
    invoiceNo: string;
    CompanyLogo: string;
    MRNo: string;
    AdmissionDate: Date;
    DischargeDate: Date;
    Room: string;
    Ward: string;
    DoctorName: string;
    ReceiptNo: string;
    Amount: number;
    Father_Husband: string;
    Department: string;
    Age: string;
    DOB: string;
    Specialty: string;
    DepartmentType: string;
    Sex: string;
} 