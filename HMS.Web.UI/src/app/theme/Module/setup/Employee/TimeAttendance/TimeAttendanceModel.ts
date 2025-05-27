export class TimeAttendanceModel {
    ID: number;
    CompanyID: number;
    EmployeeID: number;
    TimeIn: Date;
    TimeOut: Date;
    TimIntxt: string;
    TimOuttxt: string;
    StatusDropDownID: number;
    StatusID: number;
    Remarks: string;
    CreatedDate: Date;
    CreatedBy: number;
    ModifiedDate: Date;
    ModifiedBy: number;
}


export class EmpTimeAttListModel {
    EmployeeID: number;
    Employee: string;
    Designation: string;
    TotalWorkingHours: number = 0;
    TotalDelayHours: number = 0;
    TotalExcusedHours: number = 0;
    TotalPresents: number = 0;
    TotalAbsents: number = 0;
    TotalHolidays: number = 0;
    TotalWeekends: number = 0;
    TotalLeaves: number = 0;
    AtttimeListDet: any[] = [];
}

export class EmpAttendanceSumaryModel {
    TotalWorkingTime: number = 0;
    TotalDelayTime: number = 0;
    TotalExcusedTime: number = 0;
    TotalAbsentHours: number = 0;
    TotalLeavesHours: number = 0;
}