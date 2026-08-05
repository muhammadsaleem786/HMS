export class UserShareModel {
    ID: number;
    CompanyID: number;
    DoctorId: number;   
    DisDoctorShare: number = 0;
    DisOwnerShare: number = 0;
    DeductOTCharges: boolean=false;
    DeductMedicineCost: boolean=false;
    DeductConsumables: boolean=false;
    DeductImplantCost: boolean=false;
    DeductLabConsumables: boolean=false;
    DeductRoomCharges: boolean = false;
    CategoryId: number;
    Category: string;
    ServiceId: number;
    LabId: number;
    ItemId: number;
    ServiceName: string;
    SharedMethod: string = 'Percentage';
    PercentageType: string;
    DoctorShare: number = 0;
    OwnerShare: string;
    CreatedBy: number;
    CreatedDate: Date;
}
