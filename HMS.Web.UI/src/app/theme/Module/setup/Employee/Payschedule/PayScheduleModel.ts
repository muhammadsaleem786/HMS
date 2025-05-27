export class Payschedule {
    ID : number;
    CompanyID : number;
    PayTypeID : number;
    ScheduleName : string;
    PeriodStartDate : Date;
    PeriodEndDate : Date;
    FallInHolidayID : number;
    PayDate : Date;
    Active : boolean;
    CreatedBy: number;
    CreatedDate: Date;
    ModifiedBy: number;
    ModifiedDate: Date;
    PopUpVisible: boolean = false;
    Lock: boolean = false;
}
