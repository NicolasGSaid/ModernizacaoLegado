import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';
import { ApiService } from '../../../core/services/api.service';
import { PaginatedResult } from '../../../core/models/paginated-result.model';
import { Tecnico, CreateTecnicoDto, UpdateTecnicoDto, TecnicoStatus } from '../models/tecnico.model';

@Injectable({
  providedIn: 'root'
})
export class TecnicoService {
  private apiService = inject(ApiService);
  private readonly endpoint = 'tecnico';

  /**
   * Busca todos os técnicos com paginação
   */
  getAll(page: number = 1, pageSize: number = 10, filtro?: string): Observable<PaginatedResult<Tecnico>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filtro) {
      params = params.set('filtro', filtro);
    }

    return this.apiService.get<PaginatedResult<Tecnico>>(this.endpoint, params);
  }

  /**
   * Busca um técnico por ID
   */
  getById(id: number): Observable<Tecnico> {
    return this.apiService.get<Tecnico>(`${this.endpoint}/${id}`);
  }

  /**
   * Cria um novo técnico
   */
  create(dto: CreateTecnicoDto): Observable<Tecnico> {
    return this.apiService.post<Tecnico>(this.endpoint, dto);
  }

  /**
   * Atualiza um técnico existente
   */
  update(id: number, dto: UpdateTecnicoDto): Observable<Tecnico> {
    return this.apiService.put<Tecnico>(`${this.endpoint}/${id}`, dto);
  }

  /**
   * Atualiza apenas o status de um técnico
   */
  updateStatus(id: number, status: TecnicoStatus): Observable<Tecnico> {
    return this.apiService.patch<Tecnico>(`${this.endpoint}/${id}/status`, { status });
  }

  /**
   * Exclui um técnico
   */
  delete(id: number): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}
