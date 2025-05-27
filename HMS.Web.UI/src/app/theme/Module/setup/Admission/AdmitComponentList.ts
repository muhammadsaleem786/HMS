import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AdmitService } from './AdmitService';
import { emr_patient, ipd_admission, ipd_admission_discharge } from './AdmitModel';
import { PaginationModel, PaginationConfig } from '../../../../CommonComponent/PaginationComponentConfig';
import { CommonService } from '../../../../CommonService/CommonService';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { forEach } from '@angular/router/src/utils/collection';
import { emr_Appointment, emr_document, emr_vital, patientInfo, DoctorInfo, InvoiceCompanyInfo } from './../Appointment/AppointmentModel';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BillingService } from './../Billing/BillingService';

declare var $: any;
@Component({
    moduleId: module.id,
    templateUrl: 'AdmitComponentList.html',
    providers: [AdmitService, BillingService],
})
export class AdmitComponentList {
    public Form2: FormGroup;
    public Form1: FormGroup;
    public ActiveToggle: boolean = false;
    public Id: string;
    public PModel = new PaginationModel();
    public PConfig = new PaginationConfig();
    public modelAdmission = new ipd_admission();
    public model = new ipd_admission_discharge();
    public PatientAdmitList: any[] = [];
    public IsList: boolean = true;
    public ID: number = 10;
    public Rights: any;
    public Keywords: any[] = [];
    previousPage: number = 1;
    nextPage: number = 1;
    totalPages: number = 0;
    pagesRange: number[] = [];
    public WardList: any[] = [];
    public RoomList: any[] = [];
    public AllBedList: any[] = [];
    public TypeList: any[] = [];
    public BedList: any[] = []; public submitted: boolean;
    public ControlRights: any;
    public IsBed: boolean = false;
    public IsWard: boolean = false;
    public IsRoom: boolean = false;
    selectedValue: string = "A";
    public CategoryList: any[] = [];
    public InvoiceCompanyInfo = new InvoiceCompanyInfo();
    public InvoiceBillModel: any[] = [];
    public SubTotal: number = 0;
    public Total: number = 0;
    public TotalDiscount: number = 0;
    public valueForUser: any;
    @ViewChild("AllotNo") AllotNo: TemplateRef<any>;
    @ViewChild("InvoiceModal") InvModal: TemplateRef<any>;
    openInvoiceModal(InvoiceModal: any, id: any, patientid: any) {
        this.loader.ShowLoader();
        this._BillingService.GetBillByAdmissionId(id, patientid).then(m => {
            if (m.IsSuccess) {
                this.InvoiceBillModel = m.ResultSet.result;
                if (m.ResultSet.result != null && m.ResultSet.result.length > 0) {
                    this.InvoiceCompanyInfo.CompanyName = m.ResultSet.result[0].CompanyName;
                    this.InvoiceCompanyInfo.CompanyAddress = m.ResultSet.result[0].CompanyAddress;
                    this.InvoiceCompanyInfo.CompanyPhone = m.ResultSet.result[0].CompanyPhone;
                    this.InvoiceCompanyInfo.CompanyEmail = m.ResultSet.result[0].CompanyEmail;

                    this.InvoiceCompanyInfo.PatientName = m.ResultSet.result[0].PatientName;
                    this.InvoiceCompanyInfo.PatientAddress = m.ResultSet.result[0].PatientAddress;
                    this.InvoiceCompanyInfo.PatientEmail = m.ResultSet.result[0].PatientEmail;
                    this.InvoiceCompanyInfo.PatientMobile = m.ResultSet.result[0].PatientMobile;
                    this.InvoiceCompanyInfo.BillDate = m.ResultSet.result[0].BillDate;
                    this.InvoiceCompanyInfo.invoiceNo = m.ResultSet.result[0].ID;
                    if (m.ResultSet.result[0].CompanyLogo != null)
                        this.InvoiceCompanyInfo.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + '' + m.ResultSet.result[0].CompanyLogo;

                    this.SubTotal = 0;
                    this.Total = 0;
                    this.TotalDiscount = 0;
                    m.ResultSet.result.forEach((item, index) => {
                        this.SubTotal += parseInt(item.Price);
                        this.TotalDiscount += parseInt(item.Discount);
                    });
                    this.Total = this.SubTotal - this.TotalDiscount;


                }
                this.modalService.open(InvoiceModal, { size: 'lg' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    constructor(private modalService: NgbModal, private encrypt: EncryptionService, public _fb: FormBuilder, public _BillingService: BillingService, public _CommonService: CommonService, public loader: LoaderService, public _AdmitService: AdmitService
        , public _router: Router, public toastr: CommonToastrService,) {
        this.loader.ShowLoader();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("11");
        this.valueForUser = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));

    }
    ngOnInit() {
        this.TypeList.push({ "id": 1, "Name": "Ward" }, { "id": 2, "Name": "Room" });
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
            CheckedBy: [''],
            RemovalDate: [''],
            ConditionWound: [''],
            AdviseMedicine: [''],
            FollowUpDate: [''],
            OtherRemarks: ['']
        });
        this.Form2 = this._fb.group({
            PatientName: [''],
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
            TypeId: [''],
            WardTypeId: [''],
            BedId: [''],
            RoomId: [''],
        });
        this.PModel.SortName = "";
        this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
        this.selectPage(this.PModel.CurrentPage);
    }
    Refresh() {
        this.loader.ShowLoader();
        let selectValue = localStorage.getItem('selectedValue');
        if (selectValue != "null" && selectValue != "") {
            this.selectedValue = selectValue;
            if (selectValue == "A") {
                this.PModel.FilterID = "0";
                this.PModel.VisibleFilter = false;
            } else {
                this.PModel.FilterID = this.selectedValue
                this.PModel.VisibleFilter = false;
            }
        }
        this.PModel.VisibleColumnInfo = "PatientName#PatientName,DOB#DOB,Email#Email,Mobile#Mobile,MRNO#MRNO,CNIC#CNIC";
        this._AdmitService
            .GetList(this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText, this.PModel.FilterID, this.PModel.VisibleFilter).then(m => {
                this.PModel.TotalRecord = m.TotalRecord;
                this.PatientAdmitList = m.DataList;
                this.CategoryList = m.OtherDataModel;
                this.CategoryList.push({ "ID": "A", "Value": "All" });
                if (this.PatientAdmitList != null) {
                    this.PatientAdmitList.forEach(x => {
                        if (x.Image != null && x.Image != undefined && x.Image != "") {
                            x.Image = GlobalVariable.BASE_Temp_File_URL + '' + x.Image;
                        } else
                            x.Image = null;
                    });
                }
                this.getPages(this.PModel.TotalRecord, this.PModel.RecordPerPage);
                this.loader.HideLoader();
            });
        $('[data-toggle="tooltip"]').tooltip();
    }
    selectPage(page: number) {
        if (page == 0 || (page != 1 && this.PModel.CurrentPage == page && this.pagesRange.length > 0)) return;
        this.PModel.CurrentPage = page;
        this.Refresh();
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
    getFilterData() {
        localStorage.setItem('selectedValue', this.selectedValue);
        this.Refresh();
    }
    getTotalPages(totalRecord: number, recordPerPage: number): number {

        return Math.ceil(Math.max(totalRecord, 1) / Math.max(recordPerPage, 1));
    }
    Deshboard(id: string) {
        if (id != "0") {
            this.Id = id;
            this.IsList = false;
            this._router.navigate(['home/Appoint/saveAppoint'], { queryParams: { id: this.Id } });
        }
    }
    ViewSummery(id: any, PatientId) {
        localStorage.setItem("PatientId", PatientId);
        localStorage.setItem("AdmitId", id);
        this._router.navigate(['home/AdmitSummary']);
    }
    AddRecord(id: string) {
        if (id != "0") {
            this.loader.ShowLoader();
            this._router.navigate(['home/Admit/saveAdmit']);
        }
        this.Id = id;
        this.IsList = false;
        this._router.navigate(['home/Admit/saveAdmit'], { queryParams: { id: this.Id } });
    }
    View(id: string) {
        this.loader.ShowLoader();
        this.Id = id;
        this.IsList = false;
    }
    Delete(id: string) {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._AdmitService.Delete(id).then(m => {

                if (m.ErrorMessage != null) {

                    alert(m.ErrorMessage);
                }
                this.Refresh();
            });
        }
    }
    GetList() {
        this.Refresh();
    }
    Close(idpar) {
        this.IsList = true;
        if (idpar == 0)
            this.Id = '0';
        else
            this.Refresh();
    }
    GoBack(DefaultRoute) {
        this._router.navigate([DefaultRoute]);
    }
    ExportData(ExportType: number) {
        this.loader.ShowLoader();
        this._AdmitService.ExportData(ExportType, this.PModel.CurrentPage, this.PModel.RecordPerPage, this.PModel.VisibleColumnInfo, this.PModel.SortName, this.PModel.SortOrder, this.PModel.SearchText).then(m => {
            this.loader.HideLoader();
        });
    }
    openAllotNoModal(AllotNo, Id: any) {

        this.BedList = [];
        this.IsBed = false;
        this.IsWard = false;
        this.IsRoom = false;
        this.loader.ShowLoader();
        this._AdmitService.WardDropDown(Id).then(m => {
            if (m.IsSuccess) {
                this.WardList = m.ResultSet.WardList;
                this.RoomList = m.ResultSet.RoomList;
                this.AllBedList = m.ResultSet.BedList;
                this.modelAdmission = m.ResultSet.obj;
                if (this.modelAdmission.TypeId == 1)
                    this.IsWard = true;
                if (this.modelAdmission.TypeId == 2)
                    this.IsRoom = true;
                if (this.modelAdmission.WardTypeId != null)
                    this.changeWard(this.modelAdmission.WardTypeId);
                this.modalService.open(AllotNo, { size: 'md' });
            }
            this.loader.HideLoader();
        });

    }
    changeType(e) {
        this.IsBed = false;
        this.IsWard = false;
        this.IsRoom = false;
        if (e == 1) {
            this.modelAdmission.RoomId = null;
            this.IsWard = true;
        }
        if (e == 2) {
            this.modelAdmission.WardTypeId = null;
            this.modelAdmission.BedId = null;
            this.IsRoom = true;
        }

    }
    changeWard(e) {
        this.IsBed = true;
        this.BedList = [];
        this.BedList = this.AllBedList.filter(a => a.DependedDropDownValueID == e);
    }
    selectBed(id: any) {
        this.modelAdmission.BedId = null;
        this.modelAdmission.BedId = id;
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this._AdmitService.SaveOrUpdate(this.modelAdmission).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this.modalService.dismissAll(this.AllotNo);
                    this.Refresh();
                    this.loader.HideLoader();
                }
                else {
                    this.toastr.Error('Error', result.ErrorMessage);
                    this.loader.HideLoader();
                }
            });
        }
    }

    DischargePatient(Id: any) {
        this.loader.ShowLoader();
        this._AdmitService.DischargePatient(Id).then(m => {
            if (m.IsSuccess) {
                this.toastr.Success(m.Message);
                this.Refresh();
                this.loader.HideLoader();
            }
            else {
                this.toastr.Error('Error', m.ErrorMessage);
                this.loader.HideLoader();
            }
        });
    }

    ViewReportModal(ReportModal: any, AdmissionId: any, PatientId: any) {
        this.loader.ShowLoader();
        this._AdmitService.GetDischargRpt(AdmissionId, PatientId).then(m => {
            if (m.IsSuccess) {
                localStorage.setItem("PatientID", PatientId);
                localStorage.setItem("AdmissionId", AdmissionId);
                this.model = new ipd_admission_discharge();
                if (m.ResultSet.Detaildata.length != 0) {
                    this.model = m.ResultSet.Detaildata[0];                   
                }
                if (m.ResultSet.Masterdata != null) {
                    this.InvoiceCompanyInfo.CompanyName = m.ResultSet.Masterdata[0].CompanyName;
                    this.InvoiceCompanyInfo.CompanyAddress = m.ResultSet.Masterdata[0].CompanyAddress1;
                    this.InvoiceCompanyInfo.CompanyPhone = m.ResultSet.Masterdata[0].Phone;
                    this.InvoiceCompanyInfo.CompanyEmail = m.ResultSet.Masterdata[0].Email;
                    this.InvoiceCompanyInfo.PatientName = m.ResultSet.Masterdata[0].PatientName;
                    this.InvoiceCompanyInfo.MRNo = m.ResultSet.Masterdata[0].MRNO;
                    this.InvoiceCompanyInfo.DoctorName = m.ResultSet.Masterdata[0].DrName;
                    this.InvoiceCompanyInfo.Father_Husband = m.ResultSet.Masterdata[0].Father_Husband;
                    this.InvoiceCompanyInfo.Room = m.ResultSet.Masterdata[0].Room;
                    this.InvoiceCompanyInfo.Ward = m.ResultSet.Masterdata[0].Ward;
                    this.InvoiceCompanyInfo.Department = m.ResultSet.Masterdata[0].Dept;
                    this.InvoiceCompanyInfo.Age = m.ResultSet.Masterdata[0].Age;
                    this.InvoiceCompanyInfo.DOB = m.ResultSet.Masterdata[0].DOB;
                    this.InvoiceCompanyInfo.Specialty = m.ResultSet.Masterdata[0].Specialty;
                    this.InvoiceCompanyInfo.DepartmentType = m.ResultSet.Masterdata[0].Value;
                    this.InvoiceCompanyInfo.AdmissionDate = m.ResultSet.Masterdata[0].AdmissionDate;
                    this.InvoiceCompanyInfo.Sex = m.ResultSet.Masterdata[0].Sex;

                    //this.model.Temperature = m.ResultSet.Masterdata[0].Temperature;
                    //this.model.Weight = m.ResultSet.Masterdata[0].Weight;
                    //this.model.BP = m.ResultSet.Masterdata[0].Value;
                    //this.model.RespiratoryRate = m.ResultSet.Masterdata[0].RespiratoryRate;
                    if (m.ResultSet.Masterdata[0].CompanyLogo != null)
                        this.InvoiceCompanyInfo.CompanyLogo = GlobalVariable.BASE_Temp_File_URL + '' + m.ResultSet.Masterdata[0].CompanyLogo;
                }
                this.modalService.open(ReportModal, { size: 'lg' });
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    SaveOrUpdateDischarge(isValid: boolean): void {
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.model.AdmissionId = parseInt(localStorage.getItem("AdmissionId"));
            this.model.PatientId = parseInt(localStorage.getItem("PatientID"));
            this._AdmitService.SaveOrUpdateDischargeRpt(this.model).then(m => {
                localStorage.removeItem("PatientID");
                localStorage.removeItem("AdmissionId");
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
    }
}