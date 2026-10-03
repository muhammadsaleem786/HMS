export class SaleInvoiceModel {
    constructor() {
        let objsale_invoice_dt = new sale_invoice_dt();
        this.sale_invoice_dt.push(objsale_invoice_dt);
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
    sale_invoice_dt: Array<sale_invoice_dt> = [];
}
export class sale_invoice_dt {
    ID: number;
    CompanyID: number;
    InvoiceID: number;
    ItemID: number;   
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