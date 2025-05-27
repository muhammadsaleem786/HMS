import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
//  import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Router } from '@angular/router';
import { GlobalVariable } from '../AngularConfig/global'
import { Observable } from 'rxjs';
import { EncryptionService } from './encryption.service';
import { CommonToastrService } from '../CommonService/CommonToastrService';
import { ToastrService } from 'ngx-toastr';
@Injectable()
export class HttpService {
    constructor(private http: Http, private router: Router, public toastr: CommonToastrService, private encrypt: EncryptionService) {
    }
    public Get(url: string, data: any): Promise<any> {
        let headers = this.CreateAuthorizationHeader();
        return this.http.get(url, { headers: headers, search: data, withCredentials: true })
            .toPromise()
            .then(response => response.json())
            .catch(error => this.handleError(error));
    }
    public Post(url: string, data: any): Promise<any> {
        let headers = this.CreateAuthorizationHeader();
        return this.http
            .post(url, data, { headers: headers })
            .toPromise()
            .then(e => e)
            .catch(error => this.handleError(error));
    }
    public Put(url: string, data: any): Promise<any> {
        let headers = this.CreateAuthorizationHeader();
        return this.http
            .put(url, JSON.stringify(data), { headers: headers })
            .toPromise()
            .then(e => e)
            .catch(error => this.handleError(error));
    }
    public Delete(url: string, data: any): Promise<any> {
        let headers = this.CreateAuthorizationHeader();
        return this.http.get(url, { headers: headers, search: data, withCredentials: true })
            .toPromise()
            .then(response => response.json())
            .catch(error => this.handleError(error));
    }
    public ExportDataDownload(ApiUrl: string, FilePath: string): void {
        var url = ApiUrl + '/api/Download/DownloadFile?FilePath=' + FilePath;
        //window.open(url);
        window.location.href = url;
    }
    public CreateAuthorizationHeader(): Headers {
        if (localStorage.getItem("currentUser") !== null && localStorage.getItem("company") !== null) {
            let UserObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem("currentUser")));
            let CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem("company")));
            let headers = new Headers({ 'Content-Type': 'application/json;charset=utf-8' });
            if (localStorage.getItem("Token") !== null)
                headers.set("Authorization", "Bearer " + this.encrypt.decryptionAES(localStorage.getItem("Token")));
            if (CompanyObj.CompanyID !== null)
                headers.set("CompanyID", CompanyObj.CompanyID);
            if (UserObj.ID !== null)
                headers.set("UserID", UserObj.ID);
            if (UserObj !== null)
                headers.set("User", UserObj);
            return headers;
        }
    }
    private handleError(error: any): Promise<any> {
        if (error.status === 401) {
            this.toastr.ShowFullWidthError('Session Expired', 'Your session has expired. Redirecting to login');
            this.redirectToLogin();
        } else if (error.status === 403) {
            this.toastr.ShowFullWidthError('Access Denied', 'You do not have permission to perform this action.');
        } else {
            const errorMessage = ('The system has encountered an error, please try again. If the issue persists, please contact us for assistance.');
            this.toastr.ShowFullWidthError('An error occurred', errorMessage);
            this.redirectToLogin();
        }
        return Promise.reject(error.message || error);
    }
    private redirectToLogin(): void {
            setTimeout(function () {
            window.location.href = "/";
        }, 6000)
    }
}

