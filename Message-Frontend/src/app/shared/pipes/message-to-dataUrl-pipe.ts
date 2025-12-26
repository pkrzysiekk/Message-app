import { Pipe, PipeTransform } from '@angular/core';
import { Message } from '../../core/services/message/models/message';

@Pipe({
  name: 'messageToImagePipe',
})
export class MessageToDataUrlPipe implements PipeTransform {
  transform(message: Message): string {
    const srcPrefix = 'data:';
    const srcSuffix = ';base64,';
    return srcPrefix + message.type + srcSuffix + message.content;
  }
}
