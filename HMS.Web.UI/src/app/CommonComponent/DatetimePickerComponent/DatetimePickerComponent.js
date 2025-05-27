"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require("@angular/core");
var forms_1 = require("@angular/forms");
var noop = function () {
};
exports.CUSTOM_INPUT_CONTROL_VALUE_ACCESSOR = {
    provide: forms_1.NG_VALUE_ACCESSOR,
    useExisting: core_1.forwardRef(function () { return DatetimePickerComponent; }),
    multi: true
};
var DatetimePickerComponent = (function () {
    function DatetimePickerComponent() {
        //The internal data model
        this.innerValue = "";
        this.model = { date: null };
        //Placeholders for the callbacks which are later providesd
        //by the Control Value Accessor
        this.onTouchedCallback = noop;
        this.onChangeCallback = noop;
        this.DatePickerOptions = {
            // other options...
            dateFormat: 'dd/mm/yyyy',
        };
    }
    Object.defineProperty(DatetimePickerComponent.prototype, "value", {
        //get accessor
        get: function () {
            return this.innerValue;
        },
        //set accessor including call the onchange callback
        set: function (v) {
            if (v !== this.innerValue) {
                this.innerValue = v;
                this.onChangeCallback(v);
            }
        },
        enumerable: true,
        configurable: true
    });
    ;
    //Set touched on blur
    DatetimePickerComponent.prototype.onBlur = function () {
        this.onTouchedCallback();
    };
    //From ControlValueAccessor interface
    DatetimePickerComponent.prototype.writeValue = function (value) {
        if (value !== this.innerValue) {
            if (value === undefined || value == "" || value == null) {
                this.value = null;
                this.model = null;
            }
            else {
                var CDate = new Date(value);
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
    };
    //From ControlValueAccessor interface
    DatetimePickerComponent.prototype.registerOnChange = function (fn) {
        this.onChangeCallback = fn;
    };
    //From ControlValueAccessor interface
    DatetimePickerComponent.prototype.registerOnTouched = function (fn) {
        this.onTouchedCallback = fn;
    };
    DatetimePickerComponent.prototype.onDateChanged = function (event) {
        // event properties are: event.date, event.jsdate, event.formatted and event.epoc
        if (event.jsdate == null)
            this.value = null;
        else {
            this.value = this.GetDateValue(event.jsdate);
        }
    };
    DatetimePickerComponent.prototype.GetDateValue = function (jsdate) {
        var d = new Date(jsdate);
        return d.getFullYear() + "-" + ('0' + (d.getMonth() + 1)).slice(-2) + '-' + ('0' + d.getDate()).slice(-2);
    };
    return DatetimePickerComponent;
}());
DatetimePickerComponent = __decorate([
    core_1.Component({
        selector: 'Datetime-Component',
        moduleId: module.id,
        templateUrl: 'DatetimePickerComponent.html',
        providers: [exports.CUSTOM_INPUT_CONTROL_VALUE_ACCESSOR]
    }),
    __metadata("design:paramtypes", [])
], DatetimePickerComponent);
exports.DatetimePickerComponent = DatetimePickerComponent;
//# sourceMappingURL=DatetimePickerComponent.js.map