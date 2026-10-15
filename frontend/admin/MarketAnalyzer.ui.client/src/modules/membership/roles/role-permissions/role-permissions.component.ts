import { ChangeDetectorRef, Component, Input } from '@angular/core';
import { GlobalImports } from '@imports';
import { Roles } from '../../membership-models';
import { apiUrl, bind, buildUrl, getValue, setValue, toggleHeight } from '@utils';
import { HttpClient } from '@angular/common/http';
import { ApiResult } from '@typings';
import { CheckBoxChanged } from '@components/checkbox/checkbox.component';
import { GrowlService } from '@services/growl.service';

@Component({
  selector: 'role-permissions',
  imports: [...GlobalImports],
  templateUrl: './role-permissions.component.html',
  styleUrl: './role-permissions.component.scss'
})
export class RolePermissionsComponent {

  loading = false;
  permissions: Permission[] = [];

  constructor(
    private readonly growl: GrowlService,
    private readonly cdr: ChangeDetectorRef,
    private readonly httpClient: HttpClient) {

  }

  @Input()
  get role(): Roles | null {
    return getValue(this, 'role');
  }
  set role(value: Roles | null) {
    if (value === this.role) {
      return;
    }

    setValue(this, 'role', value);
    if (value) {
      this.loadPermissions(value);
    }
  }

  saveChanges(): void {
    const routes: number[] = [];
    this.permissions.forEach(controller => {
      controller.children.forEach(item => {
        if (item.authorized) {
          routes.push(item.routeId);
        }
      });
    });

    const data = {
      roleId: this.role!.id,
      routes: routes
    };

    this.httpClient
      .post<ApiResult>(apiUrl('Role/UpdatePermissions'), data)
      .subscribe(response => {
        if (response.successed) {
          this.growl.success({ content: 'ویرایش دسترسی‌ها با موفقیت انجام شد.' });
        }
      });
  }

  toggle(element: HTMLElement, permission: Permission): void {
    permission.expanded = !permission.expanded;
    this.cdr.markForCheck();
    toggleHeight(element);
  }

  permissionChanged(item: CheckBoxChanged<Permission>): void {
    item.value.authorized = item.checked;
  }

  private loadPermissions(role: Roles): void {
    this.loading = true;
    this.cdr.markForCheck();

    this.httpClient
      .get<ApiResult<Route[]>>(buildUrl(apiUrl('Role/Permissions'), { roleId: role.id }))
      .subscribe(response => {
        if (response.successed) {
          this.permissions = response.data.map(this.mapItem);
        }
        else {
          this.permissions = [];
        }

        this.loading = false;
        this.cdr.markForCheck();
      });
  }

  @bind()
  private mapItem(item: Route): Permission {
    return {
      routeId: item.routeId,
      routeName: item.routeName,
      authorized: item.authorized,
      children: item.children ? item.children.map(this.mapItem) : [],
      //Extendeds
      tracker: `${this.role!.id}:${item.routeId}:${item.routeName}`,
      expanded: false
    };
  }

}


interface Route {
  routeId: number;
  routeName: string;
  authorized: boolean;
  children: Route[];
}

interface Permission {
  tracker: string;
  routeId: number;
  routeName: string;
  authorized: boolean;
  children: Permission[];
  expanded: boolean;
}
