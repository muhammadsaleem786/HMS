export class SaleModel {
    constructor() {
        let objpur_sale_dt = new pur_sale_dt();
        this.pur_sale_dt.push(objpur_sale_dt);
    }
    ID: number;
    CompanyID: number;
    CustomerId: number;
    SaleTypeID: number;
    Date: Date;  
    SubTotal: number;
    DiscountType: number = 1;
    Discount: number=0;
    ItemId: string;
    DiscountAmount: number;
    SaleHoldId: number=0;
    IsItemLevelDiscount: number;
    TaxAmount: number;
    Total: number;    
    ItemName: string;
    pur_sale_dt: Array<pur_sale_dt> = [];
}
export class pur_sale_dt {
    ID: number;
    CompanyID: number;
    SaleID: number;
    ItemID: number;
    ItemName: string;
    ItemDescription: string;
    Quantity: number;
    Rate: number;
    DiscountType: number = 1;
    Discount: number;
    DiscountAmount: number = 0; 
    TotalAmount: number;
    ItemTypeId: number;
    TypeValue: string;
    CategoryId: number;
    Stock: number;
    BatchSarialNumber: number;
    ExpiredWarrantyDate: Date;
    GroupId: number;
}

export class SaleHoldModel {
    constructor() {
        let objpur_sale_hold_dt = new pur_sale_hold_dt();
        this.pur_sale_hold_dt.push(objpur_sale_hold_dt);
    }
    ID: number;
    CompanyID: number;
    CustomerId: number;
    SaleTypeID: number;
    Date: Date;
    SubTotal: number;
    DiscountType: number = 1;
    Discount: number = 0;
    ItemId: string;
    DiscountAmount: number;
    IsItemLevelDiscount: number;
    TaxAmount: number;
    Total: number;
    SaleHoldId: number = 0;
    ItemName: string;
    pur_sale_hold_dt: Array<pur_sale_hold_dt> = [];
}
export class pur_sale_hold_dt {
    ID: number;
    CompanyID: number;
    SaleID: number;
    ItemID: number;
    ItemName: string;
    ItemDescription: string;
    Quantity: number;
    Rate: number;
    DiscountType: number = 1;
    Discount: number;
    DiscountAmount: number = 0;
    TotalAmount: number;
    ItemTypeId: number;
    TypeValue: string;
    CategoryId: number;
    Stock: number;
    BatchSarialNumber: number;
    ExpiredWarrantyDate: Date;
    GroupId: number;
}