# PrimeNG Setup - LegacyProcs Frontend

## 📦 Instalação Completa

### Versões Instaladas
- **PrimeNG:** 17.18.0 (compatível com Angular 18)
- **PrimeIcons:** latest
- **Tema:** Lara Light Blue

### Comando de Instalação
```bash
npm install primeng@^17.18.0 primeicons --legacy-peer-deps
```

---

## 🎨 Configuração do Tema

### 1. Imports em `styles.css`

```css
/* PrimeNG Theme - Lara Light Blue */
@import 'primeng/resources/themes/lara-light-blue/theme.css';
@import 'primeng/resources/primeng.css';
@import 'primeicons/primeicons.css';

/* PrimeNG Custom Variables and Overrides */
@import './styles/primeng/variables.scss';
@import './styles/primeng/overrides.scss';
```

### 2. Estrutura de Arquivos

```
frontend/src/
├── styles/
│   ├── primeng/
│   │   ├── variables.scss    # Variáveis CSS customizadas
│   │   └── overrides.scss    # Overrides de componentes
│   └── styles.css            # Import principal
```

---

## 🎨 Design System

### Cores Primárias

```scss
:root {
  --primary-color: #3f51b5;        /* Azul principal (header) */
  --primary-color-text: #ffffff;
  
  /* Status Colors */
  --green-500: #4caf50;   /* Success */
  --blue-500: #2196f3;    /* Info */
  --yellow-500: #ff9800;  /* Warning */
  --red-500: #f44336;     /* Danger */
}
```

### Classes de Status

```scss
.status-pendente      { background: var(--yellow-500); }
.status-em-andamento  { background: var(--blue-500); }
.status-concluida     { background: var(--green-500); }
.status-cancelada     { background: var(--red-500); }
```

---

## 🧩 Componentes Disponíveis

### Tabela (p-table)
```html
<p-table [value]="data()" [paginator]="true" [rows]="10">
  <ng-template pTemplate="header">
    <tr>
      <th>Coluna</th>
    </tr>
  </ng-template>
  <ng-template pTemplate="body" let-item>
    <tr>
      <td>{{ item.campo }}</td>
    </tr>
  </ng-template>
</p-table>
```

### Botões (p-button)
```html
<p-button 
  label="Salvar" 
  icon="pi pi-save" 
  severity="primary"
  (onClick)="onSave()">
</p-button>
```

### Badges/Tags (p-tag)
```html
<p-tag 
  [value]="status" 
  [severity]="getSeverity(status)">
</p-tag>
```

### Toast (p-toast)
```typescript
// Component
providers: [MessageService]

constructor(private messageService: MessageService) {}

showSuccess() {
  this.messageService.add({
    severity: 'success',
    summary: 'Sucesso',
    detail: 'Operação realizada com sucesso'
  });
}
```

```html
<!-- Template -->
<p-toast></p-toast>
```

### Formulários
```html
<div class="p-field">
  <label for="campo">Campo *</label>
  <input pInputText id="campo" formControlName="campo" />
  <small class="p-error" *ngIf="form.get('campo')?.hasError('required')">
    Campo obrigatório
  </small>
</div>
```

### Dropdown (p-dropdown)
```html
<p-dropdown 
  [options]="options" 
  placeholder="Selecione"
  optionLabel="label"
  optionValue="value"
  formControlName="campo">
</p-dropdown>
```

---

## 🧪 Teste de Instalação

### Acessar Componente de Teste
```
http://localhost:4200/primeng-test
```

### O que é testado:
- ✅ Botões com ícones e severidades
- ✅ Tabela com paginação
- ✅ Badges de status coloridos
- ✅ Inputs e dropdowns
- ✅ Toast notifications
- ✅ Tema e variáveis customizadas

---

## 📊 Imports Necessários por Componente

### Tabela
```typescript
import { TableModule } from 'primeng/table';
```

### Botões
```typescript
import { ButtonModule } from 'primeng/button';
```

### Tags/Badges
```typescript
import { TagModule } from 'primeng/tag';
```

### Inputs
```typescript
import { InputTextModule } from 'primeng/inputtext';
```

### Dropdown
```typescript
import { DropdownModule } from 'primeng/dropdown';
```

### Toast
```typescript
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
```

### Dialog
```typescript
import { DialogModule } from 'primeng/dialog';
```

### Confirm Dialog
```typescript
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
```

---

## 🎯 Próximos Passos

### Sprint 2: Migração de Componentes
1. Substituir mat-table por p-table em:
   - Ordem de Serviço
   - Clientes
   - Técnicos

2. Substituir mat-form-field por PrimeNG inputs

3. Adicionar p-toast para notificações

4. Implementar p-confirmDialog para exclusões

---

## ⚠️ Notas Importantes

### Compatibilidade
- PrimeNG 17.18 é compatível com Angular 18
- PrimeNG 20+ requer Angular 20+
- Usar `--legacy-peer-deps` se necessário

### Performance
- Bundle size aumentou ~4KB (aceitável)
- Lazy loading de componentes PrimeNG funciona
- Tree-shaking está ativo

### Coexistência
- Angular Material e PrimeNG podem coexistir
- Migração será gradual, componente por componente
- Sem quebra de funcionalidade existente

---

## 🐛 Troubleshooting

### Build falha com erro de peer dependencies
```bash
npm install --legacy-peer-deps
```

### Estilos não aplicam
Verificar ordem dos imports em `styles.css`

### Componentes não renderizam
Verificar se o módulo foi importado no component

### Warnings de SCSS
Warnings de "Empty sub-selector" são esperados e não afetam funcionalidade

---

## 📚 Documentação Oficial

- **PrimeNG:** https://primeng.org/
- **PrimeIcons:** https://primeng.org/icons
- **Temas:** https://primeng.org/theming

---

**✅ Setup Completo e Validado em:** 19/10/2025  
**👤 Responsável:** Nicolas Dias  
**🔖 Sprint:** 1 - Task 1.3.3
