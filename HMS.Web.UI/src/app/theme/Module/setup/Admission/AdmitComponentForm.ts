import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener, TemplateRef } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { emr_patient, ipd_admission } from './AdmitModel';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { AdmitService } from './AdmitService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { IMyDateModel } from 'mydatepicker';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { debounce } from 'lodash';
import { forEach } from '@angular/router/src/utils/collection';
import * as moment from 'moment';
declare var $: any;
@Component({
    selector: 'setup-AdmitComponentForm',
    templateUrl: './AdmitComponentForm.html',
    moduleId: module.id,
    providers: [AdmitService],
})
export class AdmitComponentForm implements OnInit {
    public Form1: FormGroup;
    public Form2: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public filterdData = [];
    public IsSameModuleName: string;
    public IsChecked: boolean;
    public IsEmpExist: boolean = false;
    public IsAdmin: boolean = false;
    public IsUpdateText: boolean = false;
    public model = new emr_patient();
    public modelAdmission = new ipd_admission();
    public PayrollRegion: string;
    public GenderList: any[] = [];
    public StatusList: any[] = [];
    public DoctorList: any[] = [];
    public BloodList: any[] = [];
    public AdmissionTypeList: any[] = [];
    public BillTypeList: any[] = [];
    public TittleList: any[] = [];
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public PatientImage: string = '';
    public IsNewImage: boolean = true;
    public CompanyInfo: any[] = []; public patientInfo: any;
    public Rights: any;
    public ControlRights: any;
    @ViewChild("content") PatientContent: TemplateRef<any>;
    openNewPatient(content) {
        this.loader.ShowLoader();
        this._AdmitService.GetEMRNO().then(m => {
            this.model = new emr_patient();
            this.model.MRNO = m.ResultSet.MRNO;
            this.BillTypeList = m.ResultSet.BillTypeList;
            this.TittleList = m.ResultSet.TittleList;
            this.model.BillTypeId = 1;
            this.model.PrefixTittleId = 1;
            this.loader.HideLoader();
            this.modalService.open(content, { size: 'lg' });
        });
    }
    constructor(public _CommonService: CommonService, public _fb: FormBuilder, public loader: LoaderService
        , public _AdmitService: AdmitService, public commonservice: CommonService, private modalService: NgbModal
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("11");
    }
    ngOnInit() {
        this.loadDropdown();
        this.Form1 = this._fb.group({
            PatientName: [''],
            Gender: [''],
            DOB: [''],
            Age: [''],
            Email: [''],
            Mobile: [''],
            ContactNo: [''],
            CNIC: [''],
            Image: [''],
            Notes: [''],
            MRNO: [''],
            BloodGroupId: [''],
            BloodGroupDropDownId: [''],
            EmergencyNo: [''],
            Address: [''],
            ReferredBy: [''],
            AnniversaryDate: [''],
            Illness_Diabetes: [''],
            Illness_Tuberculosis: [''],
            Illness_HeartPatient: [''],
            Illness_LungsRelated: [''],
            Illness_BloodPressure: [''],
            Illness_Migraine: [''],
            Illness_Other: [''],
            Allergies_Food: [''],
            Allergies_Drug: [''],
            Allergies_Other: [''],
            Habits_Smoking: [''],
            Habits_Drinking: [''],
            Habits_Tobacco: [''],
            Habits_Other: [''],
            MedicalHistory: [''],
            CurrentMedication: [''],
            HabitsHistory: [''],
            BillTypeId: [''],
            PrefixTittleId: [''],
        });
        this.Form2 = this._fb.group({
            PatientName: ['', [Validators.required]],
            Mobile: [''],
            CNIC: [''],
            AppointmentId: [''],
            AdmissionNo: [''],
            PatientId: [''],
            AdmissionTypeId: [''],
            AdmissionTypeDropdownId: [''],
            DoctorId: [''],
            AdmissionDate: [''],
            AdmissionTime: [''],
            Location: [''],
            ReasonForVisit: [''],

            Gender: [''],
            DOB: [''],
            Age: [''],
            Email: [''],
            ContactNo: [''],
            Image: [''],
            Notes: [''],
            MRNO: [''],
            BloodGroupId: [''],
            BloodGroupDropDownId: [''],
            EmergencyNo: [''],
            Address: [''],
            ReferredBy: [''],
            AnniversaryDate: [''],
            Illness_Diabetes: [''],
            Illness_Tuberculosis: [''],
            Illness_HeartPatient: [''],
            Illness_LungsRelated: [''],
            Illness_BloodPressure: [''],
            Illness_Migraine: [''],
            Illness_Other: [''],
            Allergies_Food: [''],
            Allergies_Drug: [''],
            Allergies_Other: [''],
            Habits_Smoking: [''],
            Habits_Drinking: [''],
            Habits_Tobacco: [''],
            Habits_Other: [''],
            MedicalHistory: [''],
            CurrentMedication: [''],
            HabitsHistory: [''],
            BillTypeId: [''],
            PrefixTittleId: [''],
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.id = params.id;
                if (this.id > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this.loader.ShowLoader();
                    this._AdmitService.GetById(this.id).then(m => {
                        this.modelAdmission = m.ResultSet.obj;
                        this.modelAdmission.AdmissionTime = moment(m.ResultSet.obj.AdmissionTime, "HH:mm:ss").format("LT");
                        this.modelAdmission.emr_patient_mf = m.ResultSet.obj.emr_patient_mf;
                        this.modelAdmission.PatientName = this.modelAdmission.emr_patient_mf.PatientName;
                        this.modelAdmission.PatientId = this.modelAdmission.emr_patient_mf.ID;
                        this.modelAdmission.CNIC = this.modelAdmission.emr_patient_mf.CNIC;
                        this.modelAdmission.Mobile = this.modelAdmission.emr_patient_mf.Mobile;
                        const keysToUpdate = [
                            'BloodGroupId',
                            'AnniversaryDate',
                            'ContactNo',
                            'EmergencyNo',
                            'Address',
                            'Notes',
                            'ReferredBy',
                            'Illness_Diabetes',
                            'Illness_Tuberculosis',
                            'Illness_HeartPatient',
                            'Illness_BloodPressure',
                            'Illness_Migraine',
                            'Illness_LungsRelated',
                            'Illness_Other',
                            'Allergies_Food',
                            'Allergies_Drug',
                            'Allergies_Other',
                            'Habits_Smoking',
                            'Habits_Drinking',
                            'Habits_Tobacco',
                            'Habits_Other',
                            'MedicalHistory',
                            'CurrentMedication',
                            'HabitsHistory'
                        ];
                        const formValuesToUpdate = {};
                        keysToUpdate.forEach(key => {
                            formValuesToUpdate[key] = this.modelAdmission.emr_patient_mf[key];
                        });
                        this.Form2.patchValue(formValuesToUpdate);
                        this.loader.HideLoader();
                    });

                }
            });
    }
    loadDropdown() {
        this.loader.ShowLoader();
        this._AdmitService.FormLoad().then(m => {
            if (m.IsSuccess) {
                this.GenderList = m.ResultSet.GenderList;
                this.DoctorList = m.ResultSet.DoctorList;
                this.StatusList = m.ResultSet.AllStatusList;
                this.BloodList = m.ResultSet.BloodList;
                this.BillTypeList = m.ResultSet.BillTypeList;
                this.TittleList = m.ResultSet.TittleList;
                this.AdmissionTypeList = m.ResultSet.AdmissionTypeList;
                if (this.IsEdit == false) {
                    this.model.MRNO = m.ResultSet.MRNO;
                    this.model.BillTypeId = 1;
                    this.model.PrefixTittleId = 1;
                    this.modelAdmission.AdmissionNo = m.ResultSet.AdmissionNo;
                    this.modelAdmission.AdmissionDate = new Date();
                    this.SelectTime();
                }
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    SelectTime() {
        let Step = 30;
        const currentTime = new Date();
        const hours = currentTime.getHours();
        const minutes = currentTime.getMinutes();
        const timeString = this.formatTime(hours, minutes);
        this.modelAdmission.AdmissionTime = timeString;
        $("#TimeRecorded").timepicker({
            'step': Step,
            'timeFormat': 'h:i A',
            'defaultTime': timeString
        });
    }
    formatTime(hours: number, minutes: number): string {
        const ampm = hours >= 12 ? 'PM' : 'AM';
        const hour = hours % 12 || 12;  // Convert to 12-hour format
        const minute = minutes < 10 ? '0' + minutes : minutes;
        return hour + ':' + minute + ' ' + ampm;
    }
    formatAMPM(time: any) {
        var hours = time.split(':')[0];
        var minutes = time.split(':')[1];
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        //minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            if (this.modelAdmission.AdmissionTime == undefined || this.modelAdmission.AdmissionTime == null) {
                this.toastr.Error("Error", "Please select time.");
                return;
            }
            this.loader.ShowLoader();
            if (this.modelAdmission.AdmissionTime == undefined)
                this.modelAdmission.AdmissionTime = $('#basicExample').val().toString();
            this.modelAdmission.AdmissionTime = this.Convert12TO24(this.modelAdmission.AdmissionTime);
            const formValues = this.Form2.value;
            const keysToUpdate = [
                'BloodGroupId',
                'AnniversaryDate',
                'ContactNo',
                'EmergencyNo',
                'Address',
                'Notes',
                'ReferredBy',
                'Illness_Diabetes',
                'Illness_Tuberculosis',
                'Illness_HeartPatient',
                'Illness_BloodPressure',
                'Illness_Migraine',
                'Illness_LungsRelated',
                'Illness_Other',
                'Allergies_Food',
                'Allergies_Drug',
                'Allergies_Other',
                'Habits_Smoking',
                'Habits_Drinking',
                'Habits_Tobacco',
                'Habits_Other',
                'MedicalHistory',
                'CurrentMedication',
                'HabitsHistory'
            ];
            const illnessKeys = [
                'Illness_Diabetes',
                'Illness_Tuberculosis',
                'Illness_HeartPatient',
                'Illness_BloodPressure',
                'Illness_Migraine',
                'Illness_LungsRelated',
                'Illness_Other'
            ];
            keysToUpdate.forEach(key => {
                if (this.modelAdmission.emr_patient_mf.hasOwnProperty(key)) {
                    if (illnessKeys.includes(key)) {
                        this.modelAdmission.emr_patient_mf[key] = formValues[key] === '' ? false : formValues[key];
                    } else {
                        this.modelAdmission.emr_patient_mf[key] = formValues[key];
                    }
                }
            });
            this._AdmitService.SaveOrUpdate(this.modelAdmission).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['home/Admit']);
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    Convert12TO24(time12h: any) {
        var hours = Number(time12h.match(/^(\d+)/)[1]);
        var minutes = Number(time12h.match(/:(\d+)/)[1]);
        var AMPM = time12h.match(/\s(.*)$/)[1];
        if (AMPM == "PM" && hours < 12) hours = hours + 12;
        if (AMPM == "AM" && hours == 12) hours = hours - 12;
        var sHours = hours.toString();
        var sMinutes = minutes.toString();
        return hours + ':' + minutes + ':00';
    }
    AgeChange() {
        this.model.DOB = null;
    }
    onDOBChanged(event: IMyDateModel) {
        if (event) {
            //var  ageCalc= new Date();
            var dob = new Date(this.model.DOB)
            var CurrentDate = new Date();
            var ageCalc = CurrentDate.getFullYear() - dob.getFullYear();
            this.model.Age = ageCalc;
            //this.getValidDOBDate(this.model.DOB);
        }
    }
    onAnniversaryDateChanged(event: IMyDateModel) {
        if (event) {
        }
    }
    IsNewImageEvent(FName) {
        this.IsNewImage = true;
    }
    getFileName(FName) {
        this.model.Image = FName;
    }
    ClearImageUrl() {
        this.IsNewImage = true;
        this.model.Image = '';
        this.PatientImage = '';
    }
    getImageUrlName(FName) {
        this.model.Image = FName;
        if (this.IsEdit && !this.IsNewImage) {
            this.PatientImage = GlobalVariable.BASE_File_URL + '' + FName;
        } else {
            this.PatientImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._AdmitService.Delete(this.modelAdmission.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['home/Admit']);
                this.loader.HideLoader();
            });
        }
    }
    LoadPatient() {
        //Search By Name
        $('#PatientSearchByName').autocomplete({
            source: debounce((request, response) => {
                this.loader.ShowLoader();
                this._AdmitService.searchByName(request.term).then(m => {
                    this.patientInfo = m.ResultSet.PatientInfo;
                    response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            }, 300),
            minLength: 3,
            select: (event, ui) => {
                this.modelAdmission.PatientName = ui.item.label;
                this.modelAdmission.PatientId = ui.item.value;
                this.modelAdmission.CNIC = this.patientInfo.filter(a => a.value == ui.item.value)[0].CNIC;
                this.modelAdmission.Mobile = this.patientInfo.filter(a => a.value == ui.item.value)[0].Phone;
                this._AdmitService.getPatientById(ui.item.value).then(m => {
                    if (m.IsSuccess) {
                        this.modelAdmission.emr_patient_mf = m.ResultSet.result[0].model;
                        this.loader.HideLoader();
                    } else
                        this.loader.HideLoader();
                });
            }
        });
    }
    LoadDoctor() {
        $("#DoctorSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AdmitService.DoctorSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.model.ReferredBy = ui.item.label;
                //this.emr_prescription_treatment_dynamicArray[indx].MedicineName = ui.item.label;

                return ui.item.label;
            }
        });
    }
    onPatientNameEvent(event: any) {
        if (event != null && event != undefined) {
            var patientList = this.patientInfo.filter(a => a.label == event);
            if (patientList.length == 0 || patientList.length == null) {
                swal({
                    title: "Are you sure?",
                    text: "Are you sure to add new patient.",
                    icon: "warning",
                    buttons: ['Cancel', 'Yes'],
                    dangerMode: true,
                })
                    .then((willDelete) => {
                        if (willDelete) {

                            this.model.PatientName = event;
                            this.modalService.open(this.PatientContent, { size: 'lg' });
                        }
                    });
            }
        }
    }
    PatientSaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (this.model.PatientName == undefined || this.model.PatientName == "") {
            this.toastr.Error('Error', 'Patent name is mandatory');
            isValid = false;
            return;
        }
        if (this.model.Gender == undefined || this.model.Gender == null) {
            this.toastr.Error('Error', 'Gender is mandatory');
            isValid = false;
            return;
        }
        if (this.model.Age == undefined || this.model.Age == null) {
            this.toastr.Error('Error', 'Age is mandatory');
            isValid = false;
            return;
        }
        if (this.model.CNIC == undefined || this.model.CNIC == "") {
            this.toastr.Error('Error', 'CNIC is mandatory');
            isValid = false;
            return;
        }
        if (this.model.Mobile == undefined || this.model.Mobile == "") {
            this.toastr.Error('Error', 'Mobile is mandatory');
            isValid = false;
            return;
        }
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._AdmitService.PatientSaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modelAdmission.emr_patient_mf = result.ResultSet.CurrentModel;
                    this.modelAdmission.emr_patient_mf.ID = result.ResultSet.CurrentModel.ID;
                    this.modelAdmission.PatientName = result.ResultSet.CurrentModel.PatientName;
                    this.modelAdmission.PatientId = result.ResultSet.CurrentModel.ID;
                    this.modelAdmission.CNIC = result.ResultSet.CurrentModel.CNIC;
                    this.modelAdmission.Mobile = result.ResultSet.CurrentModel.Mobile;
                    this.modalService.dismissAll(this.PatientContent);
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
    Close() {
        this.model = new emr_patient();
        this.submitted = false;
        this.IsEmpExist = false;
        this.IsAdmin = false;
        this.IsUpdateText = false;
    }
}
