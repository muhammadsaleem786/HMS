import { Component, OnInit, ViewEncapsulation, Input, EventEmitter, Output } from '@angular/core';
import { Router, NavigationStart, NavigationEnd } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Helpers } from '../helpers';
import { CompanyService } from './Module/setup/Setting/Company/company.service';
import { themeService } from './themeService';
import { LoaderService } from '../CommonService/LoaderService';
import { AsidebarService } from '../CommonService/AsidebarService';
import { CommonToastrService } from '../CommonService/CommonToastrService';
import { Company } from './Module/models/company.model';
import { NotificationAlertModel } from './themeModel';
import { ValidationVariables } from '../AngularConfig/global';
import { CommonService } from '../CommonService/CommonService';
import { EncryptionService } from '../CommonService/encryption.service';
@Component({
    selector: "",
    templateUrl: "./theme.component.html",
    encapsulation: ViewEncapsulation.None,
    providers: [CompanyService, themeService]
})
export class ThemeComponent implements OnInit {
    Form1: FormGroup;
    public submitted: boolean;
    public SettingToggle: boolean = false;
    public slideToggle: boolean = false;
    public slideToggleClass: boolean = false;
    public isCompanyExist: boolean;
    public UnreadNotificationCount: number = 0;
    public NotificationList = new Array<NotificationAlertModel>();
    public NotifAlertModel = new NotificationAlertModel();
    public Keywords: any[] = [];
    public Languageid: number;
    public IsPortalLogin: any;
    public Rights: number[] = [];
    public Modules: number[] = [];
    public CompanyObj: any;
    public model = new Company();
    constructor(public _fb: FormBuilder, private encrypt: EncryptionService, public _router: Router, public companyService: CompanyService
        , public loader: LoaderService, public _AsidebarService: AsidebarService, public _themeService: themeService
        , public toastr: CommonToastrService, public commonservice: CommonService, ) {
        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.IsPortalLogin = localStorage.getItem("PortalLogin");
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
        if (this.IsPortalLogin == 'false') {
            //this.Keywords = this.commonservice.GetKeywords("AsideNav");
            this.Languageid = JSON.parse(localStorage.getItem('lingualId'));
            if (this.Languageid == 2)
                this.slideToggleClass = true;
            else
                this.slideToggleClass = false;
        }
    }
    ngOnInit() {
        this.Form1 = this._fb.group({
            ContactPersonFirstName: ['', [Validators.required, Validators.pattern(ValidationVariables.AlphabetPattern)]],
            ContactPersonLastName: ['', [Validators.required, Validators.pattern(ValidationVariables.AlphabetPattern)]],
            GenderID: [''],
            Phone: [''],
            Fax: [''],
            Email: [''],
        });
    }
    SaveEditContactInfo(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.loader.ShowLoader();
            this.submitted = false;
            this.companyService.EditContact(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.loader.HideLoader();
                    this.toastr.Success(result.Message);
                    localStorage.setItem('Company', JSON.stringify(result.ResultSet.Company));
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    IsvalidPhone(txtbxName: string) {
        var re = new RegExp('^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$');
        if (txtbxName == 'CPM' && !re.test(this.model.Phone))
            this.model.Phone = '';
        if (txtbxName == 'WP' && !re.test(this.model.Fax))
            this.model.Fax = '';
    }
    GetProfile() {
        this.loader.ShowLoader();
        var companyID = this.CompanyObj.CompanyID;
        if (companyID) {
            this.companyService.GetById(parseInt(companyID)).then(m => {
                if (m.IsSuccess)
                    this.model = m.ResultSet;
                else {
                    this.toastr.Error('Error', 'Company not found!');
                    this._router.navigate(['/login']);
                }
                this.loader.HideLoader();
            });
        }
    }
    id = 0;
    AssignParentClass() {
        if (this.id == 1) {
            this.id = 0;
        }
        else {
            this.id = 1;
        }
    }
    HitslideToggle() {
        this.slideToggle = !this.slideToggle;
        if (!this.slideToggle) {
            let Ids: number[] = [];
            var model = this.NotificationList.forEach(x => {
                Ids.push(x.ID);
            });
            var NotifiIds = Ids.join('#');
            this._themeService.UpdateNotificationViewedStatus(NotifiIds).then(m => {
                if (m.IsSuccess) {
                    this.NotificationList = m.ResultSet;
                    this.UnreadNotificationCount = this.NotificationList.filter(x => x.IsRead == false).length;
                }
            });
        }
    }
    SettingSection() {
        this.SettingToggle = !this.SettingToggle;
    }
    SeetingMenuValSet() {
        //this.IsSetting = true;
        this.SettingToggle = !this.SettingToggle;
        this._AsidebarService.SetMenuId(2);
    }
    AddUser(id: string) {
        if (id != "0")
            this.loader.ShowLoader();
        //this.Id = id;
        //this.IsList = false;
    }    
    OnKeyPress(event): boolean {
        var inputValue = event.which;
        //allow letters and whitespaces only.
        if (!(inputValue >= 65 && inputValue <= 120) && (inputValue != 32 && inputValue != 0 && inputValue != 122 && inputValue != 121)) {
            event.preventDefault();
            return false;
        }
        return true;
    }
    GetNotifications() {
        this.loader.ShowLoader();
        this._themeService.FormLoad().then(m => {
            if (m.IsSuccess) {
                this.NotificationList = m.ResultSet;
                this.UnreadNotificationCount = this.NotificationList.filter(x => x.IsRead == false).length;
            }
            this.loader.HideLoader();
        });
    }
}