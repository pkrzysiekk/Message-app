import { MessageToDataUrlPipe } from './message-to-dataUrl-pipe';

describe('MessageToImagePipePipe', () => {
  it('create an instance', () => {
    const pipe = new MessageToDataUrlPipe();
    expect(pipe).toBeTruthy();
  });
});
