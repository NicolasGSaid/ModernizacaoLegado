import { Injectable } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { Observable, Subject } from 'rxjs';

export interface ConfirmDialogData {
  title: string;
  message: string;
  icon?: string;
  acceptLabel?: string;
  rejectLabel?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  constructor(private confirmationService: ConfirmationService) {}

  confirm(data: ConfirmDialogData): Observable<boolean> {
    const result$ = new Subject<boolean>();

    this.confirmationService.confirm({
      header: data.title,
      message: data.message,
      icon: data.icon || 'pi pi-exclamation-triangle',
      acceptLabel: data.acceptLabel || 'Confirmar',
      rejectLabel: data.rejectLabel || 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        result$.next(true);
        result$.complete();
      },
      reject: () => {
        result$.next(false);
        result$.complete();
      }
    });

    return result$.asObservable();
  }

  confirmDelete(itemName: string): Observable<boolean> {
    return this.confirm({
      title: 'Confirmar Exclusão',
      message: `Deseja realmente excluir "${itemName}"? Esta ação não pode ser desfeita.`,
      icon: 'pi pi-trash',
      acceptLabel: 'Excluir',
      rejectLabel: 'Cancelar'
    });
  }
}
