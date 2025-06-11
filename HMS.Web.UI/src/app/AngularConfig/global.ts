import { AbstractControl } from '@angular/forms';
export const GlobalVariable = {
    //BASE_Api_URL: "https://localhost:44371",
    //BASE_Web_URL: "http://localhost:4200",
    //BASE_File_URL: "https://localhost:44371/files/attachmentfiles/",
    //BASE_Temp_File_URL: "https://localhost:44371/files/temp/",
    //Live
    BASE_Api_URL: "http://40.172.150.18/api",
    BASE_Web_URL: "http://localhost:4200",
    BASE_File_URL: "http://40.172.150.18/api/files/attachmentfiles/",
    BASE_Temp_File_URL: "http://40.172.150.18/api/files/temp/",

    IsUseS3: "No",    // Yes or No for S3 Storage
    Release_Version: "6.0",
    AuthenticationToken: "",
    UserID: 0,
    UserName: "",
    CultureID: "",
    CompanyCode: "",
    Cross_Domain_Options: {
        origin: "",
        path: "/CrossDomain/crossd_iframe.html"
    },
    DDL_First_Item: "-- Select --",
};
export const CultureInfo = {
    PopUPTitle: "Arabic language",
    PopUPLanguage: "Arabic",
    Data: Array<any>()
};
export const ValidationVariables = {
    EmailPattern: "^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
    PhoneNo: "^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$",
    NumberPattern: "^[0-9+ ]+$",
    CNICPattern:"^[0-9+]{5}-[0-9+]{7}-[0-9]{1}$",
    AlphabetPattern: "^[\u0600-\u065F\u066A-\u06EF\u06FA-\u06FFa-zA-Z ]{2,20}$",
    AlphaNumeric: "^[A-Za-z0-9 _]*[A-Za-z0-9][A-Za-z0-9 _]*[A-Za-z0-9- \s!@#$%^&*()_+=-`~\\\]\[{}|';:/.,?><]*$",
    //website url like www.google.com (OR) https://www.google.com
    UrlPattern: /^((ftp|http|https):\/\/)?(www.)?(?!.*(ftp|http|https|www.))[a-zA-Z0-9_-]+(\.[a-zA-Z]+)+((\/)[\w#]+)*(\/\w+\?[a-zA-Z0-9_]+=\w+(&[a-zA-Z0-9_]+=\w+)*)?$/gm,
    NegativeNumber: "^[-]?\d*$",
};
export function OnlyAlphabets(value: string) {
    return (control: AbstractControl): { [key: string]: any } | null => {
        const val: string = control.value;
        if (val === '' || val === '^[a-zA-Z \-\']$') {
            return null;
        } else {
            return { 'OnlyAlphabets': true };
        }
    };
}
export const UserScreens = {
    Data: Array<any>()
};
export const LanguageInfo = {
    KeyWords: Array<any>()
};
export const HiddenVariable = {
    hide: false
};
export const HMSScreen: Array<{ id: number, text: string }> = [
    { id: 1, text: "/home/account" },
    { id: 2, text: "/home/com" },
    { id: 3, text: "/home/Currency" },
    { id: 4, text: "/home/Tax" },
    { id: 5, text: "/home/userole" },
    { id: 6, text: "/home/vacationsickleave" },
    { id: 7, text: "/home/masterdata" },
    { id: 8, text: "/home/Journal" },
    { id: 10, text: "/home/budget" },
    { id: 6, text: "/home/userrole/aplicationUser" },
];