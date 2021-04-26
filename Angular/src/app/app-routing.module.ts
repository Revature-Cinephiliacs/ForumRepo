import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { DiscussionComponent } from './discussion/discussion.component';
import {MovieComponent} from './movie/movie.component';
import {DiscussionListComponent} from './discussion-list/discussion-list.component';

const routes: Routes = [

  {path: 'discussion/:id', component:DiscussionComponent},
  {path: 'movie/:id', component: MovieComponent},
  {path: 'discussionlist/:id', component: DiscussionListComponent}
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forRoot(routes),
    CommonModule
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
