import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { HttpService } from '../../../CommonService/HttpService';
import { GlobalVariable } from '../../../AngularConfig/global';
import { Router, ActivatedRouteSnapshot } from '@angular/router';
import { CommonService } from '../../../CommonService/CommonService';
import { CommonToastrService } from '../../../CommonService/CommonToastrService';
@Injectable()
export class ContactService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/User"
    constructor(private http: HttpService, private _http: HttpClient, private _commonService: CommonService, private _router: Router, private toastr: CommonToastrService)
    { }
}
