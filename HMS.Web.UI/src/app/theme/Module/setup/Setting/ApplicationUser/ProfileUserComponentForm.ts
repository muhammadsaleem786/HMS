import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ApplicationUserModel } from './ApplicationUserModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { ApplicationUserService } from './ApplicationUserService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { IMyDpOptions, IMyDateModel, MyDatePicker } from 'mydatepicker';
import { ActivatedRoute, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { IMultiSelectOption, IMultiSelectSettings, IMultiSelectTexts } from 'angular-2-dropdown-multiselect';
import { GlobalVariable } from '../../../../../AngularConfig/global';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { EncryptionService } from '../../../../../CommonService/encryption.service';

declare var $: any;
@Component({
    selector: 'setup-ApplicationUserComponentForm',
    templateUrl: './ProfileUserComponentForm.html',
    moduleId: module.id,
    providers: [ApplicationUserService, FormBuilder],
})
export class ProfileUserComponentForm implements OnInit {
    public RoleList = [];
    public WorkingdayList = [];
    EmpfilteredList = [];
    public keyword: string;
    public DisplayMember: string;
    public Form1: FormGroup; // our model driven form
    public submitted: boolean; // keep track on whether form is submitted
    @Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
    @Input() ScreenName: string;
    @Input() id: number;
    public IsReadOnly = false;
    public IsDoctor = false;
    public IsEmpFound: boolean = false;
    public IsAdmin: boolean = false;
    public hide = true;
    public sub: any; public CompanyObj: any;
    public IsEdit: boolean = false;
    public model = new ApplicationUserModel();
    public Charges: any;
    public ExpiryDate: any;
    public ActivatedDate: any;
    public TotalUser: any;
    @ViewChild('closeModal') closeModal: ElementRef;
    public UserProfileImage: string = '';
    public GenderList = []; public TemplateList: any[] = [];
    public IsNewImage: boolean = true;
    public WasInside: boolean = false;
    public IsAdminRole: any = '';
    public Keywords: any[] = [];
    public SpecialtyList = [];
    @ViewChild('editorHeaderDescription') editorHeaderDescription!: ElementRef;
    @ViewChild('editorHeaderDescriptionUrdu') editorHeaderDescriptionUrdu!: ElementRef;
    @HostListener('click', ['$event'],)
    Clickoutdocument(event) {
        if (!this.WasInside)
            this.Close();
        this.WasInside = false;
    }
    IsModalClick() {
        this.WasInside = true;
    }
    public ControlRights: any;
    public ColumnSettings: IMultiSelectSettings = {
        pullRight: false,
        enableSearch: true,
        checkedStyle: 'checkboxes',
        buttonClasses: 'btn btn-default',
        selectionLimit: 0,
        closeOnSelect: false,
        showCheckAll: true,
        showUncheckAll: true,
        dynamicTitleMaxItems: 1,
        maxHeight: '300px',
    };

    public ColumnTexts: IMultiSelectTexts = {
        checkAll: 'Select all',
        //uncheckAll: 'Uncheck all',
        checked: 'checked',
        checkedPlural: 'checked',
        searchPlaceholder: 'Search...',
        defaultTitle: 'All',
    };
    constructor(public _CommonService: CommonService, public _fb: FormBuilder, public route: ActivatedRoute, public _router: Router
        , public loader: LoaderService, public _ApplicationUserService: ApplicationUserService
        , public toastr: CommonToastrService, private sanitizer: DomSanitizer, private encrypt: EncryptionService) {
        this.ControlRights = this._CommonService.ScreenRights("4");
        this.IsAdminRole = this._CommonService.RoleName();
        this.CompanyObj = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('company')));
    }
    ngOnInit() {
        this.TemplateList = [
            { "ID": 1, "Name": "Standard" },
            { "ID": 2, "Name": "Urdu" }
        ];
        this.loadRole();
        this.Form1 = this._fb.group({
            Name: [''],
            UserID: [''],
            CompanyID: [''],
            RoleID: ['', [Validators.required]],
            Email: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.EmailPattern)]],
            PhoneNo: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.NumberPattern), Validators.minLength(11), Validators.maxLength(11)]],
            AdminID: [''],
            IsDefault: [''],
            SlotTime: [''],
            AppStartTime: [''],
            IsOverLap: [''],
            AppEndTime: [''],
            DocWorkingDay: [''],
            EmployeeID: ['', [Validators.required]],
            Designation: [''],
            Qualification: [''],
            DesignationUrdu: [''],
            QualificationUrdu: [''],
            HeaderDescription: [''],
            HeaderDescriptionUrdu: [''],
            NameUrdu: [''],
            TemplateId: [''],
            Type: [''],
            SpecialtyId: [''],
            searchText: [''],
            SpecialtyDropdownId: [''],
            GenderDropdown: [''],
            IsActivated: [''],
            UserImage: [''],
            RepotFooter: [''],
            Instruction: [''],
            Pwd: ['', [Validators.required, <any>Validators.minLength(6)]],
        });
        $(".toggle-password").click(function () {
            $(this).toggleClass("fa-eye fa-eye-slash");
            var input = $($(this).attr("toggle"));
            if (input.attr("type") == "password") {
                input.attr("type", "text");
            } else {
                input.attr("type", "password");
            }
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.id = params.id;
                if (this.id > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this.loader.ShowLoader();
                    this._ApplicationUserService.GetById(this.id).then(m => {
                        this.model = m.ResultSet.UserObj;
                        this.DisplayMember = "";
                        this.IsEmpFound = true;
                        this.model.safeHeaderDescription = this.sanitizer.bypassSecurityTrustHtml(this.model.HeaderDescription);
                        this.model.safeHeaderDescriptionUrdu = this.sanitizer.bypassSecurityTrustHtml(this.model.HeaderDescriptionUrdu);

                        if (m.ResultSet.employeeObj) {
                            this.DisplayMember = `${m.ResultSet.employeeObj.FirstName} ${m.ResultSet.employeeObj.LastName}`;

                            if (m.ResultSet.employeeObj.EmployeePic != null && m.ResultSet.employeeObj.EmployeePic != undefined && m.ResultSet.employeeObj.EmployeePic != "") {
                                this.getImageUrlName(m.ResultSet.employeeObj.EmployeePic);
                                this.IsNewImage = false;
                            } else {
                                this.IsNewImage = true;
                            }
                        } else {
                            this.DisplayMember = 'No Employee Data';
                            this.IsNewImage = true;
                        }
                        var GenderList = m.ResultSet.UserObj.IsGenderDropdown;
                        if (GenderList != null && GenderList != "") {
                            const numArray = GenderList.split(',').map(Number);
                            this.model.GenderDropdown = numArray;
                        }
                        if (this.model.SlotTime != null) {
                            this.model.SlotTime = parseInt(this.model.SlotTime.split(':')[1]);
                        }
                        this.RoleList = m.ResultSet.role;
                        var findrole = this.RoleList.filter(a => a.ID == this.model.adm_user_company[0].RoleID && a.RoleName == "Doctor");
                        if (findrole.length > 0) {
                            this.IsDoctor = true;
                            this.LoadEditor();
                            this.LoadEditorUrdu();
                        }
                        else
                            this.IsDoctor = false;

                        var result = m.ResultSet.UserObj.adm_user_company[0];
                        this.IsAdmin = result.UserID == result.AdminID ? true : false;


                        this.loader.HideLoader();
                    });
                } else {
                    this.loader.HideLoader();
                }
            });
    }
    LoadEditor() {
        setTimeout(() => {
            const CKEDITOR = (window as any).CKEDITOR;
            if (this.editorHeaderDescription && this.editorHeaderDescription.nativeElement) {
                if (CKEDITOR.instances['editorHeaderDescription']) {
                    CKEDITOR.instances['editorHeaderDescription'].destroy(true);
                }
                const editor = CKEDITOR.replace('editorHeaderDescription', {
                    enterMode: (window as any).CKEDITOR.ENTER_BR,
                    shiftEnterMode: (window as any).CKEDITOR.ENTER_P,
                });
                editor.on('change', () => {
                    const html = editor.getData();
                    this.model.HeaderDescription = html;
                    this.model.safeHeaderDescription = this.sanitizer.bypassSecurityTrustHtml(html);
                });
            }
        }, 10);
    }
    TemplateChange(id: any) {
        if (id == 1) {
            this.model.DesignationUrdu = "";
            this.model.QualificationUrdu = "";
            this.model.HeaderDescription = "";
            this.model.NameUrdu = "";
        } else
            this.LoadEditorUrdu();
    }
    LoadEditorUrdu() {
        setTimeout(() => {
            const CKEDITOR = (window as any).CKEDITOR;
            if (this.editorHeaderDescriptionUrdu && this.editorHeaderDescriptionUrdu.nativeElement) {
                if (CKEDITOR.instances['editorHeaderDescriptionUrdu']) {
                    CKEDITOR.instances['editorHeaderDescriptionUrdu'].destroy(true);
                }
                const editor = CKEDITOR.replace('editorHeaderDescriptionUrdu', {
                    enterMode: (window as any).CKEDITOR.ENTER_BR,
                    shiftEnterMode: (window as any).CKEDITOR.ENTER_P,
                });
                editor.on('change', () => {
                    const html = editor.getData();
                    this.model.HeaderDescriptionUrdu = html;
                    this.model.safeHeaderDescriptionUrdu = this.sanitizer.bypassSecurityTrustHtml(html);
                });
            }
        }, 10);
    }
    onRoleChange(event) {
        var findrole = this.RoleList.filter(a => a.ID == parseInt(event) && a.RoleName == "Doctor");
        if (findrole.length > 0)
            this.IsDoctor = true;
        else
            this.IsDoctor = false;
    }
    loadRole() {
        this.model.GenderDropdown = [];
        this._ApplicationUserService.GetRoles().then(m => {
            if (m.IsSuccess)
                this.RoleList = m.ResultSet.role;
            this.WorkingdayList = m.ResultSet.WorkingdayList;
            this.GenderList = m.ResultSet.GenderList;
            this.SpecialtyList = m.ResultSet.dropdown.filter(a => a.DropDownID == 24);
        });
    }
    SaveOrUpdate(isValid: boolean): void {
        let roleid = this.model.adm_user_company[0].RoleID;
        var findrole = this.RoleList.filter(a => a.ID == roleid && a.RoleName == "Doctor");
        if (findrole.length > 0) {
            if (this.model.SlotTime == null || this.model.SlotTime == undefined)
                return this.toastr.Error('Error', 'please select slot time.')

            //if (this.model.AppStartTime == null || this.model.AppStartTime == undefined)
            //    return this.toastr.Error('Error', 'please select start time.')

            //if (this.model.AppEndTime == null || this.model.AppEndTime == undefined)
            //    return this.toastr.Error('Error', 'please select end time.')

        }
        this.submitted = true; // set form submit to true
        if (isValid) {
            var multiid = parseInt(localStorage.getItem('lingualId'));
            this.submitted = false;
            this.loader.ShowLoader();
            this.model.MultilingualId = multiid;
            if (this.model.SlotTime != null) {
                var time = '0' + ':' + this.model.SlotTime + ':0';
                this.model.SlotTime = time;
            }
            this._ApplicationUserService.SaveOrUpdate(this.model).then(m => {
                var result = JSON.parse(m._body);
                if (result.IsSuccess) {
                    this.toastr.Success(result.Message);
                    localStorage.removeItem('currentUser');
                    var user = JSON.stringify(result.ResultSet.Model);
                    localStorage.setItem('currentUser', this.encrypt.encryptionAES(user));
                    this._router.navigate(['home/setup']);
                }
                else
                    this.toastr.Error('Error', result.ErrorMessage);

                this.loader.HideLoader();
            });
        }
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result == true) {
            this.loader.ShowLoader();
            this._ApplicationUserService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null) {
                    this.toastr.Error('Error', m.ErrorMessage);
                } else {
                    this._router.navigate(['home/setup']);
                }
                this.loader.HideLoader();
            });
        }
    }
    Close() {
        if (!this.WasInside)
            this.pageClose.emit(0);
        else
            this.pageClose.emit(1);
    }
    IsNewImageEvent(FName) {
        this.IsNewImage = true;
    }
    getFileName(FName) {
        this.model.UserImage = FName;
    }
    ClearImageUrl() {
        this.IsNewImage = true;
        this.model.UserImage = '';
        this.UserProfileImage = '';
    }
    getImageUrlName(FName) {
        this.model.UserImage = FName;
        if (this.IsEdit && !this.IsNewImage) {
            this.UserProfileImage = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + FName;
        } else {
            this.UserProfileImage = GlobalVariable.BASE_Temp_File_URL + this.CompanyObj.CompanyID + '/' + FName;
        }
    }

    openManagePlan(): void {
  window.open('https://easyhms.com', '_blank');
}
}
