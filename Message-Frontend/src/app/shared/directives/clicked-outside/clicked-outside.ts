import { Directive, ElementRef, EventEmitter, HostListener, Output, output } from '@angular/core';

@Directive({
  selector: '[appClickedOutside]',
})
export class ClickedOutside {
  @Output() appClickedOutside = new EventEmitter();
  constructor(private elementRef: ElementRef) {}
  @HostListener('document:click', ['$event'])
  clickOutside(event: Event) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.appClickedOutside.emit();
    }
  }
}
