import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Company } from '../../../models/company.model';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { CompanyService } from '../Company/company.service';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { Observable } from 'rxjs';
//import SlimSelect from 'slim-select';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { FileService } from '../../../../../CommonService/FileService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
@Component({
    moduleId: module.id,
    templateUrl: './CompanyComponentForm.html',
    providers: [CompanyService],
})

export class CompanyComponentForm implements OnInit {
    public model = new Company();
    public Form1: FormGroup;
    public submitted: boolean;
    public Modules = [];
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public Rights: number[] = [];
    public PackageID: number;
    public PayrollRegion: string;
    public CompanyTypes = [];
    public LanguageTypes = [];
    public CurrencyList = [];
    public FiscalYearList = [];
    public BusinessLocationType = [];
    public DateFormatList = [];
    public CityTypeList = [];
    public TimeZoneList = [];
    public TotalEmployees = [];
    public Image: string = '';
    public IsNewImage: boolean = true;
    public isShowError: boolean = true;
    public IsPaymentShowHide: boolean = false;
    public CompanyObj: any;
    public CompanyToken: string; public ControlRights: any;
    constructor(public _fb: FormBuilder, public loader: LoaderService
        , public _CompanyService: CompanyService, public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        this.Modules = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Modules')))
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')))
        //this.Keywords = this.commonservice.GetKeywords("Setup");
        this.ControlRights = this.commonservice.ScreenRights("18");
        this.CompanyToken = localStorage.getItem('Token');
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));

    }

    ngOnInit() {
        this.GetCompany();
        this.Form1 = this._fb.group({
            CompanyName: ['', [Validators.required]],
            CompanyTypeID: [''],
            CompanyAddress1: [''],
            CompanyAddress2: [''],
            CompanyToken: [''],
            CityDropDownId: [''],
            Province: [''],
            PostalCode: [''],
            Phone: ['', [Validators.pattern(ValidationVariables.PhoneNo)]],
            Fax: [''],
            ID: [''],
            Website: [''],
            LanguageID: [''],
            GenderID: [''],
            IsCNICMandatory: [''],
            ContactPersonFirstName: [''],
            ContactPersonLastName: [''],
            IsBackDatedAppointment: [''],
            IsUpdateBillDate: [''],
            TokenNo: [''],
            Email: ['', [Validators.pattern(ValidationVariables.EmailPattern)]],
            CompanyLogo: [''],
            DateFormatId: [''],
            ReceiptFooter: [''],
            StandardShiftHours: [''],
        });
        this.loadDropdown();
        this.PayrollRegion = localStorage.getItem("PayrollRegion");
    }
    GetCompany() {

        this.loader.ShowLoader();
        var companyID = this.CompanyObj.CompanyID;
        this._CompanyService.GetById(parseInt(companyID)).then(m => {
            if (m.IsSuccess) {
                this.model = m.ResultSet;
                if (this.model.CompanyLogo != null && this.model.CompanyLogo != undefined && this.model.CompanyLogo != "") {
                    this.IsNewImage = false;
                    this.getImageUrlName(this.model.CompanyLogo);
                }
            }
            else {
                this.toastr.Error('Not Found', 'Company not found!');
                this._router.navigate(['/login']);
            }
            this.loader.HideLoader();
        });
    }
    loadDropdown() {
        this.loader.ShowLoader();
        this.commonservice.LoadDropdown("9,4,36").then(m => {
            if (m.IsSuccess) {
                let list = m.ResultSet;
                this.CompanyTypes = list.filter(f => f.DropDownID == 9);
                this.CityTypeList = list.filter(f => f.DropDownID == 4);
                this.DateFormatList = list.filter(f => f.DropDownID == 36);
                this.LanguageTypes = m.ResultSet.languaheList;
                this.loader.HideLoader();
            }
        });
    }
    IsvalidPhone(txtbxName: string) {
        var re = new RegExp('^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$');
        if (txtbxName == 'HP' && !re.test(this.model.Phone))
            this.model.Phone = '';
    }
    SaveOrUpdate(isValid: boolean): void {
        debugger
        this.submitted = true; // set form submit to true
        this.loader.ShowLoader();
        this.submitted = false;
        this._CompanyService.SaveOrUpdate(this.model).then(m => {
            var result = JSON.parse(m._body);
            if (result.IsSuccess) {

                localStorage.removeItem('company');
                var company = JSON.stringify(result.ResultSet);
                localStorage.setItem('company', this.encrypt.encryptionAES(company));



                this.toastr.Success(result.Message);
                this.loader.HideLoader();
            }
            else
                this.toastr.Error('Error', result.ErrorMessage);

            this.loader.HideLoader();
        });
    }
    ReceivePaymentShowHide(event: any) {
        if (event == true)
            this.IsPaymentShowHide = true;
        else
            this.IsPaymentShowHide = false;
    }
    IsNewImageEvent(FName) {
        this.IsNewImage = true;
    }
    getFileName(FName) {
        this.model.CompanyLogo = FName;
    }
    ClearImageUrl() {
        $(".ImgCropper").hide();
        this.IsNewImage = true;
        this.model.CompanyLogo = '';
        this.Image = '';
    }
    getImageUrlName(FName) {
        var img = $('.ImgCropper');
        if (img.length > 0) {
            $('.ImgCropper')[0].classList.remove('hide');
        }
        this.model.CompanyLogo = FName;
        if (GlobalVariable.IsUseS3 == "Yes") {
            if (!this.IsNewImage)
                this.Image = this.model.CompanyLogo == FName ? this.model.CompanyLogo : "";
            else
                this.Image = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
        else {
            if (this.IsEdit && !this.IsNewImage) {
                this.Image = GlobalVariable.BASE_File_URL + '' + FName;
            } else {
                this.Image = GlobalVariable.BASE_Temp_File_URL + '' + FName;
            }
        }
    }
}
