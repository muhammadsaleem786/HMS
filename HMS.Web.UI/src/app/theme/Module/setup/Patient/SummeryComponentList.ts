import { Component, OnInit, ViewChild, ElementRef, TemplateRef, Input, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { PatientService } from './PatientService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { emr_patient, emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo } from './../Appointment/AppointmentModel';
import { emr_patient_bill } from '../Billing/BillingModel';
import { emr_prescription_mf, emr_prescription_complaint, emr_prescription_diagnos, emr_prescription_investigation, emr_prescription_observation, emr_prescription_treatment, emr_prescription_treatment_template, emr_medicine } from './../Prescription/PrescriptionModel';
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
    templateUrl: './SummeryComponentList.html',
    moduleId: module.id,
    providers: [PatientService, AppointmentService],
})

export class SummeryComponentList implements OnInit {
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
    public sub: any; public IsLoadDocument: boolean = false;
    public IsEdit: boolean = false; public IsVital: boolean = false;
    public CompanyInfo: any[] = [];
    public Form7: FormGroup; public IsLoadBill: boolean = false;
    public BillModel = new emr_patient_bill();
    public DocumentList: any[] = []; public GenderList: any[] = [];
    public BillingList: any[] = [];
    public emr_vital_dynamicArray = []; public IsPrevious: boolean = false;
    public VitalList: any = [];
    public MedicineModel = new emr_medicine(); public BloodList: any[] = [];
    public VitalModel = new emr_vital(); public FutureAppList: any[] = []; public PreviousAppList: any[] = [];
    public Docmodel = new emr_document(); public IsNewPatientImage: boolean = true; public IsNewImage: boolean = true;
    public PatientId: any; public PatientImage: string = '';

    public ClinicName: string = "";
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
    public DoctorInfo = new DoctorInfo();
    public PatientRXmodel = new patientInfo();
    public IsPatient: boolean = false;
    public PaidAmount: number = 0; public CompanyObj: any;
    public OutStdAmount: number = 0;
    public Rights: any;
    public ControlRights: any;
    @ViewChild("PrintRx") PrintRxContent: TemplateRef<any>;
    constructor(public _fb: FormBuilder,
        public loader: LoaderService
        , public _PatientService: PatientService,
        public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public _CommonService: CommonService, public route: ActivatedRoute, public _router: Router, public _AppointmentService: AppointmentService, private modalService: NgbModal) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("11");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.ClinicName = this.CompanyObj.CompanyName;
    }
    ngOnInit() {
        this.PatientId = localStorage.getItem('PatientId');
        this.LoadPatient();

        this.Form7 = this._fb.group({
            PatientName: ['', [Validators.required]],
            Gender: ['', [Validators.required]],
            DOB: [''],
            Age: ['', [Validators.required]],
            Email: [''],
            ContactNo:[''],
            Mobile: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.NumberPattern), Validators.minLength(12), Validators.maxLength(12)]],
            CNIC: ['', [Validators.required, Validators.pattern(ValidationVariables.CNICPattern)]],
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

    }
    loadVital() {
        this.IsPatient = false;
        this._router.navigate(['home/Summary/Vital'], { queryParams: { id: this.PatientId } });
    }
    ShowAppointment() {
        this.IsPatient = false;
        this._router.navigate(['home/Summary/AppointSummery'], { queryParams: { id: this.PatientId } });
    }
    LoadBillList() {
        this.IsPatient = false;
        this._router.navigate(['home/Summary/BillSummary'], { queryParams: { id: this.PatientId } });
    }
    LoadDocuments() {
        this.IsPatient = false;
        this._router.navigate(['home/Summary/DocSummery'], { queryParams: { id: this.PatientId } });
    }
    LoadPatient() {
        this.loader.ShowLoader();
        this._AppointmentService.PatientLoadById(this.PatientId).then(m => {
            this.IsPatient = true;
            if (m.ResultSet.PaidAndOutamount.length > 0)
                this.OutStdAmount = m.ResultSet.PaidAndOutamount[0].OutAmount;
            this.BloodList = m.ResultSet.BloodList;
            this.GenderList = m.ResultSet.GenderList;
            this.MedicineList = m.ResultSet.MedicineList;
            this.BillTypeList = m.ResultSet.BillTypeList;
            this.TittleList = m.ResultSet.TittleList;
            this.model = m.ResultSet.patientInfo[0];
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
    HomePage() {
        this.IsPatient = true;
    }
    AgeChange() {
        this.model.DOB = null;
    }
    PatientSaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._PatientService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['home/Summary']);
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }

    PrintRxform(id: any) {
        this.loader.ShowLoader();
        this._AppointmentService.PrintRXById(id).then(m => {
            if (m.IsSuccess) {
                this.emr_prescription_complaint_dynamicArray = [];
                this.emr_prescription_observation_dynamicArray = [];
                this.emr_prescription_investigation_dynamicArray = [];
                this.emr_prescription_diagnos_dynamicArray = [];
                this.emr_prescription_treatment_dynamicArray = [];
                this.PatientVitalList = [];

                if (m.ResultSet.doctor != null)
                    this.DoctorInfo = m.ResultSet.doctor;

                if (this.DoctorInfo == null || this.DoctorInfo == undefined) {
                    this.DoctorInfo.Name = "";
                    this.DoctorInfo.Designation = "";
                    this.DoctorInfo.Qualification = "";
                    this.DoctorInfo.PhoneNo = "";
                }
                this.PatientRXmodel.PatientName = m.ResultSet.result.PatientName;
                if (m.ResultSet.result.ClinicIogo != null)
                    this.PatientRXmodel.ClinicIogo = GlobalVariable.BASE_Temp_File_URL + '' + m.ResultSet.result.ClinicIogo;
                this.PatientRXmodel.Age = m.ResultSet.result.Age;
                this.PatientRXmodel.AppointmentDate = this._CommonService.GetFormatDate(m.ResultSet.result.AppointmentDate);
                this.PatientVitalList = m.ResultSet.vitallist;
                if (m.ResultSet.result.emr_prescription_complaint != null) {
                    m.ResultSet.result.emr_prescription_complaint.forEach((item, index) => {
                        this.emr_prescription_complaint_dynamicArray.push(item);
                    });
                }
                if (m.ResultSet.result.emr_prescription_diagnos != null) {
                    m.ResultSet.result.emr_prescription_diagnos.forEach((item, index) => {
                        this.emr_prescription_diagnos_dynamicArray.push(item);
                    });
                }
                if (m.ResultSet.result.emr_prescription_investigation != null) {
                    m.ResultSet.result.emr_prescription_investigation.forEach((item, index) => {
                        this.emr_prescription_investigation_dynamicArray.push(item);
                    });
                }
                if (m.ResultSet.result.emr_prescription_observation != null) {

                    m.ResultSet.result.emr_prescription_observation.forEach((item, index) => {
                        this.emr_prescription_observation_dynamicArray.push(item);
                    });
                }
                if (m.ResultSet.result.emr_prescription_treatment != null) {

                    m.ResultSet.result.emr_prescription_treatment.forEach((item, index) => {
                        var medicineName = this.MedicineList.filter(a => a.Id == item.MedicineId)[0];
                        if (medicineName != null && medicineName != undefined)
                            item.MedicineName = medicineName.Medicine;
                        else
                            item.MedicineName = item.MedicineName;
                        this.emr_prescription_treatment_dynamicArray.push(item);
                    });
                }
                this.modalService.open(this.PrintRxContent, { size: 'lg' });
            } else
                this.toastr.Error('Error', m.ErrorMessage);
            this.loader.HideLoader();
        });
    }
}

