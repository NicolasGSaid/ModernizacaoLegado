import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ocorreu um erro desconhecido';

      if (error.error instanceof ErrorEvent) {
        // Erro do lado do cliente
        errorMessage = `Erro: ${error.error.message}`;
      } else {
        // Erro do lado do servidor
        if (error.status === 0) {
          errorMessage = 'Não foi possível conectar ao servidor';
        } else if (error.status === 400) {
          errorMessage = error.error?.title || 'Dados inválidos';
        } else if (error.status === 404) {
          errorMessage = 'Recurso não encontrado';
        } else if (error.status === 500) {
          errorMessage = 'Erro interno do servidor';
        } else {
          errorMessage = `Erro ${error.status}: ${error.message}`;
        }
      }

      console.error('Erro HTTP:', errorMessage, error);
      return throwError(() => error);
    })
  );
};
