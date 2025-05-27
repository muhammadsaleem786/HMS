export class emr_patient_bill {
    ID: number;
    CompanyId: number;
    AppointmentId: number;
    PatientId: number;
    ServiceId: number;
    BillDate: Date;
    Price: number;
    Discount: number;
    PaidAmount: number;
    DoctorId: number;
    PatientName: string;
    DoctorName: string;
    ServiceName: string;
    OutstandingBalance: number;
    Remarks: string;
    PartialAmount: string;
    RefundAmount: string;
    RefundDate: Date;
    PaymentDate: Date;
    emr_patient_bill_payment: Array<emr_patient_bill_payment> = [];
}

export class emr_patient_bill_payment {
    ID: number;
    BillId: number;
    CompanyId: number;
    Amount: number;
    Remarks: string;
    PaymentDate: Date;
}
