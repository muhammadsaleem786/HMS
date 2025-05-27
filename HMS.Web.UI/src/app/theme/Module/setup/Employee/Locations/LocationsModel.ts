export class LocationsModel {
    ID: number;
    CompanyID: number;
    LocationName: string;
    Address: string;
    CountryID: number;
    CityID: number;
    ZipCode: string;
    Employees?: number = 0;
}