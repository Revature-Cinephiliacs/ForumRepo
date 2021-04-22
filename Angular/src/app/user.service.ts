import { Injectable } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  currentUser:string ="";
  askingUser:string = "";
  connection:string ="";
  loggedIn:any;

  baseURL:string = "https://cinephiliacsapi.azurewebsites.net/";
  constructor(private http:HttpClient) { }

  getDiscussion(movieId:String){
    return this.http.get( this.baseURL + "forum/discussions/"+movieId);
  }

  submitDiscussion(discussion:any){
    return this.http.post( this.baseURL + "forum/discussion", discussion);
  }

  getDiscussionComments(discussionID:string){
    return this.http.get( this.baseURL + "forum/comments/" + discussionID);
  }

  postComment(newComment:any){
    return this.http.post( this.baseURL + "Forum/comment",newComment);
  }

  getCurrentDiscussion(discussionID:string){
    return this.http.get( this.baseURL + "forum/discussion/" + discussionID);
  }
}
