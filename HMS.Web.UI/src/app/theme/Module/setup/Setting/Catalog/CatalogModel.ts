export class lab_catalog {
    ID: number = 0;
    CompanyId: number;
    Name: string ;
    CategoryId: number;
    SampleId: number;
    SampleName: string;
    TAT: string;
    Price: number;
    CategoryName: string;
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