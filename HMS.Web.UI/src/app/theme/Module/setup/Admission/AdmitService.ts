import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { HttpService } from '../../../../CommonService/HttpService';
import { emr_patient, ipd_admission, ipd_admission_vital, ipd_admission_notes, ipd_input_output, ipd_medication_log, ipd_procedure_charged, ipd_admission_lab, ipd_admission_charges, ipd_admission_medication, ipd_admission_imaging, ipd_procedure_mf, ipd_procedure_medication, ipd_diagnosis, ipd_admission_discharge } from './AdmitModel';

@Injectable()
export class AdmitService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_admission";
    private urlToApiVital = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_admission_vital";
    private urlToApiCharg = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_admission_charges";
    private urlToApinote = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_admission_notes";
    private urlToApiLab = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_admission_lab";
    private urlToApiImg = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_admission_imaging";
    private urlToApiMedi = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_admission_medication";
    private urlToApiPro = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_procedure_mf";
    private urlToApiApp = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_patient";
    private urlToApiDia = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_diagnosis";
    private urlToApiRpt = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_admission_discharge";
    private urlInputToApi = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_input_output";
    private urlMedToApi = GlobalVariable.BASE_Api_URL + "/api/Admission/ipd_medication_log";
    constructor(private http: HttpService) { }

    GetList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, FilterID: string, IgnorePaging: boolean): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'FilterID': FilterID, 'IgnorePaging': IgnorePaging };
        return this.http.Get(this.urlToApi + '/PaginationWithParm', data).then(e => e);
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
    SaveOrUpdate(entity: ipd_admission): Promise<any> {
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
    DoctorSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/DoctorSearch', data).then(e => e);
    }
    searchByName(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApiApp + '/searchByName', data).then(e => e);
    }
    getPatientById(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApiApp + '/getPatientById', data).then(e => e);
    }
    GetEMRNO(): Promise<any> {
        return this.http.Get(this.urlToApiApp + '/GetEMRNO', {}).then(e => e);
    }

    PatientSaveOrUpdate(entity: emr_patient): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiApp + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiApp + '/Update', entity).then(e => e);
    }
    //Admission  charge
    ChargeSaveOrUpdate(entity: ipd_admission_charges): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiCharg + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiCharg + '/Update', entity).then(e => e);
    }
    ChargeGetById(AdmitId: string, PatientId: string): Promise<any> {
        var data = { 'AdmitId': AdmitId, 'PatientId': PatientId };
        return this.http.Get(this.urlToApiCharg + '/GetById', data).then(e => e);
    }

    //Admission  Vital
    VitalSaveOrUpdate(entity: ipd_admission_vital): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiVital + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiVital + '/Update', entity).then(e => e);
    }

    //input
    InputSaveOrUpdate(entity: ipd_input_output): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlInputToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlInputToApi + '/Update', entity).then(e => e);
    }
    GetIntakeDropDown(): Promise<any> {
        return this.http.Get(this.urlInputToApi + '/Load', {}).then(e => e);
    }
    //Medication
    GetMedicationDropDown(): Promise<any> {
        return this.http.Get(this.urlMedToApi + '/Load', {}).then(e => e);
    }
    MedicatioLogSaveOrUpdate(entity: ipd_medication_log): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlMedToApi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlMedToApi + '/Update', entity).then(e => e);
    }
    GetInputList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'IgnorePaging': false };
        return this.http.Get(this.urlInputToApi + '/Pagination', data).then(e => e);
    }

    GetVitalList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, AdmitId: string, PatientId: string, Appointmentid: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'AdmitId': AdmitId, 'PatientId': PatientId, 'Appointmentid': Appointmentid, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiVital + '/VitalPagination', data).then(e => e);
    }
    GetVilitListByAdmitId(AdmitId: string, PatientId: string): Promise<any> {
        var data = { 'AdmitId': AdmitId, 'PatientId': PatientId };
        return this.http.Get(this.urlToApiVital + '/GetVilitListByAdmitId', data).then(e => e);
    }
    GetOrderList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, AdmitId: string, PatientId: string, Appointmentid: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'AdmitId': AdmitId, 'PatientId': PatientId, 'Appointmentid': Appointmentid, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiVital + '/OrderList', data).then(e => e);
    }
    DeleteVital(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApiVital + '/Delete', data).then(e => e);
    }
    //Admission Note
    NoteSaveOrUpdate(entity: ipd_admission_notes): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApinote + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApinote + '/Update', entity).then(e => e);
    }
    GetAppointmentList(PatientId: string): Promise<any> {
        var data = { 'PatientId': PatientId };
        return this.http.Get(this.urlToApinote + '/AppointmentList', data);
    }
    NoteGetById(Id: number, PatientId: string): Promise<any> {
        var data = { 'Id': Id, 'PatientId': PatientId };
        return this.http.Get(this.urlToApinote + '/GetById', data).then(e => e);
    }
    DeleteNote(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApinote + '/Delete', data).then(e => e);
    }
    GetNoteList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, AdmitId: string, PatientId: string, Appointmentid: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'AdmitId': AdmitId, 'PatientId': PatientId, 'Appointmentid': Appointmentid, 'IgnorePaging': false };
        return this.http.Get(this.urlToApinote + '/NotePagination', data).then(e => e);
    }
    //Admission Medication List
    GetMedicationList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, AdmitId: string, PatientId: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'AdmitId': AdmitId, 'PatientId': PatientId, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiMedi + '/MedicationPagination', data).then(e => e);
    }
    MedicationSaveOrUpdate(entity: ipd_admission_medication): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiMedi + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiMedi + '/Update', entity).then(e => e);
    }
    MedicationGetById(Id: number, PatientId: string): Promise<any> {
        var data = { 'Id': Id, 'PatientId': PatientId };
        return this.http.Get(this.urlToApiMedi + '/GetById', data).then(e => e);
    }
    DeleteMedication(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApiMedi + '/Delete', data).then(e => e);
    }

    //Admission Imaging List
    GetImagingList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, AdmitId: string, PatientId: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'AdmitId': AdmitId, 'PatientId': PatientId, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiImg + '/ImagePagination', data).then(e => e);
    }
    ImagingSaveOrUpdate(entity: ipd_admission_imaging): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiImg + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiImg + '/Update', entity).then(e => e);
    }
    ImagingGetById(Id: number, PatientId: string): Promise<any> {
        var data = { 'Id': Id, 'PatientId': PatientId };
        return this.http.Get(this.urlToApiImg + '/GetById', data).then(e => e);
    }
    DeleteImaging(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApiImg + '/Delete', data).then(e => e);
    }


    //lab data
    GetLabList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, AdmitId: string, PatientId: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'AdmitId': AdmitId, 'PatientId': PatientId, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiLab + '/LabPagination', data).then(e => e);
    }
    LabSaveOrUpdate(entity: ipd_admission_lab): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiLab + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiLab + '/Update', entity).then(e => e);
    }
    LabGetById(Id: number, PatientId: string): Promise<any> {
        var data = { 'Id': Id, 'PatientId': PatientId };
        return this.http.Get(this.urlToApiLab + '/GetById', data).then(e => e);
    }
    DeleteLab(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApiLab + '/Delete', data).then(e => e);
    }
    GetLabDropDown(PatientId: string): Promise<any> {
        var data = { 'PatientId': PatientId };
        return this.http.Get(this.urlToApiLab + '/GetLabDropDown', data);
    }

    //Proedures data
    GetProeduresDropDown(PatientId: string): Promise<any> {
        var data = { 'PatientId': PatientId };
        return this.http.Get(this.urlToApiPro + '/GetProeduresDropDown', data);
    }
    GetProeduresList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, AdmitId: string, PatientId: string, Appointmentid: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'AdmitId': AdmitId, 'PatientId': PatientId, 'Appointmentid': Appointmentid, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiPro + '/ProeduresPagination', data).then(e => e);
    }
    ProcedureSaveOrUpdate(entity: ipd_procedure_mf): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiPro + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiPro + '/Update', entity).then(e => e);
    }
    DeleteProcedure(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApiPro + '/Delete', data).then(e => e);
    }
    ProcedureGetById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApiPro + '/GetById', data).then(e => e);
    }

    //diagnosis data

    GetDiagnosisList(CurrentPage: number, RecordPerPage: number, VisibleColumnInfo: string, SortName: string, SortOrder: string, SearchText: string, AdmitId: string, PatientId: string, Appointmentid: string): Promise<any> {
        var data = { 'CurrentPageNo': CurrentPage, 'RecordPerPage': RecordPerPage, 'VisibleColumnInfo': VisibleColumnInfo, 'SortName': SortName, 'SortOrder': SortOrder, 'SearchText': SearchText, 'AdmitId': AdmitId, 'PatientId': PatientId, 'Appointmentid': Appointmentid, 'IgnorePaging': false };
        return this.http.Get(this.urlToApiDia + '/ProeduresPagination', data).then(e => e);
    }
    DiagnosisSaveOrUpdate(entity: ipd_diagnosis): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiDia + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiDia + '/Update', entity).then(e => e);
    }
    DeleteDiagnosis(Id: string): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Delete(this.urlToApiDia + '/Delete', data).then(e => e);
    }
    DiagnosisGetById(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApiDia + '/GetById', data).then(e => e);
    }
    WardDropDown(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/WardDropDown', data);
    }
    DischargePatient(Id: number): Promise<any> {
        var data = { 'Id': Id };
        return this.http.Get(this.urlToApi + '/DischargePatient', data);
    }
    ///Discharge Report 

    SaveOrUpdateDischargeRpt(entity: ipd_admission_discharge): Promise<any> {
        if (isNaN(entity.ID) || entity.ID == 0)
            return this.http.Post(this.urlToApiRpt + '/Save', entity).then(e => e);
        else
            return this.http.Put(this.urlToApiRpt + '/Update', entity).then(e => e);
    }
    GetDischargRpt(AdmissionId: string, PatientId: string): Promise<any> {
        var data = { 'AdmissionId': AdmissionId, 'PatientId': PatientId };
        return this.http.Get(this.urlToApiRpt + '/GetDischargRpt', data).then(e => e);
    }
}
