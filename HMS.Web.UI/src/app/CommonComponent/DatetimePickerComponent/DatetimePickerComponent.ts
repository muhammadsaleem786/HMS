import { Component, forwardRef, Output, Input, EventEmitter, ViewChild, OnInit } from '@angular/core';
import { Response } from '@angular/http';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { EncryptionService } from '../../CommonService/Encryption.service';
import { IMyDpOptions, IMyDateModel, IMySelector, MyDatePicker, IMyInputFocusBlur } from 'mydatepicker';
const noop = () => {
};

export const CUSTOM_INPUT_CONTROL_VALUE_ACCESSOR: any = {
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => DatetimePickerComponent),
    multi: true
};

@Component({
    selector: 'Datetime-Component',
    moduleId: module.id,
    templateUrl: 'DatetimePickerComponent.html',
    providers: [CUSTOM_INPUT_CONTROL_VALUE_ACCESSOR]
})
export class DatetimePickerComponent implements OnInit, ControlValueAccessor {

    //The internal data model
    public innerValue: any = "";
    public model: Object = { date: null };
    public DateFormat = "";
    @Output() DateChangeValue: EventEmitter<IMyDateModel> = new EventEmitter<IMyDateModel>();
    @Output() OnDateInputFocus: EventEmitter<IMyInputFocusBlur> = new EventEmitter<IMyInputFocusBlur>();
    @Output() TogSelector: EventEmitter<any> = new EventEmitter<any>();
    @Input() IsDisabled: boolean = false;
    @Input() ShowClearButton: boolean = true;
    @Input() Placeholder: string = 'DD/MM/YYYY';
    @ViewChild('mydp') mydp: MyDatePicker;

    //Placeholders for the callbacks which are later providesd
    //by the Control Value Accessor
    public onTouchedCallback: () => void = noop;
    public onChangeCallback: (_: any) => void = noop;
    public DatePickerOptions: IMyDpOptions = {
        // other options...
        showInputField: true,
        openSelectorOnInputClick: true,
        dateFormat: 'dd/mm/yyyy',
        editableDateField: false,
        showWeekNumbers: true,
        markCurrentMonth: true,
    };
    public selector: IMySelector = {
        open: true
    };
    constructor(/*private encrypt: EncryptionService*/) {
    }

    ngOnInit() {
        //this.DateFormat = this.encrypt.decryptionAES(localStorage.getItem("DateFormat"));
        this.DatePickerOptions.showClearDateBtn = this.ShowClearButton;
    };

    //get accessor
    get value(): any {
        return this.innerValue;
    };

    //set accessor including call the onchange callback
    set value(v: any) {
        if (v !== this.innerValue) {
            this.innerValue = v;
            this.onChangeCallback(v);
        }
    }

    //Set touched on blur
    onBlur() {
        this.onTouchedCallback();
    }

    //From ControlValueAccessor interface
    writeValue(value: any) {

        if (value !== this.innerValue) {

            if (value === undefined || value == "" || value == null) {
                this.value = null;
                this.model = null;
            }
            else {
                let CDate = new Date(value);
                this.model = {
                    date: {
                        year: CDate.getFullYear(),
                        month: CDate.getMonth() + 1,
                        day: CDate.getDate()
                    }
                };

                this.value = this.GetDateValue(value);
            }
        }
        else if (value === undefined || value == null) {
            this.value = null;
            this.model = null;
        }
    }

    //From ControlValueAccessor interface
    registerOnChange(fn: any) {

        this.onChangeCallback = fn;
    }

    //From ControlValueAccessor interface
    registerOnTouched(fn: any) {
        this.onTouchedCallback = fn;
    }

    onDateChanged(event: IMyDateModel) {
        // event properties are: event.date, event.jsdate, event.formatted and event.epoc
        if (event.jsdate == null) {
            this.value = null;
            this.DateChangeValue.emit(null);
        }
        else {
            this.value = this.GetDateValue(event.jsdate);
            this.DateChangeValue.emit(event);
        }
    }

    GetDateValue(jsdate: any): string {
        var d = new Date(jsdate);
        return d.getFullYear() + "-" + ('0' + (d.getMonth() + 1)).slice(-2) + '-' + ('0' + d.getDate()).slice(-2);
    }

}