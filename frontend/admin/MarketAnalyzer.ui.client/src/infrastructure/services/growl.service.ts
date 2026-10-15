
import { ApplicationRef, ComponentRef, Inject, Injectable, Injector, DOCUMENT } from '@angular/core';
import { ComponentPortal, DomPortalOutlet } from "@angular/cdk/portal";
import { GrowlComponent } from '@components/growl/growl.component';



@Injectable({ providedIn: 'root' })
export class GrowlService {

  private readonly _growls: ComponentRef<GrowlComponent>[] = [];

  constructor(
    private readonly _injector: Injector,
    private readonly _appRef: ApplicationRef,
    @Inject(DOCUMENT) private readonly _document: Document) {

  }

  success(options: GrowlOptions): void {
    options = { ...options, cssClass: 'growl-success' };
    this.show(options);
  }

  info(options: GrowlOptions): void {
    options = { ...options, cssClass: 'growl-info' };
    this.show(options);
  }

  warning(options: GrowlOptions): void {
    options = { ...options, cssClass: 'growl-warning' };
    this.show(options);
  }

  error(options: GrowlOptions): void {
    options = { ...options, cssClass: 'growl-error' };
    this.show(options);
  }

  show(options: GrowlOptions): void {
    const outlet = new DomPortalOutlet(
      document.body,
      this._appRef,
      this._injector
    );

    const ref = outlet.attach(new ComponentPortal(GrowlComponent));

    ref.setInput('icon', options.icon);
    ref.setInput('title', options.title);
    ref.setInput('content', options.content);
    ref.setInput('cssClass', options.cssClass);
    ref.setInput('timeout', options.timeout || 10000);
    ref.setInput('position', options.position || 'right');

    ref.instance.closed.subscribe(() => {
      ref.destroy();
      const index = this._growls.indexOf(ref);
      this._growls.splice(index, 1);
      requestAnimationFrame(this.adjustGrowls.bind(this));
    });

    this._growls.push(ref);
    requestAnimationFrame(this.adjustGrowls.bind(this));
  }

  private adjustGrowls(): void {
    let offset = 5;
    this._growls.forEach(growl => {
      const element = growl.instance.host.nativeElement;
      element.style.bottom = `${offset}px`;
      offset += element.offsetHeight + 5;
    })
  }
}

export interface GrowlOptions {
  title?: string;
  content: string;
  icon?: string;
  position?: 'right' | 'left';
  timeout?: number;
  cssClass?: string;
}
