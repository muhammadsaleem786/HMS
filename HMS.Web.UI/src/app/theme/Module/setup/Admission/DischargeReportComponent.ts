import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { ipd_admission_discharge } from './AdmitModel';
import { Observable } from 'rxjs';
import { AdmitService } from './AdmitService';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute, NavigationEnd } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { GlobalVariable } from '../../../../AngularConfig/global';
@Component({
    templateUrl: './DischargeReportComponent.html',
    moduleId: module.id,
    providers: [AdmitService],
})

export class DischargeReportComponent implements OnInit {
    public Form1: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public ID: number = 10;
    public IsAdmin: boolean = false;
    public model = new ipd_admission_discharge();
    public PayrollRegion: string;
    public Keywords: any[] = [];
    public AdmitId: any;
    public PatientId: any;
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public ClinicName: string;
    public Rights: any; public CompanyObj: any;
    public ControlRights: any;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AdmitService: AdmitService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("24");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {
        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem('PatientId');
        this.Form1 = this._fb.group({
            AdmissionId: [''],
            PatientId: [''],
            Weight: [''],
            Temperature: [''],
            DiagnosisAdmission: [''],
            ComplaintSummary: [''],
            ConditionAdmission: [''],
            GeneralCondition: [''],
            RespiratoryRate: [''],
            BP: [''],
            Other: [''],
            Systemic: [''],
            PA: [''],
            PV: [''],
            UrineProteins: [''],
            Sugar: [''],
            Microscopy: [''],
            BloodHB: [''],
            TLC: [''],
            P: [''],
            L: [''],
            E: [''],
            ESR: [''],
            BloodSugar: [''],
            BloodGroup: [''],
            Ultrasound: [''],
            UltrasoundRemark: [''],
            XRay: [''],
            XRayRemark: [''],
            Conservative: [''],
            Operation: [''],
            Date: [''],
            Surgeon: [''],
            Assistant: [''],
            Anaesthesia: [''],
            Incision: [''],
            OperativeFinding: [''],
            OprationNotes: [''],
            OPMedicines: [''],
            OPProgress: [''],
            ConditionDischarge: [''],
            RemovalDate: [''],
            ConditionWound: [''],
            AdviseMedicine: [''],
            FollowUpDate: ['']
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.model.AdmissionId = this.AdmitId;
            this.model.PatientId = this.PatientId;
            this._AdmitService.SaveOrUpdateDischargeRpt(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    Delete(id: any) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AdmitService.DeleteVital(id).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                this.loader.HideLoader();
            });
        }
    }

}
