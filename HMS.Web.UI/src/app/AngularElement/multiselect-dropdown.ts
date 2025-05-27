/*
 * Angular 2 Dropdown Multiselect for Bootstrap
 *
 * Simon Lindh
 * https://github.com/softsimon/angular-2-dropdown-multiselect
 */

import {
    NgModule,
    Component,
    Pipe,
    OnInit,
    DoCheck,
    HostListener,
    Input,
    ElementRef,
    Output,
    EventEmitter,
    forwardRef,
    IterableDiffers
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
//import { CookieService } from 'ngx-cookie-service';
//import { CookieService as  CService} from 'ngx-cookie-service';
// import { CookieService } from 'angular2-cookie/services/cookies.service';
// import { CookieService as CService } from 'angular2-cookie/core';

const MULTISELECT_VALUE_ACCESSOR: any = {
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => MultiselectDropdown),
    multi: true
};

export interface IMultiSelectOption {
    id: any;
    name: string;
}

export interface IMultiSelectSettings {
    pullRight?: boolean;
    enableSearch?: boolean;
    checkedStyle?: 'checkboxes' | 'glyphicon' | 'fontawsome';
    buttonClasses?: string;
    selectionLimit?: number;
    closeOnSelect?: boolean;
    autoUnselect?: boolean;
    showCheckAll?: boolean;
    showUncheckAll?: boolean;
    dynamicTitleMaxItems?: number;
    maxHeight?: string;
    showSaveOption?: boolean;
}

export interface IMultiSelectTexts {
    checkAll?: string;
    uncheckAll?: string;
    checked?: string;
    checkedPlural?: string;
    searchPlaceholder?: string;
    defaultTitle?: string;
    SaveOption?: string;
    SaveCookieName?: string;
}

@Pipe({
    name: 'searchFilter'
})
export class MultiSelectSearchFilter {
    transform(options: Array<IMultiSelectOption>, args: string): Array<IMultiSelectOption> {
        return options.filter((option: IMultiSelectOption) =>
            option.name
                .toLowerCase()
                .indexOf((args || '').toLowerCase()) > -1);
    }
}

@Component({
    selector: 'ss-multiselect-dropdown',
    providers: [MULTISELECT_VALUE_ACCESSOR],
    template: `
<div class="dropdown-toggle" [ngClass]="settings.buttonClasses"
	    (click)="toggleDropdown()">{{ title }}&nbsp;<span class="caret pull-right"></span></div>
<div class="mt-relative-wrap">
	<div class="mt-dropdown">
	    <ul *ngIf="isVisible" class="dropdown-menu" [class.pull-right]="settings.pullRight" [class.dropdown-menu-right]="settings.pullRight">
		<li class="dropdown-item" *ngIf="settings.enableSearch">
		    <div class="uk-form-row">
			<input type="text" class="md-input" placeholder="{{ texts.searchPlaceholder }}"
			aria-describedby="sizing-addon3" [(ngModel)]="searchFilterText">
		    </div>
		</li>
		<li class="mt-divider" *ngIf="settings.enableSearch"></li>
		<li class="dropdown-item" *ngIf="settings.showCheckAll">
		    <a href="javascript:;" role="menuitem" tabindex="-1" (click)="checkAll()">
			<i class="material-icons">check</i>
			{{ texts.checkAll }}
		    </a>
		</li>
		<li class="dropdown-item" *ngIf="settings.showUncheckAll">
		    <a href="javascript:;" role="menuitem" tabindex="-1" (click)="uncheckAll()">
			<i class="material-icons">clear</i>
			{{ texts.uncheckAll }}
		    </a>
		</li>
        <li class="mt-divider"></li>
	    <li class="dropdown-item" *ngIf="settings.showSaveOption">
		    <a href="javascript:;" role="menuitem" tabindex="-1" (click)="SaveVisibleColumn()">
			<i class="material-icons">save</i>
			{{ texts.SaveOption }}
		    </a>
		</li>
		<li *ngIf="settings.showCheckAll || settings.showUncheckAll || settings.showSaveOption" class="mt-divider"></li>
		<li class="dropdown-item" style="cursor: pointer;"  *ngFor="let option of options | searchFilter:searchFilterText" (click)="setSelected($event, option)">
		    <a href="javascript:;" role="menuitem" tabindex="-1">
			<input *ngIf="settings.checkedStyle === 'checkboxes'" type="checkbox" [checked]="isSelected(option)" />
			<span *ngIf="settings.checkedStyle === 'glyphicon'" style="width: 16px;"
			class="glyphicon" [class.glyphicon-ok]="isSelected(option)"></span>
			<span *ngIf="settings.checkedStyle === 'fontawsome'" style="width: 16px;display: inline-block;">
			    <i *ngIf="isSelected(option)" class="fa fa-check" aria-hidden="true"></i>
			</span>
			{{ option.name }}
		    </a>
		</li>
	    </ul>
	</div>
</div>
`
})
export class MultiselectDropdown implements OnInit, DoCheck, ControlValueAccessor {

