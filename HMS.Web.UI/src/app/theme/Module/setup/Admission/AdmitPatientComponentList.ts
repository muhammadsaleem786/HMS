import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { emr_patient, ipd_admission_vital, ipd_admission_charges } from './AdmitModel';
import { Observable } from 'rxjs';
import { AdmitService } from './AdmitService';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { CommonService } from '../../../../CommonService/CommonService';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { AppointmentService } from './../Appointment/AppointmentService';

@Component({
    templateUrl: './AdmitPatientComponentList.html',
    moduleId: module.id,
    providers: [AdmitService, AppointmentService],
})

export class AdmitPatientComponentList implements OnInit {
    public Form7: FormGroup;
    public submitted: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public Id: string; public DocumentImage: string = '';
    public IsReadOnly = false;
    public ScreenLists = [];
    public Modules = [];
    public IsCNICMandatory: any;
    public ID: number = 10;
    public IsAdmin: boolean = false;
    public model = new emr_patient();
    public PayrollRegion: string; public PModel = new PaginationModel(); public PConfig = new PaginationConfig();
    public Keywords: any[] = [];
    public AdmitId: any;
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public ClinicName: string;
    public Rights: any;
    public ChargeRights: any;
    public PatientId: any;
    public emr_prescription_dynamicArray = [];
    public emr_prescription_complaint_dynamicArray = [];
    public emr_prescription_diagnos_dynamicArray = [];
    public emr_prescription_investigation_dynamicArray = [];
    public emr_prescription_observation_dynamicArray = [];
    public emr_prescription_MedicineArray = [];
    public emr_prescription_treatment_dynamicArray = [];
    public MedicineList: any[] = [];
    public BillTypeList: any[] = [];
    public TittleList: any[] = [];
    public PrimaryList: any[] = [];
    public SecondaryList: any[] = [];
    public AllergyList: any[] = [];
    public GenderName: string = ""; public CompanyObj: any;
    public PaidAmount: number = 0;
    public OutStdAmount: number = 0;
    public GenderList: any[] = [];
    public GenderIds: any;
    public BloodList: any[] = [];
    public IsNewPatientImage: boolean = true;
    public IsNewImage: boolean = true;
    public PatientImage: string = '';
    public VisitRights: any;
    nextPage: number = 1;
    totalPages: number = 0; public InvoiceBillModel: any[] = [];
    pagesRange: number[] = []; previousPage: number = 1;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService,
        public commonservice: CommonService, public _AppointmentService: AppointmentService
        , public toastr: CommonToastrService, public _CommonService: CommonService, private encrypt: EncryptionService, public route: ActivatedRoute, public _router: Router, public _AdmitService: AdmitService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.VisitRights = this._CommonService.ScreenRights("39");
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {
        this.AdmitId = localStorage.getItem('AdmitId');
        this.PatientId = localStorage.getItem('PatientId');
        this.LoadPatient();
        this.Form7 = this._fb.group({
            PatientName: ['', [Validators.required]],
            Gender: ['', [Validators.required]],
            DOB: [''],
            Age: ['', [Validators.required]],
            Email: [''],
            Mobile: ['', [Validators.required]],
            CNIC: [''],
            Image: [''],
            Notes: [''],
            MRNO: [''],
            BloodGroupId: [''],
            ContactNo: [''],
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

    }
    LoadPatient() {
        this.loader.ShowLoader();
        this._AppointmentService.AdmitPatientLoadById(this.PatientId, this.AdmitId).then(m => {
            if (m.ResultSet.PaidAndOutamount.length > 0)
                this.OutStdAmount = m.ResultSet.PaidAndOutamount[0].OutAmount;
            this.BloodList = m.ResultSet.BloodList;
            if (this.GenderIds != null) {
                this.GenderList = m.ResultSet.GenderList.filter(x => this.GenderIds.includes(x.ID));
                if (this.GenderList.length == 1)
                    this.model.Gender = this.GenderList[0].ID;
            }
            else
                this.GenderList = m.ResultSet.GenderList;

            this.MedicineList = m.ResultSet.MedicineList;
            this.BillTypeList = m.ResultSet.BillTypeList;
            this.TittleList = m.ResultSet.TittleList;
            this.model = m.ResultSet.patientInfo[0];
            this.PrimaryList = m.ResultSet.Diagnosislist.filter(a => a.IsType == "P");
            this.SecondaryList = m.ResultSet.Diagnosislist.filter(a => a.IsType == "S");
            this.AllergyList = m.ResultSet.Diagnosislist.filter(a => a.IsType == "A");
            this.model.AdmissionNo = m.ResultSet.AdmissionNo;
            if (this.model.Gender == 1)
                this.GenderName = 'Male';
            if (this.model.Gender == 2)
                this.GenderName = 'Female';
            if (this.model.Gender == 3)
                this.GenderName = 'Other';
            if (this.model.Image != null || this.model.Image != undefined) {
                this.getPatientImageUrlName(this.model.Image);
                this.IsNewPatientImage = false;
            } else this.IsNewPatientImage = true;
            this.emr_prescription_dynamicArray = [];
            this.emr_prescription_complaint_dynamicArray = [];
            this.emr_prescription_observation_dynamicArray = [];
            this.emr_prescription_investigation_dynamicArray = [];
            this.emr_prescription_diagnos_dynamicArray = [];
            this.emr_prescription_MedicineArray = [];

            this.emr_prescription_dynamicArray = m.ResultSet.prescriptionresult;
            //this.IsPatient = true;
            //this.IsVitals = false;
            //this._router.navigate(['home/AdmitSummary/AdmitNote']);
            this.loader.HideLoader();
        });
    }
    getPatientImageUrlName(FName) {
        this.model.Image = FName;
        if (this.IsEdit && !this.IsNewImage) {
            this.PatientImage = GlobalVariable.BASE_File_URL + '' + FName;
        } else {
            this.PatientImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
    }


    AgeChange() {
        this.model.DOB = null;
    }
    PatientSaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            if (this.IsCNICMandatory && (this.model.CNIC == null || this.model.CNIC == "")) {
                this.toastr.Error("Error", "Please enter cnic.");
                return;
            }
            this.loader.ShowLoader();
            this._AdmitService.PatientSaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['home/AdmitSummary']);
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }
}
