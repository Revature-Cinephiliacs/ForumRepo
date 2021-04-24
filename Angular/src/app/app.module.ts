import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { DiscussionComponent } from './discussion/discussion.component';

import { AppComponent } from './app.component';
import { MovieComponent } from './movie/movie.component';
import { DiscussionListComponent } from './discussion-list/discussion-list.component';

@NgModule({
  declarations: [
    AppComponent,
    DiscussionComponent,
    MovieComponent,
    DiscussionListComponent
  ],
  imports: [
    AppRoutingModule,
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
