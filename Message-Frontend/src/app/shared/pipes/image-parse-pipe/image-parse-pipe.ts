import { Pipe, PipeTransform } from '@angular/core';
import { Image } from '../../../core/models/image';

@Pipe({
  name: 'imageParse',
})
export class ImageParsePipe implements PipeTransform {
  transform(image?: Image): string | null {
    const srcPrefix = 'data:';
    const srcSuffix = ';base64,';
    if (!image) return null;
    return srcPrefix + image.contentType + srcSuffix + image.content;
  }
}
