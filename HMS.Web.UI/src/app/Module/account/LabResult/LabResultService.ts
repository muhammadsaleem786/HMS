import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../AngularConfig/global';
import { HttpService } from '../../../CommonService/HttpService';
import { lab_result } from './LabResultModel';
@Injectable()
export class LabResultService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/User";
    constructor(private http: HttpService) { }
    ExportData(ExportType: number, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'ExportType': ExportType, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': true };
        return this.http.Get(this.urlToApi + '/ExportData', data).then(e => {
            if (e.FilePath != "")
                this.http.ExportDataDownload(GlobalVariable.BASE_Api_URL, e.FilePath);
            return e;
        });
    }
    GetById(InvoiceNo: string, AccessCode: string): Promise<any> {
        var data = { 'InvoiceNo': InvoiceNo, 'AccessCode': AccessCode };
        return this.http.Get(this.urlToApi + '/GetById', data).then(e => e);
    }

    ReportGenerate(Id: string): Promise<any> {
        var data = { 'Id': Id};
        return this.http.Get(this.urlToApi + '/ReportGenerate', data).then(e => e);
    }
    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/Load', {}).then(e => e);
    }
    GetAllScreens(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetAllScreens', {}).then(e => e);
    }
}
