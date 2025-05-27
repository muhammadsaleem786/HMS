import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../AngularConfig/global';
import { HttpService } from '../../../CommonService/HttpService';
import { DashboardModel, DashboardFilterModel } from './DashboardModel';


@Injectable()
export class DashboardService {

    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Admin/adm_dashboard"

    constructor(private http: HttpService) { }

    GetList(filterParams: string, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'filterParams': filterParams, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/Pagination', data).then(e => e);
    }

    getData(): Promise<any> {
        var data = {};
        return this.http.Get(this.urlToApi + '/Refresh', data).then(e => e);
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
    GetPaymentDueDate(): Promise<any> {
        var data = {};
        return this.http.Get(this.urlToApi + '/PaymentDueDate', data).then(e => e);
    }
    SaveOrUpdate(entity: DashboardModel): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }

    SaveAndReturnDeptList(entity: DashboardModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/SaveAndReturnDeptList', entity).then(e => e);
    }

    Delete(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApi + '/Delete', data).then(e => e);
    }

    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/Load', {}).then(e => e);
    }

    DataLoad(FromDate: any, ToDate: any): Promise<any> {
        var data = { 'FromDate': FromDate, 'ToDate': ToDate };
        return this.http.Get(this.urlToApi + '/DataLoad', data).then(e => e);
    }
}
