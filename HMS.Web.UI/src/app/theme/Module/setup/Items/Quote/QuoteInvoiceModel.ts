export class QuoteInvoiceModel {
    constructor() {
        let objquote_invoice_dt = new quote_invoice_dt();
        this.quote_invoice_dt.push(objquote_invoice_dt);
    }
    ID: number;
    CompanyID: number;
    CustomerID: number;
    CustomerName: string;
    InvoiceNo: string;
    OrderNo: number;
    InvoiceDate: Date;
    DueDate: Date;   
    Total: number;   
    DiscountAmount: number = 0;
    Discount: number;
    SaveStatus: number;
    action: string;
    IsItemLevelDiscount: boolean = false;   
    quote_invoice_dt: Array<quote_invoice_dt> = [];
}
export class quote_invoice_dt {
    ID: number;
    CompanyID: number;
    InvoiceID: number;
    ItemID: number;
    ServiceID:number;
    ItemTypeId: number=1;
    ItemDescription: string;
    Quantity: number;
    Rate: number;
    SalePrice: number;
    Amount: number;
    Discount: number;
    DiscountAmount: number = 0;   
    DiscountType: number = 1;
    BatchSarialNumber: number;
    ExpiredWarrantyDate: Date;
    IsBatch: boolean = false;
    ItemType: string;
    Item: string;
}
export class BatchModel {
    BatchSarialNumber: number;
    ExpiredWarrantyDate: Date;
    ItemID: number;  
}