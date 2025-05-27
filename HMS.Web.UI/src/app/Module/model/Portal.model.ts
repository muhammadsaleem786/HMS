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
}