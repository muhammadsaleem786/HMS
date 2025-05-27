export class EmployeeModel {
    constructor() {
        let objLeave = new pr_employee_leave();
        this.pr_employee_leave.push(objLeave);

        let objAllowance = new pr_employee_allowance();
        this.pr_employee_allowance.push(objAllowance);

        let objDedCont = new pr_employee_ded_contribution();
        this.pr_employee_ded_contribution.push(objDedCont);

        //let objDocument = new pr_employee_document();
        //this.pr_employee_document.push(objDocument);
        this.pr_employee_document = [];
        //this.pr_employee_air_ticket = [];
        this.pr_employee_Dependent = [];
    }

    ID: number;
    CompanyID: number;
    FirstName: string;
    LastName: string;
    Gender: string = 'M';
    DOB: Date;
    StreetAddress: string;
    CityDropDownID: number;
    CityID: number;
    ZipCode: string;
    CountryDropDownID: number;
    CountryID: number;
    Email: string;
    HomePhone: string;
    Mobile: string;
    EmergencyContactPerson: string;
    EmergencyContactNo: string;
    HireDate: Date;
    JoiningDate: Date;
    PayTypeDropDownID: number;
    PayTypeID: number;
    BasicSalary: number;
    StatusDropDownID: number;
    StatusID: number;
    TerminatedDate: Date = new Date();
    FinalSettlementDate: Date = new Date();
    PaymentMethodDropDownID: number;
    PaymentMethodID: number;
    BankName: string;
    BranchName: string;
    BranchCode: string;
    AccountNo: string;
    SwiftCode: string;
    EmployeeTypeDropDownID: number;
    EmployeeTypeID: number;
    TypeStartDate: Date = new Date();
    TypeEndDate: Date = new Date();

    LocationID: number;
    DesignationID: number;
    DepartmentID: number;
    NICNoExpiryDate: Date;
    NICNo: string;
    NationalSecurityNo: string;
    NationalityDropDownID: number;
    NationalityID: number;
    EmployeeCode: string;
    PayScheduleID: number;
    EmployeePic: string;
    CreatedBy: number;
    CreatedDate: Date;
    ModifiedBy: number;
    ModifiedDate: Date;
    SubContractCompanyName: string = 'a';
    PassportNumber: string;
    PassportExpiryDate: Date;
    SCHSNO: string;
    SCHSNOExpiryDate: Date;
    MedicalInsuranceProvided: string = 'N';
    InsuranceCardNo: string;
    InsuranceCardNoExpiryDate: Date = null;
    InsuranceClassTypeDropdownID: number;
    InsuranceClassTypeID: number;
    AirTicketProvided: string = 'N';
    NoOfAirTicket: number;
    AirTicketClassTypeDropdownID: number;
    AirTicketClassTypeID: number;
    AirTicketFrequencyTypeDropdownID: number;
    AirTicketFrequencyTypeID: number;
    OriginCountryDropdownTypeID: number;
    OriginCountryTypeID: number;
    DestinationCountryDropdownTypeID: number;
    DestinationCountryTypeID: number;
    OriginCityDropdownTypeID: number;
    OriginCityTypeID: number;
    DestinationCityDropdownTypeID: number;
    DestinationCityTypeID: number;
    AirTicketRemarks: string;
    MaritalStatusTypeDropdownID: number;
    MaritalStatusTypeID: number;
    SpecialtyTypeDropdownID: number;
    SpecialtyTypeID: number;
    ClassificationTypeDropdownID: number;
    ClassificationTypeID: number;
    ContractTypeDropDownID: number;
    ContractTypeID: number;
    TotalPolicyAmountMonthly: number;
    //ID: number;
    //CompanyID: number;
    //FirstName: string;
    //LastName: string;
    //Gender: string = "M";
    //DOB: Date;
    //StreetAddress: string;
    //CityID: number;
    //ZipCode: string;
    //CountryID: number;
    //Email: string;
    //HomePhone: string;
    //Mobile: string;
    //EmergencyContactPerson: string;
    //EmergencyContactNo: string;
    //HireDate: Date;
    //JoiningDate: Date;
    //PayTypeID: number;
    //PayScheduleID: number;
    //BasicSalary: number;
    //StatusID: number;
    //PaymentMethodID: number;
    //BankName: string;
    //BranchName: string;
    //BranchCode: string;
    //AccountNo: string;
    //SwiftCode: string;
    //EmployeeTypeID: number;
    //TypeStartDate: Date = new Date();
    //TypeEndDate: Date = new Date();
    //TerminatedDate: Date = new Date();
    //FinalSettlementDate: Date = new Date();
    //LocationID: number;
    //DesignationID: number;
    //DepartmentID: number;
    //NICNo: string;
    //NationalSecurityNo: string;
    //NationalityID: number;
    //EmployeeCode: string;
    //EmployeePic: string;
    //CityDropDownID: number;
    //CountryDropDownID: number;
    //PayTypeDropDownID: number;
    //StatusDropDownID: number;
    //PaymentMethodDropDownID: number;
    //EmployeeTypeDropDownID: number;
    //NationalityDropDownID: number;

    //NoOfAirTicket: number;
    //AirTicketFrequencyTypeDropdownID: number;
    //AirTicketFrequencyTypeID: number;
    //OriginCountryDropdownTypeID: number;
    //OriginCountryTypeID: number;
    //DestinationCountryDropdownTypeID: number;
    //DestinationCountryTypeID: number;
    //OriginCityDropdownTypeID: number;
    //OriginCityTypeID: number;
    //DestinationCityDropdownTypeID: number;
    //DestinationCityTypeID: number;
    //AirTicketRemarks: string;


    adm_user_company: Array<adm_user_company> = [];
    //adm_company_location: Array<Adm_Company_Location> = [];
    //pr_department: pr_department = new pr_department();
    //pr_designation: pr_designation = new pr_designation();
    pr_employee_allowance: Array<pr_employee_allowance> = [];
    pr_employee_ded_contribution: Array<pr_employee_ded_contribution> = [];
    pr_employee_leave: Array<pr_employee_leave> = [];
    pr_employee_document: Array<pr_employee_document> = [];
    pr_employee_Dependent: Array<pr_employee_Dependent> = [];
}

