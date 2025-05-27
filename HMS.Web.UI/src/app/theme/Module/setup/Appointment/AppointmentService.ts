import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { HttpService } from '../../../../CommonService/HttpService';
import { emr_patient, emr_Appointment, emr_document, emr_vital, emr_notes_favorite } from './AppointmentModel';
import { ApplicationUserModel } from '../../setup/Setting/ApplicationUser/ApplicationUserModel';
import { emr_medicine, emr_prescription_mf } from '../Prescription/PrescriptionModel';
import { emr_patient_bill } from '../Billing/BillingModel';

@Injectable()
export class AppointmentService {
    private DocurlToApi = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_document"
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_patient"
    private urlToApiAp = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_appointment_mf"
    private MediurlToApiAp = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_medicine"
    private PresurlToApiAp = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_prescription_mf"
    private BillToApiAp = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_patient_bill"
    private VitalToApiAp = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_vital"
    private favoriteToApiAp = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_notes_favorite"

    constructor(private http: HttpService) { }
    GetPatientList(date: string, StatusId: string, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'date': date, 'StatusId': StatusId, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiAp + '/AppointList', data).then(e => e);
    }
    GetUpdatePatientList(date: string, StatusId: string, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'date': date, 'StatusId': StatusId, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiAp + '/UpdateAppointList', data).then(e => e);
    }
    GetList(): Promise<any> {
        var data = {};
        return this.http.Get(this.urlToApi + '/GetList', data).then(e => e);
    }
    ExportData(ExportType: number, CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'ExportType': ExportType, 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': true };
        return this.http.Get(this.urlToApi + '/ExportData', data).then(e => {
            if (e.FilePath != "")
                this.http.ExportDataDownload(GlobalVariable.BASE_Api_URL, e.FilePath);
            return e;
        });
    }
    DocDownload(FilePath: any) {
        if (FilePath != "")
            this.http.ExportDataDownload(GlobalVariable.BASE_Api_URL, FilePath);
        return;

    }
    GetById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/GetById', data).then(e => e);
    }
    GetDocById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.DocurlToApi + '/GetById', data).then(e => e);
    }
    IsPhoneExist(Phone: any): Promise<any> {
        var data = { 'Phone': Phone };
        return this.http.Get(this.urlToApi + '/IsPhoneExist', data).then(e => e);
    }
    SaveOrUpdate(entity: emr_patient): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApi + '/Update', entity).then(e => e);
    }

    SaveOrUpdatebill(entity: emr_patient_bill): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.BillToApiAp + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.BillToApiAp + '/Update', entity).then(e => e);
    }

    UpdateAppointment(entity: emr_Appointment): Promise<any> {
        return this.http.Put(this.urlToApiAp + '/UpdateAppointment', entity).then(e => e);
    }
    AppSaveOrUpdate(entity: emr_Appointment): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiAp + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiAp + '/Update', entity).then(e => e);
    }
    //Appointment 
    GetAdmitAppointmentDropdown(PatientId: string): Promise<any> {
        var data = { 'PatientId': PatientId };
        return this.http.Get(this.urlToApiAp + '/GetAdmitAppointmentDropdown', data);
    }
    GetEMRNO(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetEMRNO', {}).then(e => e);
    }

    PatientInfo(entity: emr_patient): Promise<any> {
        return this.http.Post(this.urlToApi + '/PatientInfo', entity).then(e => e);
    }
    searchMedicine(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.MediurlToApiAp + '/searchMedicine', data).then(e => e);
    }
    GetPatient(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetPatient', {});
    }
    ComplaintSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.PresurlToApiAp + '/ComplaintSearch', data).then(e => e);
    }
    ServiceSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.BillToApiAp + '/ServiceSearch', data).then(e => e);
    }
    AllComplaint(): Promise<any> {
        var data = {};
        return this.http.Get(this.PresurlToApiAp + '/ComplaintList', data).then(e => e);
    }
    AllInvestigation(): Promise<any> {
        var data = {};
        return this.http.Get(this.PresurlToApiAp + '/InvestigationList', data).then(e => e);
    }
    AllObservation(): Promise<any> {
        var data = {};
        return this.http.Get(this.PresurlToApiAp + '/ObservationList', data).then(e => e);
    }
    AllDiagnosis(): Promise<any> {
        var data = {};
        return this.http.Get(this.PresurlToApiAp + '/DiagnosisList', data).then(e => e);
    }
    DiagnosSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.PresurlToApiAp + '/DiagnosSearch', data).then(e => e);
    }
    InstructionSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.PresurlToApiAp + '/InstructionSearch', data).then(e => e);
    }
    ObservationSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.PresurlToApiAp + '/ObservationSearch', data).then(e => e);
    }
    InvestigationSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.PresurlToApiAp + '/InvestigationSearch', data).then(e => e);
    }
    DoctorSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/DoctorSearch', data).then(e => e);
    }


    searchByName(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/searchByName', data).then(e => e);
    }
    Delete(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApi + '/Delete', data).then(e => e);
    }
    DeleteDoc(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.DocurlToApi + '/Delete', data).then(e => e);
    }
    DownloadDoc(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.DocurlToApi + '/DownloadDoc', data).then(e => e);
    }
    DeleteAppoint(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApiAp + '/Delete', data).then(e => e);
    }

    PatientInfoLoad(id: string, Date: any, AppId: any): Promise<any> {
        var data = { 'id': id, 'Date': Date, 'AppId': AppId }
        return this.http.Get(this.urlToApiAp + '/PatientInfoLoad', data).then(e => e);
    }
    PatientLoadById(id: string): Promise<any> {
        var data = { 'id': id }
        return this.http.Get(this.urlToApiAp + '/PatientLoadById', data).then(e => e);
    }
    AdmitPatientLoadById(id: string, AdmitId: string): Promise<any> {
        var data = { 'id': id, 'AdmitId': AdmitId }
        return this.http.Get(this.urlToApiAp + '/AdmitPatientLoadById', data).then(e => e);
    }
    PatientAppointmentLoad(id: string): Promise<any> {
        var data = { 'id': id }
        return this.http.Get(this.urlToApiAp + '/PatientAppointmentLoad', data).then(e => e);
    }
    FormLoad(date: string, StatusId: string): Promise<any> {
        var data = { 'date': date, 'StatusId': StatusId }
        return this.http.Get(this.urlToApi + '/LoadData', data).then(e => e);
    }
    DropdownFilterData(date: string, StatusId: string): Promise<any> {
        var data = { 'date': date, 'StatusId': StatusId }
        return this.http.Get(this.urlToApi + '/DropdownFilterData', data).then(e => e);
    }

    MonthLoadData(fdate: string, tdate: string, StatusId: string): Promise<any> {
        var data = { 'fdate': fdate, 'tdate': tdate, 'StatusId': StatusId }
        return this.http.Get(this.urlToApi + '/MonthLoadData', data).then(e => e);
    }
    GetAllScreens(): Promise<any> {
        return this.http.Get(this.urlToApi + '/GetAllScreens', {}).then(e => e);
    }
    GetAllDoctorList(): Promise<any> {
        return this.http.Get(this.urlToApi + '/DoctorList', {}).then(e => e);
    }

    UserSave(entity: ApplicationUserModel): Promise<any> {
        return this.http.Post(this.urlToApiAp + '/UserSave', entity).then(e => e);
    }

    GetDocmentType(): Promise<any> {
        return this.http.Get(this.DocurlToApi + '/Load', {});
    }

    GetDocList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.DocurlToApi + '/Pagination', data).then(e => e);
    }
    GetBillingList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, PatientId: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'PatientId': PatientId, 'IgnorePaging': false };
        return this.http.Get(this.BillToApiAp + '/BillingPagination', data).then(e => e);
    }
    GetPreviousList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, PatientId: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'PatientId': PatientId, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiAp + '/PreviousAppointmentLoad', data).then(e => e);
    }
    AdmitAppointmentLoad(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, PatientId: string, AdmitId: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'PatientId': PatientId, 'AdmissionId': AdmitId, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiAp + '/AdmitAppointmentLoad', data).then(e => e);
    }
    DocumentSave(entity: emr_document): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.DocurlToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.DocurlToApi + '/Update', entity).then(e => e);
    }
    DocGetById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.DocurlToApi + '/GetById', data).then(e => e);
    }

    PrescriptionDropdownList(): Promise<any> {
        return this.http.Get(this.MediurlToApiAp + '/Load', {});
    }
    MedicineSave(entity: emr_medicine): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.MediurlToApiAp + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.MediurlToApiAp + '/Update', entity).then(e => e);
    }
    PrescriptionSave(entity: emr_prescription_mf): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.PresurlToApiAp + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.PresurlToApiAp + '/Update', entity).then(e => e);
    }
    TempleteList(): Promise<any> {
        return this.http.Get(this.PresurlToApiAp + '/Load', {});
    }
    TemplateLoadById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.PresurlToApiAp + '/TemplateLoadById', data).then(e => e);
    }
    PrintRXById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.PresurlToApiAp + '/PrintRXById', data).then(e => e);
    }

    GetAllVital(): Promise<any> {
        return this.http.Get(this.VitalToApiAp + '/Load', {}).then(e => e);
    }

    VitalSave(entity: emr_vital): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.VitalToApiAp + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.VitalToApiAp + '/Update', entity).then(e => e);
    }
    VitalGetById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.VitalToApiAp + '/GetById', data).then(e => e);
    }
    DeleteVital(Id: number, VitalId: number): Promise<any> {
        var data = { 'Id': Id, 'VitalId': VitalId };
        return this.http.Get(this.VitalToApiAp + '/Delete', data).then(e => e);
    }

    DeleteFavorite(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.favoriteToApiAp + '/Delete', data).then(e => e);
    }
    FavoriteSaveOrUpdate(entity: emr_notes_favorite): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.favoriteToApiAp + '/Save', entity).then(e => e);
        //else
        //    return this.http.Put(this.favoriteToApiAp + '/Update', entity).then(e => e);
    }

    DeshboardLoadDropdown(PatientId: any) {
        var data = { 'PatientId': PatientId };
        return this.http.Get(this.urlToApiAp + '/DeshboardLoadDropdown', data).then(e => e);
    }
}
