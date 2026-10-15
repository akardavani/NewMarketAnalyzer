import { ChangeDetectorRef, Component, Directive, Input, signal } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, debounceTime, distinctUntilChanged, fromEvent, Observable } from 'rxjs';
import { toggleHeight } from '@utils';

@Component({
  selector: 'fd-tree-view',
  imports: [],
  templateUrl: './tree-view.component.html',
  styleUrl: './tree-view.component.scss'
})
export class TreeViewComponent {

  keyword = new BehaviorSubject('');
  nodes: ReadonlyArray<TreeNode> = [];

  constructor(private readonly _router: Router, private readonly _cdr: ChangeDetectorRef) {

  }

  @Input('nodes')
  set items(values: ReadonlyArray<ITreeNode> | undefined | null) {
    this.nodes = !values ? [] : values.map(e => new TreeNode(e, this.keyword));
  }

  @Input()
  set searchInput(input: HTMLInputElement | undefined) {
    if (input) {
      fromEvent(input, 'input')
        .pipe(
          debounceTime(150),
          distinctUntilChanged()
        )
        .subscribe(() => {
          this.keyword.next(input.value);
          this._cdr.markForCheck();
        });
    }
  }

  navigate(event: Event, node: TreeNode): void {
    event.preventDefault();

    if (node.expandable) {
      node.expanded.set(!node.expanded());
      const target = (event.target as HTMLElement).closest('a');
      if (target) {
        const submenu = target.nextElementSibling as HTMLElement;
        if (!submenu) { return; }
        toggleHeight(submenu);
      }
    }
    else if (node.path) {
      this._router.navigateByUrl(node.path);
    }
  }

}

export interface ITreeNode {
  label: string;
  path?: string;
  icon?: string;
  children?: ITreeNode[];
}

@Directive()
class TreeNode {

  readonly label: string;
  readonly path?: string;
  readonly children?: TreeNode[];
  readonly icon?: string;
  readonly expandable: boolean;
  expanded = signal(false);
  filtered = false;

  //filtered = computed((): boolean => {
  //  let value: boolean;
  //  const keyword = this._keyword();

  //  if (!keyword) {
  //    value = false;
  //  }

  //  if (this.expandable) {
  //    value = !this.label.toLowerCase().includes(keyword.toLowerCase()) && this.children!.every(e => e.filtered());
  //    if (value) {
  //      this.expanded.set(true);
  //    }
  //  }
  //  else {
  //    value = !this.label.includes(keyword);
  //  }

  //  return value;
  //});

  constructor(node: ITreeNode, keyword: Observable<string>) {
    this.label = node.label;
    this.path = node.path;
    this.icon = node.icon;
    this.children = node.children && node.children.map(e => new TreeNode(e, keyword));
    this.expandable = !!node.children && node.children.length > 0;

    keyword.subscribe(value => {
      if (!value) {
        this.filtered = false;
      }

      if (this.expandable) {
        const filtered = !this.label.toLowerCase().includes(value.toLowerCase()) && this.children!.every(e => e.filtered);
        this.filtered = filtered;
        //if (filtered) {
        //  this.expanded.set(true);
        //}
      }
      else {
        const filtered = !this.label.includes(value);
        this.filtered = filtered;
      }

      //console.log(`${this.label}: ${this.filtered}`);
    });
  }

  get active(): boolean {
    return trimStartSlash(this.path) == trimStartSlash(document.location.pathname);
  }

}

function trimStartSlash(value: string | undefined): string | undefined {
  return (value && value.charAt(0) == '/') ?
    value.substring(1) :
    value;
}
