export class emr_expense {
    ID: number;
    CategoryId: number;
    CategoryDropdownId: number;
    PaymentMethodID: number;
    PaymentMethodDropdownID: number;
    Date: Date;
    Remark: string;
    Amount: number;
    ClinicId: number;
    InvoiceDate: Date;
    InvoiceNumber: string;
    Vendor: string;
    PaymentStatusId: number;
    PaymentStatusDropdownId: number;
    PaymentRemrks: string;
    Attachment: string;
    Image: string;
    PaymentTo: number=3;
    EmployeeId: number;
    EmployeeName: string;
    VendorId: number;
    VendorName: string;
    DoctorId: number;
    DoctorName: string;
}
