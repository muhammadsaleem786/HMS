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
    public sub: any;
    public IsEdit: boolean = false;
    public model = new ApplicationUserModel();
    @ViewChild('closeModal') closeModal: ElementRef;
    public UserProfileImage: string = '';
    public GenderList = [];
    public IsNewImage: boolean = true;
    public WasInside: boolean = false;
    public Keywords: any[] = [];
    public SpecialtyList = [];
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
        , public toastr: CommonToastrService, public commonservice: CommonService) {
        //this.Keywords = this.commonservice.GetKeywords("ApplicationUser");
        this.ControlRights = this._CommonService.ScreenRights("4");
    }
    ngOnInit() {
        this.loadRole();
        this.Form1 = this._fb.group({
            Name: [''],
            UserID: [''],
            CompanyID: [''],
            RoleID: ['', [Validators.required]],
            Email: ['', [<any>Validators.required, Validators.pattern(ValidationVariables.EmailPattern)]],
            PhoneNo: ['', [Validators.required]],
            AdminID: [''],
            IsDefault: [''],
            SlotTime: [''],
            AppStartTime: [''],
            IsOverLap: [''],
            AppEndTime: [''],
            DocWorkingDay: [''],
            GenderDropdown: [''],
            RepotFooter: [''],
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
                        if (this.model.UserImage != null || this.model.UserImage != undefined) {
                            this.getImageUrlName(this.model.UserImage);
                            this.IsNewImage = false;
                        } else this.IsNewImage = true;
                        if (m.ResultSet.UserObj.StartTime != null)
                            this.model.AppStartTime = m.ResultSet.UserObj.StartTime;
                        if (m.ResultSet.UserObj.EndTime != null)
                            this.model.AppEndTime = m.ResultSet.UserObj.EndTime;
                        var days = m.ResultSet.UserObj.OffDay;
                        if (days != null) {
                            const daysArray = days.split(',').map(Number);
                            this.model.DocWorkingDay = daysArray;
                        }                       
                        var GenderList = m.ResultSet.UserObj.IsGenderDropdown;
                        if (GenderList != null) {
                            const numArray = GenderList.split(',').map(Number);
                            this.model.GenderDropdown = numArray;
                        }  
                        if (this.model.SlotTime != null) {
                            this.model.SlotTime = parseInt(this.model.SlotTime.split(':')[1]);
                        }
                        this.RoleList = m.ResultSet.role;
                        var findrole = this.RoleList.filter(a => a.ID == this.model.adm_user_company[0].RoleID && a.RoleName == "Doctor");
                        if (findrole.length > 0)
                            this.IsDoctor = true;
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

            if (this.model.AppStartTime == null || this.model.AppStartTime == undefined)
                return this.toastr.Error('Error', 'please select start time.')

            if (this.model.AppEndTime == null || this.model.AppEndTime == undefined)
                return this.toastr.Error('Error', 'please select end time.')

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
            this.UserProfileImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        } else {
            this.UserProfileImage = GlobalVariable.BASE_Temp_File_URL + '' + FName;
        }
    }
}
