/**
 * System configuration for Angular samples
 * Adjust as necessary for your application needs.
 */
(function (global) {
    System.config({
        paths: {
            // paths serve as alias
            'npm:': 'node_modules/'
        },
        // map tells the System loader where to look for things
        map: {
            // our app is within the app folder
            app: 'app',

            // angular bundles
            '@angular/core': 'npm:@angular/core/bundles/core.umd.js',
            '@angular/common': 'npm:@angular/common/bundles/common.umd.js',
            '@angular/compiler': 'npm:@angular/compiler/bundles/compiler.umd.js',
            '@angular/platform-browser': 'npm:@angular/platform-browser/bundles/platform-browser.umd.js',
            '@angular/platform-browser-dynamic': 'npm:@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.js',
            '@angular/http': 'npm:@angular/http/bundles/http.umd.js',
            '@angular/router': 'npm:@angular/router/bundles/router.umd.js',
            '@angular/forms': 'npm:@angular/forms/bundles/forms.umd.js',
            '@angular/animations': 'npm:@angular/animations/bundles/animations.umd.js',
            'ng2-auto-complete': 'npm:ng2-auto-complete/dist/ng2-auto-complete.umd.js',
            //'@angular/animations/browser': 'npm:@angular/animations/bundles/animations-browser.umd.js',
            //'@angular/platform-browser/animations': 'npm:@angular/platform-browser/bundles/platform-browser-animations.umd.js',


            // other libraries
            'rxjs': 'npm:rxjs',
            'angular-in-memory-web-api': 'npm:angular-in-memory-web-api/bundles/in-memory-web-api.umd.js',
            'angular2-cookie': 'npm:angular2-cookie',
            'mydatepicker': 'npm:mydatepicker/bundles/mydatepicker.umd.min.js',
            'lodash': 'npm:lodash/lodash.min.js',
            'ng2-dropdown-treeview': 'npm:ng2-dropdown-treeview/bundles/ng2-dropdown-treeview.umd.js',
            'angular2-toaster': 'npm:angular2-toaster/bundles/angular2-toaster.umd.js'
        },
        // packages tells the System loader how to load when no filename and/or no extension
        packages: {
            app: {
                main: './AngularConfig/main.js',
                defaultExtension: 'js'
            },
            rxjs: {
                defaultExtension: 'js'
            },
            'angular2-cookie': {
                main: './core.js',
                defaultExtension: 'js'
            },
            'ng2-toastr': {
                defaultExtension: 'js'
            }
        }
    });
})(this);
