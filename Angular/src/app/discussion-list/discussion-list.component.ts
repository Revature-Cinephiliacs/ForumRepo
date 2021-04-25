import { Component, OnInit } from '@angular/core';

import { ForumService } from '../forum.service';
import { ActivatedRoute } from '@angular/router';
import { Discussion } from '../models';

@Component({
  selector: 'app-discussion-list',
  templateUrl: './discussion-list.component.html',
  styleUrls: ['./discussion-list.component.css']
})
export class DiscussionListComponent implements OnInit {

  discussions: any;
  searchDiscussions = [];
  newDiscussion: Discussion & {commentCount: number};
  newDiscussions = [];
  sortDiscussions: any;
  comments: any;
  commentCount: any;
  movieID:string = "";
  discussionID: any;
  DisplayList: boolean = true;
  constructor(private _forum: ForumService, private router:  ActivatedRoute) { }

  ngOnInit(): void {
    // this.DisplayList = true;
    // this.newDiscussions = [];
  
    this.movieID =  this.router.snapshot.params.id;
    console.log("Discussion List:" + this.movieID);
    this.getDiscussions();
   
  }

  //Function that will get a list of discussions associated with the
  //snapshot movie id
  async getDiscussions() {
    setTimeout(() => {
      this._forum.getDiscussion(this.movieID).subscribe(data => {
        console.log(data);
        
        this.discussions = data;
        
        this.discussions.forEach(d => {
          this.addCommentCount(d);
        });    
      });
    }, 1000);
  }

  //Function that will take in a discussion object and will
  //get the number of comments and add it to a new
  //discussion object with an added property for comment count, which is then 
  //added to a discussion list
  async addCommentCount(d: any) {
    setTimeout(() => {
      this._forum.getDiscussionComments(d.discussionid).subscribe(data =>{ 
        this.comments = data;
        if(this.comments == null)
        {
          this.commentCount = 0;
        }
        else
        {
          this.commentCount = this.comments.length;
        }
        this.newDiscussion = {
          discussionid: d.discussionid,
          movieid: d.movieid,
          userid: d.userid,
          subject: d.subject,
          topic: d.topic,
          commentCount: this.commentCount
        }
        this.newDiscussions.push(this.newDiscussion);
      });
    }, 1000);
  }

  //Function that will take in a search string and then filter
  //the dicussions to show matching results
  applyFilter(filterValue: string){
    console.log(filterValue);
    this.DisplayList = false;
    this.searchDiscussions = this.newDiscussions.filter(obj => {
      console.log(obj);
      return !!JSON.stringify(Object.values(obj)).match(new RegExp(filterValue));
    });
  }

  //Function that will get a list of discussions for a movie
  //sorted in ascending order based on number of comments
  async sortDiscussionsByCommentsAsc() {
    setTimeout(() => {
      this._forum.sortDiscussionByCommentsAsc().subscribe(data => {
        console.log(data);
        this.sortDiscussions = data;
        this.newDiscussions = [];   
      });
    }, 1000);
  }

  //Function that will get a list of discussions for a movie
  //sorted in descending order based on number of comments
  async sortDiscussionsByCommentsDesc() {
    setTimeout(() => {
      this._forum.sortDiscussionByCommentsDesc().subscribe(data => {
        console.log(data);
        this.sortDiscussions = data;
        this.newDiscussions = [];
      });
    }, 1000);
  }

}
