import { Injectable } from '@angular/core';
import { GlobalVariable } from '../AngularConfig/global';
import { HttpService } from '../CommonService/HttpService';


@Injectable()
export class themeService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Admin/adm_dashboard"

    constructor(private http: HttpService) { }
    
    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetAdmNotifications', {}).then(e => e);
    }
    UpdateNotificationViewedStatus(Ids: string): Promise<any> {
        var data = { 'Ids': Ids };
        return this.http.Get(this.urlToApi + '/UpdateNotificationViewedStatus', data).then(e => e);
    }
}
