export class ReportModel {
    ID: number;
    FromDate: Date;
    ToDate: Date;
    PatientId: number;
    DoctorId: number;
    ProcedureId: number;
    DoctorName: string;
    PatientName: string;
    ProcedureName: string;

    ItemId: number;
    ItemName: string;

    Type: number;
    Email: string;
    base64: string;
    FileName: string;
}