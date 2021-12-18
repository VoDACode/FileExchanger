import * as CryptoJS from "crypto-js";

export class AES{
  public static Encrypt(file: any, pin: string, callback: (file: File) => void): void{
    const reader = new FileReader();
    // const iv = CryptoJS.lib.WordArray.random(64/8);

    reader.onload = function (evt:ProgressEvent<FileReader>) {
      // @ts-ignore
      let encrypted = CryptoJS.AES.encrypt(evt.target.result, pin);
      // @ts-ignore
      let encryptedFile = new File([encrypted], file.name, {type: file.type, lastModified: file.lastModified});
      callback(encryptedFile);
    }
    reader.readAsDataURL(file);
  }

  public static Decrypt(file: File, pin: string, callback: (file: File) => void): void{
    let reader = new FileReader();
    reader.onload = (evt) => {
      // @ts-ignore
      let decrypted = CryptoJS.AES.decrypt(evt.target.result, pin);               // Decryption: I: Base64 encoded string (OpenSSL-format) -> O: WordArray
      let typedArray = this.convertWordArrayToBlob(decrypted, file.type);               // Convert: WordArray -> typed array
      // @ts-ignore
      let decryptFile = new File([typedArray], file.name, {type: file.type, lastModified: file.lastModified});
      callback(decryptFile);
    };
    reader.readAsBinaryString(file);
  }

  private static convertWordArrayToBlob(wordArray: CryptoJS.lib.WordArray, contentType: string) {
    let decoder = new TextDecoder('utf8');
    let arrayOfWords = wordArray.hasOwnProperty("words") ? wordArray.words : [];
    let length = wordArray.hasOwnProperty("sigBytes") ? wordArray.sigBytes : arrayOfWords.length * 4;
    let uInt8Array = new Uint8Array(length), index=0, word, i;
    for (i=0; i<length; i++) {
      word = arrayOfWords[i];
      uInt8Array[index++] = word >> 24;
      uInt8Array[index++] = (word >> 16) & 0xff;
      uInt8Array[index++] = (word >> 8) & 0xff;
      uInt8Array[index++] = word & 0xff;
    }

    let base64 = decoder.decode(uInt8Array).split(',')[1];
    const byteCharacters = atob(base64);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    return new Blob([new Uint8Array(byteNumbers)], {type: contentType});
  }
}
