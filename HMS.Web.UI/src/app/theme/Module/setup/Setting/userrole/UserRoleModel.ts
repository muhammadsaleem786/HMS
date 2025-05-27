export class UserRoleModel {
    constructor() {
        let obj = new AdmRoleDt();
        this.adm_role_dt.push(obj);
    }

    ID: number;
    CompanyID: number;
    RoleName: string;
    Description: string;
    NoOfEmployees: number;
    DependedDropDownValueID: number;
    ActiveModule: boolean;
    ActiveScreen: boolean;

    adm_role_dt: Array<AdmRoleDt> = [];
}

export class AdmRoleDt {
    ID: number = 0;
    CompanyID: number = 0;
    RoleID: number = 0;
    ScreenID: number = 0;
    DropDownScreenID: number = 0;
    ViewRights: boolean = false;
    CreateRights: boolean = false;
    DeleteRights: boolean = false;
    EditRights: boolean = false;
    Active: boolean = true;
}
