export class LoanModel {
    constructor() {
        let objLeave = new pr_loan_payment_dt();
        this.pr_loan_payment_dt.push(objLeave);
    }

    ID: number;
    CompanyID: number;
    EmployeeID: number;
    PaymentMethodDropdownID: number;
    PaymentMethodID: number;
    LoanTypeDropdownID: number;
    LoanTypeID: number;
    LoanCode: string;
    Description: string;
    PaymentStartDate: Date;
    LoanDate: Date;
    LoanAmount: number;
    DeductionType: string = 'F';
    DeductionValue: number;
    InstallmentByBaseSalary: number;
    CreatedBy: number;
    CreatedDate: Date;
    ModifiedBy: number;
    ModifiedDate: Date;
    AdjustmentDate: number;
    AdjustmentType: string;
    AdjustmentAmount: number;
    AdjustmentComments: string;
    AdjustmentBy: number;
    pr_loan_payment_dt: Array<pr_loan_payment_dt> = [];
}

export class pr_loan_payment_dt {
    ID: number;
    CompanyID: number;
    LoanID: number;
    PaymentDate: Date;
    Comment: string;
    Amount: number;
    AdjustmentDate: Date;
    AdjustmentType: string;
    AdjustmentAmount: number;
    AdjustmentComments: string;
    AdjustmentBy: number;
}


export class PayrollTempDetailModel {
    ID: number;
    ScreenType: string;
    Transaction: string;
    LoanAmount: number;
    LoanDate: Date;
    Payment: number;
    Balance: number;
    EmpName: string;
    Description: string;
    TotalBalance: number;
    DeductionType: string;
    DeductionValue: number;
    InstallmentByBaseSalary: number;
    AdjustmentAmount: number;
    AdjustmentBy: number;
    AdjustmentComments: string;
    AdjustmentDate: Date;
    AdjustmentType: string;
}


export class AdjustmentModel {
    EmployeeName: string;
    LoanScreenType: string;
    LoanMfID: number;
    LoanDtID: number;
    AdjustmentTitle: string;
    AdjustmentText: string;
    AdjustmentValue: number;
    OrignalText: string;
    OrignalValue: number;
    AdjustmentDate: Date;
    AdjustmentType: string = 'C';
    AdjustmentAmount: number;
    AdjustmentComments: string;
    AdjustmentBy: number;
    TransactionDate: Date;
}

export class PaginationSumModel {
    LoanSum: number = 0;
    PaymentSum: number = 0;
    BalanceSum: number = 0;
}

export class PaginationDetailSumModel {
    LoanSum: number = 0;
    PaymentSum: number = 0;
    BalanceSum: number = 0;
}