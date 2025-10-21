import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

/**
 * Floating Action Button (FAB) Component
 * Componente reutilizável para botões de ação flutuantes
 * 
 * @example
 * <app-fab-button 
 *   icon="add" 
 *   tooltip="Criar novo" 
 *   (onClick)="handleCreate()">
 * </app-fab-button>
 */
@Component({
  selector: 'app-fab-button',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule
  ],
  template: `
    <button 
      mat-fab 
      color="primary"
      class="fab-create"
      [matTooltip]="tooltip"
      matTooltipPosition="left"
      (click)="onClick.emit()"
      [disabled]="disabled">
      <mat-icon>{{ icon }}</mat-icon>
    </button>
  `,
  styles: [`
    .fab-create {
      position: fixed;
      bottom: 100px;
      right: 24px;
      z-index: 1000;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }
    
    .fab-create:hover:not(:disabled) {
      transform: scale(1.1);
      box-shadow: 0 6px 30px rgba(0, 0, 0, 0.25);
    }
    
    .fab-create:active:not(:disabled) {
      transform: scale(1.05);
    }
    
    /* Responsividade mobile */
    @media (max-width: 768px) {
      .fab-create {
        bottom: 90px;
        right: 16px;
      }
    }
  `]
})
export class FabButtonComponent {
  /** Ícone do Material Icons a ser exibido */
  @Input() icon = 'add';
  
  /** Texto do tooltip */
  @Input() tooltip = 'Criar novo';
  
  /** Estado desabilitado */
  @Input() disabled = false;
  
  /** Evento emitido ao clicar no botão */
  @Output() onClick = new EventEmitter<void>();
}
