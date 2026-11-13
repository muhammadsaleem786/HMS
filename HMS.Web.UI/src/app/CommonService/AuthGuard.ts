import { Injectable } from '@angular/core';
import { HMSScreen } from '../AngularConfig/global'
import { ActivatedRoute, CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CommonService } from '../CommonService/CommonService'
import { LoaderService } from '../CommonService/LoaderService';
import { EncryptionService } from '../CommonService/encryption.service';
@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private router: Router, private encrypt: EncryptionService, private ARoute: ActivatedRoute, private _commonService: CommonService, private loader: LoaderService) {
    }
    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let company: any = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        let user: any = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
        if (localStorage.getItem('IsLogin') == "true" && this._commonService.Tokenvalidate() == true) {
            var routeName = state.url.toString();
            if (company.CompanyID == null || company.CompanyID == 'undefined' || company.CompanyID == '0') {
                if (routeName == "/home/setup")
                    this.router.navigate(['/home/Appoint/company']);
                else if (routeName != "/home/Appoint/company")
                    this.router.navigate(['/home/Appoint/company']);
                return true;
            }
            // Check screen permissions
            if (this.IsScreenAllow(routeName) == false) {
                this.loader.HideLoader();
                this.router.navigate(['/login']);
                return false;
            }
            // Check account expiry
            if (user && user.ExpiryDate) {
                const expiryDate = new Date(user.ExpiryDate);
                const today = new Date();
                expiryDate.setHours(0, 0, 0, 0);
                today.setHours(0, 0, 0, 0);
                if (expiryDate < today) {
                    this.loader.HideLoader();
                    this._commonService.handleExpiry();
                    this.router.navigate(['/login']);
                    return false;
                }
            }
            // Account is active
            return true;
        }
        // not logged in so redirect to login page
        this.router.navigate(['/login']);
        return false;
    }

    public IsScreenAllow(routeName: string): boolean {
        routeName = routeName.split("?")[0];
        var ScreenRights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem("Rights")));
        var RouthFind = HMSScreen.filter(f => f.text == routeName)
        if (RouthFind.length == 0)
            return true;
        var ScreenID = RouthFind[0].id;
        if (ScreenRights.filter(f => f == ScreenID).length > 0)
            return true;
        else
            return false;
    }
}