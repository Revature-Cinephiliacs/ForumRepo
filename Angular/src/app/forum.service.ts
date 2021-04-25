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

  //URL to Forum API(temporary)
  baseURL:string = "https://localhost:5001/Forum/";
  constructor(private http:HttpClient) { }

  //Function that will make a call to the Forum API discussions/movieid endpoint
  //to retrieve a list of discussions associated with given movie id
  getDiscussion(movieId: String){
    return this.http.get( this.baseURL + "discussions/"+movieId);
  }

  //Function that will make a call to the Forum API discussion endpoint
  //to add a new discussion
  submitDiscussion(discussion:any){
    return this.http.post( this.baseURL + "discussion", discussion);
  }

  //Function that will make a call to the Forum API comments/discussionid endpoint
  //to retrieve a list of comments associated with given discussionid
  getDiscussionComments(discussionID:string){
    return this.http.get( this.baseURL + "comments/" + discussionID);
  }

  //Function that will make a call to the Forum API comment endpoint
  //to add a new comment
  postComment(newComment: any){
    console.log(newComment);
    return this.http.post( this.baseURL + "comment", newComment);
  }

  //Function that will make a call to the Forum API discussion/discussionid endpoint
  //to retrieve a discussion with the given discussionid
  getCurrentDiscussion(discussionID: string){
    return this.http.get( this.baseURL + "discussion/" + discussionID);
  }

  //Function that will make a call to the Forum API topics endpoint
  //to retrieve a list of topics
  getTopics(){
    return this.http.get( this.baseURL + "topics");
  }

  //Function that will make a call to the Forum API discussion/sort/comment/ascend
  //endpoint to sort a list of discussions in ascending order by the number of comments
  sortDiscussionByCommentsAsc(){
    return this.http.get( this.baseURL + "discussion/sort/comment/ascend");
  }

  //Function that will make a call to the Forum API discussion/sort/comment/descending
  //endpoint to sort a list of discussions in descending order by the number of comments
  sortDiscussionByCommentsDesc(){
    return this.http.get( this.baseURL + "discussion/sort/comment/descend");
  }
}
