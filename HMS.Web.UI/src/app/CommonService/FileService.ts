import { Injectable, Input, Output } from '@angular/core';
import { Http, Headers, Response, RequestOptions, ResponseContentType } from '@angular/http';
import { GlobalVariable } from '../AngularConfig/global';
import { EncryptionService } from './encryption.service';
import { Observable } from 'rxjs';


@Injectable()
export class FileService {


    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/FileServer/Upload";
    public progress = 0;
    private progressObserver: Observable<Array<number>>;
    constructor(private http: Http, private encrypt: EncryptionService) {
    }

    public Upload(self: any, files: File[], tempFunction: (self: any, value: number) => void) {
        let Observable = new Promise((resolve, reject) => {
            let formData: any = new FormData()
            let xhr = new XMLHttpRequest()
            for (let i = 0; i < files.length; i++) {
                formData.append(files[i].name, files[i], files[i].name);
                this.progress = i;
            }
            //var size = files[0].size / 1000;
            //size = size / 1000;
            //size = Math.round(size * 100) / 100;

            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {

                        resolve(JSON.parse(xhr.response))
                    } else {
                        reject(xhr.response)
                    }
                }
            }

            xhr.upload.onprogress = (event) => {
                if (event.lengthComputable) {
                    //console.log(event.loaded + " / " + event.total)  
                    tempFunction(self, Math.round(event.loaded / event.total * 100));
                }
                else
                    tempFunction(self, 100);
            };

            //if (size < 2) {
            xhr.open('POST', this.urlToApi + "/Temporary", true);
            var CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem("company")));
            var UserObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem("currentUser")));
            if (localStorage.getItem("Token") !== null)
                xhr.setRequestHeader("Authorization", "Bearer " + this.encrypt.decryptionAES(localStorage.getItem("Token")));
            if (CompanyObj.CompanyID !== null)
                xhr.setRequestHeader("CompanyID", CompanyObj.CompanyID);
            if (UserObj.ID!== null)
                xhr.setRequestHeader("UserID", UserObj.ID);
            xhr.send(formData)
            //}

            //xhr.upload.onprogress = (event) => {
            //    this.progress = Math.round(event.loaded / event.total * 100);
            //    this.progressObserver.next(this.progress);
            //}
            //xhr.open("POST", url, true);
            //xhr.send(formData);
        });
        return Observable;
    }

    //public Download(url: string, data: any) {


    //    window.open(urls);

    //    //let headers = this.CreateAuthorizationHeader();

    //    //return this.http.get(url, { headers: headers, responseType: ResponseContentType.ArrayBuffer, search: data, withCredentials: true })
    //    //    .toPromise()
    //    //    .then(response => response.arrayBuffer())
    //    //    .catch(this.handleError);

    //}

    public CreateAuthorizationHeader(): Headers {
        var UserObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem("currentUser")));
        var CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem("company")));

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

    private handleError(error: any): Promise<any> {
        console.error('An error occurred', error); // for demo purposes only
        return Promise.reject(error.message || error);

    }
}