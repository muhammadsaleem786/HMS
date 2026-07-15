export class emr_investigation {
    constructor() {
        let objemr_investigation_Item = new emr_investigation_Item();
        this.emr_investigation_Item.push(objemr_investigation_Item);
    }
    ID: number = 0;
    Investigation: string;
    CategoryId: number;
    SampleId: number;
    SampleName: string;
    TAT: string;
    Price: number;
    CategoryName: string;
    IsItem: boolean;
    emr_investigation_Item: Array<emr_investigation_Item> = [];
}
export class emr_investigation_Item {
    ID: number;
    InvestigationId: number;
    ItemId: number;
    ItemName: string;
    Quantity: number;
}
export class lab_parameter {
    ID: number = 0;
    CompanyId: number;
    ParameterName: string;
    UnitId: number;
    ReferenceRange: string;
    NormalMinimum: string;
    NormalMaximum: string;
    CriticalHigh: number;
    CriticalLow: number;
    Description: string;
    UnitName: string;
    CatalogId: number;
}





