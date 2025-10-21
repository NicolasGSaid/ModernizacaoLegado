import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
// PrimeNG Imports
import { InputTextModule } from 'primeng/inputtext';
import { InputMaskModule } from 'primeng/inputmask';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { ClienteService } from '../../services/cliente.service';
import { NotificationService } from '../../../../core/services/notification.service';

@Component({
  selector: 'app-cliente-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    // Material
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    // PrimeNG
    InputTextModule,
    InputMaskModule,
    ButtonModule,
    CardModule
  ],
  templateUrl: './cliente-form-primeng.component.html',
  styleUrls: ['./cliente-form-primeng.component.css']
})
export class ClienteFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private clienteService = inject(ClienteService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  clienteId?: number;
  form!: FormGroup;

  ngOnInit(): void {
    this.form = this.fb.group({
      razaoSocial: ['', [Validators.required, Validators.minLength(3)]],
      nomeFantasia: [''],
      cnpj: ['', [Validators.required, Validators.pattern(/^\d{14}$/)]],
      email: ['', [Validators.required, Validators.email]],
      telefone: [''],
      endereco: ['', [Validators.required]],
      cidade: ['', [Validators.required]],
      estado: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(2)]],
      cep: ['', [Validators.required, Validators.pattern(/^\d{8}$/)]]
    });

    // Capturar ID da rota
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.clienteId = Number(id);
      this.loadCliente();
    }
  }

  loadCliente(): void {
    this.clienteService.getById(this.clienteId!).subscribe({
      next: (cliente) => {
        this.form.patchValue(cliente);
      },
      error: () => {
        this.notificationService.error('Erro ao carregar cliente');
      }
    });
  }

  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value;
      
      if (this.clienteId) {
        this.clienteService.update(this.clienteId, formValue).subscribe({
          next: () => {
            this.notificationService.success('Cliente atualizado com sucesso');
            this.router.navigate(['/cliente']);
          },
          error: () => {
            this.notificationService.error('Erro ao atualizar cliente');
          }
        });
      } else {
        console.log('Criando cliente:', formValue);
        this.clienteService.create(formValue).subscribe({
          next: () => {
            this.notificationService.success('Cliente criado com sucesso');
            this.router.navigate(['/cliente']);
          },
          error: (error) => {
            console.error('Erro ao criar cliente:', error);
            console.error('Detalhes:', error.error);
            
            // Extrair mensagens de validação específicas
            let errorMsg = 'Erro ao criar cliente';
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
    this.router.navigate(['/cliente']);
  }
}
