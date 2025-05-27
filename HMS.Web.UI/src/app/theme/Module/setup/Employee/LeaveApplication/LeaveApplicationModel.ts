export class LeaveApplicationModel {
    ID: number;
    CompanyID: number;
    EmployeeID: number;
    LeaveTypeID: number;
    FromDate: Date;
    ToDate: Date;
    Hours: number = 0;
    disabledbox: boolean = false;
}