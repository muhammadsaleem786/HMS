export class EmployeePayrollModel {

    constructor() {
        let objpayrollDet = new pr_employee_payroll_dt();
        this.pr_employee_payroll_dt.push(objpayrollDet);
    }

    ID: number;
    CompanyID: number;
    PayScheduleID: number;
    PayDate: Date;
    EmployeeID: number;
    Status: string;
    BasicSalary: number;
    PayScheduleFromDate: Date;
    PayScheduleToDate: Date;
    FromDate: Date;
    ToDate: Date;
    AdjustmentDate: Date = new Date;
    AdjustmentType: string = 'C';
    AdjustmentAmount: number;
    AdjustmentComments: string;
    AdjustmentBy: number;

    CreatedBy: number;
    CreatedDate: Date;

    pr_employee_payroll_dt: Array<pr_employee_payroll_dt> = [];
}

export class RunPayrollModel {
    PayScheduleId: number;
    EmployeeIds: number[] = [];
}


export class PrEmpDtFilterModel {
    EmployeeIds: number[] = [];
    LocationsIds: number[] = [];
    DepartmentsIds: number[] = [];
    DesignationsIds: number[] = [];
    EmployeeTypeIds: number[] = [];
}

export class AdjustmentModel {
    AllowDedConID: number;
    AllDedContType: string
    PayScheduleID: number;
    PayDate: Date;
    EmployeeID: number;
    PayrollMfID: number;
    PayrollDtID: number;
    AdjustmentTitle: string;
    OrignalText: string;
    AdjustmentText: string;
    OrignalValue: number;
    AdjustmentValue: number;
    AdjustmentDate: Date;
    AdjustmentType: string;
    AdjustmentAmount: number;
    AdjustmentComments: string;
    AdjustmentBy: number;
}

export class pr_employee_payroll_dt {
    ID: number;
    PayrollID: number;
    EmployeeID: number;
    CompanyID: number;
    PayScheduleID: number;
    PayDate: Date;
    Type: string;
    AllowDedID: number;
    Amount: number;
    Taxable: boolean;
    AdjustmentDate: Date;
    AdjustmentType: string;
    AdjustmentAmount: number;
    AdjustmentComments: string;
    RefID: number = 0;
}

export class SalaryModel {
    PayScheduleId: number;
    EmployeeIds: string;
}

export class ProrateModel {
    FromDate: Date;
    ToDate: Date;
}

export class PrEmpDtSalaryTotalModel {
    TotalBaseSalary: number = 0;
    TotalAllowance: number = 0;
    TotalDeduction: number = 0;
    TotalGrossSalary: number = 0;
    TotalTax: number = 0;
    TotalNetSalary: number = 0;
}
