import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { emr_patient } from './PatientModel';
import { LoaderService } from '../../../../CommonService/LoaderService';
import { PatientService } from './PatientService';
import { ValidationVariables } from '../../../../AngularConfig/global';
import { CommonService } from '../../../../CommonService/CommonService';
import { PhoneMaskDirective } from '../../../../CommonComponent/PhoneMaskDirective';
import { EncryptionService } from '../../../../CommonService/encryption.service';
import { CommonToastrService } from '../../../../CommonService/CommonToastrService';
import { IMyDateModel } from 'mydatepicker';
import { GlobalVariable } from '../../../../AngularConfig/global';
import { Observable } from 'rxjs';
//import SlimSelect from 'slim-select';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
declare var $: any;
@Component({
    selector: 'setup-PatientComponentForm',
    templateUrl: './PatientComponentForm.html',
    moduleId: module.id,
    providers: [PatientService],
})
export class PatientComponentForm implements OnInit {
    public Form1: FormGroup;
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
    public PayrollRegion: string;
    public GenderList: any[] = [];
    public StatusList: any[] = [];
    public DoctorList: any[] = [];
    public BloodList: any[] = [];
    public BillTypeList: any[] = [];
    public TittleList: any[] = [];
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public PatientImage: string = '';
    public IsNewImage: boolean = true;
    public CompanyInfo: any[] = [];
    public GenderIds: any;
    public IsCNICMandatory: any;
    public Rights: any; public CompanyObj: any;
    public ControlRights: any;
    constructor(public _CommonService: CommonService, public _fb: FormBuilder, public loader: LoaderService
        , public _PatientService: PatientService, public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService,  public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("11");
        let Users = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));

        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.IsCNICMandatory = this.CompanyObj.IsCNICMandatory;
        this.GenderIds = Users.IsGenderDropdown;
    }
    ngOnInit() {
        this.loadDropdown();
        this.Form1 = this._fb.group({
            PatientName: ['', [Validators.required]],
            Gender: ['', [Validators.required]],
            DOB: [''],
            Age: ['', [Validators.required]],
            Email: [''],
            ContactNo: [''],
            Mobile: ['', [Validators.required]],
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
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.id = params.id;
                if (this.id > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this.loader.ShowLoader();
                    this._PatientService.GetById(this.id).then(m => {
                        this.model = m.ResultSet;
                        if (this.model.Image != null || this.model.Image != undefined) {
                            this.getImageUrlName(this.model.Image);
                            this.IsNewImage = false;
                        } else this.IsNewImage = true;
                        this.loader.HideLoader();
                    });

                }
            });
    }
    loadDropdown() {
        this.loader.ShowLoader();
        this._PatientService.FormLoad().then(m => {
            if (m.IsSuccess) {
                if (this.GenderIds != null) {
                    this.GenderList = m.ResultSet.GenderList.filter(x => this.GenderIds.includes(x.ID));
                    if (this.IsEdit == false) {
                        this.model.MRNO = m.ResultSet.MRNO;
                        this.model.BillTypeId = 1;
                        this.model.PrefixTittleId = 1;
                    }
                    if (this.GenderList.length == 1)
                        this.model.Gender = this.GenderList[0].ID;
                    if (this.model.Gender == 2)
                        this.model.PrefixTittleId = 1;
                    if (this.model.Gender == 1)
                        this.model.PrefixTittleId = 2;
                    this.GenderChnage(this.model.Gender);
                }
                else
                    this.GenderList = m.ResultSet.GenderList;
                this.DoctorList = m.ResultSet.DoctorList.DoctorList;
                this.StatusList = m.ResultSet.AllStatusList;
                this.BloodList = m.ResultSet.BloodList;
                this.BillTypeList = m.ResultSet.BillTypeList;
                this.TittleList = m.ResultSet.TittleList;
              
                this.loader.HideLoader();
            } else
                this.loader.HideLoader();
        });
    }
    GenderChnage(evnt: any) {
        var genderid = parseInt(evnt);
        if (genderid == 2 && this.model.PrefixTittleId == 1)
            this.model.PrefixTittleId = 2
        if (genderid == 1 && this.model.PrefixTittleId == 2)
            this.model.PrefixTittleId = 1

    }
    SaveOrUpdate(isValid: boolean): void {
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
            this._PatientService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    this._router.navigate(['home/Patient']);
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
        //if (GlobalVariable.IsUseS3 == "Yes") {
        //    if (!this.IsNewImage)
        //        this.Image = this.model.CompanyLogo == FName ? this.model.CompanyLogo : "";
        //    else
        //        this.Image = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        //}
        //else {
        if (this.IsEdit && !this.IsNewImage) {
            this.PatientImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        } else {
            this.PatientImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
        //}
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._PatientService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                else
                    this._router.navigate(['home/Patient']);
                this.loader.HideLoader();
            });
        }
    }

    LoadDoctor() {
        $("#DoctorSearch").autocomplete({
            source: (request, response) => {
                this.loader.ShowLoader();
                this._PatientService.DoctorSearch(request.term).then(m => {
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
    Close() {
        this.model = new emr_patient();
        this.submitted = false;
        this.IsEmpExist = false;
        this.IsAdmin = false;
        this.IsUpdateText = false;
    }
}
