import { Component, inject } from '@angular/core';
import { GlobalImports } from '@imports';
import { AppDataService } from '@services/app-data.service';
import { BrandService } from '@services/brand.service';

@Component({
  selector: 'fd-dashboard',
  imports: [...GlobalImports],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  protected readonly brand = inject(BrandService);
  protected readonly appData = inject(AppDataService);
}
