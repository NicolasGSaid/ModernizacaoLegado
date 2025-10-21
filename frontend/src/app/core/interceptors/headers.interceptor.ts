import { HttpInterceptorFn } from '@angular/common/http';

/**
 * Interceptor que garante que todas as requisições HTTP tenham os headers corretos
 * Especialmente importante para garantir Content-Type: application/json
 */
export const headersInterceptor: HttpInterceptorFn = (req, next) => {
  console.log('🔧 Headers Interceptor EXECUTADO');
  console.log('   URL:', req.url);
  console.log('   Method:', req.method);
  console.log('   Body:', req.body);
  console.log('   Headers antes:', req.headers.get('Content-Type'));
  
  // FORÇA Content-Type: application/json para todas as requisições com body
  if (req.body) {
    const clonedReq = req.clone({
      setHeaders: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      }
    });
    console.log('   Headers depois:', clonedReq.headers.get('Content-Type'));
    console.log('   Body serializado:', JSON.stringify(req.body));
    return next(clonedReq);
  }

  return next(req);
};
