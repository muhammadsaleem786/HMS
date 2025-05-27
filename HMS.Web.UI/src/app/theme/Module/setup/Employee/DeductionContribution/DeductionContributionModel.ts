export class DeductionContributionModel {
    ID: number;
    CompanyID: number;
    Category: string = "C";
    DeductionContributionName: string;
    DeductionContributionType: string = "F";
    DeductionContributionValue: number = 0;
    Taxable: boolean;
    Default: boolean;
    StartingBalance: boolean;
    SystemGenerated: boolean = false;
    DeductionContributionPercentageValue?: number;
}