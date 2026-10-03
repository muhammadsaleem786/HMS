export class SalePaymentModel {
    ID: number;
    CompanyId: number;
    InvoiceId: number;
    PaymentMethodDropdownID: number;
    PaymentMethodID: number;
    PaymentDate: Date;
    Amount: number;   
    Notes: string;
}
