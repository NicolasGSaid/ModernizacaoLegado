import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
// PrimeNG Imports
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { OrdemServicoService } from '../../services/ordem-servico.service';
import { TecnicoService } from '../../../tecnico/services/tecnico.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { OrdemServicoStatus } from '../../models/ordem-servico.model';
import { Tecnico } from '../../../tecnico/models/tecnico.model';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-ordem-servico-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    // Material (manter temporariamente)
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatAutocompleteModule,
    // PrimeNG
    InputTextModule,
    InputTextareaModule,
    DropdownModule,
    ButtonModule,
    CardModule
  ],
  templateUrl: './ordem-servico-form-primeng.component.html',
  styleUrls: ['./ordem-servico-form-primeng.component.css']
})
export class OrdemServicoFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private ordemServicoService = inject(OrdemServicoService);
  private tecnicoService = inject(TecnicoService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  ordemId?: number;
  form!: FormGroup;
  statusOptions = Object.values(OrdemServicoStatus);
  tecnicos: Tecnico[] = [];

  ngOnInit(): void {
    this.form = this.fb.group({
      titulo: ['', [Validators.required, Validators.minLength(3)]],
      descricao: ['', [Validators.required]],
      tecnico: ['', [Validators.required]],
      status: [OrdemServicoStatus.Pendente]
    });

    // Carregar técnicos
    this.loadTecnicos();

    // Pegar ID da rota
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.ordemId = Number(id);
      this.loadOrdem();
    }
  }

  loadTecnicos(): void {
    this.tecnicoService.getAll(1, 100).subscribe({
      next: (result) => {
        this.tecnicos = result.data.filter(t => t.status === 'Ativo');
      },
      error: () => {
        this.notificationService.error('Erro ao carregar técnicos');
      }
    });
  }

  loadOrdem(): void {
    this.ordemServicoService.getById(this.ordemId!).subscribe({
      next: (ordem) => {
        this.form.patchValue(ordem);
      },
      error: () => {
        this.notificationService.error('Erro ao carregar ordem de serviço');
      }
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value;
      
      if (this.ordemId) {
        // Log para debug
        console.log('Atualizando ordem ID:', this.ordemId);
        console.log('Dados enviados:', JSON.stringify(formValue, null, 2));
        
        this.ordemServicoService.update(this.ordemId, formValue).subscribe({
          next: () => {
            this.notificationService.success('Ordem de serviço atualizada com sucesso');
            this.router.navigate(['/ordem-servico']);
          },
          error: (error) => {
            console.error('Erro completo:', error);
            console.error('Detalhes do erro:', JSON.stringify(error.error, null, 2));
            
            // Extrair mensagens de validação específicas
            let errorMsg = 'Erro ao atualizar ordem de serviço';
            if (error.error?.errors) {
              const validationErrors = Object.values(error.error.errors).flat();
              errorMsg = validationErrors.join(', ');
            } else if (error.error?.message) {
              errorMsg = error.error.message;
            } else if (error.message) {
              errorMsg = error.message;
            }
            
            this.notificationService.error(errorMsg);
          }
        });
      } else {
        console.log('Criando nova ordem');
        console.log('Dados enviados:', formValue);
        
        this.ordemServicoService.create(formValue).subscribe({
          next: () => {
            this.notificationService.success('Ordem de serviço criada com sucesso');
            this.router.navigate(['/ordem-servico']);
          },
          error: (error) => {
            console.error('Erro completo:', error);
            const errorMsg = error?.error?.message || error?.message || 'Erro ao criar ordem de serviço';
            this.notificationService.error(errorMsg);
          }
        });
      }
    } else {
      console.log('Formulário inválido:', this.form.errors);
      this.notificationService.warning('Por favor, preencha todos os campos obrigatórios');
    }
  }

  onCancel(): void {
    this.router.navigate(['/ordem-servico']);
  }
}
