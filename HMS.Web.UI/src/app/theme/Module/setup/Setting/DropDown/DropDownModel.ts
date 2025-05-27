export class sys_drop_down_value {
    ID: number = 0;
    Value: string;
    DropDownID: number ;
    DependedDropDownValueID: number;
    DependedDropDownID : number;
    SystemGenerated: boolean;
    Unit: string;
}

//export class sys_drop_down_mf {
//    ID: number;
//    Name: string;
//    constructor() {
//        let objsys_drop_down_Value = new sys_drop_down_value();
//        this.sys_drop_down_value.push(objsys_drop_down_Value);
//    }
//    sys_drop_down_value: Array<sys_drop_down_value> = [];
//}
