import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
// PrimeNG Imports
import { InputTextModule } from 'primeng/inputtext';
import { InputMaskModule } from 'primeng/inputmask';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TecnicoService } from '../../services/tecnico.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { TecnicoStatus } from '../../models/tecnico.model';

@Component({
  selector: 'app-tecnico-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    // PrimeNG
    InputTextModule,
    InputMaskModule,
    DropdownModule,
    ButtonModule,
    CardModule
  ],
  templateUrl: './tecnico-form-primeng.component.html',
  styleUrls: ['./tecnico-form-primeng.component.css']
})
export class TecnicoFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private tecnicoService = inject(TecnicoService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  tecnicoId?: number;
  form!: FormGroup;
  statusOptions = Object.values(TecnicoStatus);
  // Force TypeScript recompile

  ngOnInit(): void {
    this.form = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      telefone: ['', [Validators.required]],
      especialidade: ['', [Validators.required]],
      status: [TecnicoStatus.Ativo]
    });

    // Capturar ID da rota
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.tecnicoId = Number(id);
      this.loadTecnico();
    }
  }

  loadTecnico(): void {
    this.tecnicoService.getById(this.tecnicoId!).subscribe({
      next: (tecnico) => {
        this.form.patchValue(tecnico);
      },
      error: () => {
        this.notificationService.error('Erro ao carregar técnico');
      }
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value;
      
      if (this.tecnicoId) {
        this.tecnicoService.update(this.tecnicoId, formValue).subscribe({
          next: () => {
            this.notificationService.success('Técnico atualizado com sucesso');
            this.router.navigate(['/tecnico']);
          },
          error: (error) => {
            console.error('Erro ao atualizar técnico:', error);
            console.error('Detalhes:', error.error);
            
            let errorMsg = 'Erro ao atualizar técnico';
            if (error.error?.errors) {
              const validationErrors = Object.entries(error.error.errors)
                .map(([field, messages]: [string, any]) => `${field}: ${Array.isArray(messages) ? messages.join(', ') : messages}`)
                .join('\n');
              errorMsg = validationErrors;
            } else if (error.error?.message) {
              errorMsg = error.error.message;
            }
            
            this.notificationService.error(errorMsg);
          }
        });
      } else {
        console.log('Criando técnico:', formValue);
        this.tecnicoService.create(formValue).subscribe({
          next: () => {
            this.notificationService.success('Técnico criado com sucesso');
            this.router.navigate(['/tecnico']);
          },
          error: (error) => {
            console.error('Erro ao criar técnico:', error);
            console.error('Detalhes:', error.error);
            
            let errorMsg = 'Erro ao criar técnico';
            if (error.error?.errors) {
              const validationErrors = Object.entries(error.error.errors)
                .map(([field, messages]: [string, any]) => `${field}: ${Array.isArray(messages) ? messages.join(', ') : messages}`)
                .join('\n');
              errorMsg = validationErrors;
            } else if (error.error?.message) {
              errorMsg = error.error.message;
            }
            
            this.notificationService.error(errorMsg);
          }
        });
      }
    } else {
      this.notificationService.error('Por favor, preencha todos os campos obrigatórios');
    }
  }

  onCancel(): void {
    this.router.navigate(['/tecnico']);
  }
}
