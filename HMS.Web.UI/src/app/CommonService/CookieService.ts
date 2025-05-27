import { Injectable } from '@angular/core';

@Injectable()
export class CookieService {


    setCookie(Name: string, value: any, exp_hours: number): any {

        var dt = new Date();
        dt.setTime(dt.getTime() + (exp_hours * 3600000)); // Convert days to ms
        document.cookie = Name + "=" + value + "; expires=" + dt.toUTCString(); // add attrib=value to the cookie and set an expiration date
    }

    getCookie(Name: string): any {

        var split_cookie = document.cookie.split("; ");
        Name += "=";
        for (var i = 0; i < split_cookie.length; i++)
            if (~split_cookie[i].indexOf(Name)) // if the attribute is somewhere in the cookie string
                // im using an ugly bitflip operator here, it could as well be an if(...!=-1)
                return split_cookie[i].substring(Name.length + split_cookie[i].indexOf(Name), split_cookie[i].length);
        return "";

    }

    removeCookie(Name: string): any {
        this.setCookie(Name, "", -1);
    }
}