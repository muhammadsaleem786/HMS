import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

if (environment.production) {
    enableProdMode();
}

platformBrowserDynamic().bootstrapModule(AppModule)
    .catch(err => console.log(err));

//(window as any).trackEvent = function (eventName: string, params: any = {}): void {
//    debugger;
//    if (params.email && typeof params.email === 'string') {
//        if (!params.email.endsWith('@gmail.com')) {
//            console.warn('Email is not from the @gmail.com domain');
//        }
//    }
//    if (window.gtag) {
//        window.gtag('event', eventName, params);
//    } else {
//        console.warn('Google Analytics is not initialized');
//    }
//};

(window as any).trackEvent = function (eventName: string, params: any = {}): void {
    debugger;

    // Define internal domains
    const internalDomains = ['gmail.com']; // Replace with your actual internal domains

    // Determine if the user is internal or external
    let userType = 'unknown';
    if (params.email && typeof params.email === 'string') {
        const emailDomain = params.email.split('@')[1]; // Extract the domain from email
        userType = internalDomains.includes(emailDomain) ? 'internal' : 'external';
    }

    // Add userType to event parameters
    params.user_type = userType;

    // Send event to Google Analytics
    if (window.gtag) {
        window.gtag('event', eventName, params);
    } else {
        console.warn('Google Analytics is not initialized');
    }
};