    @Input() options: Array<IMultiSelectOption>;
    @Input() settings: IMultiSelectSettings;
    @Input() texts: IMultiSelectTexts;
    @Output() selectionLimitReached = new EventEmitter();
    @Output() dropdownClosed = new EventEmitter();
    @Output() valueChanged = new EventEmitter();

    @HostListener('document: click', ['$event.target'])
    onClick(target: HTMLElement) {
        let parentFound = false;
        while (target != null && !parentFound) {
            if (target === this.element.nativeElement) {
                parentFound = true;
            }
            target = target.parentElement;
        }
        if (!parentFound) {
            this.isVisible = false;
        }
    }

    model: number[];
    title: string;
    differ: any;
    numSelected: number = 0;
    isVisible: boolean = false;
    searchFilterText: string = '';
    defaultSettings: IMultiSelectSettings = {
        pullRight: false,
        enableSearch: false,
        checkedStyle: 'checkboxes',
        buttonClasses: 'btn btn-default btn-secondary',
        selectionLimit: 0,
        closeOnSelect: false,
        autoUnselect: false,
        showCheckAll: false,
        showUncheckAll: false,
        dynamicTitleMaxItems: 3,
        maxHeight: '300px',
    };
    defaultTexts: IMultiSelectTexts = {
        checkAll: 'Check all',
        uncheckAll: 'Uncheck all',
        checked: 'checked',
        checkedPlural: 'checked',
        searchPlaceholder: 'Search...',
        defaultTitle: 'Select',
    };

    constructor(private element: ElementRef,
        private differs: IterableDiffers) {
        this.differ = differs.find([]).create(null);
    }

    ngOnInit() {
        this.settings = Object.assign(this.defaultSettings, this.settings);
        this.texts = Object.assign(this.defaultTexts, this.texts);
        this.title = this.texts.defaultTitle;
    }

    onModelChange: Function = (_: any) => { };
    onModelTouched: Function = () => { };


    writeValue(value: any): void {
        if (value !== undefined) {
            this.model = value;
        }
    }

    registerOnChange(fn: Function): void {
        this.onModelChange = fn;
    }

    registerOnTouched(fn: Function): void {
        this.onModelTouched = fn;
    }

    ngDoCheck() {
        let changes = this.differ.diff(this.model);
        if (changes) {
            this.updateNumSelected();
            this.updateTitle();
            this.valueChanged.emit();
        }
    }

    clearSearch() {
        this.searchFilterText = '';
    }

    toggleDropdown() {
        this.isVisible = !this.isVisible;
        if (!this.isVisible) {
            this.dropdownClosed.emit();
        }
    }

    isSelected(option: IMultiSelectOption): boolean {
        return this.model && this.model.indexOf(option.id) > -1;
    }

    setSelected(event: Event, option: IMultiSelectOption) {
        if (!this.model) {
            this.model = [];
        }
        let index = this.model.indexOf(option.id);
        if (index > -1) {
            this.model.splice(index, 1);
        } else {
            if (this.settings.selectionLimit === 0 || this.model.length < this.settings.selectionLimit) {
                this.model.push(option.id);
            } else {
                if (this.settings.autoUnselect) {
                    this.model.push(option.id);
                    this.model.shift();
                } else {
                    this.selectionLimitReached.emit(this.model.length);
                    return;
                }
            }
        }
        if (this.settings.closeOnSelect) {
            this.toggleDropdown();
        }
        this.onModelChange(this.model);
    }

    updateNumSelected() {
        this.numSelected = this.model && this.model.length || 0;
    }

    updateTitle() {
        if (this.numSelected === 0) {
            this.title = this.texts.defaultTitle;
        } else if (this.settings.dynamicTitleMaxItems >= this.numSelected) {
            this.title = this.options
                .filter((option: IMultiSelectOption) =>
                    this.model && this.model.indexOf(option.id) > -1
                )
                .map((option: IMultiSelectOption) => option.name)
                .join(', ');
        } else {
            this.title = this.numSelected
                + ' '
                + (this.numSelected === 1 ? this.texts.checked : this.texts.checkedPlural);
        }
    }

    checkAll() {
        
        this.model = this.options.map(option => option.id);
        this.onModelChange(this.model);
    }

    uncheckAll() {
        
        this.model = [];
        this.onModelChange(this.model);
    }

    SaveVisibleColumn() {
        // this._cookieService.putObject(this.defaultTexts.SaveCookieName, this.model);
        this.toggleDropdown();
    }
}

@NgModule({
    imports: [CommonModule, FormsModule],
    exports: [MultiselectDropdown],
    declarations: [MultiselectDropdown, MultiSelectSearchFilter],
    providers: [],
})
export class MultiselectDropdownModule { }
