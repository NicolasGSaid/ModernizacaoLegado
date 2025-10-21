import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';
import { ApiService } from '../../../core/services/api.service';
import { PaginatedResult } from '../../../core/models/paginated-result.model';
import { 
  OrdemServico, 
  CreateOrdemServicoDto, 
  UpdateOrdemServicoDto,
  OrdemServicoStatus 
} from '../models/ordem-servico.model';

@Injectable({
  providedIn: 'root'
})
export class OrdemServicoService {
  private apiService = inject(ApiService);
  private readonly endpoint = 'ordemservico';

  /**
   * Busca todas as ordens de serviço com paginação
   */
  getAll(page: number = 1, pageSize: number = 10, filtro?: string): Observable<PaginatedResult<OrdemServico>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (filtro) {
      params = params.set('filtro', filtro);
    }

    return this.apiService.get<PaginatedResult<OrdemServico>>(this.endpoint, params);
  }

  /**
   * Busca uma ordem de serviço por ID
   */
  getById(id: number): Observable<OrdemServico> {
    return this.apiService.get<OrdemServico>(`${this.endpoint}/${id}`);
  }

  /**
   * Cria uma nova ordem de serviço
   */
  create(dto: CreateOrdemServicoDto): Observable<OrdemServico> {
    return this.apiService.post<OrdemServico>(this.endpoint, dto);
  }

  /**
   * Atualiza uma ordem de serviço existente
   */
  update(id: number, dto: UpdateOrdemServicoDto): Observable<OrdemServico> {
    // Backend .NET 8 CQRS espera o objeto completo com todos os campos
    const updateCommand = {
      id: id,
      titulo: dto.titulo,
      descricao: dto.descricao,
      tecnico: dto.tecnico,
      status: dto.status
    };
    return this.apiService.put<OrdemServico>(`${this.endpoint}/${id}`, updateCommand);
  }

  /**
   * Atualiza apenas o status de uma ordem de serviço
   */
  updateStatus(id: number, status: OrdemServicoStatus): Observable<OrdemServico> {
    return this.apiService.patch<OrdemServico>(`${this.endpoint}/${id}/status`, { status });
  }

  /**
   * Exclui uma ordem de serviço
   */
  delete(id: number): Observable<void> {
    return this.apiService.delete<void>(`${this.endpoint}/${id}`);
  }
}
