import { Injectable } from "@angular/core";
import * as CryptoTS from 'crypto-ts';

@Injectable({
    providedIn: "root"
})
export class EncryptionService {
    private readonly AES_KEY_VERSION = 'v2';
    private readonly AES_SECRET_KEY = 'secretkey@321';
    key: string = "z!!!!!!!1sdfadsf56adf456asdfasdf";
    appProperties = {
        VALUES: {
            KEY: "MTIzNDU2Nzg5MEFCQ0RFRkdISUpLTE1O",
            IV: "MTIzNDU2Nzg="
        }
    }

    constructor() { }

    encryptionAES(msg: string): string {
        if (!msg) return '';

        return this.AES_KEY_VERSION + ':' +
            CryptoTS.AES.encrypt(msg, this.AES_SECRET_KEY).toString();
    }

    //encryptionAES(msg) {
    //    // Encrypt
    //    if (msg != null) {
    //        const ciphertext = CryptoTS.AES.encrypt(msg, 'secretkey@321');
    //        return ciphertext.toString();
    //    } else
    //        return "";
    //}
    decryptionAES(msg: string): string {
        try {
            if (!msg) return '';

            // 🔥 OLD DATA DETECTED → FORCE LOGOUT
            if (!msg.startsWith(this.AES_KEY_VERSION + ':')) {
                alert('fail');
                this.clearAuthStorage();
                return '';
            }

            const encrypted = msg.substring(this.AES_KEY_VERSION.length + 1);
            const bytes = CryptoTS.AES.decrypt(encrypted, this.AES_SECRET_KEY);
            return bytes.toString(CryptoTS.enc.Utf8) || '';
        } catch {
            alert('fail');
            this.clearAuthStorage();
            return '';
        }
    }

    //decryptionAES(msg) {
    //    localStorage.clear();
    //    localStorage.removeItem("currentUser");
    //    localStorage.removeItem("company");
    //    localStorage.removeItem("Token");
    //    // Decrypt
    //    try {
    //        if (!msg) return "";

    //        const bytes = CryptoTS.AES.decrypt(msg, 'secretkey@321');
    //        const plaintext = bytes.toString(CryptoTS.enc.Utf8);

    //        if (!plaintext) return "";

    //        return plaintext;
    //    }
    //    catch (e) {
    //        console.error('AES Decryption failed:', e);
    //        return "";
    //    }

    //    }
    clearAuthStorage() {
        localStorage.removeItem('currentUser');
        localStorage.removeItem('company');
        localStorage.removeItem('Token');
        window.location.href = '/login'; // force re-login
    }
}
