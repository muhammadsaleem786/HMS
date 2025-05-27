export class InvoiceModel {
    constructor() {
        let objpur_invoice_dt = new pur_invoice_dt();
        this.pur_invoice_dt.push(objpur_invoice_dt);
    }
    ID: number;
    CompanyID: number;
    VendorID: number;
    VendorName: string;
    BillNo: string;
    OrderNo: number;
    BillDate: Date;
    DueDate: Date;   
    Total: number;   
    DiscountAmount: number = 0;
    Discount: number;
    SaveStatus: number;
    action: string;
    IsItemLevelDiscount: boolean = false;   
    pur_invoice_dt: Array<pur_invoice_dt> = [];
}
export class pur_invoice_dt {
    ID: number;
    CompanyID: number;
    InvoiceID: number;
    ItemID: number;   
    ItemDescription: string;
    Quantity: number;
    Rate: number;
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