import { Component, OnInit, ViewChild, ElementRef, TemplateRef, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AppointmentService } from './AppointmentService';
import { emr_patient, emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo, InvoiceCompanyInfo, emr_notes_favorite } from './AppointmentModel';
import { emr_prescription_mf, emr_prescription_complaint, emr_prescription_diagnos, emr_prescription_investigation, emr_prescription_observation, emr_prescription_treatment, emr_prescription_treatment_template, emr_medicine } from './../Prescription/PrescriptionModel';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../CommonService/CommonService';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { emr_patient_bill } from '../Billing/BillingModel';
import { NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { GlobalVariable } from '../../../../AngularConfig/global';
import * as moment from 'moment/moment';
import { IMyDateModel } from 'mydatepicker';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { ApplicationUserModel } from '../../setup/Setting/ApplicationUser/ApplicationUserModel';
import { ApplicationUserService } from '../../setup/Setting/ApplicationUser/ApplicationUserService';
import { IMultiSelectOption, IMultiSelectSettings, IMultiSelectTexts } from 'angular-2-dropdown-multiselect';
import { BillingService } from './../Billing/BillingService';
import { PatientService } from './../Patient/PatientService'; import { ActivatedRoute } from '@angular/router';
import { debug } from 'util'; import { filter } from 'rxjs/operators';
import { forEach } from '@angular/router/src/utils/collection';
import { interval, Subscription } from 'rxjs';
import { SaleService } from '../Items/Sale/SaleService'
declare var $: any;
import swal from 'sweetalert';
import { debounce } from 'lodash';
import { EncryptionService } from '../../../../CommonService/encryption.service';
@Component({
    moduleId: module.id,
    templateUrl: 'AppointmentComponentList.html',
    providers: [AppointmentService, ApplicationUserService, BillingService, PatientService, SaleService],
})
export class AppointmentComponentList implements OnInit {
    mySubscription: Subscription;
    hour: number;
    minute: number;
    second: number;
    time = { hour: 13, minute: 30 };
    hourStep = 1;
    minuteStep = 15;
    meridian = true;
    closeResult = '';
    PatientDetailInfo: boolean = false;
    toggleMeridian() {
        this.meridian = !this.meridian;
    }
    public model = new emr_patient();
    public Docmodel = new emr_document();
    public AppointmentModel = new emr_Appointment();
    public MedicineModel = new emr_medicine();
    public VitalModel = new emr_vital();
    public Form1: FormGroup;
    public Form2: FormGroup;
    public Form3: FormGroup;
    public Form4: FormGroup;
    public Form5: FormGroup;
    public Form6: FormGroup;
    public Form7: FormGroup;
    public Form8: FormGroup;
    public Form9: FormGroup;
    public submitted: boolean;
    public ActiveToggle: boolean = false;
    public Id: string;
    public PModel = new PaginationModel();
    public FavoriteModel = new emr_notes_favorite();
    public PatientId: any;
    public PConfig = new PaginationConfig();
    public AppointmentList: any[] = [];
    public GenderList: any[] = [];
    public UnitList: any[] = [];
    public MadicineTypeList: any[] = [];
    public TemplateList: any[] = [];
    public MedicineList: any[] = [];
    public PatientVitalList: any[] = [];
    public DoctorList: any[] = [];
    public DoctorClanderList: any[] = [];
    public FutureAppList: any[] = [];
    public PreviousAppList: any[] = [];
    public BillTypeList: any[] = [];
    public TittleList: any[] = [];
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public ControlRights: any;
    public DoctAddRights: any;
    public PatientControlRights: any;
    public IsEdit: boolean = false;
    public IsLoadDocument: boolean = false;
    public IsLoadBill: boolean = false;
    public IsPrevious: boolean = false;
    public Keywords: any[] = [];
    public PatientImage: string = '';
    public DocumentImage: string = '';
    public IsNewImage: boolean = true;
    public IsNewPatientImage: boolean = true;
    public IsVital: boolean = false;
    public PatientList: any[] = [];
    public ClinicList: any[] = [];
    public PatientNameList: any[] = [];
    public StatusList: any[] = [];
    public ReminderList: any[] = [];
    public Followuplist: any[] = [];
    public AllStatusList: any[] = [];
    public Events: any[];
    public DisabledSlot: any;
    public Resource: any[];
    public dow: any[];
    public dayofweek: any[] = [1, 2, 3, 4, 5, 6, 0];
    public AppointmentDate: Date;
    public StatusID: number = 0;
    public isDrop: boolean = false;
    public Step: number = 30;
    public RoleList = [];
    public DocumentTypeList = [];
    public WorkingdayList = [];
    public DocumentList: any[] = [];
    public BillingList: any[] = [];
    public BloodList: any[] = [];
    public CurntPrevDateList: any[] = [];
    public VitalList: any = [];
    public IsShowDoctorIds: any;
    public patientInfo: any;
    public ServiceType: any;
    public DoctorInfo = new DoctorInfo();
    public PatientRXmodel = new patientInfo();
    public PatientName: any;
    public TokenNo: any;
    //prescription
    public BillModel = new emr_patient_bill();
    public PrescriptionModel = new emr_prescription_mf();
    public complaintModel = new emr_prescription_complaint();
    public diagnosModel = new emr_prescription_diagnos();
    public investigationModel = new emr_prescription_investigation();
    public observationModel = new emr_prescription_observation();
    public emr_prescription_complaint_dynamicArray = [];
    public emr_prescription_diagnos_dynamicArray = [];
    public emr_prescription_investigation_dynamicArray = [];
    public emr_prescription_observation_dynamicArray = [];
    public emr_prescription_treatment_dynamicArray = [];
    public emr_vital_dynamicArray = [];
    public PrescriptionExist: boolean = false;
    public InvoiceBillModel: any[] = [];
    public SubTotal: number = 0;
    public Total: number = 0;
    public TotalDiscount: number = 0;
    public IsPatient: boolean = false;
    public isDayClik: boolean = false;
    public AppointId: number;
    public AppointDate: any;
    public RoleName: string;
    public IsPreviousApp: boolean = false;
    public InvoiceCompanyInfo = new InvoiceCompanyInfo();
    public Usermodel = new ApplicationUserModel(); public sub: any;
    public SpecialtyList = [];
    public ComplaintList: any[] = [];
    public ObservationList: any[] = [];
    public InvestigationsList: any[] = [];
    public DiagnosisList: any[] = [];
    public message: string = "";
    public GenderIds: any;
    public IsCNICMandatory: any;
    public SearhPatientId: any;
    public CompanyObj: any;
    public IsBackDatedAppointment: boolean;
    public RowNo: number;
    public DurationList: any[] = [];
    public InstructionsList: any[] = [];
    public UsersObj: any;
    public ColumnSettings: IMultiSelectSettings = {
        pullRight: false,
        enableSearch: true,
        checkedStyle: 'checkboxes',
        buttonClasses: 'btn btn-default',
        selectionLimit: 0,
        closeOnSelect: false,
        showCheckAll: true,
        showUncheckAll: true,
        dynamicTitleMaxItems: 3,
        displayAllSelectedText: true,
        maxHeight: '1000px',
    };
    public ColumnTexts: IMultiSelectTexts = {
        checkAll: 'Select all',
        uncheckAll: 'UnSelect all',
        checked: 'checked',
        checkedPlural: 'checked',
        searchPlaceholder: 'Search...',
        defaultTitle: 'All',
    };
    @ViewChild("longContent") ModelContent: TemplateRef<any>;
    @ViewChild("content") PatientContent: TemplateRef<any>;
    @ViewChild("PrintRx") PrintRxContent: TemplateRef<any>;
    @ViewChild("DocContent") DocContent: TemplateRef<any>;
    @ViewChild("EmailModal") EmailModal: TemplateRef<any>;
    @ViewChild("Confirmation") ModelConfirmation: TemplateRef<any>;
    @ViewChild("MedicineTemplate") MedicineTemplateContent: TemplateRef<any>;
    @ViewChild("MedicinePresModal") MedicinePresModal: TemplateRef<any>;
    myControl = new FormControl();
    myNameControl = new FormControl();
    openAppointmentModal(longContent) {
        this.AppointmentModel = new emr_Appointment();
        this.modalService.open(longContent, { size: 'lg' });
        setTimeout(() => {
            if (this.Rights.indexOf(12) > -1) {
                this.AppointmentModel.ServiceId = this.ServiceType.ID;
                this.AppointmentModel.ServiceName = this.ServiceType.ServiceName;
                this.AppointmentModel.Price = this.ServiceType.Price;
                this.AppointmentModel.TokenNo = this.TokenNo;
                this.AppointmentModel.AppointmentDate = new Date();
                this.AppointmentModel.BillDate = new Date();
                if (this.ReminderList.length > 0)
                    this.AppointmentModel.ReminderId = this.ReminderList[0].ID;
                this.CalAmount();
            }
            if (this.PrescriptionModel.PatientId != null && this.PrescriptionModel.PatientId != undefined) {
                this.AppointmentModel.PatientId = this.PrescriptionModel.PatientId;
                this.AppointmentModel.PatientName = this.PrescriptionModel.PatientName;
                var patientList = this.PatientList.filter(a => a.PatientName == this.AppointmentModel.PatientName);
                if (patientList != null) {
                    this.AppointmentModel.CNIC = patientList[0].CNIC;
                    this.AppointmentModel.Mobile = patientList[0].Mobile;
                }
                this.AppointmentModel.DoctorId = this.PrescriptionModel.DoctorId;
            }
            this.LoadSearchableDropdown();
        }, 1000);
    }
    openAppointmentApptMoreDetailModal(ApptMoreDetail) {
        this.modalService.open(ApptMoreDetail, { size: 'lg' });
    }
    openDoctorModal(DocContent) {
        this._ApplicationUserService.GetRoles().then(m => {
            if (m.IsSuccess) {
                this.RoleList = m.ResultSet.role;
                this.WorkingdayList = m.ResultSet.dropdown.filter(a => a.DropDownID === 12);
                this.SpecialtyList = m.ResultSet.dropdown.filter(a => a.DropDownID === 24);
                const Id = this.RoleList.filter(a => a.RoleName == "Doctor");
                this.Usermodel.adm_user_company[0].RoleID = Id[0].ID;
                this.modalService.open(DocContent, { size: 'md' });
            }
        });
    }
    OpenMedicinePrescription() {
        this.loader.ShowLoader();
        this._AppointmentService.PrescriptionDropdownList().then(m => {
            if (m.IsSuccess) {
                this.loader.HideLoader();
                this.UnitList = m.ResultSet.UnitList.filter(a => a.DropDownID == 14);
                this.MadicineTypeList = m.ResultSet.UnitList.filter(a => a.DropDownID == 15);
                this.InstructionsList = m.ResultSet.InstructionList;
                this.modalService.open(this.MedicinePresModal, { size: 'lg' });
            } else
                this.loader.HideLoader();
        });
    }
    OpemMedicineTemplate(MedicineTemplate) {
        this.loader.ShowLoader();
        this._AppointmentService.TempleteList().then(m => {
            if (m.IsSuccess) {
                this.loader.HideLoader();
                this.TemplateList = m.ResultSet.result;
                this.modalService.open(MedicineTemplate, { size: 'md' });
            } else
                this.loader.HideLoader();
        });
    }
    OpenRxPrint(PrintRx) {
        this.modalService.open(PrintRx, { size: 'lg' });
    }
    openNewPatient(content) {
        this.loader.ShowLoader();
        this._AppointmentService.GetEMRNO().then(m => {
            this.model = new emr_patient();
            this.model.PrefixTittleId = 1;
            if (this.GenderList.length == 1) {
                this.model.Gender = this.GenderList[0].ID;
                if (this.model.Gender == 2)
                    this.model.PrefixTittleId = 1;
                if (this.model.Gender == 1)
                    this.model.PrefixTittleId = 2;
                this.GenderChnage(this.model.Gender);
            }
            this.model.PatientName = this.AppointmentModel.PatientName;
            this.model.MRNO = m.ResultSet.MRNO;
            this.BillTypeList = m.ResultSet.BillTypeList;
            this.TittleList = m.ResultSet.TittleList;
            this.model.BillTypeId = 1;
            this.loader.HideLoader();
            this.modalService.open(content, { size: 'lg' });
        });

    }
    constructor(public _fb: FormBuilder, private encrypt: EncryptionService, public route: ActivatedRoute, public _PatientService: PatientService, public _BillingService: BillingService, private modalService: NgbModal, public _CommonService: CommonService, public loader: LoaderService, public _AppointmentService: AppointmentService
        , public _router: Router,
        private cdr: ChangeDetectorRef, public _SaleService: SaleService,
        public toastr: CommonToastrService, public _ApplicationUserService: ApplicationUserService) {
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.DoctAddRights = this._CommonService.ScreenRights("3");
        this.ControlRights = this._CommonService.ScreenRights("13");
        this.PatientControlRights = this._CommonService.ScreenRights("11");
        this.RoleName = this.encrypt.decryptionAES(localStorage.getItem('RoleName'));
        this.UsersObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.IsCNICMandatory = this.CompanyObj.IsCNICMandatory;

        this.GenderIds = this.UsersObj.IsGenderDropdown;
        this.mySubscription = interval(60000).subscribe((x => {
        }));
    }
    ngOnInit() {
        this.DurationList = [
            { "ID": 1, "Name": "Day (s)" },
            { "ID": 2, "Name": "Week (s)" },
            { "ID": 3, "Name": "Month (s)" },
            { "ID": 4, "Name": "Daily" },
            { "ID": 5, "Name": "Weekly" },
            { "ID": 6, "Name": "Monthly" },
            { "ID": 7, "Name": "Continuously" },
            { "ID": 8, "Name": "When Required" },
            { "ID": 9, "Name": "STAT" },
            { "ID": 10, "Name": "PRN" }
        ];
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPatientPage(this.PModel.CurrentPage);
        $('#PatientNameSearch').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.searchByName(request.term).then(m => {
                    response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.PatientName = ui.item.label;
                $("#PatientNameSearch").val(ui.item.label);
                this.SearhPatientId = ui.item.value;
                let appid = 0;
                var appointfind = this.AppointmentList.filter(a => a.PatientId == ui.item.value);
                if (appointfind.length > 0)
                    appid = appointfind[0].ID;
                this.showPatientDetail(ui.item.value, this.AppointmentDate, appid);
                return ui.item.label;
            }
        });
        this.Form1 = this._fb.group({
            PatientName: ['', [Validators.required]],
            Gender: ['', [Validators.required]],
            DOB: [''],
            Age: ['', [Validators.required]],
            Email: [''],
            Mobile: ['', [<any>Validators.required]],
            CNIC: [''],
            Image: [''],
            Notes: [''],
            MRNO: [''],
            BillTypeId: [''],
            PrefixTittleId: [''],
            Father_Husband: [''],
        });
        this.Form2 = this._fb.group({
            PatientName: [''],
            PatientId: ['', [Validators.required]],
            // CNIC: ['', [Validators.required, Validators.pattern(ValidationVariables.CNICPattern)]],
            CNIC: [''],
            Mobile: ['', [<any>Validators.required]],
            DoctorId: [''],
            PatientProblem: [''],
            Notes: [''],
            AppointmentDate: ['', [Validators.required]],
            AppointmentTime: [''],
            StatusId: [''],
            ReminderId: [''],
            //bill field
            AppointmentId: [''],
            ServiceId: [''],
            BillDate: [''],
            Price: [''],
            Discount: [''],
            PaidAmount: [''],
            DoctorName: [''],
            ServiceName: [''],
            Remarks: [''],
            OutstandingBalance: [''],
        });
        this.Form3 = this._fb.group({
            Name: [''],
            UserID: [''],
            CompanyID: [''],
            RoleID: ['', [Validators.required]],
            Email: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.EmailPattern)]],
            PhoneNo: ['', [<any>Validators.required]],
            AdminID: [''],
            IsDefault: [''],
            SlotTime: ['', [Validators.required]],
            AppStartTime: ['', [Validators.required]],
            IsOverLap: [''],
            AppEndTime: ['', [Validators.required]],
            DocWorkingDay: [''],
            Designation: [''],
            Qualification: [''],
            Type: [''],
            SpecialtyId: [''],
            SpecialtyDropdownId: [''],
            Pwd: ['', [Validators.required, <any>Validators.minLength(6)]],
        });
        this.Form4 = this._fb.group({
            Remarks: [''],
            DocumentUpload: [''],
            Date: ['', [Validators.required]],
            DocumentTypeId: ['', [Validators.required]],
        });
        this.Form5 = this._fb.group({
            Medicine: [''],
            UnitId: [''],
            TypeId: [''],
            Measure: [''],
            InstructionId: ['']
        });
        this.Form6 = this._fb.group({
            IsTemplate: [''],
            AppointmentDate: [''],
            PatientId: [''],
            ClinicId: [''],
            PatientName: [''],
            DoctorId: ['', [Validators.required]],
            FollowUpDate: [''],
            FollowUpTime: [''],
            IsCreateAppointment: [''],
            AppointmentId: [''],
            TemplateName: [''],
            Notes: [''],
            AppointDate: [''],
            SelectedComplaint: [''],
            SelectedInvestigation: [''],
            SelectedObservation: [''],
            SelectedDiagnos: [''],
            Day: [''],
            FollowUpNotes: ['']
        });
        this.Form7 = this._fb.group({
            PatientName: ['', [Validators.required]],
            Gender: ['', [Validators.required]],
            DOB: [''],
            Age: ['', [Validators.required]],
            Email: [''],
            ContactNo: [''],
            Mobile: ['', [<any>Validators.required]],
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
            Father_Husband: [''],
        });
        this.Form8 = this._fb.group({
            Measure: [''],
            Date: [''],
            VitalId: ['', [Validators.required]],
            VitalDropdownId: [''],
            PatientId: [''],
        });
        this.Form9 = this._fb.group({
            AppointmentId: [''],
            PatientId: ['', [Validators.required]],
            ServiceId: ['', [Validators.required]],
            BillDate: ['', [Validators.required]],
            Price: ['', [Validators.required]],
            Discount: [''],
            PaidAmount: ['', [Validators.required]],
            PatientName: [''],
            DoctorName: [''],
            ServiceName: [''],
            DoctorId: [''],
            OutstandingBalance: ['']
        });
    }
    loadVital() {
        this.IsPatient = false;
        this._router.navigate(['home/Appoint/VitalPres'], { queryParams: { id: this.PatientId } });
    }
    ShowAppointment() {
        this.IsPatient = false;
        this._router.navigate(['home/Appoint/AppointPres'], { queryParams: { id: this.PatientId } });
    }
    LoadBillList() {
        this.IsPatient = false;
        this._router.navigate(['home/Appoint/BillPres'], { queryParams: { id: this.PatientId } });
    }
    LoadDocuments() {
        this.IsPatient = false;
        this._router.navigate(['home/Appoint/DocPres'], { queryParams: { id: this.PatientId } });
    }
    PrescriptionPage() {
        this.IsPatient = true;
    }
    selectPatientPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        let date = this._CommonService.GetFormatDate(new Date());
        let StatusID = '0';
        this.GetPatientList(date, StatusID);
    }
    GetPatientList(date: any, StatusID: any) {
        this.AppointmentDate = new Date(date);
        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "PatientName#PatientName,AppointmentTime#AppointmentTime";
        this._AppointmentService
            .GetPatientList(date.toString(), StatusID, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                if (this.GenderIds != null) {
                    this.GenderList = m.GenderList.filter(x => this.GenderIds.includes(x.ID));
                }
                else
                    this.GenderList = m.GenderList;
                this.DoctorList = m.DoctorList;
                this.DoctorClanderList = m.DoctorCalander;
                this.AppointmentList = m.AppointmentList;
                this.IsBackDatedAppointment = m.IsBackDatedAppointment;
                if (this.AppointmentList != null) {
                    this.AppointmentList.forEach(x => {
                        if (x.Image != null && x.Image != undefined && x.Image != "") {
                            x.Image = GlobalVariable.BASE_Temp_File_URL + '' + x.Image;
                        }
                    });
                }
                let CurntDate = this._CommonService.GetFormatDate(new Date());
                var appdate = this._CommonService.GetFormatDate(this.AppointmentDate);
                if (appdate < CurntDate)
                    this.IsPreviousApp = true;
                else
                    this.IsPreviousApp = false;
                this.StatusList = m.AllStatusList.filter(a => a.ID != 1);
                this.AllStatusList = m.AllStatusList;
                this.ReminderList = m.Reminderlist;
                this.Followuplist = m.Followuplist;
                this.PatientList = m.PatientInfo;
                this.IsShowDoctorIds = m.IsShowDoctorIds;
                this.ClinicList = m.ClinicList;
                this.MedicineList = m.MedicineList;
                this.BloodList = m.BloodList;
                this.UnitList = m.UnitList;
                this.MadicineTypeList = m.MadicineTypeList;

                this.ServiceType = m.ServiceType[0];
                this.TokenNo = m.TokenNo;
                if (m.ServiceType != null) {
                    this.AppointmentModel.ServiceId = m.ServiceType.ID;
                    this.AppointmentModel.ServiceName = m.ServiceType.ServiceName;
                    this.AppointmentModel.Price = m.ServiceType.Price;
                }
                this.BindEvent();
                this.LoadCalendar(date);
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    Reload() {
        let date = this._CommonService.GetFormatDate(new Date());
        let StatusID = '0';
        this.GetLatestPatientList(date, StatusID);
    }
    GetLatestPatientList(date: any, StatusID: any) {
        this.AppointmentDate = new Date(date);
        this.PModel.VisibleColumnInfo = "PatientName#PatientName,AppointmentTime#AppointmentTime";
        this._AppointmentService
            .GetPatientList(date.toString(), StatusID, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                if (this.GenderIds != null) {
                    this.GenderList = m.GenderList.filter(x => this.GenderIds.includes(x.ID));
                }
                else
                    this.GenderList = m.GenderList;
                this.DoctorList = m.DoctorList;
                this.DoctorClanderList = m.DoctorCalander;
                this.AppointmentList = m.AppointmentList;
                this.IsBackDatedAppointment = m.IsBackDatedAppointment;
                if (this.AppointmentList != null) {
                    this.AppointmentList.forEach(x => {
                        if (x.Image != null && x.Image != undefined && x.Image != "") {
                            x.Image = GlobalVariable.BASE_Temp_File_URL + '' + x.Image;
                        }
                    });
                }
                let CurntDate = this._CommonService.GetFormatDate(new Date());
                var appdate = this._CommonService.GetFormatDate(this.AppointmentDate);
                if (appdate < CurntDate)
                    this.IsPreviousApp = true;
                else
                    this.IsPreviousApp = false;
                this.StatusList = m.AllStatusList.filter(a => a.ID != 1);
                this.AllStatusList = m.AllStatusList;
                this.ReminderList = m.Reminderlist;
                this.Followuplist = m.Followuplist;
                this.PatientList = m.PatientInfo;
                this.IsShowDoctorIds = m.IsShowDoctorIds;
                this.ClinicList = m.ClinicList;
                this.MedicineList = m.MedicineList;
                this.BloodList = m.BloodList;
                this.UnitList = m.UnitList;
                this.MadicineTypeList = m.MadicineTypeList;
                this.ServiceType = m.ServiceType[0];
                this.TokenNo = m.TokenNo;
                if (m.ServiceType != null) {
                    this.AppointmentModel.ServiceId = m.ServiceType.ID;
                    this.AppointmentModel.ServiceName = m.ServiceType.ServiceName;
                    this.AppointmentModel.Price = m.ServiceType.Price;
                }
                this.BindEvent();
                this.LoadCalendar(date);
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    onFocusOutEvent(event: any, Id: any) {
        if (event != null && event != undefined) {
            var patientList = this.PatientList.filter(a => a.ID == Id);
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
                            this.loader.ShowLoader();
                            this.model.PatientName = event;
                            this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(new Date());
                            this.PrescriptionModel.ClinicId = this.CompanyObj.CompanyID;
                            //this.PatientDetailInfo = true;
                            this.modalService.open(this.PatientContent, { size: 'lg', backdrop: 'static' });
                            this.Addtreatment();
                            this.loader.HideLoader();
                        }
                    });

            }
        }
    }
    onPatientNameEvent(event: any, id: any) {
        if (event != null && event != undefined && event != "") {
            var patientList = this.PatientList.filter(a => a.ID == id);
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
                            this.loader.ShowLoader();
                            this._AppointmentService.GetEMRNO().then(m => {
                                this.model.MRNO = m.ResultSet.MRNO;
                                this.model.PatientName = event;

                                this.BillTypeList = m.ResultSet.BillTypeList;
                                this.TittleList = m.ResultSet.TittleList;

                                this.model.BillTypeId = 1;
                                this.model.PrefixTittleId = 1;

                                this.modalService.open(this.PatientContent, { size: 'lg', backdrop: 'static' });
                                this.loader.HideLoader();
                            });
                        }
                    });
                //if (confirm("Are you sure to add patient")) {
                //    this.loader.ShowLoader();
                //    this._AppointmentService.GetEMRNO().then(m => {
                //        this.model.MRNO = m.ResultSet.MRNO;
                //        this.model.PatientName = event;
                //        this.modalService.open(this.PatientContent, { size: 'lg' });
                //        this.loader.HideLoader();
                //    });
                //}
            }

        }
    }
    LoadDoctor() {
        $("#DoctorSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.DoctorSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.model.ReferredBy = ui.item.label;
                return ui.item.label;
            }
        });
    }
    handlePrint(val: any) {
        let selectedDate = new Date(this.AppointmentModel.AppointmentDate);
        let currentDate = new Date();
        selectedDate.setHours(0, 0, 0, 0);
        currentDate.setHours(0, 0, 0, 0);
        if (this.IsBackDatedAppointment == false && selectedDate < currentDate) {
            this.toastr.Error("You can not add Appointment in previous date and time.");
            return;
        }
        if (this.AppointmentModel.Discount > this.AppointmentModel.Price) {
            this.toastr.Error("discount not greater price amount.");
            return;
        }
        this.submitted = true;
        if (this.AppointmentModel.PatientId == undefined || this.AppointmentModel.PatientId == null) {
            this.toastr.Error("Error", "Please select patient.");
            return;
        }
        if (this.AppointmentModel.AppointmentDate == undefined || this.AppointmentModel.AppointmentDate == null) {
            this.toastr.Error("Error", "Please select appointment date.");
            return;
        }
        if (this.AppointmentModel.AppointmentTime == undefined || this.AppointmentModel.AppointmentTime == null) {
            this.toastr.Error("Error", "Please select time.");
            return;
        }
        if (this.AppointmentModel.ServiceId == undefined || this.AppointmentModel.ServiceId == null) {
            this.toastr.Error("Error", "Please select service.");
            return;
        }
        if (this.AppointmentModel.BillDate == undefined || this.AppointmentModel.BillDate == null) {
            this.toastr.Error("Error", "Please select bill date.");
            return;
        }
        if (this.AppointmentModel.Price == undefined || this.AppointmentModel.Price == null) {
            this.toastr.Error("Error", "Please select price.");
            return;
        }
        let IsExistFollowUp = Array.isArray(this.Followuplist)
            ? this.Followuplist.filter(a =>
                a.PatientId === this.AppointmentModel.PatientId &&
                a.Mobile === this.AppointmentModel.Mobile
            )
            : [];
        if (IsExistFollowUp.length > 0) {
            swal({
                title: "Are you sure?",
                text: "Are you sure you want to update the existing follow-up?",
                icon: "warning",
                buttons: ['Cancel', 'Yes'],
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete && val == "P") {
                        this.AppointmentModel.IsExistFollowUp = true;
                        this.AppointmentPrint(val);
                    }
                    else if (val == "P") {
                        this.AppointmentModel.IsExistFollowUp = false;
                        this.AppointmentPrint(val);
                    }
                    else {
                        this.AppointmentModel.IsExistFollowUp = false;
                        this.AppointmentSaveOrUpdate(true);
                    }
                });
            return;
        } else {
            if (val == "P") {
                this.AppointmentModel.IsExistFollowUp = false;
                this.PrintContent();
                this.AppointmentSaveOrUpdate(true);
            } else {
                this.AppointmentModel.IsExistFollowUp = false;
                this.AppointmentSaveOrUpdate(true);
            }
        }
    }
    AppointmentPrint(val: any) {
        this.submitted = false;
        this.loader.ShowLoader();
        if (this.AppointmentModel.AppointmentTime == undefined)
            this.AppointmentModel.AppointmentTime = $('#basicExample').val();
        this.AppointmentModel.AppointmentTime = this.Convert12TO24(this.AppointmentModel.AppointmentTime);
        if (this.AppointmentModel.StatusId == undefined || this.AppointmentModel.StatusId == null)
            this.AppointmentModel.StatusId = 25;
        this._AppointmentService.AppSaveOrUpdate(this.AppointmentModel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                if (val == "P") {
                    this.PrintContent();
                }
                this.modalService.dismissAll(this.ModelContent);
                this.toastr.Success(result.Message);
                this.CurntPrevDateList = result.ResultSet.CurntPrevDateList;
                this.PModel.SortName = "";
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.selectPatientPage(this.PModel.CurrentPage);
                this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(this.AppointmentModel.AppointmentDate);
                this.AppointmentModel = new emr_Appointment();
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    PrintContent() {
        const printSection = document.getElementById('print-section');
        const printContents = printSection ? printSection.innerHTML : '';
        if (printContents) {
            const popupWin = window.open('', '_blank', 'width=800,height=600');
            if (popupWin) {
                popupWin.document.open();
                popupWin.document.write(`
  <html>
    <head>
      <title>Print</title>
    </head>
    <body onload="window.print();window.close()">${printContents}</body>
  </html>
`);
                popupWin.document.close();
            }
        }
    }

    LoadMedicine(indx: any) {
        var id = ("#txtMedicine_" + indx);
        $(id).autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._SaleService.searchByNamePrescription(request.term).then(m => {

                    if (m.ResultSet.ItemInfo.length == 0) {
                        this.emr_prescription_treatment_dynamicArray[indx].MedicineId = null;
                        this.emr_prescription_treatment_dynamicArray[indx].Measure = null;
                        this.emr_prescription_treatment_dynamicArray[indx].Instructions = null;
                        this.emr_prescription_treatment_dynamicArray[indx].InstructionId = null;
                    }
                    response(m.ResultSet.ItemInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.emr_prescription_treatment_dynamicArray[indx].MedicineId = ui.item.value;
                this.emr_prescription_treatment_dynamicArray[indx].MedicineName = ui.item.label;
                this.emr_prescription_treatment_dynamicArray[indx].Measure = ui.item.Measure;
                this.emr_prescription_treatment_dynamicArray[indx].Instructions = ui.item.Instructions;
                this.emr_prescription_treatment_dynamicArray[indx].InstructionId = ui.item.InstructionId;
                $(id).val(ui.item.label);
                return ui.item.label;
            }
        });
    }
    LoadInstructions(indx: any) {
        var id = ("#txtInstructions_" + indx);
        $(id).autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.InstructionSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.emr_prescription_treatment_dynamicArray[indx].InstructionId = ui.item.value;
                this.emr_prescription_treatment_dynamicArray[indx].Instructions = ui.item.label;
                $(id).val(ui.item.label);
                return ui.item.label;
            }
        });
    }
    BindEvent() {
        this.Resource = [];
        this.Events = [];
        if (this.DoctorClanderList != null) {
            this.DoctorClanderList.forEach(x => {
                let AppObj = this.AppointmentList.filter(a => a.DoctorId == x.ID);
                let doctorobj = this.DoctorList.filter(a => a.ID == x.ID);
                if (this.DoctorClanderList.length == this.DoctorList.length) {
                    this.DoctorList.filter(a => a.ID == x.ID)[0].IsDoctor = true;
                }
                else {
                    if (doctorobj != null && doctorobj.length != 0)
                        this.DoctorList.filter(a => a.ID == x.ID)[0].IsDoctor = true;
                }
                if (x.OffDay != null && x.OffDay != '') {
                    var arry = x.OffDay.split(',').map(function (item) {
                        return parseInt(item, 10);
                    });
                    var art = JSON.stringify(arry);
                    var filtered = this.dayofweek.filter(
                        function (e) {
                            return this.indexOf(e) < 0;
                        },
                        art
                    );
                    this.dow = filtered;
                } else
                    this.dow = this.dayofweek;
                this.Resource.push({
                    'id': x.ID,
                    'title': x.Name,
                    businessHours: [{
                        dow: this.dow, // Monday - Friday
                        start: x.StartTime,
                        end: x.EndTime,
                    }],
                });
            });
        }
        this.AppointmentList.forEach(x => {
            this.Events.push({
                'id': x.ID,
                'resourceId': x.DoctorId,
                'title': x.PatientName,
                'start': x.StartDate,
                'end': x.EndDate,
                'color': x.Color,
            });
        });
    }
    LoadCalendar(Date) {
        $('#calendar').fullCalendar('destroy');
        $('#calendar').fullCalendar({
            defaultView: 'agendaDay',
            editable: true,
            eventDurationEditable: false,
            minTime: "01:00:00",
            maxTime: "24:00:00",
            slotDuration: '00:15:00',
            droppable: true,
            selectable: true,
            nowIndicator: true,
            scrollTime: moment().format("HH") + ":00:00",
            defaultDate: moment(Date),
            allDaySlot: false,
            eventLimit: false,
            header: {
                left: 'prev,next today',
                center: 'title',
                right: ''
            },
            selectConstraint: "businessHours",
            customButtons: {
                prev: {
                    icon: "left-single-arrow",
                    click: this.getPrevAppointments.bind(this),
                },
                next: {
                    icon: "right-single-arrow",
                    click: this.getNextAppointments.bind(this),
                },
                today: {
                    text: 'today',
                    click: this.getTodayAppointments.bind(this),
                }
            },
            resources: this.Resource,
            events: this.Events,
            eventDrop: (event, delta, revertFunc, resource) => {
                var dateTime = event.start.format();
                var time = dateTime.split('T');
                let CalendarDate = time[0];
                let CalendarTime = time[1];
                swal({
                    title: "Are you sure?",
                    text: "Are you sure to drag and drop.",
                    icon: "warning",
                    buttons: ['Cancel', 'Ok'],
                    dangerMode: true,
                })
                    .then((willDelete) => {
                        if (willDelete) {
                            this.CheckSlotAvailable(CalendarDate, CalendarTime, event.resourceId);
                            if (this.isDrop) {
                                this.AppointmentModel.ID = event.id;
                                this.AppointmentModel.AppointmentTime = CalendarTime;
                                this.AppointmentModel.DoctorId = event.resourceId;
                                this._AppointmentService.UpdateAppointment(this.AppointmentModel).then(m => {
                                    var result = JSON.parse(m._body);
                                    if (result.IsSuccess) {
                                        this.loader.HideLoader();
                                        this.GetPatientList(CalendarDate, 0);

                                    }
                                    else
                                        this.toastr.Error('Error', result.ErrorMessage);
                                    this.loader.HideLoader();
                                });

                            } else {
                                this.toastr.Error("Error", this.message);
                                revertFunc();

                            }
                        } else {
                            revertFunc();
                        }
                    });
            },
            eventClick: (event) => {
                var dateTime = event.start.format();
                var time = dateTime.split('T');
                let CalendarDate = time[0];
                let CalendarTime = time[1];
                var Appobj = this.AppointmentList.filter(a => a.ID == parseInt(event.id));
                this.AppointmentModel = new emr_Appointment();
                this.AppointmentModel = Appobj[0];
                this.AppointmentModel.ID = event.id;
                this.AppointmentModel.CNIC = Appobj[0].CNIC;
                this.AppointmentModel.DoctorName = Appobj[0].DoctorName;
                this.AppointmentModel.PatientId = Appobj[0].PatientId;
                this.AppointmentModel.PatientName = Appobj[0].PatientName;
                this.AppointmentModel.Notes = Appobj[0].Note;
                this.AppointmentModel.StatusId = Appobj[0].StatusId;
                this.AppointmentModel.ReminderId = Appobj[0].ReminderId;
                this.AppointmentModel.ServiceId = this.ServiceType.ID;
                this.AppointmentModel.ServiceName = this.ServiceType.ServiceName;
                this.OnClickAppointment(CalendarDate, CalendarTime, event.resourceId, event.id);
            },
            dayClick: (date, jsEvent, view, resource) => {

                var dateTime = date.format();
                var time = dateTime.split('T');
                let CalendarDate = time[0];
                let CalendarTime = time[1];
                this.CheckDayClik(date, CalendarTime);
                if (this.isDayClik) {
                    let dateDay = date._d.getDay();
                    for (var i = 0; i < resource.businessHours.length; i++) {
                        if (resource.businessHours[i].dow.includes(dateDay)) {
                            this.AddAppointment(CalendarDate, CalendarTime, resource.id);
                        }
                    }
                } else {
                    this.toastr.Error(this.message);
                }
            },
            eventMouseover: (info) => {
                var Appobj = this.AppointmentList.filter(a => a.ID == parseInt(info.id));
                var tooltip = '<div class="tooltipevent"> <div class="col"> <strong> Name </strong> <p> ' + Appobj[0].PatientName + ' </p> </div> <div class="col"> <strong> Doctor Name </strong> <p> ' + Appobj[0].DoctorName + ' </p> </div><div class="col"><hr></div> <div class="col"> <strong> Mr/CNIC </strong> <p> ' + Appobj[0].CNIC + ' </p> </div> <div class="col"> <strong> Created </strong> <p> ' + Appobj[0].CreatedBy + ' </p> </div> <div class="col"> <strong> Appt No </strong> <p> <span class="badge badge-primary">' + Appobj[0].ID + '  </span> </p> </div> </div>';
                var $tool = $(tooltip).appendTo('body');
                this.Tooltip(event);
                $(this).mouseover(function (e) {
                    $(this).css('z-index', 10000);
                    $tool.fadeIn('500');
                    $tool.fadeTo('10', 1.9);
                })
                    .mousemove(function (e) {
                        $tool.css('top', e.pageY - 100);
                        $tool.css('left', e.pageX + 20);
                    });
            },
            eventMouseout: (info) => {
                $(this).css('z-index', 8);
                $('.tooltipevent').remove();
            },
        });
    }
    CheckDayClik(date: any, time: any) {
        let selectedDate = new Date(date);
        let currentDate = new Date();
        selectedDate.setHours(0, 0, 0, 0);
        currentDate.setHours(0, 0, 0, 0);
        if (this.IsBackDatedAppointment == false && selectedDate < currentDate) {
            this.message = "You can not add Appointment in previous date and time.";
            this.isDayClik = false;
        } else
            this.isDayClik = true;

    }
    OnClickAppointment(date: any, time: any, doctorId: any, appointmentId: any) {
        var doctorid = this.DoctorClanderList.filter(a => a.ID == parseInt(doctorId));
        var Appobj = this.AppointmentList.filter(a => a.DoctorId == parseInt(doctorId) && a.StartDate.split("T")[1] == time && a.ID == appointmentId);
        var slotTime = new Date();
        slotTime.setHours(time.split(":")[0], time.split(":")[1], time.split(":")[2]);
        var timeA = new Date();
        timeA.setHours(doctorid[0].StartTime.split(":")[0], doctorid[0].StartTime.split(":")[1], doctorid[0].StartTime.split(":")[2]);
        var timeB = new Date();
        timeB.setHours(doctorid[0].EndTime.split(":")[0], doctorid[0].EndTime.split(":")[1], doctorid[0].EndTime.split(":")[2]);
        if (timeA <= slotTime && timeB >= slotTime) {
            if (this.Rights.indexOf(12) > -1) {
                this.AppointmentModel.ServiceId = Appobj[0].ServiceId;
                this.AppointmentModel.ServiceName = Appobj[0].ServiceName;
                this.AppointmentModel.Price = Appobj[0].Price;
                this.AppointmentModel.Discount = Appobj[0].Discount;
                this.AppointmentModel.PaidAmount = Appobj[0].PaidAmount;
                this.AppointmentModel.TokenNo = Appobj[0].TokenNo;
                this.AppointmentModel.Remarks = Appobj[0].Remarks;
                this.AppointmentModel.AppointmentDate = new Date();
                this.AppointmentModel.PatientProblem = Appobj[0].PatientProblem;
                this.AppointmentModel.StatusId = Appobj[0].StatusId;
                this.AppointmentModel.ReminderId = Appobj[0].ReminderId;

                this.CalAmount();
            }
            if (doctorid.length > 0)
                this.AppointmentModel.DoctorId = doctorid[0].ID;
            else
                this.AppointmentModel.DoctorId = parseInt(doctorId);
            this.Step = doctorid[0].SlotTime;
            this.AppointmentModel.AppointmentDate = date;
            this.AppointmentModel.BillDate = date;
            let time1 = this.calculateTimeFormate(time);
            this.AppointmentModel.AppointmentTime = time1;
            this.AppointmentModel.MrNo = Appobj[0].MRNO;
            this.modalService.open(this.ModelContent, { size: 'lg' });
            if (Appobj.length > 0) {
                this.FindDisabledSlot(Appobj);
                $("#basicExample").timepicker({
                    'disableTimeRanges': this.DisabledSlot,
                    'step': this.Step,
                    'timeFormat': 'h:i A'
                });
            } else {
                $("#basicExample").timepicker({
                    'step': this.Step,
                    'timeFormat': 'h:i A'
                });
            }
            setTimeout(() => {
                this.LoadSearchableDropdown();
                this.cdr.detectChanges();
            }, 1000);
        }
        else {
            //this.toastr.Error("slot not available.");
            // alert('Slot Not Available');
        }
    }
    Tooltip(event: any) {
        $('.tooltipevent').css('top', event.pageY - 80);
        $('.tooltipevent').css('left', event.pageX + 20);
    }
    SelectTime() {
        if (this.AppointmentModel.DoctorId != null && this.AppointmentModel.DoctorId != undefined) {
            var doctorid = this.DoctorClanderList.filter(a => a.ID == this.AppointmentModel.DoctorId);
            var Appobj = this.AppointmentList.filter(a => a.DoctorId == this.AppointmentModel.DoctorId);
            if (doctorid.length > 0) {
                let minTime = this.formatAMPM(doctorid[0].StartTime);
                let maxTime = this.formatAMPM(doctorid[0].EndTime);
                if (doctorid[0].SlotTime != null) {
                    this.Step = parseInt(doctorid[0].SlotTime.split(':')[1]);
                }
                if (Appobj.length > 0 && doctorid.length > 0) {
                    this.FindDisabledSlot(Appobj);
                    $("#basicExample").timepicker({
                        'disableTimeRanges': this.DisabledSlot,
                        'minTime': minTime,
                        'maxTime': maxTime,
                        'step': this.Step,
                        'timeFormat': 'h:i A'
                    });
                } else {
                    $("#basicExample").timepicker({
                        'minTime': minTime,
                        'maxTime': maxTime,
                        'step': this.Step,
                        'timeFormat': 'h:i A'
                    });
                }
            } else {
                let Step = 30;
                $("#basicExample").timepicker({
                    'disableTimeRanges': this.DisabledSlot,
                    'step': Step,
                    'timeFormat': 'h:i A'
                });
            }
            $('#basicExample').timepicker('show');
            if ($("#basicExample").val() != null || $("#basicExample").val() != "")
                this.AppointmentModel.AppointmentTime = $("#basicExample").val();
        }
        else {
            this.toastr.Error("please select doctor.")
        }
    }
    ChnageSelectTime() {
        if (this.AppointmentModel.DoctorId != null && this.AppointmentModel.DoctorId != undefined) {
            var doctorid = this.DoctorClanderList.filter(a => a.ID == this.AppointmentModel.DoctorId);
            var Appobj = this.AppointmentList.filter(a => a.DoctorId == this.AppointmentModel.DoctorId);
            if (doctorid.length > 0) {
                let minTime = this.formatAMPM(doctorid[0].StartTime);
                let maxTime = this.formatAMPM(doctorid[0].EndTime);
                if (doctorid[0].SlotTime != null) {
                    this.Step = parseInt(doctorid[0].SlotTime.split(':')[1]);
                }
                if (Appobj.length > 0 && doctorid.length > 0) {
                    this.FindDisabledSlot(Appobj);
                    $("#basicExample").timepicker({
                        'disableTimeRanges': this.DisabledSlot,
                        'minTime': minTime,
                        'maxTime': maxTime,
                        'step': this.Step,
                        'timeFormat': 'h:i A'
                    });
                } else {
                    $("#basicExample").timepicker({
                        'minTime': minTime,
                        'maxTime': maxTime,
                        'step': this.Step,
                        'timeFormat': 'h:i A'
                    });
                }
            } else {
                let Step = 30;
                $("#basicExample").timepicker({
                    'disableTimeRanges': this.DisabledSlot,
                    'step': Step,
                    'timeFormat': 'h:i A'
                });
            }
            $('#basicExample').timepicker('show');
            if ($("#basicExample").val() != null || $("#basicExample").val() != "")
                this.AppointmentModel.AppointmentTime = $("#basicExample").val();

            $('#basicExample').timepicker('remove');
        }
        else {
            this.toastr.Error("please select doctor.")
        }
    }
    formatAMPM(time: any) {
        var hours = time.split(':')[0];
        var minutes = time.split(':')[1];
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }
    DoctorChange() {
        this.AppointmentModel.AppointmentTime = null;
        this.AppointmentModel.AppointmentTime = '';
        $('#basicExample').timepicker('remove');
    }
    FollowUpSelectTime() {
        if (this.PrescriptionModel.DoctorId != null && this.PrescriptionModel.DoctorId != undefined) {
            var doctorid = this.DoctorClanderList.filter(a => a.ID == this.PrescriptionModel.DoctorId);
            var Appobj = this.AppointmentList.filter(a => a.DoctorId == this.PrescriptionModel.DoctorId);
            if (doctorid[0].SlotTime != null) {
                this.Step = parseInt(doctorid[0].SlotTime.split(':')[1]);
            }
            if (Appobj.length > 0) {
                this.FindDisabledSlot(Appobj);
                $("#FollowupbasicTime").timepicker({
                    'disableTimeRanges': this.DisabledSlot,
                    'step': this.Step,
                    'timeFormat': 'h:i A'
                });
            } else {
                $("#FollowupbasicTime").timepicker({
                    'step': this.Step,
                    'timeFormat': 'h:i A'
                });
            }
            if ($("#FollowupbasicTime").val() != null && $("#FollowupbasicTime").val() != "")
                this.PrescriptionModel.FollowUpTime = $("#FollowupbasicTime").val();
        }
        else {
            this.toastr.Error("please select doctor.")
        }
    }
    ChnageFollowUpSelectTime() {
        if (this.PrescriptionModel.DoctorId != null && this.PrescriptionModel.DoctorId != undefined) {
            var doctorid = this.DoctorClanderList.filter(a => a.ID == this.PrescriptionModel.DoctorId);
            var Appobj = this.AppointmentList.filter(a => a.DoctorId == this.PrescriptionModel.DoctorId);
            if (doctorid[0].SlotTime != null) {
                this.Step = parseInt(doctorid[0].SlotTime.split(':')[1]);
            }
            if (Appobj.length > 0) {
                this.FindDisabledSlot(Appobj);
                $("#FollowupbasicTime").timepicker({
                    'disableTimeRanges': this.DisabledSlot,
                    'step': this.Step,
                    'timeFormat': 'h:i A'
                });
            } else {
                $("#FollowupbasicTime").timepicker({
                    'step': this.Step,
                    'timeFormat': 'h:i A'
                });
            }
            if ($("#FollowupbasicTime").val() != null && $("#FollowupbasicTime").val() != "")
                this.PrescriptionModel.FollowUpTime = $("#FollowupbasicTime").val();

            $('#FollowupbasicTime').timepicker('remove');
        }
        else {
            this.toastr.Error("please select doctor.")
        }
    }
    LoadSearchableDropdown() {
        //Search By Name
        $('#PatientSearchByName').autocomplete({
            source: debounce((request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.searchByName(request.term).then(m => {
                    this.patientInfo = m.ResultSet.PatientInfo;
                    response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            }, 300),
            minLength: 3,
            select: (event, ui) => {
                this.AppointmentModel.PatientName = ui.item.PatientName;
                this.AppointmentModel.PatientId = ui.item.value;
                this.AppointmentModel.CNIC = this.patientInfo.filter(a => a.value == ui.item.value)[0].CNIC;
                this.AppointmentModel.Mobile = this.patientInfo.filter(a => a.value == ui.item.value)[0].Phone;
                this.AppointmentModel.ReminderId = ui.item.ReminderId;
                this.AppointmentModel.MrNo = ui.item.MRNO;
                this.AppointmentModel.ServiceId = ui.item.ServiceID;
                this.AppointmentModel.ServiceName = ui.item.ServiceName;
                this.AppointmentModel.Price = ui.item.ServicePrice;
            }
        });
    }
    formatAppointmentTime(time: string): string {
        if (time != undefined) {
            let trimmedTime = time.replace(/\s+/g, '');
            const timePattern = /^([1-9]):([0-5][0-9])(AM|PM)$/i;
            if (timePattern.test(trimmedTime)) {
                return '0' + trimmedTime;
            }
            return time;
        } else {
            this.toastr.Error("please select time.");
        }

    }
    LoadService() {
        $("#ServiceSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._BillingService.ServiceSearch(request.term).then(m => {
                    if (m.ResultSet.result.length == 0)
                        $("#ServiceSearch").val('');
                    if (m.ResultSet.result.length != 0)
                        response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.AppointmentModel.ServiceId = ui.item.value;
                this.AppointmentModel.ServiceName = ui.item.label;
                return ui.item.label;
            }
        });
    }
    CurrentConvert12TO24(time12h: any) {
        const [time, modifier] = time12h.split(' ');
        let [hours, minutes] = time.split(':');
        if (modifier === 'pm') {
            hours = parseInt(hours, 10) + 12;
        }
        return hours + ':' + minutes + new Date().getSeconds();
    }
    getMonthAppointments(arg) {
        var view = $('#calendar').fullCalendar('getView');
        var strview = view.name;
        if (strview == "month") {
            var start = $('#calendar').fullCalendar('getView').title;
        }
        if (strview == "agendaWeek") {
            var get_month = $('#calendar').fullCalendar('getDate');
            var date = get_month._d;
            var fdate = this._CommonService.GetFormatDate(date);
            date.setDate(date.getDate() + 7);
            var todate = this._CommonService.GetFormatDate(date);
            var yyyy = date.getFullYear();
            var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getmonth() is zero-based
            var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
            var tdate = yyyy + '-' + mm + '-' + dd;
            this.MonthWeekFilter(fdate, tdate);
        }
        if (strview == "agendaDay") {
            let date = this._CommonService.GetFormatDate(new Date());
            this.PrevNextFilter(date);
        }
    }
    getNextAppointments(arg) {
        var get_month = $('#calendar').fullCalendar('getDate');
        var date = get_month._d;
        date.setDate(date.getDate() + 1);
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getmonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var pndate = yyyy + '-' + mm + '-' + dd;
        this.PrevNextFilter(pndate);
        this.AppointmentDate = new Date(pndate);
    }
    getTodayAppointments(arg) {
        var get_month = $('#calendar').fullCalendar('getDate');
        var date = new Date();
        date.setDate(date.getDate());
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getmonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var pndate = yyyy + '-' + mm + '-' + dd;
        this.PrevNextFilter(pndate);
        this.AppointmentDate = new Date(pndate);
    }
    getPrevAppointments(arg) {
        var get_month = $('#calendar').fullCalendar('getDate');
        var date = get_month._d;
        date.setDate(date.getDate() - 1);
        var yyyy = date.getFullYear();
        var mm = date.getMonth() < 9 ? "0" + (date.getMonth() + 1) : (date.getMonth() + 1); // getmonth() is zero-based
        var dd = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var pndate = yyyy + '-' + mm + '-' + dd;
        this.PrevNextFilter(pndate);
        this.AppointmentDate = new Date(pndate);
    }
    AddAppointment(date: any, time: any, doctorId: any) {
        if (this.ControlRights.CreateRights) {
            this.AppointmentModel = new emr_Appointment();
            var doctorid = this.DoctorClanderList.filter(a => a.ID == parseInt(doctorId));
            var Appobj = this.AppointmentList.filter(a => a.DoctorId == parseInt(doctorId));
            var slotTime = new Date();
            slotTime.setHours(time.split(":")[0], time.split(":")[1], time.split(":")[2]);
            var timeA = new Date();
            timeA.setHours(doctorid[0].StartTime.split(":")[0], doctorid[0].StartTime.split(":")[1], doctorid[0].StartTime.split(":")[2]);
            var timeB = new Date();
            timeB.setHours(doctorid[0].EndTime.split(":")[0], doctorid[0].EndTime.split(":")[1], doctorid[0].EndTime.split(":")[2]);
            if (timeA <= slotTime && timeB >= slotTime) {
                if (this.Rights.indexOf(12) > -1) {
                    this.AppointmentModel.ServiceId = this.ServiceType.ID;
                    this.AppointmentModel.ServiceName = this.ServiceType.ServiceName;
                    this.AppointmentModel.Price = this.ServiceType.Price;
                    this.AppointmentModel.Discount = 0;
                    this.AppointmentModel.PaidAmount = 0;
                    this.AppointmentModel.TokenNo = this.TokenNo;
                    this.AppointmentModel.StatusId = 25;
                    this.AppointmentModel.Remarks = "";
                    this.AppointmentModel.AppointmentDate = new Date();
                    if (this.ReminderList.length > 0)
                        this.AppointmentModel.ReminderId = this.ReminderList[0].ID;

                    this.CalAmount();
                }
                if (doctorid.length > 0)
                    this.AppointmentModel.DoctorId = doctorid[0].ID;
                else
                    this.AppointmentModel.DoctorId = parseInt(doctorId);
                this.Step = doctorid[0].SlotTime;
                this.AppointmentModel.AppointmentDate = date;
                this.AppointmentModel.PatientProblem = "";
                this.AppointmentModel.BillDate = date;
                let time1 = this.calculateTimeFormate(time);
                this.AppointmentModel.AppointmentTime = time1;
                this.modalService.open(this.ModelContent, { size: 'lg' });
                if (Appobj.length > 0) {
                    this.FindDisabledSlot(Appobj);
                    $("#basicExample").timepicker({
                        'disableTimeRanges': this.DisabledSlot,
                        'step': this.Step,
                        'timeFormat': 'h:i A'
                    });
                } else {
                    $("#basicExample").timepicker({
                        'step': this.Step,
                        'timeFormat': 'h:i A'
                    });
                }
                setTimeout(() => {
                    this.LoadSearchableDropdown();
                    this.cdr.detectChanges();
                }, 1000);
            }
        } else {
            this.toastr.Error("you don’t have permission to create appointment");
            return;
        }
    }
    FindDisabledSlot(Appobj: any) {
        this.DisabledSlot = "";
        Appobj.forEach(x => {
            var stime = this.calculateTimeFormateDisabled(x.StartDate.split('T')[1]);
            var etime = this.calculateTimeFormateDisabled(x.EndDate.split('T')[1]);
            this.DisabledSlot += stime + ',' + etime + ',';
        });
        var time_arr = this.DisabledSlot.split(","), new_arr = [];
        time_arr.forEach(function (v, k, arr) {
            if (k % 2 & 1) new_arr.push([arr[k - 1], arr[k]]);  // check for odd keys
        })
        this.DisabledSlot = new_arr;
    }
    calculateTimeFormateDisabled(time: any) {
        let hours = time.split(":")[0];
        let minutes = time.split(":")[1];
        let ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12;
        minutes = minutes.toString().padStart(2, '0');
        let strTime = hours + ':' + minutes + '' + ampm;
        return strTime;
    }
    calculateTimeFormate(time: any) {
        let hours = time.split(":")[0];
        let minutes = time.split(":")[1];
        let ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12;
        minutes = minutes.toString().padStart(2, '0');
        let strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }
    CheckSlotAvailable(date: any, time: any, doctorId: any) {
        let selectedDate = new Date(date);
        let currentDate = new Date();
        selectedDate.setHours(0, 0, 0, 0);
        currentDate.setHours(0, 0, 0, 0);
        if (this.IsBackDatedAppointment == false && selectedDate < currentDate) {
            this.message = "You can not add Appointment in previous date and time.";
            this.isDrop = false;
        }
        else {
            var doctorid = this.DoctorClanderList.filter(a => a.ID == parseInt(doctorId));
            var slotTime = new Date();
            slotTime.setHours(time.split(":")[0], time.split(":")[1], time.split(":")[2]);
            var timeA = new Date();
            timeA.setHours(doctorid[0].StartTime.split(":")[0], doctorid[0].StartTime.split(":")[1], doctorid[0].StartTime.split(":")[2]);
            var timeB = new Date();
            timeB.setHours(doctorid[0].EndTime.split(":")[0], doctorid[0].EndTime.split(":")[1], doctorid[0].EndTime.split(":")[2]);

            if (timeA <= slotTime && timeB >= slotTime) {
                this.isDrop = true;
            }
            else {
                this.isDrop = false;
            }
        }
    }
    MonthWeekFilter(fdate, tdate) {
        this.loader.ShowLoader();
        this._AppointmentService.MonthLoadData(fdate.toString(), tdate.toString(), '0').then(m => {

            if (this.GenderIds != null) {
                this.GenderList = m.ResultSet.GenderList.filter(x => this.GenderIds.includes(x.ID));
                if (this.GenderList.length == 1)
                    this.model.Gender = this.GenderList[0].ID;
            }
            else
                this.GenderList = m.ResultSet.GenderList;

            this.DoctorList = m.ResultSet.DoctorList.DoctList;
            this.DoctorClanderList = m.ResultSet.DoctorCalander;
            this.AppointmentList = m.ResultSet.AppointmentList;
            this.StatusList = m.AllStatusList.filter(a => a.ID != 1);
            this.AllStatusList = m.AllStatusList;
            this.BindEvent();
            let date = this._CommonService.GetFormatDate(new Date());
            this.LoadCalendar(date);
            this.loader.HideLoader();
        });
    }
    PrevNextFilter(date) {
        this.loader.ShowLoader();
        this.GetPatientList(date.toString(), '0');

    }
    Convert12TO24(time12h: any) {
        if (time12h != "") {
            var hours = Number(time12h.match(/^(\d+)/)[1]);
            var minutes = Number(time12h.match(/:(\d+)/)[1]);
            var AMPM = time12h.match(/\s(.*)$/)[1];
            if (AMPM == "pm" && hours < 12) hours = hours + 12;
            if (AMPM == "am" && hours == 12) hours = hours - 12;
            return hours + ':' + minutes + ':00';
        }
    }
    StatusUpdateApp(StatusID, id, DoctorId) {
        this.loader.ShowLoader();
        if (StatusID == 1)
            StatusID = 0;
        else
            StatusID = StatusID;
        let date = this._CommonService.GetFormatDate(new Date());
        this.AppointmentModel.ID = id;
        this.AppointmentModel.StatusId = StatusID;
        this.AppointmentModel.DoctorId = DoctorId;
        this._AppointmentService.UpdateAppointment(this.AppointmentModel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {

                let date = this._CommonService.GetFormatDate(new Date());
                this.AppointmentDate = new Date();
                this.GetPatientList(date.toString(), '0');
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }

        });
    }
    StatusChangeFilter(StatusID) {
        this.loader.ShowLoader();
        if (StatusID == 1)
            StatusID = 0;
        else
            StatusID = StatusID;
        let date = this._CommonService.GetFormatDate(new Date());
        this.AppointmentDate = new Date(date);
        this.loader.ShowLoader();
        this.PModel.VisibleColumnInfo = "PatientName#PatientName,AppointmentTime#AppointmentTime";
        this._AppointmentService
            .GetUpdatePatientList(date.toString(), StatusID, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                if (this.GenderIds != null) {
                    this.GenderList = m.GenderList.filter(x => this.GenderIds.includes(x.ID));
                }
                else
                    this.GenderList = m.GenderList;
                this.DoctorList = m.DoctorList;
                this.DoctorClanderList = m.DoctorCalander;
                this.AppointmentList = m.AppointmentList;
                this.IsBackDatedAppointment = m.IsBackDatedAppointment;
                if (this.AppointmentList != null) {
                    this.AppointmentList.forEach(x => {
                        if (x.Image != null && x.Image != undefined && x.Image != "") {
                            x.Image = GlobalVariable.BASE_Temp_File_URL + '' + x.Image;
                        }
                    });
                }
                let CurntDate = this._CommonService.GetFormatDate(new Date());
                var appdate = this._CommonService.GetFormatDate(this.AppointmentDate);
                if (appdate < CurntDate)
                    this.IsPreviousApp = true;
                else
                    this.IsPreviousApp = false;
                this.StatusList = m.AllStatusList.filter(a => a.ID != 1);
                this.AllStatusList = m.AllStatusList;
                this.ReminderList = m.Reminderlist;
                this.Followuplist = m.Followuplist;
                this.PatientList = m.PatientInfo;
                this.IsShowDoctorIds = m.IsShowDoctorIds;
                this.ClinicList = m.ClinicList;
                this.MedicineList = m.MedicineList;
                this.BloodList = m.BloodList;
                this.UnitList = m.UnitList;
                this.MadicineTypeList = m.MadicineTypeList;
                this.ServiceType = m.ServiceType[0];
                this.TokenNo = m.TokenNo;
                if (m.ServiceType != null) {
                    this.AppointmentModel.ServiceId = m.ServiceType.ID;
                    this.AppointmentModel.ServiceName = m.ServiceType.ServiceName;
                    this.AppointmentModel.Price = m.ServiceType.Price;
                }
                this.BindEvent();
                this.LoadCalendar(date);
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
    }
    FilterPatient(id: any) {
        let StatusID = 0;
        if (id == 1 || id == 2)
            StatusID = id;
        else
            StatusID = 0;
        this.loader.ShowLoader();
        var Curntdate = new Date(this.AppointmentDate);
        if (id == 3)
            Curntdate.setDate(Curntdate.getDate() + 1);
        if (id == 4)
            Curntdate = new Date();
        if (id == 5)
            Curntdate.setDate(Curntdate.getDate() - 1);
        let date = this._CommonService.GetFormatDate(Curntdate);
        this.AppointmentDate = new Date(date);
        this._AppointmentService.DropdownFilterData(date.toString(), StatusID.toString()).then(m => {
            if (this.GenderIds != null) {
                this.GenderList = m.ResultSet.GenderList.filter(x => this.GenderIds.includes(x.ID));
                //if (this.GenderList.length == 1)
                //    this.model.Gender = this.GenderList[0].ID;
            }
            else
                this.GenderList = m.ResultSet.GenderList;
            this.AllStatusList = m.ResultSet.AllStatusList;
            this.DoctorList = m.ResultSet.DoctorList.DoctList;
            this.DoctorClanderList = m.ResultSet.DoctorCalander;
            this.AppointmentList = m.ResultSet.AppointmentList;
            if (this.AppointmentList != null) {
                this.AppointmentList.forEach(x => {
                    if (x.Image != null && x.Image != undefined && x.Image != "") {
                        x.Image = GlobalVariable.BASE_Temp_File_URL + '' + x.Image;
                    }
                });
            }
            this.BindEvent();
            this.LoadCalendar(date);
            this.loader.HideLoader();
        });

    }
    CheckBoxStatusChange() {
        if (this.StatusID == 1)
            var StatusId = 0;
        else
            var StatusId = this.StatusID;
        let date = this._CommonService.GetFormatDate(new Date());
        this.GetPatientList(date.toString(), StatusId);
    }
    PatientSaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            if (this.IsCNICMandatory) {
                if (this.model.CNIC === null || this.model.CNIC === "") {
                    this.toastr.Error("Error", "Please enter CNIC.");
                    return;
                }
            }
            this.loader.ShowLoader();
            this._AppointmentService.IsPhoneExist(this.model.Mobile).then(m => {
                this.loader.HideLoader();
                if (m.IsSuccess) {
                    this.AddUpdatePatient();
                    //swal({
                    //    title: "Are you sure?",
                    //    text: "Are you sure want to add another patient against this " + this.model.Mobile + "",
                    //    icon: "warning",
                    //    buttons: ['Cancel', 'Yes'],
                    //    dangerMode: true,
                    //})
                    //    .then((willDelete) => {
                    //        if (willDelete) {
                    //            this.AddUpdatePatient();
                    //        }
                    //    });

                } else {
                    this.AddUpdatePatient();
                }
            });

        }
    }
    AddUpdatePatient() {
        this.loader.ShowLoader();
        this._AppointmentService.SaveOrUpdate(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                $('.PatientModal').closest('.modal').removeClass('d-block show').prev('.modal-backdrop').removeClass('show').hide();
                this.PatientList = [];
                this.PatientList = result.ResultSet.Model;
                this.AppointmentModel.PatientId = result.ResultSet.CurrentModel.ID;
                this.AppointmentModel.CNIC = result.ResultSet.CurrentModel.CNIC;
                this.AppointmentModel.Mobile = result.ResultSet.CurrentModel.Mobile;
                this.AppointmentModel.PatientName = result.ResultSet.CurrentModel.PatientName;
                this.AppointmentModel.Notes = result.ResultSet.CurrentModel.Notes;
                this.AppointmentModel.MrNo = result.ResultSet.CurrentModel.MRNO;
                this.PrescriptionModel.PatientId = result.ResultSet.CurrentModel.ID;
                this.PrescriptionModel.PatientName = result.ResultSet.CurrentModel.PatientName;
                this.AppointmentModel.StatusId = 25;
                this.AppointmentModel.ServiceId = this.ServiceType.ID;
                this.AppointmentModel.ServiceName = this.ServiceType.ServiceName;
                if (this.ReminderList.length > 0)
                    this.AppointmentModel.ReminderId = this.ReminderList[0].ID;

                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    IsPhoneExist(Phone: any) {
        this.loader.ShowLoader();
        this._AppointmentService.IsPhoneExist(Phone).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    onPrint() {
        if (this.AppointmentModel.ServiceId == undefined || this.AppointmentModel.ServiceId == null) {
            this.toastr.Error("Error", "Please select service.");
            return;
        }
    }
    AppointmentSaveOrUpdate(isValid: boolean): void {
        let selectedDate = new Date(this.AppointmentModel.AppointmentDate);
        let currentDate = new Date();
        selectedDate.setHours(0, 0, 0, 0);
        currentDate.setHours(0, 0, 0, 0);
        if (this.IsBackDatedAppointment == false && selectedDate < currentDate) {
            this.toastr.Error("You can not add Appointment in previous date and time.");
            return;
        }
        if (this.AppointmentModel.Discount > this.AppointmentModel.Price) {
            this.toastr.Error("discount not greater price amount.");
            return;
        }
        this.submitted = true;
        if (this.AppointmentModel.PatientId == undefined || this.AppointmentModel.PatientId == null) {
            this.toastr.Error("Error", "Please select patient.");
            return;
        }
        if (this.AppointmentModel.AppointmentDate == undefined || this.AppointmentModel.AppointmentDate == null) {
            this.toastr.Error("Error", "Please select appointment date.");
            return;
        }
        if (this.AppointmentModel.AppointmentTime == undefined || this.AppointmentModel.AppointmentTime == null) {
            this.toastr.Error("Error", "Please select time.");
            return;
        }
        if (this.AppointmentModel.ServiceId == undefined || this.AppointmentModel.ServiceId == null) {
            this.toastr.Error("Error", "Please select service.");
            return;
        }
        if (this.Rights.indexOf(12) > -1) {
            if (this.AppointmentModel.ServiceId == undefined || this.AppointmentModel.ServiceId == null) {
                this.toastr.Error("Error", "Please select service.");
                return;
            }
            if (this.AppointmentModel.BillDate == undefined || this.AppointmentModel.BillDate == null) {
                this.toastr.Error("Error", "Please select bill date.");
                return;
            }
            if (this.AppointmentModel.Price == undefined || this.AppointmentModel.Price == null) {
                this.toastr.Error("Error", "Please select price.");
                return;
            }
        }
        this.submitted = false;
        this.loader.ShowLoader();
        if (this.AppointmentModel.AppointmentTime == undefined)
            this.AppointmentModel.AppointmentTime = $('#basicExample').val();
        this.AppointmentModel.AppointmentTime = this.Convert12TO24(this.AppointmentModel.AppointmentTime);
        if (this.AppointmentModel.StatusId == undefined || this.AppointmentModel.StatusId == null)
            this.AppointmentModel.StatusId = 25;
        this._AppointmentService.AppSaveOrUpdate(this.AppointmentModel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.modalService.dismissAll(this.ModelContent);
                this.toastr.Success(result.Message);
                this.CurntPrevDateList = result.ResultSet.CurntPrevDateList;
                this.PModel.SortName = "";
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.selectPatientPage(this.PModel.CurrentPage);
                this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(this.AppointmentModel.AppointmentDate);
                this.AppointmentModel = new emr_Appointment();
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    RefershCalendar(event: IMyDateModel) {
        this.FilterRecord(this.AppointmentDate);
    }
    FilterRecord(Date) {
        if (Date != null) {
            this.GetPatientList(Date.toString(), '0');
            this.PatientDetailInfo = false;
        }
    }
    CalAmount() {
        if (this.AppointmentModel.Discount == undefined || this.AppointmentModel.Discount == null)
            this.AppointmentModel.Discount = 0;
        if (this.AppointmentModel.Price == undefined || this.AppointmentModel.Price == null)
            this.AppointmentModel.Price = 0;

        this.AppointmentModel.PaidAmount = this.AppointmentModel.Price - this.AppointmentModel.Discount;
        this.AppointmentModel.OutstandingBalance = this.AppointmentModel.Price - this.AppointmentModel.Discount - this.AppointmentModel.PaidAmount;

    }
    PaidCalAmount() {
        if (this.AppointmentModel.Discount == undefined || this.AppointmentModel.Discount == null)
            this.AppointmentModel.Discount = 0;
        if (this.AppointmentModel.Price == undefined || this.AppointmentModel.Price == null)
            this.AppointmentModel.Price = 0;
        this.AppointmentModel.OutstandingBalance = this.AppointmentModel.Price - this.AppointmentModel.Discount - this.AppointmentModel.PaidAmount;
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._AppointmentService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Appoint/saveAppoint']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Appoint/saveAppoint'], { queryParams: { id: this.Id } });
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string, SystemGenerated: boolean) {
        if (SystemGenerated) {
            this.toastr.Error('Error', "Sorry, predefined roles cannot be edited or deleted. Clone the role instead.");
        } else {
            var result = confirm("Are you sure you want to delete selected record.");
            if (result == true) {
                this.loader.ShowLoader();
                this._AppointmentService.Delete(id).then(m => {

                    if (m.ErrorMessage != null) {

                        alert(m.ErrorMessage);
                    }
                    let date = this._CommonService.GetFormatDate(new Date());
                    this.AppointmentDate = new Date(date);
                    this.GetPatientList(date, 0);
                });
            }
        }
    }
    IsNewImageEvent(FName) {
        this.IsNewImage = true;
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
    onDocDateChanged(event: IMyDateModel) {
        if (event) {
        }
    }
    onDOBChanged(event: IMyDateModel) {
        if (event) {
            var dob = new Date(this.model.DOB)
            var CurrentDate = new Date();
            var ageCalc = CurrentDate.getFullYear() - dob.getFullYear();
            this.model.Age = ageCalc;
        }
    }
    onAppDateChanged(event: IMyDateModel) {
        let CurrentDate = this._CommonService.GetFormatDate(new Date());
        let date = this._CommonService.GetFormatDate(event.jsdate);
        if (date < CurrentDate) {
            this.toastr.Error("You can not add appointment previous date.");
            return;
        }
        if (this.PrescriptionModel.PatientId != null && this.PrescriptionModel.PatientId != undefined) {
            var dob = new Date(this.AppointmentModel.AppointmentDate)
            this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(dob);
        }
    }
    getValidDOBDate(dat: Date) {
    }
    GetList() {
        let date = this._CommonService.GetFormatDate(new Date());
        this.AppointmentDate = new Date(date);
        this.GetPatientList(date, 0);
    }
    Close(idpar) {
        this.IsList = true;
        if (idpar == 0)
            this.Id = '0';
        else {
            let date = this._CommonService.GetFormatDate(new Date());
            this.AppointmentDate = new Date(date);
            this.GetPatientList(date, 0);
        }
    }
    onAppointDateChange(event: any) {
        if (event.target.selectedIndex != null) {
            var reslt = this.CurntPrevDateList[event.target.selectedIndex];
            this.PrescriptionModel.AppointmentDate = reslt.AppointmentDate;
            this.AppointDate = reslt.AppointmentDate;
            this.PrescriptionModel.AppointmentId = reslt.ID;
            this.showPatientDetail(reslt.PatientId, reslt.AppointmentDate, reslt.ID);
        } else {
            this.PrescriptionModel = new emr_prescription_mf();
        }
    }
    showPatientDetail(id: any, date: any, AppId: any) {
        if (id != undefined && AppId != undefined) {
            $("#PatientNameSearch").val("");
            this.SearhPatientId = 0;
            this.IsLoadDocument = false;
            this.IsLoadBill = false;
            this.IsPrevious = false;
            this.IsVital = false;
            this.PrescriptionExist = false;
            this.loader.ShowLoader();
            this.AppointDate = date;
            this.AppointId = AppId;
            this.PatientId = id;
            localStorage.setItem("PatientId", id);
            var selectdate = this._CommonService.GetFormatDate(date);
            this.PrescriptionModel = new emr_prescription_mf();
            this._AppointmentService.PatientInfoLoad(id, selectdate, AppId).then(m => {
                this.PrescriptionModel.AppointmentDate = selectdate;
                this.PrescriptionModel.AppointmentId = AppId;

                var appointfind = m.ResultSet.AppointmentList.filter(a => a.PatientId == id);
                if (appointfind.length > 0) {
                    let CurntDate = this._CommonService.GetFormatDate(new Date());
                    var appdate = this._CommonService.GetFormatDate(appointfind[0].StartDate);
                    if (appdate < CurntDate) {
                        this.PrescriptionExist = true;
                        this.IsPreviousApp = true;
                    }
                    else {
                        this.PrescriptionExist = false;
                        this.IsPreviousApp = false;
                    }
                } else {
                    this.PrescriptionExist = true;
                }
                this.IsPatient = true;
                this.PatientDetailInfo = true;
                this.model = m.ResultSet.patientInfo[0];
                this.BillTypeList = m.ResultSet.BillTypeList;
                this.TittleList = m.ResultSet.TittleList;
                if (m.ResultSet.prescription_mf.length > 0) {
                    this.PrescriptionModel = m.ResultSet.prescription_mf[0];
                    this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(m.ResultSet.prescription_mf[0].AppointmentDate);
                    if (m.ResultSet.prescription_mf[0].FollowUpDate != null && m.ResultSet.prescription_mf[0].FollowUpDate != undefined)
                        this.PrescriptionModel.FollowUpDate = m.ResultSet.prescription_mf[0].FollowUpDate;
                    if (m.ResultSet.prescription_mf[0].FollowUpTime != null && m.ResultSet.prescription_mf[0].FollowUpTime != undefined)
                        this.PrescriptionModel.FollowUpTime = m.ResultSet.prescription_mf[0].FollowUpTime;
                    this.PrescriptionModel.Notes = m.ResultSet.prescription_mf[0].Notes;
                    this.PrescriptionModel.IsCreateAppointment = m.ResultSet.prescription_mf[0].IsCreateAppointment;
                    this.PrescriptionModel.IsTemplate = m.ResultSet.prescription_mf[0].IsTemplate;


                }
                localStorage.setItem("PatientName", this.model.PatientName);
                if (this.model.Image != null && this.model.Image != undefined && this.model.Image != "") {
                    this.getPatientImageUrlName(this.model.Image);
                    this.IsNewPatientImage = false;
                } else this.IsNewPatientImage = true;

                this.PrescriptionModel.AppointmentDate = selectdate;
                this.PrescriptionModel.AppointmentId = AppId;
                this.PrescriptionModel.PatientName = m.ResultSet.patientInfo[0].PatientName;
                this.PrescriptionModel.PatientId = id;
                this.PrescriptionModel.ClinicId = this.CompanyObj.CompanyID;
                this.PrescriptionModel.DoctorId = m.ResultSet.Doctorid;

                var appointfind = m.ResultSet.AppointmentList.filter(a => a.ID == AppId);

                if (appointfind.length > 0 && appointfind[0].AppointmentTime != null && this.PrescriptionModel.IsCreateAppointment == true)
                    this.PrescriptionModel.FollowUpTime = appointfind[0].AppointmentTime;
                if (this.CurntPrevDateList.length == 0) {
                    this.CurntPrevDateList = [];
                    this.CurntPrevDateList = m.ResultSet.CurntPrevDateList;
                }
                this.emr_prescription_complaint_dynamicArray = [];
                this.emr_prescription_diagnos_dynamicArray = [];
                this.emr_prescription_investigation_dynamicArray = [];
                this.emr_prescription_observation_dynamicArray = [];
                this.emr_prescription_treatment_dynamicArray = [];
                this.ComplaintList = [];
                this.ObservationList = [];
                this.InvestigationsList = [];
                this.DiagnosisList = [];
                m.ResultSet.prescription_complaint.forEach((item, index) => {
                    var list = m.ResultSet.ComplaintList.filter(a => a.id == item.ComplaintId);
                    if (list.length > 0 || item.ComplaintId == null) {
                        if (item.ComplaintId != null) {
                            item.ComplaintId = list[0].id;
                            item.favoriteId = list[0].favoriteId;
                            item.Isfavorite = list[0].Isfavorite;
                            item.IsSelected = true;
                        } else
                            item.IsSelected = true;
                    } else
                        item.IsSelected = false;
                    if (item.ComplaintId != null && list.length == 0) {
                        item.favoriteId = null;
                        item.Isfavorite = 0;
                    }
                    this.emr_prescription_complaint_dynamicArray.push(item);
                });
                m.ResultSet.ComplaintList.forEach((item, index) => {
                    var list1 = m.ResultSet.prescription_complaint.filter(a => a.ComplaintId == item.id);
                    if (list1.length == 0 || list1 == undefined) {
                        item.IsSelected = false;
                        item.ComplaintId = item.id;
                        this.emr_prescription_complaint_dynamicArray.push(item);
                    }
                });
                m.ResultSet.prescription_diagnos.forEach((item, index) => {
                    var list = m.ResultSet.DiagnosisList.filter(a => a.id == item.DiagnosId);
                    if (list.length > 0 || item.DiagnosId == null) {
                        if (item.DiagnosId != null) {
                            item.DiagnosId = list[0].id;
                            item.favoriteId = list[0].favoriteId;
                            item.Isfavorite = list[0].Isfavorite;
                            item.IsSelected = true;
                        } else
                            item.IsSelected = true;
                    }
                    else
                        item.IsSelected = false;
                    if (item.DiagnosId != null && list.length == 0) {
                        item.favoriteId = null;
                        item.Isfavorite = 0;
                    }
                    this.emr_prescription_diagnos_dynamicArray.push(item);
                });
                m.ResultSet.DiagnosisList.forEach((item, index) => {
                    var list1 = m.ResultSet.prescription_diagnos.filter(a => a.DiagnosId == item.id);
                    if (list1.length == 0 || list1 == undefined) {
                        item.IsSelected = false;
                        item.DiagnosId = item.id;
                        this.emr_prescription_diagnos_dynamicArray.push(item);
                    }
                });
                m.ResultSet.prescription_investigation.forEach((item, index) => {
                    var list = m.ResultSet.InvestigationsList.filter(a => a.id == item.InvestigationId);
                    if (list.length > 0 || item.InvestigationId == null) {
                        if (item.InvestigationId != null) {
                            item.InvestigationId = list[0].id;
                            item.favoriteId = list[0].favoriteId;
                            item.Isfavorite = list[0].Isfavorite;
                            item.IsSelected = true;
                        } else
                            item.IsSelected = true;
                    }
                    else
                        item.IsSelected = false;
                    if (item.InvestigationId != null && list.length == 0) {
                        item.favoriteId = null;
                        item.Isfavorite = 0;
                    }
                    this.emr_prescription_investigation_dynamicArray.push(item);
                });
                m.ResultSet.InvestigationsList.forEach((item, index) => {
                    var list1 = m.ResultSet.prescription_investigation.filter(a => a.InvestigationId == item.id);
                    if (list1.length == 0 || list1 == undefined) {
                        item.IsSelected = false;
                        item.InvestigationId = item.id;
                        this.emr_prescription_investigation_dynamicArray.push(item);
                    }
                });
                m.ResultSet.prescription_observation.forEach((item, index) => {
                    var list = m.ResultSet.ObservationList.filter(a => a.id == item.ObservationId);
                    if (list.length > 0 || item.ObservationId == null) {
                        if (item.ObservationId != null) {
                            item.ObservationId = list[0].id;
                            item.favoriteId = list[0].favoriteId;
                            item.Isfavorite = list[0].Isfavorite;
                            item.IsSelected = true;
                        } else
                            item.IsSelected = true;
                    }
                    else
                        item.IsSelected = false;
                    if (item.ObservationId != null && list.length == 0) {
                        item.favoriteId = null;
                        item.Isfavorite = 0;
                    }
                    this.emr_prescription_observation_dynamicArray.push(item);
                });
                m.ResultSet.ObservationList.forEach((item, index) => {
                    var list1 = m.ResultSet.prescription_observation.filter(a => a.ObservationId == item.id);
                    if (list1.length == 0 || list1 == undefined) {
                        item.IsSelected = false;
                        item.ObservationId = item.id;
                        this.emr_prescription_observation_dynamicArray.push(item);
                    }
                });
                m.ResultSet.prescription_treatment.forEach((item, index) => {

                    var medicineName = this.MedicineList.filter(a => a.Id == item.MedicineId)[0];
                    if (medicineName != null && medicineName != undefined)
                        item.MedicineName = medicineName.Medicine;
                    else
                        item.MedicineName = item.MedicineName;
                    this.emr_prescription_treatment_dynamicArray.push(item);
                });
                this.ComplaintList = m.ResultSet.ComplaintList;
                this.DiagnosisList = m.ResultSet.DiagnosisList;
                this.InvestigationsList = m.ResultSet.InvestigationsList;
                this.ObservationList = m.ResultSet.ObservationList;
                this.loader.HideLoader();
            });
        }
    }
    IsNewPatientImageEvent(FName) {
        this.IsNewImage = true;
    }
    getFileName(FName) {
        this.model.Image = FName;
    }
    ClearPatientImageUrl() {
        this.IsNewImage = true;
        this.model.Image = '';
        this.PatientImage = '';
    }
    getPatientImageUrlName(FName) {
        this.model.Image = FName;

        if (this.IsEdit && !this.IsNewImage) {
            this.PatientImage = GlobalVariable.BASE_File_URL + '' + FName;
        } else {
            this.PatientImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
    }
    ShowCalander() {
        this.PatientDetailInfo = !this.PatientDetailInfo;
        this.IsLoadDocument = false;
        this.IsLoadBill = false;
        this.IsPrevious = false;
        this.IsVital = false;
        this.AppointId = null;
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            var multiid = parseInt(localStorage.getItem('lingualId'));
            this.submitted = false;
            this.loader.ShowLoader();
            this.Usermodel.MultilingualId = multiid;
            if (this.Usermodel.SlotTime != null) {
                var time = '0' + ':' + this.Usermodel.SlotTime + ':0'
                this.Usermodel.SlotTime = time;
            }
            this._AppointmentService.UserSave(this.Usermodel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.DoctorList = [];
                    this.DoctorList = result.ResultSet.DoctorList.DoctList;
                    this.DoctorClanderList = result.ResultSet.DoctorCalander;
                    this.Usermodel = new ApplicationUserModel();
                    this.modalService.dismissAll(this.DocContent);
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    getPages(totalRecord: number, recordPerPage: number) {

        if (!isNaN(totalRecord))
            this.totalPages = this.getTotalPages(totalRecord, recordPerPage);
        this.getpagesRange();
    }
    getpagesRange() {
        if (this.pagesRange.indexOf(this.PModel.CurrentPage) == -1 || this.totalPages <= 10)
            this.papulatePagesRange();
        else if (this.pagesRange.length == 1 && this.totalPages > 10)
            this.papulatePagesRange();
    }
    papulatePagesRange() {
        this.pagesRange = [];
        var Result = Math.ceil(Math.max(this.PModel.CurrentPage, 1) / Math.max(this.PModel.RecordPerPage, 1));
        this.previousPage = ((Result - 1) * this.PModel.RecordPerPage)
        this.nextPage = (Result * this.PModel.RecordPerPage) + 1;
        if (this.nextPage > this.totalPages)
            this.nextPage = this.totalPages;
        for (var i = 1; i <= 10; i++) {
            if ((this.previousPage + i) > this.totalPages) return;
            this.pagesRange.push(this.previousPage + i)
        }
    }
    getTotalPages(totalRecord: number, recordPerPage: number): number {

        return Math.ceil(Math.max(totalRecord, 1) / Math.max(recordPerPage, 1));
    }
    onAppointmentDateChanged(event: IMyDateModel) {
        alert();
        if (event) {
        }
    }
    onFollowUpDateChanged(event: IMyDateModel) {

        if (event) {
        }
    }
    SaveMedicine(Checked: any, index: any) {
        if (Checked) {
            let FindRowObj = this.emr_prescription_treatment_dynamicArray[index];
            var MedicineId = FindRowObj.MedicineId;
            if (MedicineId == null || MedicineId == undefined) {
                this.RowNo = index;
                this.MedicineModel.Medicine = FindRowObj.MedicineName;
                this.OpenMedicinePrescription();
            } else
                this.toastr.Error('Error', "Already exist this medicine.");
        }

    }
    MedicineSaveOrUpdate(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._AppointmentService.MedicineSave(this.MedicineModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.emr_prescription_treatment_dynamicArray[this.RowNo].MedicineId = result.ResultSet.ID
                    this.emr_prescription_treatment_dynamicArray[this.RowNo].Measure = result.ResultSet.Measure;
                    this.emr_prescription_treatment_dynamicArray[this.RowNo].Instructions = result.ResultSet.Instructions;
                    this.emr_prescription_treatment_dynamicArray[this.RowNo].InstructionId = result.ResultSet.InstructionId;
                    this.modalService.dismissAll(this.MedicinePresModal);
                    this.toastr.Success(result.Message);
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    onAnniversaryDateChanged(event: IMyDateModel) {
        if (event) {
        }
    }
    public DoctorIds: any[] = [];
    IsShowDoctor(checkedValue: any, id: any) {
        this.DoctorIds = [];
        if (id != null && id != undefined) {
            if (this.IsShowDoctorIds != null)
                this.DoctorIds = this.IsShowDoctorIds;
            this.DoctorIds = JSON.parse("[" + (this.DoctorIds) + "]");

            if (checkedValue) {
                if (this.DoctorIds.length != 0) {
                    this.DoctorIds = this.DoctorIds.filter(function (item) {
                        return item !== id
                    });
                }
            } else {
                this.DoctorIds.push(id);
            }

            let ids = this.DoctorIds;
            this.loader.ShowLoader();
            this._ApplicationUserService.UserUpdate(ids.toString()).then(m => {
                if (m.IsSuccess) {
                    this.IsShowDoctorIds = m.ResultSet.IsShowDoctorIds;
                    this.DoctorList = m.ResultSet.DoctorList.DoctList;
                    this.DoctorClanderList = m.ResultSet.DoctorCalander;
                    this.BindEvent();
                    this.LoadCalendar(this.AppointmentDate);
                } else
                    this.toastr.Error('Error', m.ErrorMessage);
                this.loader.HideLoader();
            });
        }
    }
    onDateChanged(event: IMyDateModel) {
        if (event) {

        }
    }
    AgeChange() {
        this.model.DOB = null;
    }
    GenderChnage(evnt: any) {
        var genderid = parseInt(evnt);
        if (genderid == 2 && this.model.PrefixTittleId == 1)
            this.model.PrefixTittleId = 2
        if (genderid == 1 && this.model.PrefixTittleId == 2)
            this.model.PrefixTittleId = 1
    }
    //Search By Complaint
    LoadComplaint() {
        $('#ComplaintSearch').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.ComplaintSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.complaintModel = new emr_prescription_complaint();
                var findComp = this.emr_prescription_complaint_dynamicArray.filter(a => a.Complaint == ui.item.label);
                if (findComp.length == 0) {
                    this.complaintModel.Complaint = ui.item.label;
                    this.complaintModel.ComplaintId = ui.item.value;
                    this.complaintModel.Isfavorite = 0;
                    this.complaintModel.favoriteId = null;
                    this.complaintModel.IsSelected = true;
                    this.emr_prescription_complaint_dynamicArray.push(this.complaintModel);
                    this.complaintModel = new emr_prescription_complaint();
                } else {
                    this.emr_prescription_complaint_dynamicArray.filter(a => a.Complaint == ui.item.label)[0].IsSelected = true;
                }

            }
        });
    }
    //Search By Observation
    LoadObservation() {
        $('#ObservationSearch').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.ObservationSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.observationModel = new emr_prescription_observation();
                var findComp = this.emr_prescription_observation_dynamicArray.filter(a => a.Observation == ui.item.label);
                if (findComp.length == 0) {
                    this.observationModel.Observation = ui.item.label;
                    this.observationModel.ObservationId = ui.item.value;
                    this.observationModel.Isfavorite = 0;
                    this.observationModel.favoriteId = null;
                    this.observationModel.IsSelected = true;
                    this.emr_prescription_observation_dynamicArray.push(this.observationModel);
                    this.observationModel = new emr_prescription_observation();
                } else {
                    this.emr_prescription_observation_dynamicArray.filter(a => a.Observation == ui.item.label)[0].IsSelected = true;
                }

            }
        });
    }
    //Search By Investigation
    LoadInvestigation() {
        $('#InvestigationSearch').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.InvestigationSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {

                this.investigationModel = new emr_prescription_investigation();
                var findComp = this.emr_prescription_investigation_dynamicArray.filter(a => a.Investigation == ui.item.label);
                if (findComp.length == 0) {
                    this.investigationModel.Investigation = ui.item.label;
                    this.investigationModel.InvestigationId = ui.item.value;
                    this.investigationModel.Isfavorite = 0;
                    this.investigationModel.favoriteId = null;
                    this.investigationModel.IsSelected = true;
                    this.emr_prescription_investigation_dynamicArray.push(this.investigationModel);
                    this.investigationModel = new emr_prescription_investigation();
                } else {
                    this.emr_prescription_investigation_dynamicArray.filter(a => a.Investigation == ui.item.label)[0].IsSelected = true;
                }
            }
        });
    }
    //Search By Diagnos
    LoadDiagnos() {
        $('#DiagnosSearch').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.DiagnosSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.diagnosModel = new emr_prescription_diagnos();
                var findComp = this.emr_prescription_diagnos_dynamicArray.filter(a => a.Diagnos == ui.item.label);
                if (findComp.length == 0) {
                    this.diagnosModel.Diagnos = ui.item.label;
                    this.diagnosModel.DiagnosId = ui.item.value;
                    this.diagnosModel.Isfavorite = 0;
                    this.diagnosModel.favoriteId = null;
                    this.diagnosModel.IsSelected = true;
                    this.emr_prescription_diagnos_dynamicArray.push(this.diagnosModel);
                    this.diagnosModel = new emr_prescription_diagnos();
                } else {
                    this.emr_prescription_diagnos_dynamicArray.filter(a => a.Diagnos == ui.item.label)[0].IsSelected = true;
                }
            }
        });
    }
    ExistComplaint(val: any) {
        if (val != undefined) {
            var obj = this.complaintModel;
            var findComp = this.emr_prescription_complaint_dynamicArray.filter(a => a.Complaint == val.Complaint);
            if (findComp.length == 0) {
                obj.Complaint = val.Complaint;
                obj.ComplaintId = val.id;
                obj.IsSelected = true;
                this.emr_prescription_complaint_dynamicArray.push(obj);
            }
            else
                this.emr_prescription_complaint_dynamicArray.filter(a => a.Complaint == val.Complaint)[0].IsSelected = !this.emr_prescription_complaint_dynamicArray.filter(a => a.Complaint == val.Complaint)[0].IsSelected;
            this.complaintModel = new emr_prescription_complaint();
        }
    }
    AddComplaint(val: any) {
        if (val != undefined) {
            var obj = this.complaintModel;
            var findComp = this.emr_prescription_complaint_dynamicArray.filter(a => a.Complaint == val);
            if (findComp.length == 0) {
                obj.Complaint = val;
                obj.ComplaintId = val.id;
                obj.IsSelected = true;
                this.emr_prescription_complaint_dynamicArray.push(obj);
            } else
                this.emr_prescription_complaint_dynamicArray.filter(a => a.Complaint == val)[0].IsSelected = true;
            this.complaintModel = new emr_prescription_complaint();

        }
    }
    ExistObservation(val: any) {
        if (val != undefined) {
            var obj = this.observationModel;
            var findComp = this.emr_prescription_observation_dynamicArray.filter(a => a.Observation == val.Observation);
            if (findComp.length == 0) {
                obj.Observation = val.Observation;
                obj.ObservationId = val.id;
                obj.IsSelected = true;
                this.emr_prescription_observation_dynamicArray.push(obj);
            }
            else
                this.emr_prescription_observation_dynamicArray.filter(a => a.Observation == val.Observation)[0].IsSelected = !this.emr_prescription_observation_dynamicArray.filter(a => a.Observation == val.Observation)[0].IsSelected;
            this.observationModel = new emr_prescription_observation();
        }
    }
    AddObservation(val: any) {
        if (val != undefined) {
            var obj = this.observationModel;
            var findComp = this.emr_prescription_observation_dynamicArray.filter(a => a.Observation == val);
            if (findComp.length == 0) {
                obj.Observation = val;
                obj.ObservationId = val.id;
                obj.IsSelected = true;
                this.emr_prescription_observation_dynamicArray.push(obj);
            } else
                this.emr_prescription_observation_dynamicArray.filter(a => a.Observation == val)[0].IsSelected = true;
            this.observationModel = new emr_prescription_observation();

        }
    }
    ExistInvestigations(val: any) {
        if (val != undefined) {
            var obj = this.investigationModel;
            var findComp = this.emr_prescription_investigation_dynamicArray.filter(a => a.Investigation == val.Investigation);
            if (findComp.length == 0) {
                obj.Investigation = val.Investigation;
                obj.InvestigationId = val.id;
                obj.IsSelected = true;
                this.emr_prescription_investigation_dynamicArray.push(obj);
            }
            else
                this.emr_prescription_investigation_dynamicArray.filter(a => a.Investigation == val.Investigation)[0].IsSelected = !this.emr_prescription_investigation_dynamicArray.filter(a => a.Investigation == val.Investigation)[0].IsSelected;
            this.investigationModel = new emr_prescription_investigation();
        }
    }
    AddInvestigations(val: any) {

        if (val != undefined) {
            var obj = this.investigationModel;
            var findComp = this.emr_prescription_investigation_dynamicArray.filter(a => a.Investigation == val);
            if (findComp.length == 0) {
                obj.Investigation = val;
                obj.InvestigationId = val.id;
                obj.IsSelected = true;
                this.emr_prescription_investigation_dynamicArray.push(obj);
            } else
                this.emr_prescription_investigation_dynamicArray.filter(a => a.Investigation == val)[0].IsSelected = true;
            this.investigationModel = new emr_prescription_investigation();

        }
    }
    ExistDiagnosis(val: any) {
        if (val != undefined) {
            var obj = this.diagnosModel;
            var findComp = this.emr_prescription_diagnos_dynamicArray.filter(a => a.Diagnos == val.Diagnos);
            if (findComp.length == 0) {
                obj.Diagnos = val.Diagnos;
                obj.DiagnosId = val.id;
                obj.IsSelected = true;
                this.emr_prescription_diagnos_dynamicArray.push(obj);
            }
            else
                this.emr_prescription_diagnos_dynamicArray.filter(a => a.Diagnos == val.Diagnos)[0].IsSelected = !this.emr_prescription_diagnos_dynamicArray.filter(a => a.Diagnos == val.Diagnos)[0].IsSelected;
            this.diagnosModel = new emr_prescription_diagnos();
        }
    }
    AddDiagnosis(val: any) {
        if (val != undefined) {
            var obj = this.diagnosModel;
            var findComp = this.emr_prescription_diagnos_dynamicArray.filter(a => a.Diagnos == val);
            if (findComp.length == 0) {
                obj.Diagnos = val;
                obj.DiagnosId = val.id;
                obj.IsSelected = true;
                this.emr_prescription_diagnos_dynamicArray.push(obj);
            } else
                this.emr_prescription_diagnos_dynamicArray.filter(a => a.Diagnos == val)[0].IsSelected = true;
            this.diagnosModel = new emr_prescription_diagnos();
        }
    }
    Addtreatment() {
        var obj = new emr_prescription_treatment();
        this.emr_prescription_treatment_dynamicArray.push(obj);
        this.loader.HideLoader();
    }
    RemoveRow(rowno: any) {
        if (this.emr_prescription_treatment_dynamicArray.length > 1) {
            this.emr_prescription_treatment_dynamicArray.splice(rowno, 1);
        }
    }
    PrescriptionSaveOrUpdate(isValid: boolean): void {
        this.submitted = true;
        if (isValid) {
            if (this.PrescriptionModel.AppointmentDate == undefined && this.PrescriptionModel.AppointmentDate == null) {
                this.toastr.Error("Error", "Please select Appointment Date.");
                return;
            }
            if (this.PrescriptionModel.IsTemplate == true && this.PrescriptionModel.TemplateName == undefined && this.PrescriptionModel.TemplateName == null) {
                this.toastr.Error("Error", "Please enter template name.");
                return;
            }
            if (this.PrescriptionModel.IsCreateAppointment == true && this.PrescriptionModel.FollowUpTime == undefined && this.PrescriptionModel.FollowUpTime == null) {
                this.toastr.Error("Error", "Please select time.");
                return;
            }
            this.submitted = false;
            this.loader.ShowLoader();
            this.PrescriptionModel.emr_prescription_complaint = [];
            this.PrescriptionModel.emr_prescription_diagnos = [];
            this.PrescriptionModel.emr_prescription_investigation = [];
            this.PrescriptionModel.emr_prescription_observation = [];
            this.PrescriptionModel.emr_prescription_complaint = this.emr_prescription_complaint_dynamicArray.filter(a => a.IsSelected == true);
            this.PrescriptionModel.emr_prescription_diagnos = this.emr_prescription_diagnos_dynamicArray.filter(a => a.IsSelected == true);
            this.PrescriptionModel.emr_prescription_investigation = this.emr_prescription_investigation_dynamicArray.filter(a => a.IsSelected == true);
            this.PrescriptionModel.emr_prescription_observation = this.emr_prescription_observation_dynamicArray.filter(a => a.IsSelected == true);
            if (this.PrescriptionModel.FollowUpTime != undefined)
                this.PrescriptionModel.FollowUpTime = this.Convert12TO24(this.PrescriptionModel.FollowUpTime);
            this.PrescriptionModel.emr_prescription_treatment = this.emr_prescription_treatment_dynamicArray.filter(a => a.MedicineName != null);
            this.PrescriptionModel.AppointmentId = this.AppointId;
            this._AppointmentService.PrescriptionSave(this.PrescriptionModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.PatientDetailInfo = false;
                    this.AppointId = null;
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    LoadTemplate(id: any) {
        this.loader.ShowLoader();
        this._AppointmentService.TemplateLoadById(id).then(m => {
            this.emr_prescription_complaint_dynamicArray = [];
            this.emr_prescription_diagnos_dynamicArray = [];
            this.emr_prescription_investigation_dynamicArray = [];
            this.emr_prescription_observation_dynamicArray = [];
            this.emr_prescription_treatment_dynamicArray = [];
            if (m.IsSuccess) {

                this.modalService.dismissAll(this.MedicineTemplateContent);
                this.MedicineList = m.ResultSet.MedicineList;
                this.PrescriptionModel = m.ResultSet.result;
                this.PrescriptionModel.ID = 0;
                this.PrescriptionModel.AppointmentId = this.AppointId;
                this.PrescriptionModel.AppointmentDate = this.AppointDate;
                var app = this.AppointmentList.filter(a => a.ID == this.AppointId);
                if (app != null) {
                    this.PrescriptionModel.DoctorId = app[0].DoctorId;
                    this.PrescriptionModel.PatientId = app[0].PatientId;
                }
                //this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(m.ResultSet.result.AppointmentDate);
                this.PrescriptionModel.PatientName = m.ResultSet.result.emr_patient_mf.PatientName;
                this.PrescriptionModel.TemplateName = m.ResultSet.result.emr_prescription_treatment_template[0].TemplateName;

                this.emr_prescription_complaint_dynamicArray = [];
                this.emr_prescription_diagnos_dynamicArray = [];
                this.emr_prescription_investigation_dynamicArray = [];
                this.emr_prescription_observation_dynamicArray = [];
                this.emr_prescription_treatment_dynamicArray = [];
                if (this.PrescriptionModel.emr_prescription_complaint != null) {
                    this.PrescriptionModel.emr_prescription_complaint.forEach((item, index) => {
                        var list = this.ComplaintList.filter(a => a.id == item.ComplaintId);
                        if (list.length > 0 || item.ComplaintId == null) {
                            if (item.ComplaintId != null) {
                                item.ComplaintId = list[0].id;
                                item.favoriteId = list[0].favoriteId;
                                item.Isfavorite = list[0].Isfavorite;
                                item.IsSelected = true;
                            } else
                                item.IsSelected = true;
                        } else
                            item.IsSelected = false;
                        if (item.ComplaintId != null && list.length == 0) {
                            item.favoriteId = null;
                            item.Isfavorite = 0;
                        }
                        item.PatientId = app[0].PatientId;
                        this.emr_prescription_complaint_dynamicArray.push(item);
                    });
                    this.ComplaintList.forEach((item, index) => {
                        var list1 = this.PrescriptionModel.emr_prescription_complaint.filter(a => a.ComplaintId == item.id);
                        if (list1.length == 0 || list1 == undefined) {
                            item.IsSelected = false;
                            item.ComplaintId = item.id;
                            this.emr_prescription_complaint_dynamicArray.push(item);
                        }
                    });
                } else {
                    this.ComplaintList.forEach((item, index) => {
                        item.IsSelected = false;
                        item.ComplaintId = item.id;
                        this.emr_prescription_complaint_dynamicArray.push(item);

                    });
                }
                if (this.PrescriptionModel.emr_prescription_diagnos != null) {
                    this.PrescriptionModel.emr_prescription_diagnos.forEach((item, index) => {
                        var list = this.DiagnosisList.filter(a => a.id == item.DiagnosId);
                        if (list.length > 0 || item.DiagnosId == null) {
                            if (item.DiagnosId != null) {
                                item.DiagnosId = list[0].id;
                                item.favoriteId = list[0].favoriteId;
                                item.Isfavorite = list[0].Isfavorite;
                                item.IsSelected = true;
                            } else
                                item.IsSelected = true;
                        }
                        else
                            item.IsSelected = false;
                        if (item.DiagnosId != null && list.length == 0) {
                            item.favoriteId = null;
                            item.Isfavorite = 0;
                        }
                        item.PatientId = app[0].PatientId;
                        this.emr_prescription_diagnos_dynamicArray.push(item);
                    });
                    this.DiagnosisList.forEach((item, index) => {
                        var list1 = this.PrescriptionModel.emr_prescription_diagnos.filter(a => a.DiagnosId == item.id);
                        if (list1.length == 0 || list1 == undefined) {
                            item.IsSelected = false;
                            item.DiagnosId = item.id;
                            this.emr_prescription_diagnos_dynamicArray.push(item);
                        }
                    });
                } else {
                    this.DiagnosisList.forEach((item, index) => {
                        item.IsSelected = false;
                        item.DiagnosId = item.id;
                        this.emr_prescription_diagnos_dynamicArray.push(item);

                    });
                }

                if (this.PrescriptionModel.emr_prescription_investigation != null) {
                    this.PrescriptionModel.emr_prescription_investigation.forEach((item, index) => {
                        var list = this.InvestigationsList.filter(a => a.id == item.InvestigationId);
                        if (list.length > 0 || item.InvestigationId == null) {
                            if (item.InvestigationId != null) {
                                item.InvestigationId = list[0].id;
                                item.favoriteId = list[0].favoriteId;
                                item.Isfavorite = list[0].Isfavorite;
                                item.IsSelected = true;
                            } else
                                item.IsSelected = true;
                        }
                        else
                            item.IsSelected = false;
                        if (item.InvestigationId != null && list.length == 0) {
                            item.favoriteId = null;
                            item.Isfavorite = 0;
                        }
                        item.PatientId = app[0].PatientId;
                        this.emr_prescription_investigation_dynamicArray.push(item);
                    });
                    this.InvestigationsList.forEach((item, index) => {
                        var list1 = this.PrescriptionModel.emr_prescription_investigation.filter(a => a.InvestigationId == item.id);
                        if (list1.length == 0 || list1 == undefined) {
                            item.IsSelected = false;
                            item.InvestigationId = item.id;
                            this.emr_prescription_investigation_dynamicArray.push(item);
                        }
                    });
                } else {
                    this.InvestigationsList.forEach((item, index) => {
                        item.IsSelected = false;
                        item.InvestigationId = item.id;
                        this.emr_prescription_investigation_dynamicArray.push(item);

                    });
                }

                if (this.PrescriptionModel.emr_prescription_observation != null) {
                    this.PrescriptionModel.emr_prescription_observation.forEach((item, index) => {
                        var list = this.ObservationList.filter(a => a.id == item.ObservationId);
                        if (list.length > 0 || item.ObservationId == null) {
                            if (item.ObservationId != null) {
                                item.ObservationId = list[0].id;
                                item.favoriteId = list[0].favoriteId;
                                item.Isfavorite = list[0].Isfavorite;
                                item.IsSelected = true;
                            } else
                                item.IsSelected = true;
                        }
                        else
                            item.IsSelected = false;
                        if (item.ObservationId != null && list.length == 0) {
                            item.favoriteId = null;
                            item.Isfavorite = 0;
                        }
                        item.PatientId = app[0].PatientId;
                        this.emr_prescription_observation_dynamicArray.push(item);
                    });
                    this.ObservationList.forEach((item, index) => {
                        var list1 = this.PrescriptionModel.emr_prescription_observation.filter(a => a.ObservationId == item.id);
                        if (list1.length == 0 || list1 == undefined) {
                            item.IsSelected = false;
                            item.ObservationId = item.id;
                            this.emr_prescription_observation_dynamicArray.push(item);
                        }
                    });
                } else {
                    this.ObservationList.forEach((item, index) => {
                        item.IsSelected = false;
                        item.ObservationId = item.id;
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
                        item.PatientId = app[0].PatientId;
                        this.emr_prescription_treatment_dynamicArray.push(item);
                    });
                }
            } else
                this.toastr.Error('Error', m.ErrorMessage);
            //this.AppointId = null;
            this.loader.HideLoader();
        });
    }
    PrintAndSave() {
        if (this.PrescriptionModel.AppointmentDate == undefined || this.PrescriptionModel.AppointmentDate == null) {
            this.toastr.Error("Error", "Please select Date.");
            return;
        }
        if (this.PrescriptionModel.IsTemplate == true && this.PrescriptionModel.TemplateName == undefined && this.PrescriptionModel.TemplateName == null) {
            this.toastr.Error("Error", "Please enter template name.");
            return;
        }
        if (this.PrescriptionModel.IsCreateAppointment == true && this.PrescriptionModel.FollowUpTime == undefined && this.PrescriptionModel.FollowUpTime == null) {
            this.toastr.Error("Error", "Please select time.");
            return;
        }
        if (this.emr_prescription_treatment_dynamicArray.length == 0 || this.emr_prescription_treatment_dynamicArray[0].MedicineName == undefined) {
            this.toastr.Error("Error", "Please select at leatest one medicine.");
            return;
        }
        this.loader.ShowLoader();

        this.PrescriptionModel.emr_prescription_complaint = [];
        this.PrescriptionModel.emr_prescription_diagnos = [];
        this.PrescriptionModel.emr_prescription_investigation = [];
        this.PrescriptionModel.emr_prescription_observation = [];
        if (this.PrescriptionModel.FollowUpTime != undefined)
            this.PrescriptionModel.FollowUpTime = this.Convert12TO24(this.PrescriptionModel.FollowUpTime);
        this.PrescriptionModel.emr_prescription_treatment = this.emr_prescription_treatment_dynamicArray;
        this.PrescriptionModel.emr_prescription_complaint = this.emr_prescription_complaint_dynamicArray.filter(a => a.IsSelected == true);
        this.PrescriptionModel.emr_prescription_diagnos = this.emr_prescription_diagnos_dynamicArray.filter(a => a.IsSelected == true);
        this.PrescriptionModel.emr_prescription_investigation = this.emr_prescription_investigation_dynamicArray.filter(a => a.IsSelected == true);
        this.PrescriptionModel.emr_prescription_observation = this.emr_prescription_observation_dynamicArray.filter(a => a.IsSelected == true);
        this._AppointmentService.PrescriptionSave(this.PrescriptionModel).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {
                this.toastr.Success(result.Message);
                this.emr_prescription_treatment_dynamicArray = [];
                this.PatientVitalList = [];

                this.PatientDetailInfo = !this.PatientDetailInfo;
                this.PrescriptionModel = result.ResultSet.Model;
                if (result.ResultSet.patientInfo != null) {
                    this.PatientRXmodel = result.ResultSet.patientInfo;
                    if (this.PatientRXmodel.ClinicIogo != null)
                        this.PatientRXmodel.ClinicIogo = GlobalVariable.BASE_Temp_File_URL + '' + result.ResultSet.patientInfo.ClinicIogo;
                    this.PrescriptionModel.PatientName = result.ResultSet.patientInfo.PatientName;
                }
                if (result.ResultSet.doctor != null)
                    this.DoctorInfo = result.ResultSet.doctor;

                if (this.DoctorInfo == null || this.DoctorInfo == undefined) {
                    this.DoctorInfo.Name = "";
                    this.DoctorInfo.Designation = "";
                    this.DoctorInfo.Qualification = "";
                    this.DoctorInfo.PhoneNo = "";
                }
                if (this.PatientRXmodel == undefined || this.PatientRXmodel == null) {
                    this.PatientRXmodel.PatientName = "";
                    this.PatientRXmodel.Age = "";
                }
                this.PatientRXmodel.AppointmentDate = this._CommonService.GetFormatDate(this.PrescriptionModel.AppointmentDate);
                this.PatientVitalList = result.ResultSet.vitallist;

                if (this.PrescriptionModel.emr_prescription_complaint != null) {
                    this.PrescriptionModel.emr_prescription_complaint.forEach((item, index) => {
                        var list = this.ComplaintList.filter(a => a.id == item.ComplaintId);
                        if (list.length > 0 || item.ComplaintId == null) {
                            if (item.ComplaintId != null)
                                item.ComplaintId = list[0].id;
                            item.IsSelected = true;
                        }
                        else
                            item.IsSelected = false;
                        this.emr_prescription_complaint_dynamicArray.push(item);
                    });
                    this.ComplaintList.forEach((item, index) => {
                        var list1 = this.PrescriptionModel.emr_prescription_complaint.filter(a => a.ComplaintId == item.id);
                        if (list1.length == 0 || list1 == undefined) {
                            item.IsSelected = false;
                            item.ComplaintId = item.id;
                            this.emr_prescription_complaint_dynamicArray.push(item);
                        }
                    });
                } else {
                    this.ComplaintList.forEach((item, index) => {

                        item.IsSelected = false;
                        item.ComplaintId = item.id;
                        this.emr_prescription_complaint_dynamicArray.push(item);

                    });
                }

                if (this.PrescriptionModel.emr_prescription_diagnos != null) {
                    this.PrescriptionModel.emr_prescription_diagnos.forEach((item, index) => {
                        var list = this.DiagnosisList.filter(a => a.id == item.DiagnosId);
                        if (list.length > 0 || item.DiagnosId == null) {
                            if (item.DiagnosId != null)
                                item.DiagnosId = list[0].id;
                            item.IsSelected = true;
                        }
                        else
                            item.IsSelected = false;
                        this.emr_prescription_diagnos_dynamicArray.push(item);
                    });
                    this.DiagnosisList.forEach((item, index) => {
                        var list1 = this.PrescriptionModel.emr_prescription_diagnos.filter(a => a.DiagnosId == item.id);
                        if (list1.length == 0 || list1 == undefined) {
                            item.IsSelected = false;
                            item.DiagnosId = item.id;
                            this.emr_prescription_diagnos_dynamicArray.push(item);
                        }
                    });
                } else {
                    this.DiagnosisList.forEach((item, index) => {
                        item.IsSelected = false;
                        item.DiagnosId = item.id;
                        this.emr_prescription_diagnos_dynamicArray.push(item);

                    });
                }

                if (this.PrescriptionModel.emr_prescription_investigation != null) {
                    this.PrescriptionModel.emr_prescription_investigation.forEach((item, index) => {
                        var list = this.InvestigationsList.filter(a => a.id == item.InvestigationId);
                        if (list.length > 0 || item.InvestigationId == null) {
                            if (item.InvestigationId != null)
                                item.InvestigationId = list[0].id;
                            item.IsSelected = true;
                        }
                        else
                            item.IsSelected = false;
                        this.emr_prescription_investigation_dynamicArray.push(item);
                    });
                    this.InvestigationsList.forEach((item, index) => {
                        var list1 = this.PrescriptionModel.emr_prescription_investigation.filter(a => a.InvestigationId == item.id);
                        if (list1.length == 0 || list1 == undefined) {
                            item.IsSelected = false;
                            item.InvestigationId = item.id;
                            this.emr_prescription_investigation_dynamicArray.push(item);
                        }
                    });
                } else {
                    this.InvestigationsList.forEach((item, index) => {
                        item.IsSelected = false;
                        item.InvestigationId = item.id;
                        this.emr_prescription_investigation_dynamicArray.push(item);
                    });
                }

                if (this.PrescriptionModel.emr_prescription_observation != null) {
                    this.PrescriptionModel.emr_prescription_observation.forEach((item, index) => {
                        var list = this.ObservationList.filter(a => a.id == item.ObservationId);
                        if (list.length > 0 || item.ObservationId == null) {
                            if (item.ObservationId != null)
                                item.ObservationId = list[0].id;
                            item.IsSelected = true;
                        }
                        else
                            item.IsSelected = false;
                        this.emr_prescription_observation_dynamicArray.push(item);
                    });
                    this.ObservationList.forEach((item, index) => {
                        var list1 = this.PrescriptionModel.emr_prescription_observation.filter(a => a.ObservationId == item.id);
                        if (list1.length == 0 || list1 == undefined) {
                            item.IsSelected = false;
                            item.ObservationId = item.id;
                            this.emr_prescription_observation_dynamicArray.push(item);
                        }
                    });
                } else {
                    this.ObservationList.forEach((item, index) => {
                        item.IsSelected = false;
                        item.ObservationId = item.id;
                        this.emr_prescription_observation_dynamicArray.push(item);

                    });
                }


                if (this.PrescriptionModel.emr_prescription_treatment != null) {
                    this.PrescriptionModel.emr_prescription_treatment.forEach((item, index) => {
                        var medicineName = this.MedicineList.filter(a => a.Id == item.MedicineId)[0];
                        if (medicineName != null && medicineName != undefined)
                            item.MedicineName = medicineName.Medicine;
                        else
                            item.MedicineName = item.MedicineName;

                        item.MedicineType = medicineName.Type;
                        item.TypeId = medicineName.TypeId;
                        this.emr_prescription_treatment_dynamicArray.push(item);
                    });
                }
                this.AppointId = null;
                this.modalService.open(this.PrintRxContent, { size: 'lg' });
            }
            else
                this.toastr.Error('Error', result.ErrorMessage);

            this.loader.HideLoader();
        });
    }
    AddFavorite(val: any, Type: any) {
        if (val != undefined && val != null) {
            this.loader.ShowLoader();
            this.FavoriteModel = new emr_notes_favorite();
            this.FavoriteModel.DoctorId = this.PrescriptionModel.DoctorId;
            if (Type == "C") {
                if (val.favoriteId != null && val.favoriteId != undefined) {
                    this._AppointmentService.DeleteFavorite(val.favoriteId).then(m => {
                        if (m.ErrorMessage != null)
                            this.toastr.Error('Error', m.ErrorMessage);
                        this.emr_prescription_complaint_dynamicArray.filter(a => a.ComplaintId == val.ComplaintId)[0].Isfavorite = 0;
                        this.emr_prescription_complaint_dynamicArray.filter(a => a.ComplaintId == val.ComplaintId)[0].favoriteId = null;
                        this.loader.HideLoader();
                    });
                }
                else {
                    if (val.Complaint != null && val.Complaint != "") {
                        this.FavoriteModel.ReferenceId = val.ComplaintId;
                        this.FavoriteModel.ReferenceType = "C";
                    }
                    this._AppointmentService.FavoriteSaveOrUpdate(this.FavoriteModel).then(m => {
                        var result = JSON.parse(m._body);
                        if (result.IsSuccess) {
                            this.emr_prescription_complaint_dynamicArray.filter(a => a.ComplaintId == val.ComplaintId)[0].Isfavorite = 1;
                            this.emr_prescription_complaint_dynamicArray.filter(a => a.ComplaintId == val.ComplaintId)[0].favoriteId = result.ResultSet.ID;
                            this.toastr.Success(result.Message);
                        }
                        else
                            this.toastr.Error('Error', result.ErrorMessage);

                        this.loader.HideLoader();
                    });
                }
            }
            else if (Type == "O") {
                if (val.favoriteId != null && val.favoriteId != undefined) {
                    this._AppointmentService.DeleteFavorite(val.favoriteId).then(m => {
                        if (m.ErrorMessage != null)
                            this.toastr.Error('Error', m.ErrorMessage);
                        this.emr_prescription_observation_dynamicArray.filter(a => a.ObservationId == val.ObservationId)[0].Isfavorite = 0;
                        this.emr_prescription_observation_dynamicArray.filter(a => a.ObservationId == val.ObservationId)[0].favoriteId = null;
                        this.loader.HideLoader();
                    });
                }
                else {
                    if (val.Observation != null && val.Observation != "") {
                        this.FavoriteModel.ReferenceId = val.ObservationId;
                        this.FavoriteModel.ReferenceType = "O";
                    }
                    this._AppointmentService.FavoriteSaveOrUpdate(this.FavoriteModel).then(m => {
                        var result = JSON.parse(m._body);
                        if (result.IsSuccess) {
                            this.emr_prescription_observation_dynamicArray.filter(a => a.ObservationId == val.ObservationId)[0].Isfavorite = 1;
                            this.emr_prescription_observation_dynamicArray.filter(a => a.ObservationId == val.ObservationId)[0].favoriteId = result.ResultSet.ID;
                            this.toastr.Success(result.Message);
                        }
                        else
                            this.toastr.Error('Error', result.ErrorMessage);

                        this.loader.HideLoader();
                    });
                }
            }
            else if (Type == "I") {
                if (val.favoriteId != null && val.favoriteId != undefined) {
                    this._AppointmentService.DeleteFavorite(val.favoriteId).then(m => {
                        if (m.ErrorMessage != null)
                            this.toastr.Error('Error', m.ErrorMessage);
                        this.emr_prescription_investigation_dynamicArray.filter(a => a.InvestigationId == val.InvestigationId)[0].Isfavorite = 0;
                        this.emr_prescription_investigation_dynamicArray.filter(a => a.InvestigationId == val.InvestigationId)[0].favoriteId = null;
                        this.loader.HideLoader();
                    });
                }
                else {

                    if (val.Investigation != null && val.Investigation != "") {
                        this.FavoriteModel.ReferenceId = val.InvestigationId;
                        this.FavoriteModel.ReferenceType = "I";
                    }
                    this._AppointmentService.FavoriteSaveOrUpdate(this.FavoriteModel).then(m => {
                        var result = JSON.parse(m._body);
                        if (result.IsSuccess) {
                            this.emr_prescription_investigation_dynamicArray.filter(a => a.InvestigationId == val.InvestigationId)[0].Isfavorite = 1;
                            this.emr_prescription_investigation_dynamicArray.filter(a => a.InvestigationId == val.InvestigationId)[0].favoriteId = result.ResultSet.ID;
                            this.toastr.Success(result.Message);
                        }
                        else
                            this.toastr.Error('Error', result.ErrorMessage);

                        this.loader.HideLoader();
                    });
                }
            }
            else if (Type == "D") {
                if (val.favoriteId != null && val.favoriteId != undefined) {
                    this._AppointmentService.DeleteFavorite(val.favoriteId).then(m => {
                        if (m.ErrorMessage != null)
                            this.toastr.Error('Error', m.ErrorMessage);
                        this.emr_prescription_diagnos_dynamicArray.filter(a => a.DiagnosId == val.DiagnosId)[0].Isfavorite = 0;
                        this.emr_prescription_diagnos_dynamicArray.filter(a => a.DiagnosId == val.DiagnosId)[0].favoriteId = null;
                        this.loader.HideLoader();
                    });
                }
                else {
                    if (val.Diagnos != null && val.Diagnos != "") {
                        this.FavoriteModel.ReferenceId = val.DiagnosId;
                        this.FavoriteModel.ReferenceType = "D";
                    }
                    this._AppointmentService.FavoriteSaveOrUpdate(this.FavoriteModel).then(m => {
                        var result = JSON.parse(m._body);
                        if (result.IsSuccess) {
                            this.emr_prescription_diagnos_dynamicArray.filter(a => a.DiagnosId == val.DiagnosId)[0].Isfavorite = 1;
                            this.emr_prescription_diagnos_dynamicArray.filter(a => a.DiagnosId == val.DiagnosId)[0].favoriteId = result.ResultSet.ID;
                            this.toastr.Success(result.Message);
                        }
                        else
                            this.toastr.Error('Error', result.ErrorMessage);

                        this.loader.HideLoader();
                    });
                }
            }
        }
    }
    OnDayEnter() {
        if (this.PrescriptionModel.Day != null && this.PrescriptionModel.Day != undefined) {
            let day = parseInt(this.PrescriptionModel.Day);
            var date = new Date();
            var followDate = date.setDate(date.getDate() + day);
            this.PrescriptionModel.FollowUpDate = new Date(followDate);
        }

    }
    SendEmail() {
        if (this.model.Email != null && this.model.Email != "") {
            this.PrescriptionModel.Email = this.model.Email;
            this.modalService.dismissAll(this.EmailModal);
            if (this.PrescriptionModel.AppointmentDate == undefined || this.PrescriptionModel.AppointmentDate == null) {
                this.toastr.Error("Error", "Please select Date.");
                return;
            }
            if (this.PrescriptionModel.IsTemplate == true && this.PrescriptionModel.TemplateName == undefined && this.PrescriptionModel.TemplateName == null) {
                this.toastr.Error("Error", "Please enter template name.");
                return;
            }
            if (this.PrescriptionModel.IsCreateAppointment == true && this.PrescriptionModel.FollowUpTime == undefined && this.PrescriptionModel.FollowUpTime == null) {
                this.toastr.Error("Error", "Please select time.");
                return;
            }
            if (this.emr_prescription_treatment_dynamicArray.length == 0 || this.emr_prescription_treatment_dynamicArray[0].MedicineName == undefined) {
                this.toastr.Error("Error", "Please select at leatest one medicine.");
                return;
            }
            this.loader.ShowLoader();
            this.PrescriptionModel.emr_prescription_complaint = [];
            this.PrescriptionModel.emr_prescription_diagnos = [];
            this.PrescriptionModel.emr_prescription_investigation = [];
            this.PrescriptionModel.emr_prescription_observation = [];
            if (this.PrescriptionModel.FollowUpTime != undefined)
                this.PrescriptionModel.FollowUpTime = this.Convert12TO24(this.PrescriptionModel.FollowUpTime);
            this.PrescriptionModel.emr_prescription_treatment = this.emr_prescription_treatment_dynamicArray;
            this.PrescriptionModel.emr_prescription_complaint = this.emr_prescription_complaint_dynamicArray.filter(a => a.IsSelected == true);
            this.PrescriptionModel.emr_prescription_diagnos = this.emr_prescription_diagnos_dynamicArray.filter(a => a.IsSelected == true);
            this.PrescriptionModel.emr_prescription_investigation = this.emr_prescription_investigation_dynamicArray.filter(a => a.IsSelected == true);
            this.PrescriptionModel.emr_prescription_observation = this.emr_prescription_observation_dynamicArray.filter(a => a.IsSelected == true);
            this._AppointmentService.PrescriptionSave(this.PrescriptionModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        } else {
            this.modalService.open(this.EmailModal, { size: 'sm' });
        }
    }
}