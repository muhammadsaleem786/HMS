import { Component, Input, Output, OnInit, OnChanges, EventEmitter, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { UserRoleModel, AdmRoleDt } from './UserRoleModel';
import { LoaderService } from '../../../../../CommonService/LoaderService';
import { UserRoleService } from './UserRoleService';
import { ValidationVariables } from '../../../../../AngularConfig/global';
import { CommonService } from '../../../../../CommonService/CommonService';
import { CommonToastrService } from '../../../../../CommonService/CommonToastrService';
import { EncryptionService } from '../../../../../CommonService/encryption.service';
import { Observable } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { filter } from 'rxjs/operators';
@Component({
    selector: 'setup-UserRoleComponentForm',
    templateUrl: './UserRoleComponentForm.html',
    moduleId: module.id,
    providers: [UserRoleService],
})

export class UserRoleComponentForm implements OnInit {
    public Form1: FormGroup; // our model driven form
    public submitted: boolean; // keep track on whether form is submitted
    //@Output() pageClose: EventEmitter<number> = new EventEmitter<number>();
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
    public IsAdministrator: boolean = false;
    public IsUpdateText: boolean = false;
    public model = new UserRoleModel();
    public PayrollRegion: string;
    adm_role_dtList = new Array<AdmRoleDt>();
    Structure_DynamicList: any = [];
    public StructureList: any = [];
    public AccountList: any = [];
    //@ViewChild('closeModal') closeModal: ElementRef;
    //public WasInside: boolean = false;
    public Keywords: any[] = [];
    public sub: any;
    public IsEdit: boolean = false;
    public CompanyInfo: any[] = [];
    public Rights: any;
    public ControlRights: any;
    constructor(public _CommonService: CommonService, public _fb: FormBuilder, public loader: LoaderService
        , public _UserRoleService: UserRoleService, public commonservice: CommonService
        , public toastr: CommonToastrService, private encrypt: EncryptionService, public route: ActivatedRoute, public _router: Router) {
        this.PayrollRegion = this.commonservice.getPayrollRegion();
        //this.Keywords = this.commonservice.GetKeywords("UserRole");

        this.Rights = JSON.parse(this.encrypt.decryptionAES(localStorage.getItem('Rights')));
        this.ControlRights = this._CommonService.ScreenRights("2");
    }

    ngOnInit() {
        this.Form1 = this._fb.group({
            RoleName: ['', [Validators.required]],
            Description: [''],
        });
        this.sub = this.route.queryParams
            .pipe(filter(params => params.id))
            .subscribe(params => {
                this.model.ID = params.id;
                if (this.model.ID > 0) {
                    this.loader.ShowLoader();
                    this.IsEdit = true;
                    this._UserRoleService.GetById(this.model.ID).then(m => {
                        this.ScreenLists = m.ResultSet.AllScreen.map(m => {
                            return {
                                'RoledtID': undefined, 'ScreenID': m.ID, 'ViewRights': false, 'CreateRights': false, 'DeleteRights': false, 'EditRights': false,
                                'ScreenName': m.Value, 'Active': true, 'ModuleID': m.DependedDropDownValueID, 'ModuleName': m.ModuleName, 'CreatedBy': undefined, 'CreatedDate': undefined, 'DropDownScreenID': m.DropDownID
                            }
                        });
                        var result = this.ScreenLists.filter(function (elem, index, self) {
                            return self.map(x => x.ModuleID).indexOf(elem.ModuleID) == index;
                        });
                        this.Modules = result.map(m => { return { 'ModuleID': m.ModuleID, 'ModuleName': m.ModuleName, 'Active': true } });

                        this.model = m.ResultSet.rolelist;
                        this.IsEmpExist = m.ResultSet.rolelist.adm_user_company.length > 0 ? true : false;
                        var SList = m.ResultSet.rolelist.adm_role_dt;
                        var RList = [];

                        if (m.ResultSet.rolelist.SystemGenerated && m.ResultSet.rolelist.RoleName == "Administrator")
                            this.IsAdministrator = true;
                        else
                            this.IsAdministrator = false;

                        this.IsAdmin = m.ResultSet.rolelist.SystemGenerated ? true : false;
                        this.IsUpdateText = m.ResultSet.rolelist.IsUpdateText ? true : false;
                        // screen checkboxs checked if matching records found in database else uncheck other screen
                        this.ScreenLists.filter(f => {
                            RList = SList.filter(sf => { return sf.ScreenID == f.ScreenID });
                            if (RList.length == 0)
                                f.Active = false;
                            else {
                                f.Active = true;
                                f.ViewRights = RList[0].ViewRights;
                                f.CreateRights = RList[0].CreateRights;
                                f.DeleteRights = RList[0].DeleteRights;
                                f.EditRights = RList[0].EditRights;
                                f.CreatedBy = RList[0].CreatedBy;
                                f.CreatedDate = RList[0].CreatedDate;
                                f.DropDownScreenID = RList[0].DropDownScreenID;
                                f.RoledtID = RList[0].ID;
                            }
                        })
                        this.Modules.filter(m => {
                            var screenList = this.ScreenLists.filter(x => x.ModuleID == m.ModuleID)
                            if (screenList.length == screenList.filter(s => { return s.Active == true }).length)
                                m.Active = true;
                            else
                                m.Active = false;
                        })

                        this.loader.HideLoader();
                    });

                } else {
                    this.LoadScreensWithModule();
                }
            });
    }
    LoadScreensWithModule() {
        this.loader.ShowLoader();
        this._UserRoleService.GetAllScreens().then(m => {
            this.ScreenLists = m.ResultSet.map(m => {
                return {
                    'RoledtID': undefined, 'ScreenID': m.ID, 'ViewRights': false, 'CreateRights': false, 'DeleteRights': false, 'EditRights': false,
                    'ScreenName': m.Value, 'Active': true, 'ModuleID': m.DependedDropDownValueID, 'ModuleName': m.ModuleName, 'CreatedBy': undefined, 'CreatedDate': undefined, 'DropDownScreenID': m.DropDownID
                }
            });
            var result = this.ScreenLists.filter(function (elem, index, self) {
                return self.map(x => x.ModuleID).indexOf(elem.ModuleID) == index;
            });
            this.Modules = result.map(m => { return { 'ModuleID': m.ModuleID, 'ModuleName': m.ModuleName, 'Active': true } });
        });
        this.loader.HideLoader();
    }
    GetScreensOfModule(ModuleID) {
            return this.ScreenLists.filter(x => x.ModuleID == ModuleID);
    }
    ModuleChk(ModuleID) {
        var Active = this.Modules.filter(f => { return f.ModuleID == ModuleID })[0].Active;
        var screenList = this.GetScreensOfModule(ModuleID);
        screenList.filter(f => { f.Active = Active; });
    }
    ShowScreenName(ScreenID) {
        let IsShowScreen: boolean = true;
        var Active = this.ScreenLists.filter(x => x.ScreenID == ScreenID);

        return IsShowScreen;
    }
    AllChk(ScreenID) {
        var Active = this.ScreenLists.filter(x => x.ScreenID == ScreenID);
        if (Active[0].Active == true) {
            Active[0].Active = false;
            Active[0].ViewRights = false;
            Active[0].CreateRights = false;
            Active[0].DeleteRights = false;
            Active[0].EditRights = false;
        } else {
            Active[0].Active = true;
            Active[0].ViewRights = true;
            Active[0].CreateRights = true;
            Active[0].DeleteRights = true;
            Active[0].EditRights = true;
        }
    }
    ScreenChk(ModuleID) {
        var screenList = this.GetScreensOfModule(ModuleID);
        if (screenList.length == screenList.filter(f => { return f.Active == true }).length)
            this.Modules.filter(f => { return f.ModuleID == ModuleID }).filter(f => { f.Active = true; });
        else
            this.Modules.filter(f => { return f.ModuleID == ModuleID }).filter(f => { f.Active = false; });
    }
    SaveOrUpdate(isValid: boolean): void {
        this.submitted = true; // set form submit to true
        if (isValid) {
            this.submitted = false;
            this.loader.ShowLoader();
            this.model.adm_role_dt = new Array<AdmRoleDt>();
            this.model.adm_role_dt = this.ScreenLists.filter(f => f.Active == true).map(m => {
                return {
                    'ID': m.RoledtID, 'CompanyID': 0, 'RoleID': 0, 'ScreenID': m.ScreenID, 'ViewRights': m.ViewRights, 'CreateRights': m.CreateRights, 'DeleteRights': m.DeleteRights, 'EditRights': m.EditRights, 'Active': m.Active, 'CreatedBy': m.CreatedBy, 'CreatedDate': m.CreatedDate, 'DropDownScreenID': m.DropDownScreenID
                }
            })
            if (this.model.adm_role_dt.length > 0) {
                this._UserRoleService.SaveOrUpdate(this.model).then(m => {
                    var result = JSON.parse(m._body);
                    if (result.IsSuccess) {
                        this.toastr.Success(result.Message);
                        if (result.ResultSet.ControlLevelRights != null) {
                            localStorage.removeItem('ControlLevelRights');
                            var coltrollevel = JSON.stringify(result.ResultSet.ControlLevelRights);
                            localStorage.setItem('ControlLevelRights', this.encrypt.encryptionAES(coltrollevel));
                        }
                        this._router.navigate(['/home/userrole']);
                    }
                    else {
                        this.toastr.Error('Error', result.ErrorMessage);
                        this.loader.HideLoader();
                    }

                });
            } else {
                this.toastr.Warning('Select Module', 'Please select atleast one module.!');
                this.loader.HideLoader();
            }
        }
    }
    Delete() {
        var result = confirm("Are you sure you want to delete selected record.");
        if (result) {
            this.loader.ShowLoader();
            this._UserRoleService.Delete(this.model.ID.toString()).then(m => {
                if (m.ErrorMessage != null)
                    this.toastr.Error('Error', m.ErrorMessage);
                //this.WasInside = true;
                //this.closeModal.nativeElement.click();
            });
        }
    }
    Close() {
        //if (!this.WasInside)
        //    this.pageClose.emit(0);
        //else
        //    this.pageClose.emit(1);

        this.ResetRights();
        this.model = new UserRoleModel();
        //this.adm_role_dtList = new Array<AdmRoleDt>();
        this.submitted = false;
        this.IsEmpExist = false;
        this.IsAdmin = false;
        this.IsUpdateText = false;
    }
    ResetRights() {
        this.Modules.filter(f => { f.Active = true; })
        this.ScreenLists.filter(f => { f.RoledtID = undefined, f.Active = true; f.CreatedBy = undefined; f.CreatedDate = undefined; f.CreatedDate = undefined })
    }
}
