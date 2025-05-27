export class ItemModel {
    ID: number;
    CompanyID: number;
    ItemTypeDropDownID: number;
    ItemTypeId: number;
    Name: string;    
    SKU: string;
    Image: string;
    UnitDropDownID: number;
    UnitID: number;
    CategoryDropDownID: number;
    InstructionId: number;
    CategoryID: number;   
    TrackInventory: boolean = false;
    IsActive: boolean = true;
    CostPrice: number;
    SalePrice: number;
    InventoryOpeningStock: number;
    InventoryStockPerUnit: number;
    InventoryStockQuantity: number;
    SaveStatus: number;
    GroupId: number;
    POSItem: boolean = false;
    action: string;
}

export class ImpoertItemModel {
    Group: string;
    GroupId: number;
    Type: string;
    ItemTypeId: number;
    Name: string;
    SKU: string;
    Unit: string;
    UnitID: number;
    Category: string;
    CategoryID: number;   
    CostPrice: number;
    SalePrice: number;
    Status: boolean;
    IsActive: boolean;
    TrackInventory: boolean;
    InventoryOpeningStock: number;
    InventoryStockPerUnit: number;
    InventoryStockQuantity: number;
    POSItem: boolean ;
    ErrorDescription: string;

}
export class CheckImportExistModel {
    FirstName: string;
    LastName: string;
    Email: string;
}
export class ImportRecordStatusModel {
    TotalRecords: number = 0;
    NewRecords: number = 0;
    DuplicateRecords: number = 0;
    Errors: number = 0;
}
export class ImportEmpDataModel {
    FileName: string = 'SheetJS.xlsx';
    SavedFileName: string = '';
}