export class adm_user_company {
    ID: number;
    UserID: number;
    CompanyID: number;
    EmployeeID: number;
    RoleID: number;
    AdminID: number;
    IsDefault: boolean;
}


export class Salary {
    BasicSalary: number = 0;
    GrossSalary: number = 0;
    TaxPerPeriod: number = 0;
    NetSalary: number = 0;
    AllowanceTotal: number = 0;
    TaxableAllowanceTotal: number = 0;
    ContributionTotal: number = 0;
    DeductionTotal: number = 0;
    TaxableDeductionTotal: number = 0;

    CalculateSalary() {
        debugger
        //this.GrossSalary = this.BasicSalary + this.TaxableAllowanceTotal + this.TaxableDeductionTotal- this.DeductionTotal;

        this.GrossSalary = this.BasicSalary + this.AllowanceTotal;
        //if (this.TaxableAllowanceTotal > 0) {
        //    this.GrossSalary += this.TaxableAllowanceTotal;
        //}
        //if (this.TaxableDeductionTotal > 0) {
        //    this.GrossSalary += this.TaxableDeductionTotal;
        //}

        this.NetSalary = this.BasicSalary + this.AllowanceTotal - this.DeductionTotal;
        this.TaxPerPeriod = 0;


        //this.NetSalary = this.BasicSalary + this.AllowanceTotal - this.DeductionTotal;
        //this.TaxPerPeriod = 0;
    }
}

export class pr_employee_leave {
    ID: number;
    CompanyID: number;
    EmployeeID: number;
    LeaveTypeID: number;
    Hours: number;
}

//export class Payschedule {
//    ID: number;
//    CompanyID: number;
//    PayTypeID: number;
//    ScheduleName: string;
//    PeriodStartDate: Date;
//    PeriodEndDate: Date;
//    FallInHolidayID: number;
//    PayDate: Date;
//    Active: boolean;
//    PopUpVisible: boolean = false;
//}


export class pr_employee_allowance {
    ID: number;
    CompanyID: number;
    EmployeeID: number
    EffectiveFrom: Date;
    EffectiveTo: Date;
    PayScheduleID: number;
    AllowanceID: number;
    Percentage: number;
    Amount: number;
    Taxable: boolean;
    IsHouseOrTransAllow: boolean = true;
}

