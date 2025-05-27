export class AllowanceModel {
    ID: number;
    CompanyID: number;
    CategoryID: number;
    AllowanceName: string = 'a';
    AllowanceType: string = "F";
    AllowanceValue: number;
    Taxable: boolean = true;
    Default: boolean;
    SystemGenerated: boolean;
    AllowancePercentageValue?: number;
}