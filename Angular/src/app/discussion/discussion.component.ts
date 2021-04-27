import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { ForumService } from '../forum.service';
import { NestedComment } from '../models'
@Component({
  selector: 'app-discussion',
  templateUrl: './discussion.component.html',
  styleUrls: ['./discussion.component.css']
})
export class DiscussionComponent implements OnInit {

  comments: any;
  nestedComments = [];
  parentComments = [];
  discussionID:string = "";
  discussion: any;
  subject: any;
  displaySpoilers: any = false;
  user: any;
  displayReplyForm = false;
  displayMessageForm = true;
  parentid: string;

  newComment: any = {
    discussionid: 0,
    userid: "",
    text: "",
    isspoiler: false,
    parentcommentid: null
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
  }

  // Function that retrieves comments for a dicussion
  async getComments() {
    setTimeout(() => {
      this._forum.getDiscussionComments(this.discussionID).subscribe(data =>{ 
        console.log(data);
        this.comments = data;
      });
    }, 1000);
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

  //Function that will add a new parent comment to the discussion
  postComment(){
    if(this.isEmpty(this.newComment.text)){
      console.log("Please enter a comment");
    }else{
      this.newComment.userid = "b23dbdad-3179-4b9a-b514-0164ee9547f3"; // just for testing purpose, need to remove it later.
      this._forum.postComment(this.newComment).subscribe(data => console.log(data));
      this.getComments();
    }
    console.log(this.newComment);
  }

  //Function that will show the reply form and hide the new comment form
  showReplyForm(commentparentid:string){
    this.displayReplyForm = true;
    this.displayMessageForm = false;
    this.parentid = commentparentid;
    console.log("Reply to: " + commentparentid);
    console.log("This parent id" + this.parentid);
  }

  cancelReply()
  {
    this.displayReplyForm = false;
    this.displayMessageForm = true;
    console.log("cancel")
  }

  postReply()
  {
    console.log("Post reply" + this.parentid);
    if(this.isEmpty(this.newComment.text)){
      console.log("Please enter a comment");
    }else{
      this.newComment.userid = "b23dbdad-3179-4b9a-b514-0164ee9547f3"; // just for testing purpose, need to remove it later.
      this.newComment.parentcommentid = this.parentid;
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

  showReplies(commentid:string)
  {
    let replyComments = [];
    console.log(commentid)
    this.comments.forEach(comment => {
      if(comment.parentCommentid == commentid)
      {
        let newNestedComment = this.createNewNestedComment(comment);
        replyComments.push(newNestedComment);
      }
    });

    console.log("Show replies array: ");
    console.log(replyComments);

    return replyComments;
  }

  testingStuff(){
    console.log("test");
    //var parentComments = [];
    this.comments.forEach(ct => {
      let newNestedComment = this.createNewNestedComment(ct);
      this.nestedComments.push(newNestedComment);
    });

    console.log("New nested Comment list: ");
    console.log(this.nestedComments);
    this.nestedComments.forEach(nc => {
      if(nc.parentcommentid == null)
      {
        this.parentComments.push(nc);
      }
    });

    this.parentComments.forEach(pc => {
      this.addReplies(pc);
    });
    
    console.log("Added Replies");
    console.log(this.nestedComments);

  }

  createNewNestedComment(comment:any)
  {
    let newNestedComment = new NestedComment(
      comment.commentid,
      comment.discussionid,
      comment.userid,
      comment.text,
      comment.isspoiler,
      comment.parentCommentid,
      []
    );

    return newNestedComment;
  }

  addReplies(parent: any)
  {
    console.log(parent);
    for(let i = 0; i < this.nestedComments.length; i++)
    {
      if(this.nestedComments[i].parentcommentid == parent.commentid)
      {
        console.log(parent)
        //child is found so add to its replies
        parent.replies.push(this.nestedComments[i]);

        //check if the child has replies
        this.addReplies(this.nestedComments[i]);
      }
    }
  }

}