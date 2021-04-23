import { Injectable } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ForumService {
  currentUser:string ="";
  askingUser:string = "";
  connection:string ="";
  loggedIn:any;

  baseURL:string = "https://localhost:5001/Forum/";
  constructor(private http:HttpClient) { }

  getDiscussion(movieId: String){
    return this.http.get( this.baseURL + "discussions/"+movieId);
  }

  submitDiscussion(discussion:any){
    return this.http.post( this.baseURL + "discussion", discussion);
  }

  getDiscussionComments(discussionID:string){
    return this.http.get( this.baseURL + "comments/" + discussionID);
  }

  postComment(newComment: any){
    console.log(newComment);
    return this.http.post( this.baseURL + "comment", newComment);
  }

  getCurrentDiscussion(discussionID: string){
    console.log("here")
    return this.http.get( this.baseURL + "discussion/" + discussionID);
  }

  getTopics(){
    return this.http.get( this.baseURL + "topics");
  }
}
