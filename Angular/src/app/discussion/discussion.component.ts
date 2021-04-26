import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ForumService } from '../forum.service';

@Component({
  selector: 'app-discussion',
  templateUrl: './discussion.component.html',
  styleUrls: ['./discussion.component.css']
})
export class DiscussionComponent implements OnInit {

  comments: any;
  discussionID:string = "";
  discussion: any;
  subject: any;
  displaySpoilers: any = false;
  user: any;

  pageNum: number = 1;
  sortingOrder:string = "timeD";
  numOfComments: number;
  newComment: any = {
    discussionid: 0,
    userid: "",
    text: "",
    isspoiler: false
  };

  constructor(private _forum: ForumService, private router:  ActivatedRoute) { }

  ngOnInit(): void {

    this.discussionID =  this.router.snapshot.params.id;
    console.log(this.discussionID);
    this.newComment.discussionid = this.router.snapshot.params.id;
    this.displayInput();
    this.getComments();
    this._forum.getCurrentDiscussion(this.discussionID).subscribe(data => {
      console.log(data);
      this.discussion = data;
      this.subject = this.discussion.subject;
    });
    this._forum.getDiscussionComments(this.discussionID).subscribe(data =>{ this.numOfComments = data.length})
  }

  // Function that retrieves comments for a dicussion
  async getComments() {
    setTimeout(() => {
      this._forum.getDiscussionCommentsPage(this.discussionID, this.pageNum, this.sortingOrder).subscribe(data =>{ 
        console.log(data);
        this.comments = data;
      });
    }, 1000);
  }

    //get next comments page
    onNext(){
      this.comments = [];
      this.pageNum++;
      this.getComments();
    }
    //get previous comments page
    onPrev(){
      this.comments = [];
      this.pageNum--;
      this.getComments();
    }
  
  //Function that will check if a user is logged in
  //if the user is not, elements on the page will be hidden
  displayInput(){
    if(localStorage.getItem("loggedin"))
    {
      this.user = localStorage.getItem("loggedin");
      this.newComment.userid= JSON.parse(this.user).userid;
      console.log("User Logged In");
    }else{
      console.log("Hide inputs");
    }
  }

  //Function that returns the discussion id
  getDicussionID(){
    console.log("Dicussion ID " +this.discussionID);
    return this.discussionID;
  }

  //Function that will add a new post to the discussion
  postComment(){
    if(this.isEmpty(this.newComment.text)){
      console.log("Please enter a comment");
    }else{
      this.newComment.userid = "b23dbdad-3179-4b9a-b514-0164ee9547f3"; // just for testing pourpose, need to remove it later.
      this._forum.postComment(this.newComment).subscribe(data => console.log(data));
      this.getComments();
    }
    console.log(this.newComment);
  }

  //Displays a spoiler(unblurs it)
  showSpoilers() {
    this.displaySpoilers = true;
    console.log(this.displaySpoilers);
  }

  //Function returns whether a comment is shown or not(spoiler vs shown/not a spoiler)
  spoilersShown() {
    return this.displaySpoilers;
  }

  isEmpty(testSTR:string){
    return (testSTR == "");
  }
}