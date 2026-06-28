import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../AngularConfig/global'
import { HttpService } from '../../../../CommonService/HttpService';
import { lab_patient_test, lab_result_mf } from './CatalogModel';
@Injectable()
export class LabService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_investigation"
    private urlToApiPatient = GlobalVariable.BASE_Api_URL + "/api/Lab/lab_patient_test"
    private urlToApiresult = GlobalVariable.BASE_Api_URL + "/api/Lab/lab_result_mf"
    constructor(private http: HttpService) { }

    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiPatient + '/Pagination', data).then(e => e);
    }
    GetListWithParm(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, FilterID: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'FilterID': FilterID, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiPatient + '/PaginationWithParm', data).then(e => e);
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
        return this.http.Get(this.urlToApiPatient + '/GetById', data).then(e => e);
    }
    SelectedPatientLoadTest(Id: any): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApiPatient + '/SelectedPatientLoadTest', data).then(e => e);
    }
    

    GetPrintById(Id: any, serialNo: any): Promise<any> {
        var data = { 'Id': Id, 'serialNo': serialNo };
        return this.http.Get(this.urlToApiPatient + '/GetPrintById', data).then(e => e);
    }
    GetByCategoryId(Id: any): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetByCategoryId', data).then(e => e);
    }
    CategorySearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/CategorySearch', data).then(e => e);
    }

    PrioritySearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApiPatient + '/PrioritySearch', data).then(e => e);
    }
    TestSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApiPatient + '/TestSearch', data).then(e => e);
    }
    SearchPatient(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApiPatient + '/SearchPatient', data).then(e => e);
    }

    SaveOrUpdate(entity: lab_patient_test): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiPatient + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiPatient + '/Update', entity).then(e => e);
    }
    UpdateStatus(entity: lab_patient_test): Promise<any> {
        return this.http.Put(this.urlToApiPatient + '/UpdateStatus', entity).then(e => e);
    }
    SaveOrUpdateResult(entity: lab_result_mf): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiresult + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiresult + '/Update', entity).then(e => e);
    }
    GetBillByInvoiceNo(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApiPatient + '/GetBillByInvoiceNo', data).then(e => e);
    }
    GenerateReport(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApiPatient + '/GenerateReport', data).then(e => e);
    }
    Delete(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApiPatient + '/Delete', data).then(e => e);
    }
    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/Load', {}).then(e => e);
    }
    GetAllScreens(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetAllScreens', {}).then(e => e);
    }
}
