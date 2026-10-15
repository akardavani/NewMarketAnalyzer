import { Component, Type } from '@angular/core';
import { CrudGrid } from '@directives/crud-grid';
import { Roles, RolesFilter } from '../../membership-models';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ColDef, GridReadyEvent, ICellRendererParams } from 'ag-grid-enterprise';
import { RolesService } from './roles.service';
import { EditRoleComponent } from '../edit-role/edit-role.component';
import { CreateRoleComponent } from '../create-role/create-role.component';
import { apiUrl, bind } from '@utils';
import { defineActionCellParams, firstCellValueGetter, formatCurrency, IActionCommand } from '@grid-utils';
import { CheckedCellRendererComponent } from '@components/checked-cell-renderer/checked-cell-renderer.component';
import { ActionCellRendererComponent } from '@components/action-cell-renderer/action-cell-renderer.component';
import { GridImports } from '@imports';
import { Nullable } from '@typings';
import { RolePermissionsComponent } from '../role-permissions/role-permissions.component';

@Component({
  selector: 'fd-roles',
  imports: [...GridImports, RolePermissionsComponent],
  providers: [RolesService],
  templateUrl: './roles.component.html',
  styleUrl: './roles.component.scss'
})
export class RolesComponent extends CrudGrid<Roles, RolesFilter> {

  protected override deleteUrl: string = apiUrl('Role/Remove');
  protected override editComponent: Type<any> = EditRoleComponent;
  protected override createComponent: Type<any> = CreateRoleComponent;

  override columnDefs: ColDef<Roles, any>[] = [
    { headerName: "ردیف", valueGetter: firstCellValueGetter(), sortable: false },
    { headerName: 'نام', field: 'name' },
    { headerName: 'نام نمایشی', field: 'displayName' },
    { headerName: 'فعال', field: 'isActive', cellRenderer: CheckedCellRendererComponent },
    { headerName: 'قابل ویرایش', field: 'editable', cellRenderer: CheckedCellRendererComponent },
    { headerName: 'تعداد کاربران', field: 'usersCount', valueFormatter: formatCurrency },
    {
      headerName: 'عملیات',
      sortable: false,
      cellRenderer: ActionCellRendererComponent,
      cellRendererParams: defineActionCellParams(this.createActionCommands)
    }
  ];

  selectedRole: Nullable<Roles> = null;

  constructor(service: RolesService) {
    super(service);
  }

  override onGridReady(event: GridReadyEvent<Roles>): void {
    super.onGridReady(event);
    event.api.setGridOption('onRowClicked', (event) => {
      if (event.data != this.selectedRole) {
        this.selectedRole = event.data!;
        this.cdr.markForCheck();
      }
    });
  }

  protected override buildFilter(builder: FormBuilder): FormGroup {
    return builder.group({
      name: [null],
      displayName: [null],
      isActive: [null]
    });
  }

  @bind()
  private createActionCommands(params: ICellRendererParams<Roles>): IActionCommand[] {
    const actions: IActionCommand[] = !params.data?.editable ?
      [] :
      [
        {
          tooltip: 'ویرایش',
          icon: 'pa pa-edit',
          action: this.edit
        },
        {
          tooltip: 'حذف',
          icon: 'pa pa-delete',
          action: this.delete
        },
      ];

    return actions;
  }

}

interface RoleRouteNode {
  authorized: boolean;
  children: RoleRouteNode[];
  routeId: number;
  routeName: string;
  expanded?: boolean;
}
