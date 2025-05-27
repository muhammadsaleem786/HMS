import { Injectable } from '@angular/core';
import { GlobalVariable } from '../../../AngularConfig/global';
import { HttpService } from '../../../CommonService/HttpService';
import { ReportModel } from '../../Module/Reports/ReportModel';
@Injectable()
export class ReportService {
    private urlToApi = GlobalVariable.BASE_Api_URL + "/api/Admin/adm_Report"
    private urlToApiPatient = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_patient"
    private urlToApiservice = GlobalVariable.BASE_Api_URL + "/api/Appointment/emr_service_mf"
    private urlToApisale = GlobalVariable.BASE_Api_URL + "/api/Admin/pur_sale_mf"
    constructor(private http: HttpService) { }   
    FormLoad(): Promise<any> {
        return this.http.Get(this.urlToApi + '/Load', {}).then(e => e);
    }
    ExportData(emp: any): Promise<any> {

        return this.http.Get(this.urlToApi + '/ExportData', emp).then(e => {
            if (e.FilePath != "")
                this.http.ExportDataDownload(GlobalVariable.BASE_Api_URL, e.FilePath);

            return e;
        });

    }
    StockGroupSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApi + '/searchByName', data).then(e => e);
    }
    ServiceSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApiservice + '/searchService', data).then(e => e);
    }
    ItemSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApisale + '/searchByName', data).then(e => e);
    }
    DoctorSearch(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApiPatient + '/DoctorSearch', data).then(e => e);
    }
    searchByName(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApiPatient + '/searchByName', data).then(e => e);
    }
    searchByClinic(term: string): Promise<any> {
        var data = { 'term': term };
        return this.http.Get(this.urlToApiPatient + '/searchByClinic', data).then(e => e);
    }
    AppointmentRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/AppointmentRpt', entity).then(e => e);
    }
    FeeRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/FeeRpt', entity).then(e => e);
    } 
    ProcedureRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/ProcedureRpt', entity).then(e => e);
    } 
    ItemRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/ItemRpt', entity).then(e => e);
    } 
    PurchaseRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/PurchaseRpt', entity).then(e => e);
    } 
    CurrentStockRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/CurrentStockRpt', entity).then(e => e);
    } 
    PatientDetailRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/PatientDetailRpt', entity).then(e => e);
    } 
    ClinicwiseRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/ClinicwiseRpt', entity).then(e => e);
    } 
    DocReferredToRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/DocReferredToRpt', entity).then(e => e);
    } 
    DoctorswiseFeeRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/DoctorswiseFeeRpt', entity).then(e => e);
    } 
    DoctorswisePaymentRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/DoctorswisePaymentRpt', entity).then(e => e);
    } 
    DoctorswiseSummaryPaymentRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/DoctorswiseSummaryPaymentRpt', entity).then(e => e);
    } 
    DocReferredByRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/DocReferredByRpt', entity).then(e => e);
    } 
    CashFlowRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/CashFlowRpt', entity).then(e => e);
    } 
    ProfitLossRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/ProfitLossRpt', entity).then(e => e);
    } 
    MedicinewiseRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/MedicinewiseRpt', entity).then(e => e);
    } 
    OutstandingRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/OutstandingRpt', entity).then(e => e);
    } 
    PaymentSummaryRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/PaymentSummaryRpt', entity).then(e => e);
    } 
    TreatmentwiseRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/TreatmentwiseRpt', entity).then(e => e);
    } 

    BirthdayRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/BirthdayRpt', entity).then(e => e);
    } 
    DetailedPatientRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/DetailedPatientRpt', entity).then(e => e);
    } 
    FollowupRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/FollowupRpt', entity).then(e => e);
    } 
    PatientOutstandingRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/PatientOutstandingRpt', entity).then(e => e);
    } 
    PaymentRpt(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/PaymentRpt', entity).then(e => e);
    }  

    SendEmail(entity: ReportModel): Promise<any> {
        return this.http.Post(this.urlToApi + '/SendEmail', entity).then(e => e);
    } 
}