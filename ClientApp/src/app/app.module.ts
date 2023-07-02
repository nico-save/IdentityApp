import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavbarComponent } from './navbar/navbar.component';
import { FooterComponent } from './footer/footer.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './account/register/register.component';
import { LoginComponent } from './account/login/login.component';
import { ValidationMessagesComponent } from './shared/components/errors/validation-messages/validation-messages.component';
import { NotFoundComponent } from './shared/components/errors/not-found/not-found.component';
import { SharedModule } from './shared/shared.module';
import { PlayComponent } from './play/play.component';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    FooterComponent,
    HomeComponent,
    PlayComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedModule,
    BrowserAnimationsModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}