export class pr_employee_ded_contribution {
    ID: number;
    CompanyID: number;
    EmployeeID: number
    EffectiveFrom: Date;
    EffectiveTo: Date;
    PayScheduleID: number;
    Category: string;
    DeductionContributionID: number;
    Percentage: number;
    Amount: number;
    StartingBalance: number = 0;
    Taxable: boolean;

}

export class EmpBulkUpdateModel {
    BulkUpdateCategoryType: string = 'A';
    AllConDedId: number = 0;
    AllConDedValueType: string;
    Amount: number = 0;
    Taxable: boolean = false;
    Name: string = '';
    AllConDedTypes: any[] = [];
    EmployeeIds: number[] = [];
    LocationsIds: number[] = [];
    DepartmentsIds: number[] = [];
    DesignationsIds: number[] = [];
    EmployeeTypeIds: number[] = [];
}
export class BulkEmpModel {
    ID: number;
    Employee: string;
    ExistingAmount: number;
    Amount: number;
    Category: string;
    Taxable: boolean;
    Remove: boolean;
    UseExistingAmount: boolean;
    BasicSalary: number;
}

export class BulkEmpSumAmountModel {
    TotalExistingAmount: number = 0;
    TotalAmount: number = 0;
}

export class pr_employee_document {
    ID: number;
    CompanyID: number;
    EmployeeID: number;
    DocumentTypeID: number;
    DocumentTypeDropdownID: number;
    Description: string;
    AttachmentPath: string = '';
    UploadDate: Date;
    ExpireDate: Date;
}

export class ImportEmpDataModel {
    FileName: string = 'SheetJS.xlsx';
    //FileData: Array<EmployeeImportModel> = new Array<EmployeeImportModel>();
    SavedFileName: string = '';
}

export class EmployeeImportModel {
    ErrorDescription: string;
    FirstName: string;
    LastName: string;
    Gender: string;
    DOB: Date;
    CNIC: string;
    EmployeeAddress: string;
    City: string;
    Country: string;
    PostalCode: string;
    HomePhone: string;
    WorkPhone: string;
    EmergencyContact: string;
    EmergencyPhone: string;
    Email: string;
    EmployeeCode: string;
    Department: string;
    Designation: string;
    Location: string;
    HireDate: Date;
    JoiningDate: Date;
    Salary: number;
    SalaryPaymentMethod: string;
    BankName: string;
    BranchName: string;
    BranchCode: string;
    AccountTitle: string;
    AccountNumber: string;
    IsSolutionToErrorManadory: boolean = true;
}

export class ImportRecordStatusModel {
    TotalRecords: number = 0;
    NewRecords: number = 0;
    DuplicateRecords: number = 0;
    Errors: number = 0;
}

export class CheckImportExistModel {
    FirstName: string;
    LastName: string;
    Email: string;
}

export class pr_employee_Dependent {
    ID: number;
    CompanyID: number;
    EmployeeID: number;
    RelationshipTypeID: number;
    RelationshipDropdownID: number;
    IsEmergencyContact: boolean = false;
    IsTicketEligible: boolean = false;
    FirstName: string = '';
    LastName: string = '';
    Gender: string = 'M';
    NationalityTypeDropdownID: number;
    NationalityTypeID: number;
    IdentificationNumber: string = '';
    PassportNumber: string = '';
    MaritalStatusTypeDropdownID: number;
    MaritalStatusTypeID: number;
    DOB: Date = undefined;
    Remarks: string = '';
    
}

//export class pr_employee_air_ticket {
//    ID: number;
//    CompanyID: number;	
//    EmployeeID: number;
//    NoOfAirTicket: number;
//    AirTicketFrequencyTypeDropdownID: number;
//    AirTicketFrequencyTypeID: number;
//    OriginCountryDropdownTypeID: number;
//    OriginCountryTypeID: number;
//    DestinationCountryDropdownTypeID: number;
//    DestinationCountryTypeID: number;
//    OriginCityDropdownTypeID: number;
//    OriginCityTypeID: number;
//    DestinationCityDropdownTypeID: number;
//    DestinationCityTypeID: number;
//    Remarks: string;
//}