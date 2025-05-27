import { Component, Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import swal from 'sweetalert';
@Injectable()
export class CommonToastrService {
    constructor(private toastr: ToastrService) {

        this.toastr.toastrConfig.autoDismiss = true;
        this.toastr.toastrConfig.tapToDismiss = true;
        this.toastr.toastrConfig.closeButton = true;
        this.toastr.toastrConfig.preventDuplicates = true;
        this.toastr.toastrConfig.newestOnTop = true;
        this.toastr.toastrConfig.timeOut = 3000;
        this.toastr.toastrConfig.positionClass = 'toast-top-right';
        //this.toastr.toastrConfig.positionClass = 'toast-bottom-full-width';
        this.toastr.toastrConfig.autoDismiss = false;
        //get other option from documentation
    }

    Success(title?: string, message?: string) {
        this.toastr.success(message, title);

    }
    Warning(title?: string, message?: string) {
        this.toastr.warning(message, title);

    }
    ShowFullWidthError(title?: string, message?: string) {
        this.toastr.error(message, title, { positionClass: 'toast-top-center', timeOut: 5000 });
    }
    Error(title?: string, message?: string) {
        this.toastr.error(message, title);
    }
    Info(title?: string, message?: string) {
        this.toastr.info(message, title);
    }

    RemoveToastr() {
        // Immediately remove current toasts without using animation
        //this.toastr.remove();
    }

    RemoveWithAnimation() {
        // Remove current toasts using animation
        //toastr.clear();
    }
    DeleteAlert(): any {
        let isDelete = false;
        swal({
            title: "Are you sure?",
            text: "you want to delete selected record.",
            icon: "warning",
            buttons: ['Cancel', 'Ok'],
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {
                    isDelete = true;

                }
            })
        return isDelete;
    }

    // Override global options
    //toastr.success('We do have the Kapua suite available.', 'Turtle Bay Resort', {timeOut: 5000 })

}

