import { Component, OnInit, ViewEncapsulation, AfterViewInit, Output, EventEmitter } from '@angular/core';
//import { Helpers } from '../../../helpers';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CompanyService } from '../../Module/setup/Setting/Company/company.service';
import { LoaderService } from '../../../CommonService/LoaderService';
import { AsidebarService } from '../../../CommonService/AsidebarService';
import { Router } from '@angular/router';
import { ValidationVariables } from '../../../AngularConfig/global';
import { Company } from '../../Module/models/company.model';
//import { Jsonp } from '@angular/http';
import { CommonService } from '../../../CommonService/CommonService';
import { DashboardMenuService } from '../../../CommonService/DashboardMenuService';
import { User } from '../../../Module/model/User.model';
import { AuthenticationService } from '../../../CommonService/AuthenticationService';
import { CommonToastrService } from '../../../CommonService/CommonToastrService';
import { GlobalVariable } from '../../../AngularConfig/global';
import { EncryptionService } from '../../../CommonService/encryption.service';
//import * as $ from "jquery";
@Component({
    selector: 'app-asidebar',
    templateUrl: './asidebar.component.html',
    providers: [FormBuilder, CompanyService, AuthenticationService],
})
export class AsidebarComponent implements OnInit {
    Form1: FormGroup;
    Form2: FormGroup;
    @Output() profilePopup: EventEmitter<void> = new EventEmitter<void>();
    @Output() ClassAssign: EventEmitter<void> = new EventEmitter<void>();
    public submitted: boolean;
    public isCompanyExist: boolean;
    public IsFeatureslist: boolean = false;
    public UserLock: boolean = true;
    public IsPortalLogin: any;
    public model = new Company();
    public Usermodel = new User();
    public CompanyID: number;
    public PackageID: number;
    public LoadModules = [];
    public Languages = [];
    public Packages = [];
    public passwd: string = "";
    public Rights: number[] = [];
    public Modules: number[] = [];
    public PayrollRegion: string;
    public DueDate: string;
    public PortalType: string;
    public Email: string;
    public Keywords: any[] = [];
    public PaymentList: any[] = [];
    public PackgeList: any[] = [];
    public UsersObj: any;
    public admCompany = new Company();
    constructor(public _fb: FormBuilder, public _router: Router, public companyService: CompanyService
        , public commonservice: CommonService, public _authenticationService: AuthenticationService,
        public _AsidebarService: AsidebarService, public loader: LoaderService, public DashboardMenu: DashboardMenuService, public toastr: CommonToastrService, private encrypt: EncryptionService) {
        this.Modules = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Modules')));
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.UsersObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('currentUser')));
        this.Email = this.UsersObj.Email;
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        let company: any = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        this.CompanyID = company.CompanyID;


        if (company.CompanyID !== null && company.CompanyID !== 'undefined' && company.CompanyID > 0 && this.UserLock == true) {
            this.model.ID = company.CompanyID;
            this.isCompanyExist = true;
            this.DashboardMenu.ShowMenues();
            if (this.IsPortalLogin == 'false')
                this.LoadDropDown();
        } else {
            this.isCompanyExist = false;
            this.DashboardMenu.HideMenues();
        } 
        if (this.UsersObj !== null && this.UsersObj !== 'undefined') {
            this.Usermodel = this.UsersObj;
            this.passwd = this.Usermodel.Pwd;
            if (this.Usermodel.UserImage === "")
                this.Usermodel.UserImage = null;           
            if (this.Usermodel.UserImage !== null && this.Usermodel.UserImage !== undefined) {
                this.Usermodel.UserImage = GlobalVariable.BASE_Temp_File_URL + '' + this.Usermodel.UserImage;

            }
        }
    }

    Logout(): void {
        this._authenticationService.Logout();
        this._router.navigate(['/login']);
    }
    AddProfile() {
        var id = this.UsersObj.ID;
        this._router.navigate(['home/Profile/saveuser'], { queryParams: { id: id } });
    }
    LoadDropDown(): any {

        this.loader.ShowLoader();
        this.commonservice.LoadDropdown("14").then(m => {
            if (m.IsSuccess) {
                this.LoadModules = m.ResultSet.dropdownValues;
                this.Languages = m.ResultSet.languaheList;
                this.Usermodel.MultilingualId = m.ResultSet.ID;
                this.PackgeList = m.ResultSet.PackgeList;
            }
        });
        this.loader.HideLoader();
    }

    ngOnInit() {
        this.Form1 = this._fb.group({
            ContactPersonFirstName: ['', [Validators.pattern(ValidationVariables.AlphabetPattern), Validators.required]],
            ContactPersonLastName: ['', [Validators.pattern(ValidationVariables.AlphabetPattern), Validators.required]],
            GenderID: [''],
            Phone: [''],
            Fax: [''],
            Email: ['', [Validators.pattern(ValidationVariables.EmailPattern)]],
        });

        $('body').on("click", ".dropdown-menu", function (e) {
            $(this).parent().is(".open") && e.stopPropagation();
        })

    }
    changeLanguage(event: any) {
        this.loader.ShowLoader();
        this.commonservice.changeLanguage(event).then(m => {
            if (m.IsSuccess) {
                this.Languages = m.ResultSet.languaheList;
                this.Usermodel.MultilingualId = m.ResultSet.ID;
                localStorage.removeItem('MultiKeyword');
                localStorage.setItem('MultiKeyword', JSON.stringify(m.ResultSet.MultiKeyword));
                this._router.navigate(['/home/dashboard']).then(() => { window.location.reload(); });
                localStorage.setItem('lingualId', m.ResultSet.ID);

            }
        });
        this.loader.HideLoader();
    }
    changePackage(event: any) {
        this.loader.ShowLoader();
        this.commonservice.changePackage(event).then(m => {
            if (m.IsSuccess) {
                localStorage.removeItem('IsLogin');
                localStorage.removeItem('currentUser');
                localStorage.removeItem('UserID');
                localStorage.removeItem('Token');
                localStorage.removeItem('PayrollRegion');
                localStorage.removeItem('Currency');
                localStorage.removeItem('ValidTo');
                localStorage.removeItem('Modules');
                localStorage.removeItem('Rights');
                this._router.navigate(['/login']);
            }
        });
        this.loader.HideLoader();
    }
    GetContactDetail() {
        this.profilePopup.emit();
    }
    ToggleScreen() {
        this.ClassAssign.emit();
    }
    HideSetting() {
        this._AsidebarService.SetMenuId(1);
        this._router.navigate(['/home/dashboard']);
    }
    SaveFeatureList(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();

        }
    }
    Featureslist() {
        this.loader.ShowLoader();


    }
}

