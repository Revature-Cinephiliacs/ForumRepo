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
  newDiscussion: Discussion & {commentCount: number};
  newDiscussions = [];
  comments: any;
  commentCount: any;
  movieID:string = "";
  discussionID: any;

  constructor(private _forum: ForumService, private router:  ActivatedRoute) { }

  ngOnInit(): void {
    this.movieID =  this.router.snapshot.params.id;
    console.log("Discussion List:" + this.movieID);
    this.getDiscussions();
   
  }

  async getDiscussions() {
    setTimeout(() => {
      this._forum.getDiscussion(this.movieID).subscribe(data => {
        console.log(data);
        this.discussions = data;
        this.discussions.forEach(d => {
          d.discussionid;
          this.addCommentCount(d);
        });    
      });
    }, 10);
  }

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
          username: d.username,
          subject: d.subject,
          topic: d.topic,
          commentCount: this.commentCount
        }
        this.newDiscussions.push(this.newDiscussion);
      });
    }, 1000);
  }

}
