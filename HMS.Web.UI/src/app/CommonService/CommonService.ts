import { Injectable } from '@angular/core';
import { HttpService } from './HttpService';
import { EncryptionService } from './encryption.service';
import { GlobalVariable, CultureInfo, LanguageInfo, UserScreens } from '../AngularConfig/global';
import { Observable } from 'rxjs';
import { FormGroup, FormControl, FormBuilder, Validators, ValidatorFn } from '@angular/forms';

interface MultilingualModel {
    CultureID: number,
    Key: string,
    Value: string
}


@Injectable()
export class CommonService {

    private objCultureInfo: any[];
    public MultilingualModelList: MultilingualModel[] = [];
    public objModelList: any[] = [];
    public objExistingCultureIDList: any[] = [];
    private BASE_Api_URL = GlobalVariable.BASE_Api_URL;

    constructor(private http: HttpService, private encrypt: EncryptionService) {}

    GetKeywords(FormName: string): string[] {
        var KeyArray: string[] = [];
        var KeyWords: any[] = [];
        KeyWords = JSON.parse(localStorage.getItem('MultiKeyword'));
        KeyWords.filter(function(item) { return item.Form == "Common" || item.Form == FormName }).map(function(item) {
            KeyArray[item.Keyword] = item.Value;
        });

        return KeyArray;
    }
    ScreenRights(ScreenName: string): any {
        var Rights: string[] = [];
        var ScreenRights: any[] = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('ControlLevelRights')));
        if (ScreenRights.filter(function(item) { return item.ScreenID == ScreenName }).length > 0) {
            var ActionList: any[] = [];
            var Result = ScreenRights.filter(function(item) { return item.ScreenID == ScreenName });
            Rights = Result[0];
        }
        return Rights;
    }
    MinNumberValidator(control: FormControl) {

        let NUMBER = control.value;
        if (NUMBER && NUMBER < 0) {
            return {
                MinNumber: {
                    parsedDomain: NUMBER
                }
            }
        }
        return null;
    }

    MaxNumberValidator(control: FormControl) {

        let NUMBER = control.value;
        if (NUMBER && NUMBER > 999999999999999) {
            return {
                MaxNumber: {
                    parsedDomain: NUMBER
                }
            }
        }
        return null;
    }

    NumberByDigitsValidator(maxLength: number): ValidatorFn {
        return (control: FormControl): { [key: string]: any } => {
            var v: number = control.value;
            return v > maxLength ?
                { "NumberByDigits": { "requiredLength": maxLength, "actualLength": v } } :
                null;
        };
    }

    MultilingualModelInit(List: any[] = [], IsNewRecord: boolean = true): any {

        this.MultilingualModelDeleteValue("");

        if (IsNewRecord)
            return null;

        var CultureID = parseInt(GlobalVariable.CultureID);
        this.objModelList = List.filter(function(item) {
            return parseInt(item["CultureID"]) != CultureID;
        });

        this.objExistingCultureIDList = List.map(function(item) {
            return { 'CultureID': parseInt(item["CultureID"]) };
        });

        var Model = List.filter(function(item) {
            return parseInt(item["CultureID"]) == CultureID;
        });

        return Object.assign({}, Model[0]);
    }

    MultilingualModelGetValue(CultureID: number, Key: string, OldValue: string): string {


        var Result: any[] = [];

        Result = this.MultilingualModelList.filter(function(item) {
            return item.CultureID == CultureID && item.Key == Key;
        });

        if (Result.length > 0)
            return Result[0]["Value"];

        Result = this.objModelList.filter(function(item) { return item.CultureID == CultureID });

        if (Result.length > 0)
            return Result[0][Key];
        else
            return OldValue + this.MakeUniqueValue(CultureID);
    }

    MultilingualModelSetValue(Key: string, NewValue: string): void {

        for (let obj of this.objModelList)
            obj[Key] = NewValue;
    }

    MultilingualModelDeleteValue(Screen: string): void {
        //this.MultilingualModelList = this.MultilingualModelList.filter(function (item) { item.Screen != Screen });
        this.MultilingualModelList = [];
        this.objModelList = [];
        this.objExistingCultureIDList = [];
    }

    MultilingualModelAddRow<T>(List: T[]): void {
        List.push();
    }

    MultilingualModelDeleteRow<T>(List: T[], Index: number): void {
        List.splice(Index, 1);
    }

    MultilingualModelPapulate(Model: any, IsNewRecord: boolean): any[] {

        var ObjTemp: {};
        var List: any[] = [];
        var CultureID = 0;
        this.objCultureInfo = Object.assign([], CultureInfo.Data);

        if (IsNewRecord) {
            for (var i = 0; i < this.objCultureInfo.length; i++) {
                CultureID = this.objCultureInfo[i].CultureID;

                ObjTemp = Object.assign({}, Model);
                ObjTemp["CultureID"] = CultureID;

                List.push(Object.assign({}, ObjTemp));
            }
        }
        else {
            List.push(Object.assign({}, Model));
            for (let mod of this.objModelList)
                List.push(Object.assign({}, mod));
        }

        return List;
    }

    GetPropertyValue(Model: any, Name: string) {

        var arr = Name.split(".");

        while (arr.length && Model) {
            var comp = arr.shift();
            var match = new RegExp("(.+)\\[([0-9]*)\\]").exec(comp);
            if ((match !== null) && (match.length == 3)) {
                var arrayData = { arrName: match[1], arrIndex: match[2] };
                if (Model[arrayData.arrName] != undefined) {
                    Model = Model[arrayData.arrName][arrayData.arrIndex];
                } else {
                    Model = undefined;
                }
            } else {
                Model = Model[comp]
            }
        }

        var sss = Model;
        return Model;
    }

    SetPropertyValue(Model: any, Name: string, Value: any) {

        var arr = Name.split(".");

        while (arr.length && Model) {
            var comp = arr.shift();
            var match = new RegExp("(.+)\\[([0-9]*)\\]").exec(comp);
            if ((match !== null) && (match.length == 3)) {
                var arrayData = { arrName: match[1], arrIndex: match[2] };
                if (Model[arrayData.arrName] != undefined) {
                    Model = Model[arrayData.arrName][arrayData.arrIndex];
                } else {
                    Model = undefined;
                }
            } else {
                Model[comp] = Value;
                break;
            }
        }
    }

    private MakeUniqueValue(CultureID: number): string {

        if (CultureID == 1)
            CultureID = parseInt(GlobalVariable.CultureID)

        var Values = "";
        for (var i = 1; i < CultureID; i++)
            Values += "`";
        return Values;
    }

    GetScreenRights(ScreenName: string): any {

        var Rights = {};
        Rights['ActionList'] = [];
        Rights['Allow'] = function(Name: string) { return Rights['ActionList'].indexOf(Name) != -1 };

        var ScreenRights: any[] = JSON.parse(localStorage.getItem('ScreenRights'));

        if (ScreenRights.filter(function(item) { return item.ScreenAddress == ScreenName }).length > 0) {

            var ActionList: any[] = [];
            var Result = ScreenRights.filter(function(item) { return item.ScreenAddress == ScreenName });
            ActionList = Result[0].ActionList;
            Rights['ActionList'] = ActionList.map(function(item) { return item.ActionName });
            Rights['ScreenName'] = Result[0].ScreenName;
            Rights['ScreenID'] = Result[0].ScreenID;
        }

        return Rights;
    }

    GetGlobalVariable(): any {
        return JSON.parse(localStorage.getItem('GlobalVariable'));
    }

    Tokenvalidate(): boolean {

        var CurrentTime = new Date();

        var ValidTo = new Date(localStorage.getItem('ValidTo')).getTime();
        var UTCCurrentTime = new Date(CurrentTime.getUTCFullYear(), CurrentTime.getUTCMonth(),
            CurrentTime.getUTCDate(), CurrentTime.getUTCHours(), CurrentTime.getUTCMinutes(),
            CurrentTime.getUTCSeconds(), CurrentTime.getUTCMilliseconds()).getTime();

        if (ValidTo > UTCCurrentTime)
            return true;
        else
            return false;
    }
    OnlychangeLanguage(Id: string): Promise<any> {
        var data = { 'MultilingualId': Id };
        return this.http.Get(this.BASE_Api_URL + '/api/User/OnlychangeLanguage', data).then(m => {

            localStorage.setItem('MultiKeyword', JSON.stringify(m.ResultSet));
            return m;
        })
    }
    GetProductList(Keyword: string, ProductMfIds: string): Promise<any> {

        var data = { 'Keyword': Keyword, 'ProductMfIds': ProductMfIds };
        return this.http.Get(this.BASE_Api_URL + '/api/Common/Product/ProductList', data);
    }

    GetPaginationWithPageList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SearchText: string): Promise<any> {

        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SearchText': SearchText };
        return this.http.Get(this.BASE_Api_URL + '/api/Common/Product/PaginationWithPageList', data).then(e => e);
    }

    LoadDropdown(Ids: string): Promise<any> {
        var data = { 'DropdownIds': Ids };
        return this.http.Get(this.BASE_Api_URL + '/api/admin/sys_drop_down/LoadDropdown', data).then(e => e);
    }
    changeLanguage(Id: string): Promise<any> {
        var data = { 'MultilingualId': Id };
        return this.http.Get(this.BASE_Api_URL + '/api/admin/sys_drop_down/changeLanguage', data).then(e => e);
    }
    changePackage(Id: string): Promise<any> {
        var data = { 'PackageId': Id };
        return this.http.Get(this.BASE_Api_URL + '/api/admin/sys_drop_down/changePackage', data).then(e => e);
    }

    LoanLoadDropdown(): Promise<any> {
        var data = {};
        return this.http.Get(this.BASE_Api_URL + '/api/admin/sys_drop_down/LoanLoadDropdown', data).then(e => e);
    }

    getCurrency(): string {
        var currency = this.encrypt.decryptionAES(localStorage.getItem("Currency"));       
        return currency;
    }
    getPayrollRegion(): string {
        var region = this.encrypt.decryptionAES(localStorage.getItem("PayrollRegion"));
        return region;
    }
    IsPayrollRegion(): Promise<any> {
        var data = {};
        return this.http.Get(this.BASE_Api_URL + '/api/User/GetPayrollRegion', {}).then(m => {
            localStorage.setItem('PayrollRegion', m.ResultSet.PayrollRegion);
            return m;
        });
    }
    GetThousandCommaSepFormatDate(Numb): number {
        return Numb.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    };

    GetFormatDate(dat) {
        var dtFormat = this.encrypt.decryptionAES(localStorage.getItem("DateFormat"));
       var dateFormat = dtFormat.split("/");
        var date = new Date(dat);
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getMonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return yyyy + '-' + mm + '-' + dd;
        //let format;
        //if (dateFormat[0] == "DD")
        //    format = dd;
        //if (dateFormat[0] == "MM")
        //    format = dd;
        //if (dateFormat[0] == "DD")
        //    format = dd;
        //return dateFormat[0] + '-' + dateFormat[1] + '-' + dateFormat[2];
    };

    GetFormatDatePk(dat) {
        var date = new Date(dat);
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getMonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        return dd + '/' + mm + '/' + yyyy;
    };
}
