import { Directive, HostListener } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
    selector: '[formControlName][appPhoneMask]',
})
export class PhoneMaskDirective {

    constructor(public ngControl: NgControl) { }

    @HostListener('ngModelChange', ['$event'])
    onModelChange(event) {
        this.applyMask(event, false);
        //this.onInputChange(event, false);
    }

    @HostListener('keydown.backspace', ['$event'])
    keydownBackspace(event) {
        this.applyMask(event.target['value'], true);
        //this.onInputChange(event.target.value, true);
    }

    applyMask(event, backspace) {
        if (event) {
            let x = event.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,5})/);
            let value = !x[1] ? x[1] : '' + x[1] + '' + x[2] + (x[3] ? '' + x[3] : '')
            this.ngControl.valueAccessor.writeValue(value);
        }
    }

    //onInputChange(event, backspace) {
      
        //if (event.target && event.target.value) {
        //    let x = event.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,5})/);
        //    let value = !x[1] ? x[1] : '' + x[1] + '' + x[2] + (x[3] ? '' + x[3] : '')
        //    this.ngControl.valueAccessor.writeValue(value);
        //}
       // var x = event.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,4})/);
       //let value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');

       // this.ngControl.valueAccessor.writeValue(value);
        

        //let newVal = event.replace(/\D/g, '');
        //if (backspace && newVal.length <= 6) {
        //    newVal = newVal.substring(0, newVal.length - 1);
        //}
        //if (newVal.length === 0) {
        //    newVal = '';
        //} else if (newVal.length <= 3) {
        //    newVal = newVal.replace(/^(\d{0,3})/, '($1)');
        //} else if (newVal.length <= 6) {
        //    newVal = newVal.replace(/^(\d{0,3})(\d{0,3})/, '($1) ($2)');
        //} else if (newVal.length <= 10) {
        //    newVal = newVal.replace(/^(\d{0,3})(\d{0,3})(\d{0,4})/, '($1) ($2)-$3');
        //} else {
        //    newVal = newVal.substring(0, 10);
        //    newVal = newVal.replace(/^(\d{0,3})(\d{0,3})(\d{0,4})/, '($1) ($2)-$3');
        //}
        //this.ngControl.valueAccessor.writeValue(newVal);
    //}
}
