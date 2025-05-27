export class User {
    ID: number;
    Email: string;
    Pwd: string;
    CPassword: string;
    UserName: string;
    MultilingualId: number;
    Name: string;
    PhoneNo: string;
    AccountType: string = "W";
    ForgotToken: string;
    CurrentPassword: string;
    agree: boolean = false;
    UserImage: string;
    //Company
    CompanyName: string;
    CompanyTypeID: number;
    CompanyAddress1: string;
    CompanyAddress2: string;
    CompanyToken: string;
    CityDropDownId: number;
    Province: string;
    PostalCode: string;
    Phone: string;
    Fax: string;
    Website: string;
    LanguageID: number;
    GenderID: number;
    ContactPersonFirstName: string;
    ContactPersonLastName: string;
    CompanyLogo: string;
    IsShowBillReceptionist: boolean = false;
    IsApproved: boolean = false;
}
