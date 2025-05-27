export class DashboardModel {
    ID: number;
    FormDate: Date;
    ToDate: Date;
}


export class DashboardFilterModel {
    Range: string = 'ThisYear';
    PeriodStart: Date;
    PeriodEnd: Date;
    PayScheduleID: number = null;
    LocationID: number = null;
    DepartmentID: number = null;
    PeriodStartMonthYear?: string;
    PeriodEndMonthYear?: string;
}

export class AllEmpPrSummaryTotalModel {
    NoOfEmployees: string;
    TotalTax: number = 0;
    TotalAllowance: number = 0;
    TotalDeductions: number = 0;
    TotalNetSalary: number = 0;
    TotalGrossSalary: number = 0;
    TotalEmployeeProvidentFund: number = 0;
    TotalEmployerProvidentFund: number = 0;
    TotalEmployeeEOBI: number = 0;
    TotalEmployerEOBI: number = 0;
    TotalEmployeeGOSI: number = 0;
    TotalEmployerGOSI: number = 0;
    TotalGivenLoan: number = 0;
    TotalRecoveredLoan: number = 0;
}

export class SingleEmpPrSummaryTotalModel {
    EmployeeID: number;
    Name: string;
    Designation: string;
    TotalVacationHours: number;
    TotalSickHours: number;
    VacationLeaveHours: number;
    SickLeaveHours: number;
    BasicSalary: number;
    Deduction: number;
    GrossSalary: number;
    NetSalary: number;
    Tax: number;
    Allowance: number;
    EmployeeProvidentFund: number;
    EmployerProvidentFund: number;
    EmployeeEOBI: number;
    EmployerEOBI: number;
    EmployeeGOSI: number;
    EmployerGOSI: number;
    GivenLoan: number;
    RecoveredLoan: number;
}
export class ChartTotalByMonthModel {
    EmployeeID: number;
    Month: string;
    TotalTax: number;
    TotalNetSalary: number;
    TotalDeductions: number;
    TotalContributions: number;

}

export class VacationAndSickLeaveModel {
    EmployeeID: number;
    TotalVacationHours: number = 0;
    TotalSickHours: number = 0;
    VacationLeaveHours: number = 0;
    SickLeaveHours: number = 0;
    RemainingVacationHours: number = 0;
    RemainingSickHours: number = 0;
}
