# Assets - LegacyProcs Frontend

## 📁 Estrutura

```
assets/
├── images/          # Imagens (logos, banners, ilustrações)
├── icons/           # Ícones (SVG, PNG)
└── README.md        # Este arquivo
```

## 🖼️ Convenções

### Imagens
- **Formato:** PNG para transparência, JPG para fotos
- **Nomenclatura:** kebab-case (ex: `logo-empresa.png`)
- **Otimização:** Comprimir antes de commitar (TinyPNG, ImageOptim)

### Ícones
- **Formato:** SVG (preferencial) ou PNG
- **Tamanho:** Múltiplos (16x16, 24x24, 32x32, 48x48)
- **Nomenclatura:** `icon-nome-do-icone.svg`

## 📝 Como Usar

### No Component
```typescript
// TypeScript
export class MyComponent {
  logoPath = 'assets/images/logo.png';
}
```

```html
<!-- HTML -->
<img src="assets/images/logo.png" alt="Logo">
<img [src]="logoPath" alt="Logo">
```

### No CSS
```css
.background {
  background-image: url('/assets/images/background.jpg');
}
```

## 🎨 Assets Padrão (para adicionar)

### Sugestões de Assets
- `logo-alest.png` - Logo da empresa
- `logo-legacyprocs.svg` - Logo do sistema
- `empty-state.svg` - Ilustração para lista vazia
- `error-404.svg` - Página não encontrada
- `maintenance-icon.svg` - Ícone de manutenção

### Onde Encontrar Ícones Gratuitos
- [Heroicons](https://heroicons.com/) - SVG MIT License
- [Material Icons](https://fonts.google.com/icons) - Apache License
- [Feather Icons](https://feathericons.com/) - MIT License
- [Lucide](https://lucide.dev/) - ISC License

## 🚫 O Que NÃO Commitar
- Imagens muito grandes (>1MB)
- Arquivos não otimizados
- Assets de terceiros sem licença

## 📦 Build de Produção

No build (`ng build`), os assets são copiados para `dist/legacyprocs-frontend/assets/` automaticamente.

Configurado em `angular.json`:
```json
"assets": [
  "src/favicon.ico",
  "src/assets"
]
```
