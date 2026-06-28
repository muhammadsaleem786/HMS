import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { HttpService } from '../../../../../CommonService/HttpService';
import { emr_investigation, lab_parameter } from './InvestigationModel';
@Injectable()
export class InvestigationService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_investigation"
    private urlToApiPara = GlobalVariable.BASE_Api_URL + "/api/Lab/lab_parameter"
    constructor(private http: HttpService) { }

    CategorySearch(term: string, dropdownId: number): Promise<any> {
        var data = { 'term': term, 'dropdownId': dropdownId };
        return this.http.Get(this.urlToApi + '/CategorySearch', data).then(e => e);
    }
    SaveOrUpdateParameter(entity: lab_parameter): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiPara + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiPara + '/Update', entity).then(e => e);
    }
    UnitSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApiPara + '/UnitSearch', data).then(e => e);
    }
    GetParameterById(Id: any): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApiPara + '/GetById', data).then(e => e);
    }
    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/Pagination', data).then(e => e);
    }
    GetByCategoryId(Id: any): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetByCategoryId', data).then(e => e);
    }
 
    ExportData(ExportType: number, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'ExportType': ExportType, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': true };
        return this.http.Get(this.urlToApi + '/ExportData', data).then(e => {
            if (e.FilePath != "")
                this.http.ExportDataDownload(GlobalVariable.BASE_Api_URL, e.FilePath);
            return e;
        });
    }
    GetById(Id: any): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetById', data).then(e => e);
    }
    SaveOrUpdate(entity: emr_investigation): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }
    Delete(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApi + '/Delete', data).then(e => e);
    }
    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/Load', {}).then(e => e);
    }
    GetAllScreens(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetAllScreens', {}).then(e => e);
    }
}
