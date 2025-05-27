import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpService } from '../../CommonService/HttpService';
import { GlobalVariable } from '../../AngularConfig/global';
import { User } from '../model/User.model';
import { Company } from '../../theme/Module/models/company.model';
import { SetupPass } from '../../theme/Module/models/setup-pass.model';
import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { CommonService } from '../../CommonService/CommonService';
import { CommonToastrService } from '../../CommonService/CommonToastrService';
import { EncryptionService } from '../../CommonService/encryption.service';
import { debounce } from 'rxjs/operators';
@Injectable()
export class AccountService {

    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/User"

    constructor(private http: HttpService, private _http: HttpClient, private _commonService: CommonService, private _router: Router
        , private toastr: CommonToastrService, private encrypt: EncryptionService) { }

    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/Pagination', data).then(e => e);
    }


    ExportData(ExportType: number, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {

        var data = { 'ExportType': ExportType, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': true };
        return this.http.Get(this.urlToApi + '/ExportData', data).then(e => {

            if (e.FilePath != "")
                this.http.ExportDataDownload(GlobalVariable.BASE_Api_URL, e.FilePath);

            return e;
        });
    }

    GetById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetById', data).then(e => e);
    }

    IsEmailExist(Email: string): Promise<any> {
        var data = { 'Email': Email };
        return this.http.Get(this.urlToApi + '/IsEmailExist', data).then(e => e);
    }

    EmailCofirmed(token: string): Promise<any> {
        var data = { 'userId': token };
        return this.http.Get(this.urlToApi + '/Cofirmed', data).then(e => e);
    }
    passwordChanged(token: string): Promise<any> {
        var data = { 'userId': token };
        return this.http.Get(this.urlToApi + '/Cofirmed', data).then(e => e);
    }

    SaveOrUpdate(entity: User): Promise<any> {
        // if (isNaN(entity.ID) || entity.ID == 0)
        return this.http.Post(this.urlToApi + '/SaveUser', entity).then(e => e);
        // else
        //     return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }

    SaveCompany(entity: Company): Promise<any> {
        return this.http.Post(this.urlToApi + '/saveCompany', entity).then(e => e);
    }

    changePassword(entity: User): Promise<any> {
        // if (isNaN(entity.ID) || entity.ID == 0)
        return this.http.Post(this.urlToApi + '/changePassword', entity).then(e => e);
        // else
        //     return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }

    createPassword(entity: SetupPass): Promise<any> {
        return this.http.Post(this.urlToApi + '/createPassword', entity).then(e => e);
    }

    forgotpasswd(Email: string): Promise<any> {
        var data = { 'Email': Email };
        return this.http.Get(this.urlToApi + '/forgotpassword', data).then(e => e);
    }


    IsForgotTokenExist(token: string): Promise<any> {
        var data = { 'token': token };
        return this.http.Get(this.urlToApi + '/IsForgotTokenExist', data).then(e => e);
    }

    resetpasswd(entity: User): Promise<any> {
        return this.http.Post(this.urlToApi + '/resetpassword', entity).then(e => e);
    }

    ResendConfirmationEmail(Email: string): Promise<any> {
        var data = { 'Email': Email };
        return this.http.Get(this.urlToApi + '/ResendEmail', data).then(e => e);
    }

    // Login(Email: string, Pwd: string): Promise<any> {
    //     
    //     return this.http.Post(this.urlToApi + '/Login', JSON.stringify({ Email: Email, Pwd: Pwd })).then(e => e);
    // }


    Delete(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApi + '/Delete', data).then(e => e);
    }

    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/Load', {}).then(e => e);
    }

    IsUserLogin() {
        if (localStorage.getItem('IsLogin') == "true" && this._commonService.Tokenvalidate() == true)
            if (this._router.url == '/login' || this._router.url == '/register')
                this._router.navigate(['/home/dashboard']);
    }
    IsPayrollRegion(): Promise<any> {
        var data = {};
        return this.http.Get(this.urlToApi + '/GetPayrollRegion', {}).then(m => {
            localStorage.setItem('PayrollRegion', this.encrypt.encryptionAES(m.ResultSet.PayrollRegion));
            return m;
        });
    }
    Login(login: User): Promise<any> {
        return this.http.Post(this.urlToApi + '/Login', login).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                if (!result.Message) {
                    localStorage.removeItem('PayrollRegion');
                    var userid = result.ResultSet.User.ID;
                    var user = JSON.stringify(result.ResultSet.User);
                    var company = JSON.stringify(result.ResultSet.Company);
                    var modules = JSON.stringify(result.ResultSet.Modules);
                    var rights = JSON.stringify(result.ResultSet.Rights);
                    var coltrollevel = JSON.stringify(result.ResultSet.ControlLevelRights);
                    localStorage.setItem('IsLogin', "true");
                    localStorage.setItem('currentUser', this.encrypt.encryptionAES(user));
                    localStorage.setItem('company', this.encrypt.encryptionAES(company));            
                    localStorage.setItem('Token', this.encrypt.encryptionAES(result.ResultSet.Token));
                    localStorage.setItem('PayrollRegion', this.encrypt.encryptionAES(result.ResultSet.PayrollRegion.toString()));
                    localStorage.setItem('Currency', this.encrypt.encryptionAES(result.ResultSet.Currency.toString()));
                    localStorage.setItem('ValidTo', result.ResultSet.ValidTo);
                    localStorage.setItem('Modules', this.encrypt.encryptionAES(modules));
                    localStorage.setItem('ControlLevelRights', this.encrypt.encryptionAES(coltrollevel));
                    localStorage.setItem('Rights', this.encrypt.encryptionAES(rights));
                    localStorage.setItem('RoleName', this.encrypt.encryptionAES(result.ResultSet.RoleName));
                  
                }
            }
            return m;
        });
    }
    Logout(): void {
        //clear variables remove user from local storage to log user out
        localStorage.removeItem('IsLogin');
        localStorage.removeItem('currentUser');
        localStorage.removeItem('Token');
        localStorage.removeItem('PayrollRegion');
        localStorage.removeItem('Currency');
        localStorage.removeItem('ValidTo');
        localStorage.removeItem('Modules');
        localStorage.removeItem('Rights');
        localStorage.removeItem('ControlLevelRights');
        localStorage.removeItem('company');        
    }
}