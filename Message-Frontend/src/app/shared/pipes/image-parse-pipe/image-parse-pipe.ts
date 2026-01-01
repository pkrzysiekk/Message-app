import { Pipe, PipeTransform } from '@angular/core';
import { Image } from '../../../core/models/image';
@Pipe({
  name: 'imageParse',
})
export class ImageParsePipe implements PipeTransform {
  transform(image?: Image): string | null {
    const srcPrefix = 'data:';
    const srcSuffix = ';base64,';
    const placeholderImage = 'pic_placeholder.png';
    if (!image) return placeholderImage;
    return srcPrefix + image.contentType + srcSuffix + image.content;
  }
}
