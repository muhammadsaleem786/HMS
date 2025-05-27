import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { HttpService } from '../../../../CommonService/HttpService';
import { emr_patient_bill } from './BillingModel';
@Injectable()
export class BillingService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_patient_bill";
    private urlToApiPat = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_patient";
    constructor(private http: HttpService) { }

    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApi + '/Pagination', data).then(e => e);
    }

    GetIPDBillingList(PatientId: string, AdmitId: string): Promise<any> {
        var data = { 'PatientId': PatientId, 'AdmitId': AdmitId };
        return this.http.Get(this.urlToApi + '/GetIPDBillingList', data).then(e => e);
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
    GetPatientById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApiPat + '/GetById', data).then(e => e);
    }
    SaveOrUpdate(entity: emr_patient_bill): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }
    SaveOrUpdateBill(entity: emr_patient_bill): Promise<any> {
            return this.http.Put(this.urlToApi + '/UpdateBill', entity).then(e => e);
    }
    Delete(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApi + '/Delete', data).then(e => e);
    }
    GetBillByPatient(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetBillByPatient', data).then(e => e);
    }
    GetBillByPayment(AdmissionId: string, AppointmentId: string, PatientId: string): Promise<any> {
        var data = { 'AdmissionId': AdmissionId, 'AppointmentId': AppointmentId, 'PatientId': PatientId };
        return this.http.Get(this.urlToApi + '/GetBillByPayment', data).then(e => e);
    }
    GetBillByAdmissionId(Id: string, patientid: string): Promise<any> {
        var data = { 'Id': Id, 'patientid': patientid };
        return this.http.Get(this.urlToApi + '/GetBillByAdmissionId', data).then(e => e);
    }
    GetPaymentByPatient(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetPaymentByPatient', data).then(e => e);
    }

    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/Load', {}).then(e => e);
    }
    GetAllScreens(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetAllScreens', {}).then(e => e);
    }
    PatientSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/PatientSearch', data).then(e => e);
    }
    ServiceSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/ServiceSearch', data).then(e => e);
    }
}
