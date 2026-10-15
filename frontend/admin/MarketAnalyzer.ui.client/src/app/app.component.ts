import { Component, Renderer2 } from '@angular/core';
import { NavigationEnd, NavigationStart, Router, RouterModule } from '@angular/router';


@Component({
  selector: 'fd-root',
  template: '<router-outlet></router-outlet>',
  imports: [RouterModule]
})
export class AppComponent {

  title = 'MarketAnalyzer.ui.client';

  constructor(renderer: Renderer2, router: Router) {
    router.events
      .subscribe(event => {
        if (event instanceof NavigationStart) {
          renderer.removeClass(document.body, 'loaded');
        }
        else if (event instanceof NavigationEnd) {
          renderer.addClass(document.body, 'loaded');
        }
      });
  }

}
