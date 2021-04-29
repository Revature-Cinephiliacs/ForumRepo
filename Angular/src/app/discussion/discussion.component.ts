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
  pageComments: any;
  pageNum: number = 1;
  numOfComments = 0;

  discussionID:string = "";
  discussion: any;
  subject: any;
  discussionTopics: any;
  topics: any;
  currentTopics = [];
  selectedDiscussionOption = "Plot"
  displaySpoilers: any = false;
  user: any;
  displayReplyForm = false;
  displayMessageForm = true;
  parentid: string;
  sortingOrder:string = "timeD";
  displayWarning = false;
  
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
      this.discussionTopics = this.discussion.discussionTopics;
    });
    this._forum.getTopics().subscribe(data => {
      console.log(data);
      this.topics = data;
    })
    this._forum.getDiscussionComments(this.discussionID).subscribe(data =>{ 
      this.comments = data;
      this.getParentSize();
      console.log("Setting number of comments");
      console.log(this.numOfComments);
      console.log(this.comments);
    });
  }

  // Function that retrieves comments for a dicussion
  async getComments() {
    this.pageComments = [];
    setTimeout(() => {
      this._forum.getDiscussionCommentsPage(this.discussionID, this.pageNum, this.sortingOrder).subscribe(data =>{ 
        console.log(data);
        this.pageComments = data;
        this.currentTopics = [];
        this.getCurrentTopicNames();
      });
    }, 1000);
  }

  sortByCreationA(){
    this.sortingOrder = "timeA";
    this.getComments();
  }
  sortByCreationB(){
    this.sortingOrder = "timeD";
    this.getComments();
  }
  sortByLike(){
    this.sortingOrder = "likes";
    this.getComments();
  }
  sortByCommentD(){
    this.sortingOrder = "comments";
    this.getComments();
  }

  //Function that will calculate the number of comments
  //based on the number of parent comments
  getParentSize()
  {
    this.comments.forEach(pc => {
      if(pc.parentCommentid == null )
      {
        this.numOfComments++;
      }
    });

    console.log(this.numOfComments);
  }

  //get next comments page
  onNext(){
    this.pageComments = [];
    this.pageNum++;
    this.getComments();
  }
  //get previous comments page
  onPrev(){
    this.pageComments = [];
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

  //Function that will add a new parent comment to the discussion
  postComment(){
    if(this.isEmpty(this.newComment.text)){
      console.log("Please enter a comment");
    }else{
      this.newComment.userid = "b23dbdad-3179-4b9a-b514-0164ee9547f3"; // just for testing purpose, need to remove it later.
      this._forum.postComment(this.newComment).subscribe(data => console.log(data));
      this.pageNum = 1;
      this.getComments();
    }
    console.log(this.newComment);
  }

  //This function will add a reply to a comment and then
  //Redisplay the nested comments
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

  //Function that will show the reply form and hide the new comment form
  showReplyForm(commentparentid:string){
    this.displayReplyForm = true;
    this.displayMessageForm = false;
    this.parentid = commentparentid;
    console.log("Reply to: " + commentparentid);
    console.log("This parent id" + this.parentid);
  }

  //Will hide the reply form and display the new comment form
  cancelReply()
  {
    this.displayReplyForm = false;
    this.displayMessageForm = true;
    console.log("cancel")
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

  //Function that will add a like to a comment
  addLike(commentid: string){
    this._forum.addLike(commentid).subscribe(data => {
      console.log(data);
      this.getComments();
    });
  }

  //Function will sort the comments by likes
  sortByLikes(sortingorder: string)
  {
    console.log(sortingorder);
    this.sortingOrder = sortingorder;
    this.getComments();
  }

  //Function will check if the selected topic is already a topic of the
  //current discussion, if so display a warning, if not call service to add topic
  //to discussion and display updated topics
  addNewTopic()
  {
    var newTopic = this.selectedDiscussionOption;
    console.log(this.currentTopics.includes(newTopic));
    if(this.currentTopics.includes(newTopic))
    {
      this.displayWarning = true;
    }
    else{
      this.displayWarning = false;
      let id = "";
      this.topics.forEach(t => {
        if(newTopic == t.topicName)
        {
          id = t.topicId; 
        }
      });

      console.log("Add topic to dis");
      console.log("new topic id: " + id);
      console.log(this.discussionID);
      this._forum.addTopicToDiscussion(this.discussionID, id).subscribe(data => 
        {
          console.log(data);
          if(data == true){
            this.currentTopics.push(newTopic);
          }
      })
    }
    
  }

  //Function will take the topic ids in the current discussion
  //and convert them into topic names to be displayed to the user
  getCurrentTopicNames()
  {
    console.log("Get Current Topic Name");
    console.log(this.discussionTopics);
    console.log(this.topics);
    this.discussionTopics.forEach(dt => {
      this.topics.forEach(t => {
        if(dt == t.topicId)
        {
          console.log(dt);
          console.log(t.topicName);
          this.currentTopics.push(t.topicName);
        }
      });
    });

    console.log(this.currentTopics);
  }

}