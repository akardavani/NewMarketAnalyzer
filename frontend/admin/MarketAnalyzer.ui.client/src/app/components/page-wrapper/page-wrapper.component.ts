import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { CdkPortalOutletAttachedRef, Portal } from '@angular/cdk/portal';
import { ScrollbarComponent } from '@components/scrollbar/scrollbar.component';
import { ActivatedRoute, TitleStrategy } from '@angular/router';
import { PortalModule } from '@angular/cdk/portal';

@Component({
  selector: 'fd-page-wrapper',
  imports: [ScrollbarComponent, PortalModule],
  templateUrl: './page-wrapper.component.html',
  styleUrl: './page-wrapper.component.scss',
  encapsulation: ViewEncapsulation.None
})
export class PageWrapperComponent {

  readonly pageTitle: string;

  @Input()
  hasSidebar: boolean = true;

  @Input()
  showSidebar: boolean = false;

  @Input()
  portalContent: Portal<any> | null | undefined;

  @Input()
  showTitle: boolean = true;

  @Output()
  readonly portalAttached = new EventEmitter<CdkPortalOutletAttachedRef>();

  constructor(route: ActivatedRoute, st: TitleStrategy) {
    this.pageTitle = st.getResolvedTitleForRoute(route.snapshot);
  }

}
