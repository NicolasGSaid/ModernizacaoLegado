import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { OrdemServicoComponent } from './ordem-servico/ordem-servico.component';
import { TecnicoComponent } from './tecnico/tecnico.component';
import { ClienteComponent } from './cliente/cliente.component';

// ❌ PROBLEMA: Sem routing module
// ❌ PROBLEMA: Todos os componentes declarados mas não há navegação

@NgModule({
  declarations: [
    AppComponent,
    OrdemServicoComponent,
    TecnicoComponent,
    ClienteComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
