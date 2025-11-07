export class UserShareModel {
    ID: number;
    CompanyID: number;
    DoctorId: number;
    PercentageType: string;
    GeneralDoctorShare: number=0;
    GeneralOwnerShare: number = 0;
    ProcedureDoctorShare: number = 0;
    ProcedureOwnerShare: number = 0;
    DiscountDoctorShare: number = 0;
    DiscountOwnerShare: number = 0;
}
