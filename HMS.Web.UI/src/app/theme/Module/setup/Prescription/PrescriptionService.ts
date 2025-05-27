import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { HttpService } from '../../../../CommonService/HttpService';
import { emr_prescription_mf } from './PrescriptionModel';
@Injectable()
export class PrescriptionService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_prescription_mf"
    private urlToApiPatient = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_patient"
    constructor(private http: HttpService) { }

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
    SaveOrUpdate(entity: emr_prescription_mf): Promise<any> {
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
    LoadDropdown(): Promise<any> {
        return this.http.Get(this.urlToApi + '/LoadDropdown', {}).then(e => e);
    }
    PrescriptionLoad(id: string, DocId:string, Date: any): Promise<any> {
        var data = { 'id': id, 'DocId': DocId, 'Date': Date }
        return this.http.Get(this.urlToApi + '/PrescriptionLoad', data).then(e => e);
    }
    
    GetAllScreens(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetAllScreens', {}).then(e => e);
    }
    searchByName(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApiPatient + '/searchByName', data).then(e => e);
    }
}
