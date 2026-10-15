import {
  ApplicationRef,
  EventEmitter,
  Inject,
  Injectable,
  Injector,
  Renderer2,
  RendererFactory2,
  DOCUMENT,
} from '@angular/core';
import {
  ComponentPortal,
  ComponentType,
  DomPortalOutlet,
} from '@angular/cdk/portal';
import { bind, getValue, setValue } from '@utils';

@Injectable({ providedIn: 'root' })
export class ComponentHostFactory {
  private readonly _context: ComponentHostFactoryContext;

  constructor(
    rendererFactory: RendererFactory2,
    injector: Injector,
    appRef: ApplicationRef,
    @Inject(DOCUMENT) document: Document,
  ) {
    this._context = {
      appRef: appRef,
      document: document,
      injector: injector,
      renderer: rendererFactory.createRenderer(null, null),
    };
  }

  create<TComponent>(
    options: ComponentHostOptions<TComponent>,
  ): ComponentHost<TComponent> {
    return new ComponentHost(this._context, options);
  }
}

export class ComponentHost<TComponent> {
  readonly component: TComponent;
  readonly container: HTMLDivElement;
  readonly deactivated = new EventEmitter<void>();

  private readonly _outlet: DomPortalOutlet;

  constructor(
    private readonly _context: ComponentHostFactoryContext,
    private readonly _options: ComponentHostOptions<TComponent>,
  ) {
    this.container = _context.document.createElement('div');
    _context.renderer.addClass(this.container, 'dom-outlet-container');
    if (_options.containerCssClass) {
      _context.renderer.addClass(this.container, _options.containerCssClass);
    }
    const dialogPane = _context.document.querySelector(
      '.cdk-overlay-pane.fd-dialog',
    );

    if (dialogPane) {
      dialogPane.appendChild(this.container);
    } else {
      const dialogPane = _context.document.querySelector(
        '.cdk-overlay-pane.fd-dialog',
      );

      if (dialogPane) {
        dialogPane.appendChild(this.container);
      } else {
        const dialogPane = _context.document.querySelector(
          '.cdk-overlay-pane.fd-dialog',
        );

        if (dialogPane) {
          dialogPane.appendChild(this.container);
        } else {
          _context.document.body.appendChild(this.container);
        }
      }
    }

    //this._outlet = new DomPortalOutlet(
    //  this.container,
    //  undefined,
    //  _context.appRef,
    //  _context.injector,
    //  _context.document
    //);

    this._outlet = new DomPortalOutlet(
      this.container,
      _context.appRef,
      _context.injector,
    );

    this.component = this._outlet.attach(
      new ComponentPortal(_options.componentType),
    ).instance;

    if (_options.configure) {
      _options.configure(this.component);
    }
  }

  get isActive(): boolean {
    return getValue(this, 'isActive');
  }

  show(): void {
    setValue(this, 'isActive', true);

    requestAnimationFrame(() => {
      const rect = this._options.target.getBoundingClientRect();

      const display = this.container.style.display;
      const visibility = this.container.style.visibility;
      this.container.style.maxHeight = '';
      this.container.style.display = 'block';
      this.container.style.visibility = 'hidden';

      const spaceAbove = rect.top;
      const viewportHeight = window.innerHeight;
      const spaceBelow = viewportHeight - rect.bottom;
      const maxHeight = Math.max(spaceAbove, spaceBelow) - 10;
      const containerHeight = this.container.scrollHeight;

      let top: number;
      if (containerHeight <= spaceBelow || spaceBelow > spaceAbove) {
        top = rect.bottom + window.scrollY;
      } else {
        top = rect.top + window.scrollY - maxHeight;
      }

      this.container.style.top = `${top}px`;
      this.container.style.overflowY = 'auto';
      this.container.style.right = `${window.innerWidth - rect.right}px`;

      if (containerHeight > maxHeight) {
        this.container.style.maxHeight = `${maxHeight}px`;
      } else {
        this.container.style.maxHeight = '';
      }

      if (!this._options.suppressMinWidth) {
        this.container.style.minWidth = `${rect.width}px`;
      }

      this.container.style.display = display;
      this.container.style.visibility = visibility;
      this._context.renderer.addClass(this.container, 'active');

      if (!this._options.disableEscape) {
        document.addEventListener('keydown', this.handleEscape);
      }
    });
  }

  hide(): void {
    setValue(this, 'isActive', false);
    this._context.renderer.removeClass(this.container, 'active');

    if (!this._options.disableEscape) {
      document.removeEventListener('keydown', this.handleEscape);
    }
  }

  attachHandler<K extends keyof DocumentEventMap>(
    type: K,
    listener: DocumentEventListener<K>,
  ): void {
    this._context.document.addEventListener(type, listener);
  }

  detachHandler<K extends keyof DocumentEventMap>(
    type: K,
    listener: DocumentEventListener<K>,
  ): void {
    this._context.document.removeEventListener(type, listener);
  }

  destroy(): void {
    this._outlet.detach();
    this._outlet.dispose();
  }

  @bind()
  private handleEscape(event: KeyboardEvent): void {
    if (event.key == 'Escape') {
      this.hide();
      this.deactivated.emit();
    }
  }
}

interface ComponentHostFactoryContext {
  readonly document: Document;
  readonly injector: Injector;
  readonly renderer: Renderer2;
  readonly appRef: ApplicationRef;
}

type DocumentEventListener<K extends keyof DocumentEventMap> = (
  this: Document,
  ev: DocumentEventMap[K],
) => any;

export interface ComponentHostOptions<TComponent> {
  target: HTMLElement;
  disableEscape?: boolean;
  containerCssClass?: string;
  suppressMinWidth?: boolean;
  componentType: ComponentType<TComponent>;

  configure?: (component: TComponent) => void;
}
