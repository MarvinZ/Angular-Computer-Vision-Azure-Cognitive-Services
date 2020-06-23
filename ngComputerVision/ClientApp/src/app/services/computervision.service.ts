import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ComputervisionService {

  baseURLImage: string;
  baseURLurl: string;

  constructor(private http: HttpClient) {
    this.baseURLImage  = '/api/OCR';
    this.baseURLurl = '/api/Image';
  }

  getAvailableLanguage() {
    return this.http.get(this.baseURLImage)
      .pipe(response => {
        return response;
      });
  }

  getTextFromImage(image: FormData) {
    return this.http.post(this.baseURLImage, image)
      .pipe(response => {
        return response;
      });
  }

  getTextFromImageUrl(imageUrl: String) {
    var ooo = {
      'filename': imageUrl,
      'something':'algo'
    }
    return this.http.post(this.baseURLurl, ooo)
      .pipe(response => {
        return response;
      });
  }
}
