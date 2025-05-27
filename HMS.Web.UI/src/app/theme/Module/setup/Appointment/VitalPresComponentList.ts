import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { emr_patient, emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo } from './../Appointment/AppointmentModel';
import { Observable } from 'rxjs';
import { AppointmentService } from './../Appointment/AppointmentService';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { GlobalVariable } from '../../../../AngularConfig/global';
@Component({
    templateUrl: './VitalPresComponentList.html',
    moduleId: module.id,
    providers: [AppointmentService],
})

export class VitalPresComponentList implements OnInit {
    public Form4: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string; public DocumentImage: string = '';
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public filterdData = []; public PatientVitalList: any[] = [];
    public IsSameModuleName: string;
    public IsChecked: boolean;
    public IsEmpExist: boolean = false; public ID: number = 10;
    public IsAdmin: boolean = false;
    public model = new emr_patient();
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public PatientId: any;
    public sub: any;
    public IsEdit: boolean = false; public IsVital: boolean = false;
    public CompanyInfo: any[] = [];
    public emr_vital_dynamicArray = []; 
    public VitalList: any = []; public CompanyObj: any;
    public VitalModel = new emr_vital();
    public ClinicName: string;
    public Rights: any;
    public ControlRights: any;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AppointmentService: AppointmentService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("24");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {
        
        this.PatientId = localStorage.getItem('PatientId');
        this.loadVital();
    }
    onFollowUpDateChanged(event: IMyDateModel) {
        if (event) {
        }
    }
    //Vital 
    VitalChange(id: any) {
        this.emr_vital_dynamicArray = [];
        this.loader.ShowLoader();
        this._AppointmentService.VitalGetById(id).then(m => {
            if (m.IsSuccess) {
                if (m.ResultSet.vitalObj.length > 0) {
                    m.ResultSet.vitalObj.forEach((item, index) => {
                        this.emr_vital_dynamicArray.push(item);
                    });
                }
            } else
                this.toastr.Error('Error', m.ErrorMessage);

            var vital = this.VitalList.filter(a => a.ID == id)[0];
            if (vital != null && vital != undefined) {
                this.VitalModel.Unit = vital.Unit;
                this.VitalModel.Name = vital.Value;
            }
            this.loader.HideLoader();
        });
    }
    loadVital() {
        this.emr_vital_dynamicArray = [];
        this.loader.ShowLoader();
        this.IsVital = true;
        this._AppointmentService.GetAllVital().then(m => {
            this.VitalList = m.ResultSet.VitalList;
            if (m.ResultSet.vitalObj.length > 0) {
                m.ResultSet.vitalObj.forEach((item, index) => {
                    this.emr_vital_dynamicArray.push(item);
                });
            }
            var vital = this.VitalList.filter(a => a.ID == 67)[0];
            if (vital != null && vital != undefined) {
                this.VitalModel.Unit = vital.Unit;
            }
            this.loader.HideLoader();
        });
        this.VitalModel.Date = new Date();
        this.VitalModel.VitalId = 67;
        this.VitalModel.PatientId = this.PatientId;

    }
    AddVital() {
        var obj = this.VitalModel;
        if (obj.VitalId == 0 || obj.VitalId == undefined) {
            this.toastr.Error("Error", "Please enter vital name.");
            return;
        }
        if (obj.Measure == null || obj.Measure == undefined) {
            this.toastr.Error("Error", "Please enter measure value.");
            return;
        }
        if ((obj.VitalId == 68) && (obj.Measure == undefined || obj.Measure2 == undefined)) {
            this.toastr.Error("Error", "Please enter measure value.");
            return;
        }

        if (this.VitalModel.Measure2 != null && this.VitalModel.Measure2 != undefined)
            this.VitalModel.Measure = this.VitalModel.Measure + "/" + this.VitalModel.Measure2;
        this.loader.ShowLoader();
        this._AppointmentService.VitalSave(this.VitalModel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                if (result.ResultSet.vitalObj.length > 0) {
                    this.emr_vital_dynamicArray = [];
                    result.ResultSet.vitalObj.forEach((item, index) => {
                        this.emr_vital_dynamicArray.push(item);
                    });
                }
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
            this.VitalModel = new emr_vital();
            this.VitalModel.Date = new Date();
            this.VitalModel.PatientId = this.PatientId;
        });
    }
    DeleteVital(id: any, VitalId: any) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AppointmentService.DeleteVital(id, VitalId).then(m => {
                if (m.ErrorMessage != null) {
                    alert(m.ErrorMessage);
                }
                if (m.ResultSet.vitalObj.length > 0) {
                    this.emr_vital_dynamicArray = [];
                    m.ResultSet.vitalObj.forEach((item, index) => {
                        this.emr_vital_dynamicArray.push(item);
                    });
                }
                this.loader.HideLoader();
            });
        }
    }
    
}
