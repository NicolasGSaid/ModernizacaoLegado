import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

export interface BreadcrumbItem {
  label: string;
  url?: string;
  icon?: string;
}

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="breadcrumb-container" *ngIf="items.length > 0">
      <ol class="breadcrumb">
        <li class="breadcrumb-item">
          <a [routerLink]="['/']" class="breadcrumb-link">
            <i class="pi pi-home"></i>
            <span>Início</span>
          </a>
        </li>
        <li *ngFor="let item of items; let last = last" 
            class="breadcrumb-item"
            [class.active]="last">
          <i class="pi pi-angle-right separator"></i>
          <a *ngIf="!last && item.url" 
             [routerLink]="item.url" 
             class="breadcrumb-link">
            <i *ngIf="item.icon" [class]="'pi pi-' + item.icon"></i>
            <span>{{ item.label }}</span>
          </a>
          <span *ngIf="last" class="breadcrumb-current">
            <i *ngIf="item.icon" [class]="'pi pi-' + item.icon"></i>
            <span>{{ item.label }}</span>
          </span>
        </li>
      </ol>
    </nav>
  `,
  styles: [`
    .breadcrumb-container {
      padding: 1rem 2rem;
      background: linear-gradient(135deg, #667eea08 0%, #764ba208 100%);
      border-bottom: 1px solid #f0f0f0;
      animation: slideDown 0.3s ease-out;
    }

    @keyframes slideDown {
      from {
        opacity: 0;
        transform: translateY(-10px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }

    .breadcrumb {
      display: flex;
      align-items: center;
      list-style: none;
      margin: 0;
      padding: 0;
      max-width: 1400px;
      margin: 0 auto;
    }

    .breadcrumb-item {
      display: flex;
      align-items: center;
      font-size: 0.875rem;
    }

    .separator {
      margin: 0 0.5rem;
      color: #9e9e9e;
      font-size: 0.75rem;
    }

    .breadcrumb-link {
      display: flex;
      align-items: center;
      gap: 0.375rem;
      color: #667eea;
      text-decoration: none;
      padding: 0.25rem 0.5rem;
      border-radius: 6px;
      transition: all 0.2s ease;
    }

    .breadcrumb-link:hover {
      background-color: rgba(102, 126, 234, 0.1);
      transform: translateX(2px);
    }

    .breadcrumb-current {
      display: flex;
      align-items: center;
      gap: 0.375rem;
      color: #495057;
      font-weight: 500;
    }

    .breadcrumb-item.active {
      color: #495057;
    }

    @media (max-width: 768px) {
      .breadcrumb-container {
        padding: 0.75rem 1rem;
      }

      .breadcrumb {
        font-size: 0.813rem;
      }

      .breadcrumb-link span,
      .breadcrumb-current span {
        max-width: 100px;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }
    }
  `]
})
export class BreadcrumbComponent {
  @Input() items: BreadcrumbItem[] = [];
}
