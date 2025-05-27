import { Injectable } from '@angular/core';
import { HttpService } from '../CommonService/HttpService';
import { HiddenVariable, GlobalVariable } from '../AngularConfig/global';
import { Login } from '../../app/Module/model/Login.model';
import { EncryptionService } from './encryption.service';

@Injectable()
export class AuthenticationService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/User"
    constructor(private http: HttpService, private encrypt: EncryptionService) {
    }
    Login(login: Login): Promise<boolean> {
        return this.http.Post(this.urlToApi + '/Login', login).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                localStorage.removeItem('PayrollRegion');
                var userid = result.ResultSet.User.ID;
                var user = JSON.stringify(result.ResultSet.User);
                var modules = JSON.stringify(result.ResultSet.Modules);
                var rights = JSON.stringify(result.ResultSet.Rights);
                var coltrollevel = JSON.stringify(result.ResultSet.ControlLevelRights);
                var company = JSON.stringify(result.ResultSet.Company);
                localStorage.setItem('IsLogin', "true");
                localStorage.setItem('currentUser', this.encrypt.encryptionAES(user));
                localStorage.setItem('company', this.encrypt.encryptionAES(company));
                localStorage.setItem('Token', this.encrypt.encryptionAES(result.ResultSet.Token));
                localStorage.setItem('PayrollRegion', this.encrypt.encryptionAES(result.ResultSet.PayrollRegion));
                localStorage.setItem('Currency', this.encrypt.encryptionAES(result.ResultSet.Currency));
                localStorage.setItem('ValidTo', result.ResultSet.ValidTo);
                localStorage.setItem('Modules', this.encrypt.encryptionAES(modules));
                localStorage.setItem('ControlLevelRights', this.encrypt.encryptionAES(coltrollevel));
                localStorage.setItem('Rights', this.encrypt.encryptionAES(rights));
                localStorage.setItem('RoleName', this.encrypt.encryptionAES(result.ResultSet.RoleName));
            }
            else {
                alert(result.ErrorMessage);
            }
            return result.IsSuccess;
        });
    }
    Logout(): void {
        // clear variables remove user from local storage to log user out
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