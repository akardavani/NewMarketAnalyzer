import { ApplicationRef, Inject, Injectable, Injector, StaticProvider, DOCUMENT } from '@angular/core';
import { ComponentPortal, DomPortalOutlet } from "@angular/cdk/portal";

import { ModalComponent } from '@components/modal/modal.component';

@Injectable({ providedIn: 'root' })
export class ModalService {

  private _options?: ModalOptions;
  private _outlet?: DomPortalOutlet;

  constructor(
    private readonly _injector: Injector,
    private readonly _appRef: ApplicationRef,
    @Inject(DOCUMENT) private readonly _document: Document) {

  }

  open(options: ModalOptions): void {
    options = this._options = Object.assign(<ModalOptions>{
      title: '',
      content: '',
      onConfirm: (_) => { },
      buttons: [
        { label: 'تایید', data: 'ok', cssClass: 'btn btn-primary' },
        { label: 'انصراف', data: 'cancel', cssClass: 'btn btn-dark' },
      ]
    }, options);

    this._outlet = this.createHostOutlet();
    const injector = options.providers ?
      Injector.create({ providers: options.providers, parent: this._injector })
      : this._injector;

    if (typeof (options.content) == 'string') {
      const ref = this._outlet.attach(new ComponentPortal(ModalComponent));
      ref.instance.title = options.title || '';
      ref.instance.content = options.content;
      ref.instance.buttons = options.buttons!;
      ref.instance.closeClicked.subscribe(() => this.close('close-clicked'));

      if (!options.denyBackdrop) {
        ref.instance.backdropClicked.subscribe(() => this.close('backdrop-clicked'));
      }

      if (options.onConfirm) {
        ref.instance.buttonClicked.subscribe(data => {
          if (options.onConfirm!(data)) {
            this.close();
          }
        });
      }
    }
    else {
      const ref = this._outlet.attach(new ComponentPortal(options.content, null, injector));
      if (options.onLoad) {
        options.onLoad(ref.instance);
      }
    }
  }

  close(reason?: string): void {
    if (this._options && this._options.onClose) {
      this._options.onClose(reason);
    }

    if (this._outlet) {
      this._outlet.detach();
      this._outlet.dispose();
    }

    this._outlet = undefined;
    this._options = undefined;
  }

  private createHostOutlet(): DomPortalOutlet {
    const pane = this._document.createElement('div');
    this._document.body.appendChild(pane);
    return new DomPortalOutlet(
      pane,
      this._appRef,
      this._injector
    );
  }
}

export interface ModalOptions {
  title?: string;
  content: any;
  buttons?: ModalButton[];
  denyBackdrop?: boolean;
  position?: ('top' | 'center' | 'bottom');
  onConfirm?: (data: any) => boolean;
  onClose?: (reason?: string) => void;
  providers?: StaticProvider[];
  onLoad?: (component: any) => any;
}

export interface ModalButton {
  label?: string;
  icon?: string;
  cssClass?: string;
  data: any;
}
