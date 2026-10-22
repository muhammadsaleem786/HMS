import { Component, Input, Output, OnInit, OnDestroy, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener, Directive, ChangeDetectorRef, TemplateRef } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms'; import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { VoiceRecognitionService } from './voice-recognition.service';
import { emr_patient, emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo, InvoiceCompanyInfo, emr_notes_favorite } from './../Appointment/AppointmentModel';
import { emr_prescription_mf, emr_prescription_complaint, emr_prescription_diagnos, emr_prescription_investigation, emr_prescription_observation, emr_prescription_treatment, emr_medicine } from './PrescriptionModel';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { NgbTimeStruct } from '@ng-bootstrap/ng-bootstrap';
import { NgbModal, ModalDismissReasons, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PrescriptionService } from './PrescriptionService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { IMultiSelectOption, IMultiSelectSettings, IMultiSelectTexts } from 'angular-2-dropdown-multiselect';
import { Observable, from, Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { IMyDateModel } from 'mydatepicker';
import { AppointmentService } from '../Appointment/AppointmentService';
import { SaleService } from '../Items/POS/SaleService'
declare var $: any;
import { GlobalVariable } from '../../../../AngularConfig/global';
import swal from 'sweetalert';
import { data } from 'jquery';
@Component({
    templateUrl: './PrescriptionComponentForm.html',
    moduleId: module.id,
    providers: [PrescriptionService, AppointmentService, SaleService],
})
export class PrescriptionComponentForm implements OnInit, OnDestroy {
    public Form5: FormGroup;
    public Form6: FormGroup; public Form7: FormGroup;
    public Form2: FormGroup;
    public Form1: FormGroup;
    public Form8: FormGroup;
    public submitted: boolean;
    public IsBackDatedAppointment: boolean;
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public ScreenLists = [];
    public answer: any;
    public Modules = []; public CompanyObj: any;
    public filterdData = []; public FavoriteModel = new emr_notes_favorite();
    public MedicineModel = new emr_medicine();
    public vitalModel = new emr_vital();
    public UnitList: any[] = [];
    public MadicineTypeList: any[] = [];
    public ReminderList: any[] = [];
    public ClinicList: any[] = [];
    public TemplateList: any[] = [];
    public MedicineList: any[] = [];
    public DoseList: any[] = [];
    public IsSameModuleName: string;
    public IsChecked: boolean;
    public IsEmpExist: boolean = false;
    public IsAdmin: boolean = false;
    public IsUpdateText: boolean = false;
    public PayrollRegion: string;
    public Keywords: any[] = [];
    public sub: any; public Step: number = 30;
    public IsEdit: boolean = false; public currentTime: Date = new Date();
    public CompanyInfo: any[] = [];
    public GenderList: any[] = []; public CurntPrevDateList: any[] = [];
    public model = new emr_patient();
    public AppointmentModel = new emr_Appointment();
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
    public DoctorList: any[] = [];
    public Followuplist: any[] = [];
    public BloodList: any[] = [];
    public Rights: any; public StatusList: any[] = [];
    public PatientVitalList: any[] = [];
    public ControlRights: any;
    public ComplaintList: any[] = []; public AppointmentList: any[] = [];
    public ObservationList: any[] = [];
    public InvestigationsList: any[] = [];
    public AssignedDoctor: any[] = [];
    public DiagnosisList: any[] = [];
    public RoleName: string; public DisabledSlot: any;
    public PatientRXmodel = new patientInfo();
    public InstructionsList: any[] = [];
    public DurationList: any[] = [];
    public RefillList: any[] = [];
    public emr_vital_dynamicArray = [];
    public RowNo: number;
    public DoctorInfo = new DoctorInfo(); public BillTypeList: any[] = []; public AppControlRights: any;
    public TittleList: any[] = []; public PrescriptionExist: boolean = false;
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
    @ViewChild("content") PatientContent: TemplateRef<any>;
    @ViewChild("MedicinePresModal") MedicinePresModal: TemplateRef<any>;
    @ViewChild('editorClinicalNote') editorClinicalNote!: ElementRef;
    @ViewChild('editorPlanNote') editorPlanNote!: ElementRef;
    public ServiceType: any; public TokenNo: any; public patientInfo: any;
    @ViewChild("MedicineTemplate") MedicineTemplateContent: TemplateRef<any>; @ViewChild("longContent") ModelContent: TemplateRef<any>;
    @ViewChild("PrintRx") PrintRxContent: TemplateRef<any>;
    @ViewChild("DeepAIModal") DeepAIModal: TemplateRef<any>;
    activeVoiceField: string = '';
    isListening = false;
    voiceError = '';
    private subscriptions: Subscription[] = [];
    private clinicalNoteEditor: any;
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
                this.AppointmentModel.CNIC = this.patientInfo.CNIC;
                this.AppointmentModel.Mobile = this.patientInfo.Mobile;
                this.AppointmentModel.DoctorId = this.PrescriptionModel.DoctorId;
                this.AppointmentModel.ServiceId = this.patientInfo.ServiceID;
                this.AppointmentModel.ServiceName = this.patientInfo.ServiceName;
                this.AppointmentModel.Price = this.patientInfo.ServicePrice;
                this.AppointmentModel.PaidAmount = this.patientInfo.ServicePrice;
                this.AppointmentModel.AppointmentDate = new Date();
                this.AppointmentModel.BillDate = new Date();
            }
        }, 1000);
    }
    openNewPatient(content) {
        this.loader.ShowLoader();
        this._AppointmentService.GetEMRNO().then(m => {
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
    constructor(public _CommonService: CommonService, public _fb: FormBuilder, public loader: LoaderService
        , private voiceService: VoiceRecognitionService, private sanitizer: DomSanitizer, public _PrescriptionService: PrescriptionService, public commonservice: CommonService, private modalService: NgbModal
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public route: ActivatedRoute, public _router: Router, public _AppointmentService: AppointmentService, public _SaleService: SaleService) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("21");
        this.AppControlRights = this._CommonService.ScreenRights("13");
        this.RoleName = this.encrypt.decryptionAES(localStorage.getItem('RoleName'));
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
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
        this.RefillList = [
            { "ID": "-", "Name": "-" },
            { "ID": "1", "Name": "1" },
            { "ID": "2", "Name": "2" },
            { "ID": "3", "Name": "3" },
            { "ID": "4", "Name": "4" },
            { "ID": "5", "Name": "5" },
        ];
        this.Form1 = this._fb.group({
            PatientName: ['', [Validators.required]],
            Gender: ['', [Validators.required]],
            DOB: [''],
            Age: ['', [Validators.required]],
            Email: [''],
            Mobile: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.NumberPattern)]],
            CNIC: ['', [Validators.required, Validators.pattern(ValidationVariables.CNICPattern)]],
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
            CNIC: ['', [Validators.required, Validators.pattern(ValidationVariables.CNICPattern)]],
            Mobile: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.NumberPattern)]],
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
        this.Form5 = this._fb.group({
            Medicine: ['', [Validators.required]],
            UnitId: [''],
            TypeId: [''],
            Measure: [''],
            InstructionId: ['']
        });
        this.Form6 = this._fb.group({
            IsTemplate: [''],
            AppointmentDate: ['', [Validators.required]],
            PatientId: [''],
            ClinicId: ['', [Validators.required]],
            PatientName: [''],
            DoctorId: ['', [Validators.required]],
            FollowUpDate: [''],
            FollowUpTime: [''],
            IsCreateAppointment: [''],
            TemplateName: [''],
            Notes: [''],
            AppointDate: [''],
            SelectedComplaint: [''],
            SelectedInvestigation: [''],
            SelectedObservation: [''],
            SelectedDiagnos: [''],
            Day: [''],
            FollowUpNotes: [''],
            ClinicalNote: [''],
            PlanNote: [''],
            IsVCObtained: [''],
            RefillNo: [''],
        });
        this.Form7 = this._fb.group({
            PatientName: ['', [Validators.required]],
            Gender: ['', [Validators.required]],
            DOB: [''],
            Age: [''],
            Email: [''],
            Mobile: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.NumberPattern)]],
            CNIC: ['', [Validators.required]],
            Image: [''],
            Notes: [''],
            MRNO: ['', [Validators.required]],
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
        });
        this.Form8 = this._fb.group({
            Name: [''],
            Name1: [''],
            Name2: [''],
            Name3: [''],
            Name4: [''],
            Name5: [''],
            Name6: [''],
            Name7: [''],

        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.PrescriptionModel.ID = params.id;
                if (this.PrescriptionModel.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._PrescriptionService.GetById(this.PrescriptionModel.ID).then(m => {
                        if (m.ResultSet != null) {
                            this.CurntPrevDateList = [];
                            this.ComplaintList = [];
                            this.ObservationList = [];
                            this.InvestigationsList = [];
                            this.DiagnosisList = [];
                            this.emr_vital_dynamicArray = [];
                            this.emr_prescription_complaint_dynamicArray = [];
                            this.emr_prescription_diagnos_dynamicArray = [];
                            this.emr_prescription_investigation_dynamicArray = [];
                            this.emr_prescription_observation_dynamicArray = [];
                            this.PrescriptionModel.emr_prescription_complaint = [];
                            this.PrescriptionModel.emr_prescription_diagnos = [];
                            this.PrescriptionModel.emr_prescription_investigation = [];
                            this.PrescriptionModel.emr_prescription_observation = [];
                            this.emr_prescription_treatment_dynamicArray = [];
                            this.PrescriptionModel = m.ResultSet.result;
                            if (this.PrescriptionModel.FollowUpTime != null && this.PrescriptionModel.FollowUpTime != undefined)
                                this.PrescriptionModel.FollowUpTime = this.convertTo12HourFormat(this.PrescriptionModel.FollowUpTime);

                            this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(m.ResultSet.result.AppointmentDate);
                            this.CurntPrevDateList = m.ResultSet.CurntPrevDateList;
                            this.PrescriptionModel.PatientName = m.ResultSet.PatientInfo.PatientName;
                            m.ResultSet.vitalList.forEach((item, index) => {
                                this.emr_vital_dynamicArray.push(item);
                            });
                            this.MedicineList = m.ResultSet.MedicineList;
                            if (m.ResultSet.result.emr_prescription_treatment_template.length > 0)
                                this.PrescriptionModel.TemplateName = m.ResultSet.result.emr_prescription_treatment_template[0].TemplateName;
                            this.ComplaintList = m.ResultSet.ComplaintList;
                            this.DiagnosisList = m.ResultSet.DiagnosisList;
                            this.InvestigationsList = m.ResultSet.InvestigationsList;
                            this.ObservationList = m.ResultSet.ObservationList;
                            this.GenderList = m.ResultSet.GenderList;
                            this.DoctorList = m.ResultSet.DoctorList;
                            this.AssignedDoctor = m.ResultSet.AssignedDoctor;
                            this.ClinicList = m.ResultSet.ClinicList;
                            this.StatusList = m.ResultSet.StatusList;
                            this.MedicineList = m.ResultSet.MedicineList;
                            this.BloodList = m.ResultSet.BloodList;
                            this.UnitList = m.ResultSet.UnitList;
                            this.MadicineTypeList = m.ResultSet.MadicineTypeList;
                            if (this.PrescriptionModel.emr_prescription_complaint != null) {
                                this.PrescriptionModel.emr_prescription_complaint.forEach((item, index) => {
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
                                    var list1 = this.PrescriptionModel.emr_prescription_complaint.filter(a => a.ComplaintId == item.id);
                                    if (list1.length == 0 || list1 == undefined) {
                                        item.IsSelected = false;
                                        item.ComplaintId = item.id;
                                        this.emr_prescription_complaint_dynamicArray.push(item);
                                    }
                                });
                            } else {
                                m.ResultSet.ComplaintList.forEach((item, index) => {
                                    item.IsSelected = false;
                                    item.ComplaintId = item.id;
                                    this.emr_prescription_complaint_dynamicArray.push(item);
                                });
                            }
                            if (this.PrescriptionModel.emr_prescription_diagnos != null) {
                                this.PrescriptionModel.emr_prescription_diagnos.forEach((item, index) => {
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
                                    var list1 = this.PrescriptionModel.emr_prescription_diagnos.filter(a => a.DiagnosId == item.id);
                                    if (list1.length == 0 || list1 == undefined) {
                                        item.IsSelected = false;
                                        item.DiagnosId = item.id;
                                        this.emr_prescription_diagnos_dynamicArray.push(item);
                                    }
                                });
                            } else {
                                m.ResultSet.DiagnosisList.forEach((item, index) => {
                                    item.IsSelected = false;
                                    item.DiagnosId = item.id;
                                    this.emr_prescription_diagnos_dynamicArray.push(item);

                                });
                            }
                            if (this.PrescriptionModel.emr_prescription_investigation != null) {
                                this.PrescriptionModel.emr_prescription_investigation.forEach((item, index) => {
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
                                    var list1 = this.PrescriptionModel.emr_prescription_investigation.filter(a => a.InvestigationId == item.id);
                                    if (list1.length == 0 || list1 == undefined) {
                                        item.IsSelected = false;
                                        item.InvestigationId = item.id;
                                        this.emr_prescription_investigation_dynamicArray.push(item);
                                    }
                                });
                            } else {
                                m.ResultSet.InvestigationsList.forEach((item, index) => {
                                    item.IsSelected = false;
                                    item.InvestigationId = item.id;
                                    this.emr_prescription_investigation_dynamicArray.push(item);

                                });
                            }
                            if (this.PrescriptionModel.emr_prescription_investigation != null) {
                                this.PrescriptionModel.emr_prescription_observation.forEach((item, index) => {
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
                                    var list1 = this.PrescriptionModel.emr_prescription_observation.filter(a => a.ObservationId == item.id);
                                    if (list1.length == 0 || list1 == undefined) {
                                        item.IsSelected = false;
                                        item.ObservationId = item.id;
                                        this.emr_prescription_observation_dynamicArray.push(item);
                                    }
                                });
                            } else {
                                m.ResultSet.ObservationList.forEach((item, index) => {
                                    item.IsSelected = false;
                                    item.ObservationId = item.id;
                                    this.emr_prescription_observation_dynamicArray.push(item);
                                });
                            }
                            if (this.PrescriptionModel.emr_prescription_treatment != null) {
                                this.PrescriptionModel.emr_prescription_treatment.forEach((item, index) => {
                                    debugger
                                    var medicineName = this.MedicineList.filter(a => a.Id == item.MedicineId && a.Medicine == item.MedicineName)[0];
                                    if (medicineName != null && medicineName != undefined) {
                                        item.MedicineName = medicineName.Medicine;
                                        item.MedicineType = medicineName.Type;
                                        item.TypeId = medicineName.TypeId;
                                    }
                                    else
                                        item.MedicineName = item.MedicineName;
                                    this.emr_prescription_treatment_dynamicArray.push(item);
                                });
                            }
                            this.LoadEditor();
                            this.PlanEditor();
                            this.loader.HideLoader();
                        }
                        else
                            this.loader.HideLoader();
                    });
                } else {
                    this.LoadEditor();
                    this.PlanEditor();
                    this.Refresh();
                    this.Addtreatment();
                }
            });
        // voice-prescription
        this.subscriptions.push(
            this.voiceService.result$.subscribe(text => {
                this.appendVoiceText(text);
            })
        );

        this.subscriptions.push(
            this.voiceService.listening$.subscribe(value => {
                this.isListening = value;
            })
        );

        this.subscriptions.push(
            this.voiceService.error$.subscribe(error => {
                this.voiceError = error;
            })
        );
    }
    // voice-prescription
    startVoice(fieldName: string): void {
        this.voiceError = '';

        if (!this.voiceService.isSupported()) {
            this.voiceError =
                'Speech recognition is not supported in this browser.';
            return;
        }

        this.activeVoiceField = fieldName;
        this.voiceService.setLanguage('en-US');
        this.voiceService.start();
    }
    stopVoice(): void {
        this.voiceService.stop();
    }
    private appendVoiceText(text: string): void {
        debugger;
        if (!this.activeVoiceField || !text) {
            return;
        }
        const spokenText = text.trim();
        if (this.activeVoiceField === 'ClinicalNote') {
            const editor = this.clinicalNoteEditor;
            if (editor) {
                editor.focus();
                editor.insertText(spokenText + ' ');
            } else {
                const current = this.PrescriptionModel[this.activeVoiceField] || '';

                this.PrescriptionModel[this.activeVoiceField] = current
                    ? `${current} ${spokenText}`
                    : spokenText;
            }
        }
    }
    private escapeHtml(value: string): string {
        return value
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
    ngOnDestroy(): void {
        // voice-prescription
        this.voiceService.stop();
        this.subscriptions.forEach(subscription => {
            subscription.unsubscribe();
        });
        const CKEDITOR = (window as any).CKEDITOR;
        if (!CKEDITOR || !CKEDITOR.instances) {
            return;
        }
        const clinicalEditor = CKEDITOR.instances['editorClinicalNote'];

        if (clinicalEditor) {
            try {
                clinicalEditor.destroy(true);
            } catch (e) {
                console.warn('Clinical editor destroy:', e);
            }
        }

        const planEditor = CKEDITOR.instances['editorPlanNote'];
        if (planEditor) {
            try {
                planEditor.destroy(true);
            } catch (e) {
                console.warn('Plan editor destroy:', e);
            }
        }
    }
    OnChangeDoctor() {
        this.loader.ShowLoader();
        if (this.PrescriptionModel.PatientName == null || this.PrescriptionModel.PatientName == "")
            this.PrescriptionModel.PatientId = null;
        if (this.PrescriptionModel.PatientId != null && this.PrescriptionModel.DoctorId != null) {
            var selectdate = this._CommonService.GetFormatDate(new Date());
            this._PrescriptionService.PrescriptionLoad(this.PrescriptionModel.PatientId.toString(), this.PrescriptionModel.DoctorId.toString(), selectdate).then(m => {
                this.CurntPrevDateList = m.ResultSet.CurntPrevDateList;
                if (m.ResultSet.CurntPrevDateList.length == 0) {
                    this.CurntPrevDateList.push({ "AppointmentDate": this._CommonService.GetFormatDate(new Date()) });
                    this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(new Date());
                } else
                    this.PrescriptionModel.AppointmentDate = m.ResultSet.CurntPrevDateList[0].AppointmentDate;


                this.PrescriptionModel.Notes = m.ResultSet.Instruction;
                this.emr_vital_dynamicArray = [];
                m.ResultSet.vitalList.forEach((item, index) => {
                    this.emr_vital_dynamicArray.push(item);
                });
                this.loader.HideLoader();
            });
        } else {
            this.loader.HideLoader();
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
    CheckPrescription() {
        if (this.PrescriptionModel.PatientName == null || this.PrescriptionModel.PatientName == "")
            this.PrescriptionModel.PatientId = null;
        if (this.PrescriptionModel.PatientId != null && this.PrescriptionModel.DoctorId != null) {
            this.PrescriptionExist = false;
            this.loader.ShowLoader();
            var selectdate = this._CommonService.GetFormatDate(this.PrescriptionModel.AppointmentDate);
            this._PrescriptionService.PrescriptionLoad(this.PrescriptionModel.PatientId.toString(), this.PrescriptionModel.DoctorId.toString(), selectdate).then(m => {
                var appointfind = m.ResultSet.AppointmentList.filter(a => a.PatientId == this.PrescriptionModel.PatientId && this._CommonService.GetFormatDate(a.StartDate) == selectdate);
                if (appointfind.length > 0) {
                    this.PrescriptionExist = false;
                    this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(appointfind[0].StartDate);
                } else {
                    this.PrescriptionModel.AppointmentDate = selectdate;
                    this.PrescriptionExist = true;
                }
                this.IsBackDatedAppointment = m.ResultSet.IsBackDatedAppointment;
                this.patientInfo = m.ResultSet.patientInfo;
                this.MedicineList = m.ResultSet.MedicineList;
                this.CurntPrevDateList = [];
                if (m.ResultSet.CurntPrevDateList.length == 0) {
                    this.CurntPrevDateList.push({ "AppointmentDate": this._CommonService.GetFormatDate(new Date()) });
                } else
                    this.CurntPrevDateList = m.ResultSet.CurntPrevDateList;
                this.emr_prescription_complaint_dynamicArray = [];
                this.emr_prescription_diagnos_dynamicArray = [];
                this.emr_prescription_investigation_dynamicArray = [];
                this.emr_prescription_observation_dynamicArray = [];
                this.emr_prescription_treatment_dynamicArray = [];
                this.ComplaintList = [];
                this.ObservationList = [];
                this.InvestigationsList = [];
                this.DiagnosisList = [];
                this.emr_vital_dynamicArray = [];
                m.ResultSet.vitalList.forEach((item, index) => {
                    this.emr_vital_dynamicArray.push(item);
                });
                this.loadPrescriptionData(m.ResultSet);
                m.ResultSet.prescription_treatment.forEach((item, index) => {
                    var medicineName = this.MedicineList.filter(a => a.Id == item.MedicineId && a.Medicine == item.MedicineName)[0];
                    if (medicineName != null && medicineName != undefined) {
                        item.MedicineName = medicineName.Medicine;
                        item.MedicineType = medicineName.Type;
                        item.TypeId = medicineName.TypeId;
                    }
                    else
                        item.MedicineName = item.MedicineName;
                    this.emr_prescription_treatment_dynamicArray.push(item);
                });

                this.loader.HideLoader();
            });
        } else
            this.toastr.Error("please select patient and doctor.");
    }
    loadPrescriptionData(ResultSet: any) {
        this.ComplaintList = ResultSet.ComplaintList;
        this.DiagnosisList = ResultSet.DiagnosisList;
        this.InvestigationsList = ResultSet.InvestigationsList;
        this.ObservationList = ResultSet.ObservationList;

        if (ResultSet.prescription_complaint != null) {
            ResultSet.prescription_complaint.forEach((item, index) => {
                var list = ResultSet.ComplaintList.filter(a => a.id == item.ComplaintId);
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
            ResultSet.ComplaintList.forEach((item, index) => {
                var list1 = ResultSet.prescription_complaint.filter(a => a.ComplaintId == item.id);
                if (list1.length == 0 || list1 == undefined) {
                    item.IsSelected = false;
                    item.ComplaintId = item.id;
                    this.emr_prescription_complaint_dynamicArray.push(item);
                }
            });
        } else {
            ResultSet.ComplaintList.forEach((item, index) => {
                item.IsSelected = false;
                item.ComplaintId = item.id;
                this.emr_prescription_complaint_dynamicArray.push(item);

            });

        }
        if (ResultSet.prescription_diagnos != null) {
            ResultSet.prescription_diagnos.forEach((item, index) => {
                var list = ResultSet.DiagnosisList.filter(a => a.id == item.DiagnosId);
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
            ResultSet.DiagnosisList.forEach((item, index) => {
                var list1 = ResultSet.prescription_diagnos.filter(a => a.DiagnosId == item.id);
                if (list1.length == 0 || list1 == undefined) {
                    item.IsSelected = false;
                    item.DiagnosId = item.id;
                    this.emr_prescription_diagnos_dynamicArray.push(item);
                }
            });
        } else {
            ResultSet.DiagnosisList.forEach((item, index) => {
                item.IsSelected = false;
                item.DiagnosId = item.id;
                this.emr_prescription_diagnos_dynamicArray.push(item);

            });
        }
        if (ResultSet.prescription_investigation != null) {
            ResultSet.prescription_investigation.forEach((item, index) => {
                var list = ResultSet.InvestigationsList.filter(a => a.id == item.InvestigationId);
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
            ResultSet.InvestigationsList.forEach((item, index) => {
                var list1 = ResultSet.prescription_investigation.filter(a => a.InvestigationId == item.id);
                if (list1.length == 0 || list1 == undefined) {
                    item.IsSelected = false;
                    item.InvestigationId = item.id;
                    this.emr_prescription_investigation_dynamicArray.push(item);
                }
            });
        } else {
            ResultSet.InvestigationsList.forEach((item, index) => {
                item.IsSelected = false;
                item.InvestigationId = item.id;
                this.emr_prescription_investigation_dynamicArray.push(item);

            });
        }
        if (ResultSet.prescription_observation != null) {
            ResultSet.prescription_observation.forEach((item, index) => {
                var list = ResultSet.ObservationList.filter(a => a.id == item.ObservationId);
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
            ResultSet.ObservationList.forEach((item, index) => {
                var list1 = ResultSet.prescription_observation.filter(a => a.ObservationId == item.id);
                if (list1.length == 0 || list1 == undefined) {
                    item.IsSelected = false;
                    item.ObservationId = item.id;
                    this.emr_prescription_observation_dynamicArray.push(item);
                }
            });
        } else {
            ResultSet.ObservationList.forEach((item, index) => {
                item.IsSelected = false;
                item.ObservationId = item.id;
                this.emr_prescription_observation_dynamicArray.push(item);

            });
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
    LoadEditor() {
        this.destroyEditors();
        setTimeout(() => {
            const CKEDITOR = (window as any).CKEDITOR;
            if (this.editorClinicalNote && this.editorClinicalNote.nativeElement) {
                // if (CKEDITOR.instances['editorClinicalNote']) {
                //     CKEDITOR.instances['editorClinicalNote'].destroy(true);
                // }
                const editor = CKEDITOR.replace('editorClinicalNote', {
                    enterMode: (window as any).CKEDITOR.ENTER_BR,
                    shiftEnterMode: (window as any).CKEDITOR.ENTER_P,
                });
                this.clinicalNoteEditor = editor;
                editor.on('change', () => {
                    const html = editor.getData();
                    this.PrescriptionModel.ClinicalNote = html;
                    this.PrescriptionModel.safeClinicalNote = this.sanitizer.bypassSecurityTrustHtml(html);
                });
            }
        }, 10);
    }
    private destroyEditors(): void {
        const CKEDITOR = (window as any).CKEDITOR;
        if (!CKEDITOR || !CKEDITOR.instances) {
            return;
        }
        const headerEditor = CKEDITOR.instances['editorClinicalNote'];
        if (headerEditor) {
            try {
                headerEditor.destroy(true);
            } catch (e) {
                console.warn('Header CKEditor destroy warning:', e);
            }
        }
    }
    private destroyPlanEditors(): void {
        const CKEDITOR = (window as any).CKEDITOR;
        if (!CKEDITOR || !CKEDITOR.instances) {
            return;
        }
        const headerEditor = CKEDITOR.instances['editorPlanNote'];
        if (headerEditor) {
            try {
                headerEditor.destroy(true);
            } catch (e) {
                console.warn('Header CKEditor destroy warning:', e);
            }
        }
    }
    PlanEditor() {
        this.destroyPlanEditors();
        setTimeout(() => {
            const CKEDITOR = (window as any).CKEDITOR;
            if (this.editorPlanNote && this.editorPlanNote.nativeElement) {
                // if (CKEDITOR.instances['editorPlanNote']) {
                //     CKEDITOR.instances['editorPlanNote'].destroy(true);
                // }
                const editor = CKEDITOR.replace('editorPlanNote', {
                    enterMode: (window as any).CKEDITOR.ENTER_BR,
                    shiftEnterMode: (window as any).CKEDITOR.ENTER_P,
                });
                editor.on('change', () => {
                    const html = editor.getData();
                    this.PrescriptionModel.PlanNote = html;
                    this.PrescriptionModel.safePlanNote = this.sanitizer.bypassSecurityTrustHtml(html);
                });
            }
        }, 10);
    }
    Refresh() {
        this.loader.ShowLoader();
        this._PrescriptionService.LoadDropdown().then(m => {
            this.emr_prescription_complaint_dynamicArray = [];
            this.emr_prescription_diagnos_dynamicArray = [];
            this.emr_prescription_investigation_dynamicArray = [];
            this.emr_prescription_observation_dynamicArray = [];
            this.GenderList = m.ResultSet.GenderList;
            this.DoctorList = m.ResultSet.DoctorList;
            if (this.DoctorList.length == 1) {
                this.PrescriptionModel.DoctorId = this.DoctorList[0].ID;
                this.PrescriptionModel.Notes = this.DoctorList[0].Instruction;
            }
            this.AssignedDoctor = m.ResultSet.AssignedDoctor;
            this.ClinicList = m.ResultSet.ClinicList;
            if (this.ClinicList.length == 1)
                this.PrescriptionModel.ClinicId = this.ClinicList[0].CompanyID;
            this.StatusList = m.ResultSet.StatusList;
            this.BloodList = m.ResultSet.BloodList;
            this.UnitList = m.ResultSet.UnitList;
            this.MadicineTypeList = m.ResultSet.MadicineTypeList;
            this.ServiceType = m.ResultSet.ServiceType;
            this.TokenNo = m.ResultSet.TokenNo;
            this.Followuplist = m.ResultSet.Followuplist;
            this.ReminderList = m.ResultSet.ReminderList;
            this.CurntPrevDateList.push({ "AppointmentDate": this._CommonService.GetFormatDate(new Date()) });
            if (m.ResultSet.ServiceType != null) {
                this.AppointmentModel.ServiceId = m.ResultSet.ServiceType.ID;
                this.AppointmentModel.ServiceName = m.ResultSet.ServiceType.ServiceName;
                this.AppointmentModel.Price = m.ResultSet.ServiceType.Price;
            }
            this.loadPrescriptionData(m.ResultSet);
            this.loader.HideLoader();
        });

    }
    Convert12TO24(time12h: any) {

        const [time, modifier] = time12h.split(' ');
        let [hours, minutes] = time.split(':');
        //if (hours === '12') {
        //    hours = '00';
        //}
        if (modifier.toLowerCase() === 'pm') {
            hours = parseInt(hours, 10) + 12;
        }
        return hours + ':' + minutes + ':00';
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
                this.toastr.Success(result.Message);
                this.modalService.dismissAll(this.ModelContent);
                this.CurntPrevDateList = result.ResultSet.CurntPrevDateList;

                this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(this.AppointmentModel.AppointmentDate);
                this.PrescriptionModel.AppointDate = this._CommonService.GetFormatDate(this.AppointmentModel.AppointmentDate);
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
    convertTo12HourFormat(time: string): string {
        let [hours, minutes] = time.split(':').map(Number);
        const ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12 || 12; // convert 0 → 12
        return `${hours}:${minutes.toString().padStart(2, '0')} ${ampm}`;
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
                this.toastr.Success(result.Message);
                this.modalService.dismissAll(this.ModelContent);
                this.CurntPrevDateList = result.ResultSet.CurntPrevDateList;

                this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(this.AppointmentModel.AppointmentDate);
                this.PrescriptionModel.AppointDate = this._CommonService.GetFormatDate(this.AppointmentModel.AppointmentDate);
                this.AppointmentModel = new emr_Appointment();
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', result.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }
    LoadAppPatient() {
        //Search By Name
        $('#AppPatientSearchByName').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._PrescriptionService.searchByName(request.term).then(m => {
                    this.patientInfo = m.ResultSet.PatientInfo;
                    response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.AppointmentModel.PatientName = ui.item.label;
                this.AppointmentModel.PatientId = ui.item.value;
                this.AppointmentModel.CNIC = this.patientInfo.filter(a => a.value == ui.item.value)[0].CNIC;
                this.AppointmentModel.Mobile = this.patientInfo.filter(a => a.value == ui.item.value)[0].Phone;
            }
        });
    }
    LoadPatient() {
        $('#PatientSearchByName').autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._PrescriptionService.searchByName(request.term).then(m => {
                    response(m.ResultSet.PatientInfo);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.patientInfo = ui.item;
                this.PrescriptionModel.PatientName = ui.item.label;
                this.PrescriptionModel.PatientId = ui.item.value;
                this.PrescriptionModel.Age = ui.item.Age;
                if (this.PrescriptionModel.PatientId != null && this.PrescriptionModel.DoctorId != null) {
                    this._PrescriptionService.PrePrescriptionDataLoad(this.PrescriptionModel.PatientId.toString(), this.PrescriptionModel.DoctorId.toString()).then(m => {
                        this.CurntPrevDateList = m.ResultSet;
                        this.PrescriptionModel.AppointmentDate = m.ResultSet[0].AppointmentDate;
                        this.PrescriptionExist = true;
                        if (m.ResultSet.length == 0) {
                            this.CurntPrevDateList.push({ "AppointmentDate": this._CommonService.GetFormatDate(new Date()) });
                            this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(new Date());
                            this.PrescriptionExist = false;
                        }
                        this.loader.HideLoader();
                    });
                } else {
                    this.loader.HideLoader();
                }
            }
        });
    }
    onPatientNameEvent(event: any, id: any) {
        const control = this.Form2.get('PatientName');
        if (control.invalid || id == undefined) {
            this.toastr.Error("Please first select patient");
            return;
        }
        if (event != null && event != undefined) {


        }
    }
    onAppointmentDateChanged(event: IMyDateModel) {
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
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._PrescriptionService.Delete(this.PrescriptionModel.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else {
                    this.toastr.Success(m.Message);
                    this._router.navigate(['/home/Prescription']);
                }
                this.loader.HideLoader();
            });
        }
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
                        this.emr_prescription_treatment_dynamicArray[indx].IsInventory = 0;
                    }
                    const items = m.ResultSet.ItemInfo.map(item => ({
                        Name: item.label,
                        label: item.label + (item.GenericName && item.GenericName !== "" ? " (" + item.GenericName + ")" : "") + " (qty:" + item.stock + ")",
                        value: item.value,
                        Instructions: item.Instructions,
                        InstructionId: item.InstructionId,
                        IsAllowNegativeStock: item.IsAllowNegativeStock,
                        stock: item.stock,
                        IsInventory: item.IsInventory,
                    }));
                    response(items);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.emr_prescription_treatment_dynamicArray[indx].MedicineId = ui.item.value;
                this.emr_prescription_treatment_dynamicArray[indx].MedicineName = ui.item.Name;
                this.emr_prescription_treatment_dynamicArray[indx].Measure = ui.item.Measure;
                this.emr_prescription_treatment_dynamicArray[indx].IsInventory = ui.item.IsInventory;
                this.emr_prescription_treatment_dynamicArray[indx].Instructions = ui.item.Instructions;
                this.emr_prescription_treatment_dynamicArray[indx].InstructionId = ui.item.InstructionId;
                $(id).val(ui.item.label);
                return ui.item.label;
            }
        });
    }
    Close() {
        this.PrescriptionModel = new emr_prescription_mf();
        this.submitted = false;
        this.IsEmpExist = false;
        this.IsAdmin = false;
        this.IsUpdateText = false;
    }
    DoctorChange() {
        this.AppointmentModel.AppointmentTime = null;
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
            this.PrescriptionModel.AppointDate = this._CommonService.GetFormatDate(dob);
            this.PrescriptionModel.AppointmentDate = this._CommonService.GetFormatDate(dob);
        }
    }
    SelectTime() {
        if (this.AppointmentModel.DoctorId != null && this.AppointmentModel.DoctorId != undefined) {
            var doctorid = this.AssignedDoctor.filter(a => a.ID == this.AppointmentModel.DoctorId);
            var Appobj = this.AppointmentList.filter(a => a.DoctorId == this.AppointmentModel.DoctorId);
            if (doctorid.length > 0) {
                let minTime = this.formatAMPM(doctorid[0].StartTime);
                let maxTime = this.formatAMPM(doctorid[0].EndTime);
                if (doctorid[0].SlotTime == null || doctorid[0].SlotTime == undefined) {
                    this.toastr.Error("Please set the doctor's checkup duration first.")
                    return;
                }
                if (doctorid[0].SlotTime != null) {
                    this.Step = parseInt(doctorid[0].SlotTime.split(':')[1]);
                }
                if (Appobj.length > 0) {
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
            if ($("#basicExample").val() != null && $("#basicExample").val() != "")
                this.AppointmentModel.AppointmentTime = $("#basicExample").val();
        }
        else {
            this.toastr.Error("please select doctor.")
        }
    }
    ChnageSelectTime() {
        if (this.AppointmentModel.DoctorId != null && this.AppointmentModel.DoctorId != undefined) {
            var doctorid = this.AssignedDoctor.filter(a => a.ID == this.AppointmentModel.DoctorId);
            var Appobj = this.AppointmentList.filter(a => a.DoctorId == this.AppointmentModel.DoctorId);
            if (doctorid.length > 0) {
                let minTime = this.formatAMPM(doctorid[0].StartTime);
                let maxTime = this.formatAMPM(doctorid[0].EndTime);
                if (doctorid[0].SlotTime == null || doctorid[0].SlotTime == undefined) {
                    this.toastr.Error("Please set the doctor's checkup duration first.")
                    return;
                }
                if (doctorid[0].SlotTime != null) {
                    this.Step = parseInt(doctorid[0].SlotTime.split(':')[1]);
                }
                if (Appobj.length > 0) {
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
            if ($("#basicExample").val() != null && $("#basicExample").val() != "")
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
        hours = hours ? hours : 12;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }
    FollowUpSelectTime() {
        if (this.PrescriptionModel.DoctorId != null && this.PrescriptionModel.DoctorId != undefined) {
            var doctorid = this.AssignedDoctor.filter(a => a.ID == this.PrescriptionModel.DoctorId);
            var Appobj = this.AppointmentList.filter(a => a.DoctorId == this.PrescriptionModel.DoctorId);
            if (doctorid[0].SlotTime == null || doctorid[0].SlotTime == undefined) {
                this.toastr.Error("Please set the doctor's checkup duration first.")
                return;
            }
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
            var doctorid = this.AssignedDoctor.filter(a => a.ID == this.PrescriptionModel.DoctorId);
            var Appobj = this.AppointmentList.filter(a => a.DoctorId == this.PrescriptionModel.DoctorId);
            if (doctorid[0].SlotTime == null || doctorid[0].SlotTime == undefined) {
                this.toastr.Error("Please set the doctor's checkup duration first.")
                return;
            }
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
    PatientSaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._AppointmentService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.AppointmentModel = result.ResultSet.Model;
                    this.PrescriptionModel.PatientId = result.ResultSet.Model.ID;
                    this.PrescriptionModel.PatientName = result.ResultSet.Model.PatientName;
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
    AgeChange() {
        this.model.DOB = null;
    } onDOBChanged(event: IMyDateModel) {
        if (event) {
            //var  ageCalc= new Date();
            var dob = new Date(this.model.DOB)
            var CurrentDate = new Date();
            var ageCalc = CurrentDate.getFullYear() - dob.getFullYear();
            this.model.Age = ageCalc;
            //this.getValidDOBDate(this.model.DOB);
        }
    }
    onDateChanged(event: IMyDateModel) {
        if (event) {

        }
    }
    LoadService() {
        $("#ServiceSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._AppointmentService.ServiceSearch(request.term).then(m => {
                    response(m.ResultSet.result);
                    this.loader.HideLoader();
                });
            },
            minLength: 3,
            select: (event, ui) => {
                this.AppointmentModel.ServiceId = ui.item.value;
                this.AppointmentModel.ServiceName = ui.item.label;
                this.AppointmentModel.Price = ui.item.Price;
                this.AppointmentModel.PaidAmount = ui.item.Price;
                this.CalAmount();
                return ui.item.label;
            }
        });
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
            this.PrescriptionModel.emr_prescription_treatment = this.emr_prescription_treatment_dynamicArray;
            this.PrescriptionModel.emr_prescription_complaint = this.emr_prescription_complaint_dynamicArray.filter(a => a.IsSelected == true);
            this.PrescriptionModel.emr_prescription_diagnos = this.emr_prescription_diagnos_dynamicArray.filter(a => a.IsSelected == true);
            this.PrescriptionModel.emr_prescription_investigation = this.emr_prescription_investigation_dynamicArray.filter(a => a.IsSelected == true);
            this.PrescriptionModel.emr_prescription_observation = this.emr_prescription_observation_dynamicArray.filter(a => a.IsSelected == true);
            if (this.PrescriptionModel.FollowUpTime != undefined) {
                this.PrescriptionModel.FollowUpTime = this.Convert12TO24(this.PrescriptionModel.FollowUpTime);
            }
            this.PrescriptionModel.emr_prescription_treatment = this.emr_prescription_treatment_dynamicArray;
            this._AppointmentService.PrescriptionSave(this.PrescriptionModel).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['/home/Prescription']);
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
                this.PrescriptionModel.TemplateName = m.ResultSet.result.emr_prescription_treatment_template[0].TemplateName;

                this.emr_prescription_complaint_dynamicArray = [];
                this.emr_prescription_diagnos_dynamicArray = [];
                this.emr_prescription_investigation_dynamicArray = [];
                this.emr_prescription_observation_dynamicArray = [];
                this.emr_prescription_treatment_dynamicArray = [];

                if (m.ResultSet.result.emr_prescription_complaint != null) {
                    m.ResultSet.result.emr_prescription_complaint.forEach((item, index) => {
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
                        this.emr_prescription_complaint_dynamicArray.push(item);
                    });
                    this.ComplaintList.forEach((item, index) => {
                        var list1 = m.ResultSet.result.emr_prescription_complaint.filter(a => a.ComplaintId == item.id);
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
                if (m.ResultSet.result.emr_prescription_diagnos != null) {
                    m.ResultSet.result.emr_prescription_diagnos.forEach((item, index) => {
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
                        this.emr_prescription_diagnos_dynamicArray.push(item);
                    });
                    this.DiagnosisList.forEach((item, index) => {
                        var list1 = m.ResultSet.result.emr_prescription_diagnos.filter(a => a.DiagnosId == item.id);
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
                if (m.ResultSet.result.emr_prescription_investigation != null) {
                    m.ResultSet.result.emr_prescription_investigation.forEach((item, index) => {
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
                        this.emr_prescription_investigation_dynamicArray.push(item);
                    });
                    this.InvestigationsList.forEach((item, index) => {
                        var list1 = m.ResultSet.result.emr_prescription_investigation.filter(a => a.InvestigationId == item.id);
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
                if (m.ResultSet.result.emr_prescription_observation != null) {
                    m.ResultSet.result.emr_prescription_observation.forEach((item, index) => {
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
                        this.emr_prescription_observation_dynamicArray.push(item);
                    });
                    this.ObservationList.forEach((item, index) => {
                        var list1 = m.ResultSet.result.emr_prescription_observation.filter(a => a.ObservationId == item.id);
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
                        var medicineName = this.MedicineList.filter(a => a.Id == item.MedicineId && a.Medicine == item.MedicineName)[0];
                        if (medicineName != null && medicineName != undefined) {
                            item.MedicineName = medicineName.Medicine;
                            item.MedicineType = medicineName.Type;
                            item.TypeId = medicineName.TypeId;
                        }
                        else
                            item.MedicineName = item.MedicineName;
                        this.emr_prescription_treatment_dynamicArray.push(item);
                    });
                }
            } else
                this.toastr.Error('Error', m.ErrorMessage);
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
                this.emr_prescription_complaint_dynamicArray = [];
                this.emr_prescription_diagnos_dynamicArray = [];
                this.emr_prescription_investigation_dynamicArray = [];
                this.emr_prescription_observation_dynamicArray = [];
                this.emr_prescription_treatment_dynamicArray = [];
                this.PatientVitalList = [];
                this.PrescriptionModel = new emr_prescription_mf();
                this.PrescriptionModel = result.ResultSet.Model;
                if (this.PrescriptionModel.FollowUpTime != null && this.PrescriptionModel.FollowUpTime != undefined)
                    this.PrescriptionModel.FollowUpTime = this.convertTo12HourFormat(this.PrescriptionModel.FollowUpTime);

                this.MedicineList = result.ResultSet.MedicineList;
                if (result.ResultSet.patientInfo != null) {
                    this.PatientRXmodel = result.ResultSet.patientInfo;
                    this.PrescriptionModel.PatientName = result.ResultSet.patientInfo.PatientName;
                    this.PatientRXmodel.Gender = result.ResultSet.patientInfo.Gender;
                    this.PatientRXmodel.Mobile = result.ResultSet.patientInfo.Mobile;
                    this.PatientRXmodel.IsVCObtained = result.ResultSet.Model.IsVCObtained;
                    this.PatientRXmodel.CurrentTime = result.ResultSet.Model.CreatedDate;
                    this.PatientRXmodel.RefillNo = result.ResultSet.Model.RefillNo;
                    this.PatientRXmodel.Address = result.ResultSet.patientInfo.Address;
                    this.PatientRXmodel.MrNo = result.ResultSet.patientInfo.MRNO;
                    this.PatientRXmodel.NoVisit = result.ResultSet.patientInfo.NoVisit;
                }
                this.PatientRXmodel.Notes = result.ResultSet.Model.Notes;
                if (result.ResultSet.Model.ClinicalNote) {
                    this.PatientRXmodel.safeClinicalNote = this.sanitizer.bypassSecurityTrustHtml(result.ResultSet.Model.ClinicalNote);
                }
                if (result.ResultSet.Model.PlanNote) {
                    this.PatientRXmodel.safePlanNote = this.sanitizer.bypassSecurityTrustHtml(result.ResultSet.Model.PlanNote);
                }
                this.PatientRXmodel.FollowUpDate = result.ResultSet.Model.FollowUpDate;
                if (result.ResultSet.doctor.length > 0)
                    this.DoctorInfo = result.ResultSet.doctor[0];
                if (this.DoctorInfo.HeaderDescription) {
                    this.DoctorInfo.safeHeaderDescription = this.sanitizer.bypassSecurityTrustHtml(this.DoctorInfo.HeaderDescription);
                }
                if (this.DoctorInfo.HeaderDescriptionUrdu) {
                    this.DoctorInfo.safeHeaderDescriptionUrdu = this.sanitizer.bypassSecurityTrustHtml(this.DoctorInfo.HeaderDescriptionUrdu);
                }
                if (this.DoctorInfo == null || this.DoctorInfo == undefined) {
                    this.DoctorInfo.Name = "";
                    this.DoctorInfo.Designation = "";
                    this.DoctorInfo.Qualification = "";
                    this.DoctorInfo.PhoneNo = "";
                } else {
                    this.PatientRXmodel.IsShowHeader = this.DoctorInfo.IsShowHeader;
                    this.PatientRXmodel.BrandDescription = this.DoctorInfo.BrandDescription;
                    if (this.DoctorInfo.BrandImage != null)
                        this.PatientRXmodel.BrandImage = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + this.DoctorInfo.BrandImage;

                }
                if (this.PatientRXmodel == undefined || this.PatientRXmodel == null) {
                    this.PatientRXmodel.PatientName = "";
                    this.PatientRXmodel.Age = "";
                }
                this.PatientRXmodel.AppointmentDate = this._CommonService.GetFormatDate(this.PrescriptionModel.AppointmentDate);
                this.PatientVitalList = result.ResultSet.vitallist;
                var Weight = result.ResultSet.vitallist.filter(a => a.Name == "Weight");
                if (Weight.length == 0)
                    this.PatientRXmodel.Weight = "";
                else
                    this.PatientRXmodel.Weight = Weight[0].Measure;
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
                }
                if (this.PrescriptionModel.emr_prescription_treatment != null) {
                    this.PrescriptionModel.emr_prescription_treatment.forEach((item, index) => {
                        var medicineName = this.MedicineList.filter(a => a.Id == item.MedicineId && a.Medicine == item.MedicineName)[0];
                        if (medicineName != null && medicineName != undefined) {
                            item.MedicineName = medicineName.Medicine;
                            item.GenericName = medicineName.GenericName;
                            item.MedicineType = medicineName.Type;
                            item.TypeId = medicineName.TypeId;
                        }
                        else
                            item.MedicineName = item.MedicineName;
                        this.emr_prescription_treatment_dynamicArray.push(item);
                    });
                }
                debugger
                this.modalService.open(this.PrintRxContent, { size: 'lg' });
            }
            else
                this.toastr.Error('Error', result.ErrorMessage);

            this.loader.HideLoader();
        });
    }
    getMedicineLabel(medicine: any): string {
        if (this.DoctorInfo.DrugMode === 'G') {
            return medicine.GenericName || medicine.MedicineName;
        }
        return medicine.MedicineName;
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
    getDurationInUrdu(value: string): string {
        switch (value) {
            case 'Day (s)':
                return 'دن';
            case 'Week (s)':
                return 'ہفتہ';
            case 'Month (s)':
                return 'مہینہ';
            case 'Daily':
                return 'روزانہ';
            case 'Weekly':
                return 'ہفتہ وار';
            case 'Monthly':
                return 'ماہانہ';
            case 'Continuously':
                return 'مسلسل';
            case 'When Required':
                return 'جب ضرورت ہو';
            case 'STAT':
                return 'فوری';
            case 'PRN':
                return 'حسبِ ضرورت';
            default:
                return value;
        }
    }
    OpenVitalModal(VitalModal) {
        this.vitalModel = new emr_vital();
        this.modalService.open(VitalModal, { size: 'md' });
    }
    VitalSave(isValid: boolean): void {
        var obj = this.vitalModel;
        var date = this.PrescriptionModel.AppointmentDate;
        if (date == null || date == undefined) {
            this.toastr.Error("Error", "Please select date.");
            return;
        }
        this.vitalModel.Date = new Date(date);
        var PatientId = this.PrescriptionModel.PatientId;
        if (PatientId == null || PatientId == undefined) {
            this.toastr.Error("Error", "Please select Patient.");
            return;
        }
        this.vitalModel.PatientId = PatientId;
        if ((obj.Name == null || obj.Name == undefined)
            && (obj.Name1 == null || obj.Name1 == undefined)
            && (obj.Name2 == null || obj.Name2 == undefined)
            && (obj.Name3 == null || obj.Name3 == undefined)
            && (obj.Name4 == null || obj.Name4 == undefined)
            && (obj.Name5 == null || obj.Name5 == undefined)
            && (obj.Name6 == null || obj.Name6 == undefined)
            && (obj.Name7 == null || obj.Name7 == undefined)
        ) {
            this.toastr.Error("Error", "Please enter at least one vital.");
            return;
        }
        //if (this.VitalModel.Measure2 != null && this.VitalModel.Measure2 != undefined)
        //    this.VitalModel.Measure = this.VitalModel.Measure + "/" + this.VitalModel.Measure2;
        this.loader.ShowLoader();
        this._AppointmentService.VitalPrescriptionSave(this.vitalModel).then(m => {
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
            this.modalService.dismissAll();
            this.vitalModel = new emr_vital();
            this.vitalModel.Date = new Date();
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
                const index = this.emr_vital_dynamicArray.findIndex(
                    x => x.ID === id && x.VitalId === VitalId
                );

                if (index !== -1) {
                    this.emr_vital_dynamicArray.splice(index, 1);
                }
                this.loader.HideLoader();
            });
        }
    }
    async loadAIResponse(isValid: boolean): Promise<void> {

        const vitalText = this.emr_vital_dynamicArray
            .map(v => `${v.Measure} ${v.Unit}`)
            .join(', ');
        const complaintText =
            this.emr_prescription_complaint_dynamicArray.filter(a => a.IsSelected)
                .map(x => x.Complaint)
                .join(', ');
        if (!this.PrescriptionModel.ClinicalNote || this.PrescriptionModel.ClinicalNote.trim() === '' ||
            !complaintText || complaintText.trim() === '' ||
            !vitalText || vitalText.trim() === '') {
            this.toastr.Error('Validation Error', 'Please enter Clinical History, Complaint and Vital signs first.');
            return;
        }
        this.loader.ShowLoader();
        let model = {
            ClinicalHistory: this.PrescriptionModel.ClinicalNote,
            Complaint: complaintText,
            Vital: vitalText
        };
        const response = await this._PrescriptionService.LoadAIResponse(model);
        const result = JSON.parse(response._body);
        if (result.IsSuccess) {
            this.answer = result.ResultSet.Response
                .replace(/\*\*/g, '')
                .replace(/##/g, '')
                .trim();
            this.modalService.open(this.DeepAIModal, { size: 'lg' });
            this.loader.HideLoader();

        } else {
            this.toastr.Error('Error', result.ErrorMessage);
            this.loader.HideLoader();
        }
    }
    async copyAIResponse() {
        try {
            await (navigator as any).clipboard.writeText(this.answer);

            this.toastr.Success('Success', 'Copied to clipboard');
        } catch (err) {
            console.error(err);
            this.toastr.Error('Error', 'Copy failed');
        }
    }
}
