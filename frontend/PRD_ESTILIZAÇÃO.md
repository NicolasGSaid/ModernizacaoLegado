Você é um especialista em Angular 18+ e PrimeNG. Analise o código fornecido e faça a modernização completa usando PrimeNG 20+ com os seguintes requisitos:

## 🎯 Objetivo
Transformar a aplicação LegacyProcs (sistema de ordens de serviço) em uma interface moderna e profissional usando PrimeNG.

## 📋 Requisitos Técnicos

### 1. Instalação e Configuração
- Usar PrimeNG versão mais recente compatível com Angular 18
- Configurar tema Lara (padrão moderno)
- Importar apenas os módulos necessários (tree-shaking)
- Adicionar PrimeIcons para ícones modernos

### 2. Componentes a Implementar

**Navbar/Header:**
- Usar `p-menubar` ou `p-toolbar`
- Adicionar ícones PrimeIcons
- Menu responsivo com `p-menu` ou `p-sidebar` para mobile
- Logo com ícone de ferramenta (`pi-wrench` ou similar)

**Tabela de Ordens:**
- Substituir tabela HTML por `p-table`
- Implementar:
  - Paginação (`paginator="true"`)
  - Busca/filtro global no input de busca
  - Ordenação por colunas
  - Badges coloridos para status (`p-badge` ou `p-tag`):
    - "Pendente" = amarelo/warning
    - "Em Andamento" = azul/info
  - Botões de ação com ícones (`pi-pencil`, `pi-trash`)
  - Hover effects elegantes
  - Loading skeleton durante carregamento

**Botão Flutuante (+):**
- Usar `p-button` com `rounded` e `severity="help"`
- Posição fixed no canto inferior direito
- Ícone `pi-plus`
- Tooltip com `pTooltip="Nova Ordem"`

**Modais/Dialogs:**
- `p-dialog` para criar/editar ordens
- `p-confirmDialog` para confirmação de exclusão
- Formulários com `p-inputText`, `p-textarea`, `p-calendar`, `p-dropdown`

**Notificações:**
- Implementar `p-toast` para feedback de ações

### 3. Estilização

**Cores e Tema:**
- Manter o azul do header (#3f51b5 ou similar)
- Usar variáveis CSS do PrimeNG para consistência
- Adicionar dark mode toggle opcional

**Layout:**
- Container responsivo
- Spacing adequado (usar classes PrimeNG: p-3, p-4, mt-3, etc)
- Cards com `p-card` se necessário
- Footer fixo no bottom

**Animações:**
- Transitions suaves em hover
- Ripple effect nos botões
- Fade in/out nos dialogs

### 4. Código Limpo

- Usar standalone components se o projeto permitir
- TypeScript strict mode
- Interfaces para models (ServiceOrder, Status, etc)
- Services separados para API calls
- RxJS observables para operações assíncronas
- Signals do Angular 18 se apropriado

### 5. Funcionalidades Extras

- Exportar tabela para CSV/Excel (`p-table` tem suporte nativo)
- Filtros avançados por coluna
- Seleção múltipla com checkboxes
- Actions em lote (excluir múltiplos)
- Loading states elegantes
- Empty state quando não há dados

## 📝 Estrutura Esperada

Forneça:
1. **package.json** - dependências necessárias
2. **app.config.ts** - configuração PrimeNG
3. **service-orders.component.ts** - lógica do componente
4. **service-orders.component.html** - template PrimeNG
5. **service-orders.component.css** - estilos customizados mínimos
6. **models/service-order.model.ts** - interfaces TypeScript
7. **Instruções de instalação** passo a passo

## 🎨 Referências Visuais

- Tema base: Lara Light (moderno e clean)
- Inspiração: Dashboards SaaS modernos
- Prioridade: UX intuitiva e visual profissional

## ⚠️ Importante

- Manter toda lógica de negócio existente
- Código deve ser production-ready
- Comentários apenas onde necessário
- Seguir Angular style guide oficial
- Acessibilidade (ARIA labels, keyboard navigation)

## 🚀 Entrega

Gere o código completo, funcional e pronto para copy-paste. Seja criativo mas mantenha profissionalismo e usabilidade.