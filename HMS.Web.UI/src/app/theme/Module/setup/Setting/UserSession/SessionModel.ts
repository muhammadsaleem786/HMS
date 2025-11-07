export class SessionModel {
   
    ID: number=0;
    CompanyId: number;
    ShiftStartTime: Date;
    StartTime: string;
    ShiftEndTime: Date;
    StartingCash: number;
    TransactionsCount: number;    
    TotalCollection: number;    
    Refunds: number;    
    ClosingBalance: number;    
    OpeningComments: string;    
    ClosingComments: string;
    ExpensesAmount: Number;
    OPDBalance: Number;
    IPDBalance: Number;
    PharmacyBalance: number;
    OtherIncome: number;
    PaymentMethodID: number;
    PaymentMethodDropdownID: number;
    PurchaseExpensesAmount: number;
}
