export class emr_service_mf {
    constructor() {
        let objemr_service_item = new emr_service_item();
        this.emr_service_item.push(objemr_service_item);
    }
    ID: number;
    ServiceName: string;
    Price: number;
    IsItem: boolean;
    RefCode: string;
    SpecialityId: number;
    emr_service_item: Array<emr_service_item> = [];
}
export class emr_service_item {
    ID: number;
    ServiceId: number;
    ItemId: number;
    ItemName: string;
    Quantity: number;
}


