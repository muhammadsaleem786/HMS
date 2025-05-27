export class ApplicationUserModel {
    constructor() {
        let obj = new adm_user_company();
        this.adm_user_company.push(obj);

        let objuser_payment = new user_payment();
        this.user_payment2.push(objuser_payment);
    }
    ID: number;
    Name: string;
    Email: string;
    EmployeeID: number;
    PhoneNo: string;
    Pwd: string;
    MultilingualId: number;
    SlotTime: any;
    AppStartTime: string;
    AppEndTime: string;
    OffDay: string;
    IsOverLap: boolean;
    IsShowDoctor: string;
    Qualification: string;
    Designation: string;    
    DesignationUrdu: string;
    QualificationUrdu: string;
    HeaderDescription: string;
    NameUrdu: string;
    IsActivated: boolean = false;
    Type: string;
    SpecialtyId: number;
    ExpiryDate: Date;
    SpecialtyDropdownId: number;
    RepotFooter: string;
    TemplateId: number=1;
    UserImage: string;
    GenderDropdown: number[] = [];
    DoctorIds: number[] = [];
    DocWorkingDay: number[] = [];
    adm_user_company: Array<adm_user_company> = [];
    user_payment2: Array<user_payment> = [];
}
export class user_payment {
    ID: number;
    UserId: number;
    CompanyId: number;
    Amount: number;
    Remarks: string;
    Date: Date;
}
export class adm_user_company {
    ID: number = 0;
    UserID: number = 0;
    CompanyID: number = 0;
    RoleID: number;
    RoleName: number;
    EmployeeID: number = undefined;
    AdminID: number = 0;
    IsDefault: boolean = false;

}
export class EmployeeImportModel {
    ErrorDescription: string;
    FirstName: string;
    LastName: string;
    Gender: string;
    DOB: string = "";
    CNIC: string;
    EmployeeAddress: string;
    City: string;
    Country: string;
    PostalCode: string;
    HomePhone: string;
    Fax: string;
    EmergencyContact: string;
    EmergencyPhone: string;
    Email: string;
    EmployeeCode: string;
    Department: string;
    Designation: string;
    Location: string;
    ShiftName: string;
    HireDate: string = "";
    JoiningDate: string = "";
    Salary: number;
    SalaryPaymentMethod: string;
    BankName: string;
    BranchName: string;
    BranchCode: string;
    AccountTitle: string;
    AccountNumber: string;
    IsSolutionToErrorManadory: boolean = true;

    TaxID: number;
    LoanID: number;
}
export class ImportRecordStatusModel {
    TotalRecords: number = 0;
    NewRecords: number = 0;
    DuplicateRecords: number = 0;
    Errors: number = 0;
}
export class ImportUserDataModel {
    FileName: string = 'SheetJS.xlsx';
    //FileData: Array<EmployeeImportModel> = new Array<EmployeeImportModel>();
    SavedFileName: string = '';
}
export class CheckImportExistModel {
    EmployeeID: number;
    RoleName: string;
    Password: string;
    Email: string;
}
export class UserImportModel {
    ErrorDescription: string;
    EmployeeID: number;
    RoleName: string;
    Password: string;
    Email: string;
}


