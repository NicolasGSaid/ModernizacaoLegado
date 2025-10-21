import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';
import { ApiService } from '../../../core/services/api.service';
import { PaginatedResult } from '../../../core/models/paginated-result.model';
import { Cliente, CreateClienteDto, UpdateClienteDto } from '../models/cliente.model';

@Injectable({
  providedIn: 'root'
})
export class ClienteService {
  private apiService = inject(ApiService);
  private readonly endpoint = 'cliente';

  /**
   * Busca todos os clientes com paginação
   */
  getAll(page: number = 1, pageSize: number = 10, busca?: string): Observable<PaginatedResult<Cliente>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (busca) {
      params = params.set('busca', busca);
    }

    return this.apiService.get<PaginatedResult<Cliente>>(this.endpoint, params);
  }

  /**
   * Busca um cliente por ID
   */
  getById(id: number): Observable<Cliente> {
    return this.apiService.get<Cliente>(`${this.endpoint}/${id}`);
  }

  /**
   * Cria um novo cliente
   */
  create(dto: CreateClienteDto): Observable<Cliente> {
    return this.apiService.post<Cliente>(this.endpoint, dto);
  }

  /**
   * Atualiza um cliente existente
   */
  update(id: number, dto: UpdateClienteDto): Observable<Cliente> {
    return this.apiService.put<Cliente>(`${this.endpoint}/${id}`, dto);
  }

  /**
   * Exclui um cliente
   */
  delete(id: number): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}
